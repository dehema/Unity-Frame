using DB;
using System;
using System.Collections;
using UnityEngine;

namespace DB
{
    public class DBFloat : DBObject
    {
        public new float Value
        {
            get
            {
                return (float)_value;
            }
            set
            {
                this.Parse(value);
            }
        }

        public DBFloat() : base()
        {
            this._value = default(float);
        }

        public DBFloat(float value) : base()
        {
            this._value = value;
        }

        public override bool SetVal(object _obj, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            this.Dispatchers = dispatcher;
            return this.Parse(Convert.ToSingle(_obj));
        }
    }
}