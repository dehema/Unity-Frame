using UnityEngine;

namespace Rain.UI
{
    public class DelegateComponent : MonoBehaviour
    {
        public ViewParam ViewParam;
        // 窗口添加
        public void Add()
        {
            // 触发窗口组件上添加到父节点后的事件
            ViewParam.BaseView?.Added(ViewParam.UIid, ViewParam.Guid, ViewParam.Params);
            
            if (ViewParam.Callbacks != null && ViewParam.Callbacks.OnAdded != null)
            {
                ViewParam.Callbacks.OnAdded(ViewParam.Params, ViewParam.UIid);
            }
        }

        // 删除节点，该方法只能调用一次，将会触发OnBeforeRemoved回调
        public void Remove(bool isDestroy)
        {
            if (ViewParam.Valid)
            {
                // 触发窗口组件上移除之前的事件
                ViewParam.BaseView?.BeforeRemove();

                // 通知外部对象窗口组件上移除之前的事件（关闭窗口前的关闭动画处理）
                if (ViewParam.Callbacks != null && ViewParam.Callbacks.OnBeforeRemove != null)
                {
                    ViewParam.Callbacks.OnBeforeRemove();
                    Removed(ViewParam, isDestroy);
                }
                else
                {
                    Removed(ViewParam, isDestroy);
                }
            }
        }

        // 窗口组件中触发移除事件与释放窗口对象
        private void Removed(ViewParam viewParam, bool isDestroy)
        {
            viewParam.Valid = false;
            
            if (viewParam.Callbacks != null && viewParam.Callbacks.OnRemoved != null)
            {
                viewParam.Callbacks.OnRemoved(viewParam.Params, viewParam.UIid);
            }
            
            ViewParam?.BaseView?.Removed();
            
            if (isDestroy)
            {
                Destroy(gameObject);
            }
            else
            {
                if (gameObject.activeSelf)
                {
                    gameObject.SetActive(false);
                }
                gameObject.transform.SetParent(null, false);
            }
        }

        private void OnDestroy()
        {
            ViewParam = null;
        }
    }
}
