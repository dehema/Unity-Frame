using System;
using System.Collections.Generic;

namespace Rain.Core
{

    public class DBHandler
    {
        private static int _bindHandle = 0;
        private static object _lock = new object();
        public static int GetBindToken()
        {
            lock (_lock)
            {
                return _bindHandle++;
            }
        }

        public enum UpdateMethod
        {
            Append,
            Replace,
        }

        private Dictionary<DBObject, List<DBBinding>> _bindingListDict = new Dictionary<DBObject, List<DBBinding>>();
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bind"></param>
        /// <param name="method"></param>
        public void Bind(DBObject obj, DBBinding bind, UpdateMethod method)
        {
            if (this._bindingListDict.ContainsKey(obj))
            {
                if (method == UpdateMethod.Replace)
                {
                    this.UnBind(obj);
                }
            }
            else
            {
                this._bindingListDict.Add(obj, new List<DBBinding>());
            }
            this._bindingListDict[obj].Add(bind);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        /// <param name="obj"></param>
        public void UnBind(DBObject obj)
        {
            if (!this._bindingListDict.ContainsKey(obj))
            {
                return;
            }
            foreach (var item in this._bindingListDict[obj])
            {
                item.UnBind();
            }
            this._bindingListDict[obj].Clear();
        }

        /// <summary>
        /// 解除所有绑定
        /// </summary>
        public void UnBindAll()
        {
            foreach (var item in this._bindingListDict.Values)
            {
                foreach (var item2 in item)
                {
                    item2.UnBind();
                }
            }
            this._bindingListDict.Clear();
        }
    }

    public class DBBinding
    {
        public Action<DBBinding> callfunc;
        public int token;
        public void UnBind()
        {
            this.callfunc(this);
        }
    }
}
