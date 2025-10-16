using System;

namespace Rain.Core
{
    public class DBLong : DBObject
    {
        public new long Value
        {
            get
            {
                return (long)_value;
            }
            set
            {
                this.Parse(value);
            }
        }

        public DBLong() : base()
        {
            this._value = default(long);
        }

        public DBLong(long val) : base()
        {
            this._value = val;
        }

        public override bool SetVal(object _obj, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            this.Dispatchers = dispatcher;
            return this.Parse(Convert.ToInt64(_obj));
        }
    }
}
