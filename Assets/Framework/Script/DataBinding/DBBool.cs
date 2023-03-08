using System;
using System.Collections;
using UnityEngine;

namespace DB
{
    public class DBBool : DBObject
    {
        public new bool Value
        {
            get
            {
                return bool.Parse(_value.ToString());
            }
            set
            {
                this.Parse(value);
            }
        }

        public DBBool() : base()
        {
            this._value = default(bool);
        }

        public DBBool(bool val) : base()
        {
            this._value = val;
        }

        public override bool SetVal(object _obj, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            this.Dispatchers = dispatcher;
            return this.Parse(Convert.ToBoolean(_obj));
        }
    }
}