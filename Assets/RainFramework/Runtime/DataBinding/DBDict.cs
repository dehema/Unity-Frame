using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Rain.Core
{
    /// <summary>
    /// 泛型字典类型DBObject，支持在声明时指定value类型
    /// 使用方式：DBDict<int, CityBuildingData> cityBuildings = new DBDict<int, CityBuildingData>();
    /// </summary>
    public abstract class DBDict<TKey, TValue> : DBObject, IDictionary<TKey, TValue> where TValue : DBClass
    {
        protected Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        public DBDict()
        {
            base._value = _dict;
        }

        #region IDictionary<TKey, TValue> 实现

        public TValue this[TKey key]
        {
            get
            {
                return _dict.ContainsKey(key) ? _dict[key] : null;
            }
            set
            {
                _dict[key] = value;
                this.Emit(DBAction.Update);
            }
        }

        public ICollection<TKey> Keys => _dict.Keys;

        public ICollection<TValue> Values => _dict.Values;

        public int Count => _dict.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            _dict.Add(key, value);
            this.Emit(DBAction.Update);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dict.Add(item.Key, item.Value);
            this.Emit(DBAction.Update);
        }

        public new void Clear()
        {
            _dict.Clear();
            this.Emit(DBAction.Update);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dict.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dict).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            bool result = _dict.Remove(key);
            if (result)
            {
                this.Emit(DBAction.Update);
            }
            return result;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            bool result = _dict.Remove(item.Key);
            if (result)
            {
                this.Emit(DBAction.Update);
            }
            return result;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        #endregion

        #region DBObject 重写

        public override object Value
        {
            get
            {
                return _dict;
            }
            set
            {
                if (value is Dictionary<TKey, TValue> dict)
                {
                    _dict = dict;
                    this.Emit(DBAction.Update);
                }
                else if (value is IDictionary<TKey, TValue> idict)
                {
                    _dict = new Dictionary<TKey, TValue>(idict);
                    this.Emit(DBAction.Update);
                }
                else
                {
                    base.Parse(value);
                }
            }
        }

        public override bool SetVal(object _obj, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            this.Dispatchers = dispatcher;
            
            if (_obj is Dictionary<TKey, TValue> dict)
            {
                _dict = dict;
                this.Emit(method);
                return true;
            }
            else if (_obj is IDictionary<TKey, TValue> idict)
            {
                _dict = new Dictionary<TKey, TValue>(idict);
                this.Emit(method);
                return true;
            }
            else if (_obj is Dictionary<TKey, object> dictObj)
            {
                // 处理从JSON反序列化来的Dictionary<TKey, object>数据
                _dict.Clear();
                foreach (var kvp in dictObj)
                {
                    var convertedValue = ConvertValue(kvp.Value);
                    if (convertedValue != null)
                    {
                        _dict[kvp.Key] = convertedValue;
                    }
                }
                this.Emit(method);
                return true;
            }
            else if (_obj is string jsonStr)
            {
                return LoadFromJson(jsonStr);
            }
            
            return base.Parse(_obj);
        }

        #endregion

        #region 便捷方法

        /// <summary>
        /// 获取指定键的值，如果不存在返回null
        /// </summary>
        public TValue GetValue(TKey key)
        {
            return _dict.TryGetValue(key, out TValue value) ? value : null;
        }

        /// <summary>
        /// 设置指定键的值
        /// </summary>
        public void SetValue(TKey key, TValue value)
        {
            this[key] = value;
        }

        /// <summary>
        /// 检查是否包含指定键
        /// </summary>
        public bool HasKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// 获取所有键
        /// </summary>
        public TKey[] GetAllKeys()
        {
            return _dict.Keys.ToArray();
        }

        /// <summary>
        /// 获取所有值
        /// </summary>
        public TValue[] GetAllValues()
        {
            return _dict.Values.ToArray();
        }

        /// <summary>
        /// 转换为JSON字符串
        /// </summary>
        public string ToJsonString()
        {
            return ToJson();
        }

        /// <summary>
        /// 从JSON字符串加载
        /// </summary>
        public bool LoadFromJson(string json)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<TKey, object>>(json);
                if (dict != null)
                {
                    _dict.Clear();
                    foreach (var kvp in dict)
                    {
                        TValue convertedValue = ConvertValue(kvp.Value);
                        if (convertedValue != null)
                        {
                            _dict[kvp.Key] = convertedValue;
                        }
                    }
                    this.Emit(DBAction.Update);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"{GetType().Name} LoadFromJson失败: {ex.Message}");
            }
            return false;
        }

        #endregion

        #region 抽象方法

        /// <summary>
        /// 转换为JSON字符串 - 子类需要实现
        /// </summary>
        public abstract string ToJson();

        /// <summary>
        /// 转换值类型 - 子类需要实现
        /// </summary>
        protected abstract TValue ConvertValue(object value);

        #endregion

        #region 重写ToString

        public override string ToString()
        {
            return $"{GetType().Name}[Count={Count}]";
        }

        #endregion
    }
}