using Rain.Core;
using UnityEngine;

namespace Rain.Core
{
    public abstract class ModuleSingletonMono<T> : MonoBehaviour where T : class, IModule
    {
        private static T _instance;
        public static T Ins
        {
            get
            {
                if (_instance == null)
                    Debug.LogError($"模块 {typeof(T)} 未创建。");
                return _instance;
            }
        }

        private void Awake()
        {
            //防止创建多余单例
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