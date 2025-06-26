using Rain.Core;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoModuleCenter : MonoBehaviour
    {
        void Start()
        {
            //初始化模块中心
            ModuleCenter.Initialize(this);

            // 创建模块，（参数可选，优先级越小越早轮询）
            int priority = 100;
            ModuleCenter.CreateModule<TimerMgr>(priority);

            // 通过ModuleCenter调用模块方法
            ModuleCenter.GetModule<TimerMgr>().GetServerTime();

            // 通过获取实例调用模块方法
            TimerMgr.Ins.GetServerTime();
        }
    }

    [UpdateRefresh]
    [LateUpdateRefresh]
    [FixedUpdateRefresh]
    public class DemoModuleCenterClass : ModuleSingleton<DemoModuleCenterClass>, IModule
    {
        public void OnInit(object createParam)
        {
            // 模块创建初始化
        }

        public void OnUpdate()
        {
            // 模块Update
        }

        public void OnLateUpdate()
        {
            // 模块LateUpdate
        }

        public void OnFixedUpdate()
        {
            // 模块FixedUpdate
        }

        public void OnTermination()
        {
            // 模块销毁
            base.Destroy();
        }
    }
}
