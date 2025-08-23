using UnityEngine;

namespace Rain.Core
{
    /// <summary>
    /// 对象池句柄销毁监听器 - 当附加此组件的GameObject被销毁时，会通知PoolMgr清理相关的对象池
    /// </summary>
    public class PoolHandleDestroyer : MonoBehaviour
    {
        private PoolMgr poolMgr;

        /// <summary>
        /// 初始化监听器
        /// </summary>
        /// <param name="mgr">对象池管理器实例</param>
        public void Initialize(PoolMgr mgr)
        {
            poolMgr = mgr;
        }

        /// <summary>
        /// 当GameObject被销毁时调用
        /// </summary>
        private void OnDestroy()
        {
            if (poolMgr != null)
            {
                poolMgr.OnHandleDestroyed(gameObject);
            }
        }
    }
}
