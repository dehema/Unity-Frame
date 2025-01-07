
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DB
{
    public abstract class DBObject
    {
        protected object _value;
        protected bool _active = true;
        private Dictionary<int, Action<DBModify>> _delegateDic;
        protected DBDispatcher _dispatcher = null;

        public DBDispatcher Dispatchers
        {
            set
            {
                this._dispatcher = value;
            }
        }

        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                Parse(value);
            }
        }

        public bool IsActive
        {
            get
            {
                return _active;
            }
        }

        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool Parse(object value)
        {
            this._active = true;
            if (!EqualityComparer<object>.Default.Equals(_value, value))
            {
                this._value = value;
                this.Emit(DBAction.Update);
                return true;
            }
            return false;
        }

        public DBHandler.Binding Bind(Action<DBModify> callfunc)
        {
            if (this._delegateDic is null)
            {
                this._delegateDic = new Dictionary<int, Action<DBModify>>();
            }
            callfunc(new DBModify { value = this._value, action = DBAction.Init });
            int token = DBHandler.GetBindToken();
            this._delegateDic.Add(token, callfunc);
            return new DBHandler.Binding()
            {
                callfunc = this.UnBind,
                token = token
            };
        }


        public void Bind(Action<DBModify> callfunc, DBHandler handler, DBHandler.UpdateMethod method = DBHandler.UpdateMethod.Append)
        {
            handler.Bind(this, this.Bind(callfunc), method);
        }

        public void UnBind(DBHandler.Binding handler)
        {
            if (_delegateDic.ContainsKey(handler.token))
            {
                _delegateDic.Remove(handler.token);
            }
        }

        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="action"></param>
        protected void Emit(DBAction action)
        {
            if (this._delegateDic is null)
            {
                return;
            }
            var modify = new DBModify { value = this._value, action = action };
            foreach (var item in this._delegateDic.Values)
            {
                if (item != null)
                {
                    if (_dispatcher != null)
                    {
                        _dispatcher.Add(this, item, modify);
                    }
                    else
                    {
                        item(modify);
                    }
                }
            }
        }

        /// <summary>
        /// 清除绑定对象
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            if (this._active)
            {
                this._active = false;
                this.Emit(DBAction.Remove);
                return true;
            }
            return false;
        }

        public virtual bool SetVal(object _obj, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            this.Dispatchers = dispatcher;
            return this.Parse(_obj);
        }
    }
}