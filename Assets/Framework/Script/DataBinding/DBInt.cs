using System;
using System.Collections;
using UnityEngine;

namespace DB
{
    public class DBInt : DBObject
    {
        public new int Value
        {
            get
            {
                return (int)_value;
            }
            set
            {
                this.Parse(value);
            }
        }

        public DBInt() : base()
        {
            this._value = default(int);
        }

        public DBInt(int val) : base()
        {
            this._value = val;
        }

        public override bool SetVal(object _obj, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            this.Dispatchers = dispatcher;
            return this.Parse(Convert.ToInt32(_obj));
        }
    }
}