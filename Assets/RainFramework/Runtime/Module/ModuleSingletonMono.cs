using F8Framework.Core;
using UnityEngine;
using static Reporter;

namespace F8Framework.Core
{
    public abstract class ModuleSingletonMono<T> : MonoBehaviour where T : class, IModule
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    LogF8.LogError($"ģ�� {typeof(T)} δ������");
                return _instance;
            }
        }

        private void Awake()
        {
            //��ֹ�������൥��
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}