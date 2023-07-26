using com.adjust.sdk;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Core.Tokens;

namespace DB
{
    public class DBClass : DBObject
    {
        private enum MemberType
        {
            DBObject,
            Bool,
            Int,
            String,
            Float
        }

        private Dictionary<string, MemberType> _fieldType;
        protected Dictionary<string, System.Reflection.FieldInfo> _field;

        public Dictionary<string, System.Reflection.FieldInfo> Field { get { return _field; } }

        protected new Dictionary<string, DBObject> _value;
        public new Dictionary<string, DBObject> Value
        {
            get
            {
                if (_value == null)
                {
                    this._value = new Dictionary<string, DBObject>();
                    foreach (var field in this.GetType().GetFields())
                    {
                        var value = field.GetValue(this) as DBObject;
                        if (value != null)
                        {
                            this._value.Add(field.Name, value);
                        }
                    }
                }
                return _value;
            }
        }

        public DBClass() : base()
        {
            _field = new Dictionary<string, System.Reflection.FieldInfo>();
            _fieldType = new Dictionary<string, MemberType>();
            // ³õÊ¼»¯×Ö¶Î, ±ãÓÚ bind
            foreach (var field in this.GetType().GetFields())
            {
                if (field.FieldType.IsSubclassOf(typeof(DBObject)))
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

                _field.Add(field.Name, field);
            }
            base._value = this;
        }

        public string ToJson()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var item in this.Value)
            {
                dict[item.Key] = item.Value.Value;
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
                switch (this._fieldType[_key])
                {
                    case MemberType.DBObject:
                        DBObject dbObj = _field[_key].GetValue(this) as DBObject;
                        dbObj.SetVal(_val);
                        break;
                    default:
                        SetVal(_key, _value);
                        return false;
                }
            }
            return true;
        }
    }

    public class DBClassModel
    {
    }
}