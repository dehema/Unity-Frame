using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using YamlDotNet.Core.Tokens;

namespace Rain.Core
{
    public class DBClass : DBObject
    {
        private enum MemberType
        {
            DBObject,
            Bool,
            Int,
            String,
            Float,
            DictClass,
            Enum,
        }

        private Dictionary<string, MemberType> _fieldType;
        protected Dictionary<string, System.Reflection.FieldInfo> _field;//包含所有字段

        public Dictionary<string, System.Reflection.FieldInfo> Field { get { return _field; } }

        protected new Dictionary<string, DBObject> _value;//包含所有DB字段
        public new Dictionary<string, DBObject> Value
        {
            get
            {
                if (_value == null)
                {
                    this._value = new Dictionary<string, DBObject>();
                    foreach (var item in _field)
                    {
                        var field = item.Value;
                        if (field.GetValue(this) is DBObject dbObject)
                        {
                            this._value.Add(field.Name, dbObject);
                        }
                        //else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))// 获取字典值的类型
                        //{
                        //    Type valueType = field.FieldType.GetGenericArguments()[1];
                        //    if (valueType.IsSubclassOf(typeof(DBClass)))
                        //    {
                        //Dictionary<int, DBClass> dbClass = field.GetValue(this) as Dictionary<int, DBClass>;
                        //this._value.Add(field.Name, dbClass);
                        //    }
                        //}
                    }
                }
                return _value;
            }
        }

        public DBClass() : base()
        {
            _field = new Dictionary<string, System.Reflection.FieldInfo>();
            _fieldType = new Dictionary<string, MemberType>();
            // 初始化字段, 便于 bind
            foreach (FieldInfo field in this.GetType().GetFields())
            {
                if (field.GetCustomAttribute<NonSerializedAttribute>() != null) //不序列化
                    continue;
                if (field.FieldType.IsEnum)
                {
                    this._fieldType[field.Name] = MemberType.Enum;
                }
                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(DBDictClass<,>))
                {
                    this._fieldType[field.Name] = MemberType.DictClass;
                }
                else if (field.FieldType == typeof(bool))
                {
                    this._fieldType[field.Name] = MemberType.Bool;
                }
                else if (field.FieldType == typeof(int))
                {
                    this._fieldType[field.Name] = MemberType.Int;
                }
                else if (field.FieldType == typeof(string))
                {
                    this._fieldType[field.Name] = MemberType.String;
                }
                else if (field.FieldType == typeof(float))
                {
                    this._fieldType[field.Name] = MemberType.Float;
                }
                else if (field.FieldType.IsSubclassOf(typeof(DBObject)))
                {
                    var value = field.GetValue(this);
                    if (value == null)
                    {
                        var types = new System.Type[0];
                        value = field.FieldType.GetConstructor(types).Invoke(null);
                        field.SetValue(this, value);
                    }
                    this._fieldType[field.Name] = MemberType.DBObject;
                }
                else
                {
                    this._fieldType[field.Name] = MemberType.DBObject;
                }

                _field.Add(field.Name, field);
            }
            base._value = this;
        }

        public string ToJson()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var field in _field)
            {
                string fieldName = field.Key;
                var fieldInfo = field.Value;
                var fieldValue = fieldInfo.GetValue(this);

                // 根据字段类型进行序列化
                if (fieldValue != null && fieldValue.GetType().IsGenericType && fieldValue.GetType().GetGenericTypeDefinition() == typeof(DBDictClass<,>))
                {
                    var toJsonMethod = fieldValue.GetType().GetMethod("ToJson");
                    string jsonStr = toJsonMethod.Invoke(fieldValue, null) as string;
                    dict[fieldName] = jsonStr;
                }
                else if (fieldValue is DBObject dbObj)
                {
                    // 对于普通DBObject，只序列化Value
                    dict[fieldName] = dbObj.Value;
                }
                else
                {
                    // 对于普通字段，直接序列化
                    dict[fieldName] = fieldValue;
                }
            }
            string str = JsonConvert.SerializeObject(dict);
            return str;
        }

        public bool SetVal(string _key, object _val)
        {
            switch (this._fieldType[_key])
            {
                case MemberType.Bool:
                    this._field[_key].SetValue(this, (bool)_val);
                    break;
                case MemberType.Int:
                    this._field[_key].SetValue(this, int.Parse(_val.ToString()));
                    break;
                case MemberType.String:
                    this._field[_key].SetValue(this, _val.ToString());
                    break;
                case MemberType.Float:
                    this._field[_key].SetValue(this, float.Parse(_val.ToString()));
                    break;
                case MemberType.Enum:
                    this._field[_key].SetValue(this, Enum.ToObject(this._field[_key].FieldType, _val));
                    break;
                default:
                    return false;
            }
            return true;
        }

        public bool SetVal(Dictionary<string, object> _objDict, DBAction method = DBAction.Update, DBDispatcher dispatcher = null)
        {
            if (_objDict == null)
            {
                return false;
            }
            this.Dispatchers = dispatcher;
            foreach (var rkey in _objDict)
            {
                string _key = rkey.Key;
                object _val = rkey.Value;
                if (!Value.ContainsKey(_key))
                {
                    SetVal(_key, _val);
                    continue;
                }
                switch (this._fieldType[_key])
                {
                    case MemberType.DBObject:
                        DBObject dbObj = _field[_key].GetValue(this) as DBObject;
                        dbObj.SetVal(_val);
                        break;
                    case MemberType.DictClass:
                        var dbDictClass = _field[_key].GetValue(this);
                        if (dbDictClass != null)
                        {
                            // 使用反射调用DBDictClass的SetVal方法
                            var setValMethod = dbDictClass.GetType().GetMethod("SetVal", new Type[] { typeof(object), typeof(DBAction), typeof(DBDispatcher) });
                            if (setValMethod != null)
                            {
                                setValMethod.Invoke(dbDictClass, new object[] { _val, DBAction.Update, null });
                            }
                        }
                        break;
                }
            }
            return true;
        }
    }
}
