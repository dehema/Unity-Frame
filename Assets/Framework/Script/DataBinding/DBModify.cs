using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB
{
    /// <summary>
    /// 数据更新类型
    /// </summary>
    public enum DBAction
    {
        Init,
        Update,
        Add,
        Remove,
    }

    /// <summary>
    /// 数据变动
    /// </summary>
    public class DBModify
    {
        /// <summary>
        /// 数据更新类型
        /// </summary>
        public object value;
        public DBAction action;

        public T Value<T>()
        {
            return (T)this.value;
        }

        public override string ToString()
        {
            return string.Format("action: {0}, value: {1}", action, value);
        }
    }

    public class DBKeyModify : DBModify
    {
        /// <summary>
        ///  数据的 key
        /// </summary>
        public string key;
        /// <summary>
        /// 是否是第一个
        /// </summary>
        public bool isFirst;
        /// <summary>
        /// 是否是最后一个
        /// </summary>
        public bool isLast;

        public override string ToString()
        {
            return string.Format("action: {0}, key: {1}, value: {2}", action, key, value);
        }
    }
}