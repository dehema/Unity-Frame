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
                    Debug.LogError($"ģ�� {typeof(T)} δ������");
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