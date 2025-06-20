using System;
using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoStorage : MonoBehaviour
    {
        public class ClassInfo
        {
            public string Initial = "initial";
        }

        public ClassInfo Info = new ClassInfo();

        void Start()
        {
            // 设置密钥，自动加密所有数据（编辑器下不加密）
            RA.Storage.SetEncrypt(new Util.OptimizedAES("AES_Key", "AES_IV"));
            
            // 设置UserId，用户私有的Key
            RA.Storage.SetUser("12345");
            
            // 使用基础类型
            RA.Storage.SetString("Key1", "value", user: true);
            RA.Storage.GetString("Key1", "", user: true);
            
            RA.Storage.SetInt("Key2", 1);
            RA.Storage.GetInt("Key2");
            
            RA.Storage.SetBool("Key3", true);
            RA.Storage.GetBool("Key3");
            
            RA.Storage.SetFloat("Key4", 1.1f);
            RA.Storage.GetFloat("Key4");

            // 使用数据类
            RA.Storage.SetObject("Key5", Info);
            ClassInfo info2 = RA.Storage.GetObject<ClassInfo>("Key5");
            RLog.Log(info2.Initial);

            RA.Storage.Save();
            RA.Storage.Clear();
        }
    }
}
