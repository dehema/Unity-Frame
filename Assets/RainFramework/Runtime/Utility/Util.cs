using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Rain.Core
{

    public static partial class Util
    {
        #region Log
        // 唯一公开方法：自动处理各种类型
        public static void Log(object obj) => LogInternal(obj, "", 0);

        // 内部递归方法
        private static void LogInternal(object obj, string prefix = "", int depth = 0)
        {
            if (depth > 3) { Debug.Log($"{prefix}..."); return; } // 限制递归深度
            if (obj == null) { Debug.Log($"{prefix}null"); return; }

            var type = obj.GetType();

            // 值类型和字符串
            if (type.IsPrimitive || type == typeof(string))
            {
                Debug.Log($"{prefix}{obj}");
                return;
            }

            // Unity对象
            if (obj is UnityEngine.Object unityObj)
            {
                Debug.Log($"{prefix}{type.Name}: {unityObj.name}");
                LogFields(obj, prefix, depth);
                return;
            }

            // 集合类型
            if (obj is IEnumerable collection)
            {
                LogCollection(collection, prefix, depth);
                return;
            }

            // 其他对象
            Debug.Log($"{prefix}{type.Name}");
            LogFields(obj, prefix, depth);
        }

        // 打印对象字段
        private static void LogFields(object obj, string prefix, int depth)
        {
            foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                LogInternal(field.GetValue(obj), $"{prefix}{field.Name}: ", depth + 1);
            }
        }

        // 打印集合
        private static void LogCollection(IEnumerable collection, string prefix, int depth)
        {
            int index = 0;
            foreach (var item in collection)
            {
                LogInternal(item, $"{prefix}[{index++}]: ", depth + 1);
            }
        }
        #endregion
    }
}
