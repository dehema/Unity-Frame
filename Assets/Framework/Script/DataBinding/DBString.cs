using DB;
using System.Collections;
using UnityEngine;

namespace DB
{
    public class DBString : DBObject
    {
        public new string Value
        {
            get
            {
                return (string)_value;
            }
            set
            {
                this.Parse(value);
            }
        }
        public DBString() : base()
        {
            this._value = default(string);
        }
        public DBString(string value) : base()
        {
            this._value = value;
        }

        public override bool SetVal(object _obj, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            this.Dispatchers = dispatcher;
            return this.Parse(_obj);
        }
    }
}