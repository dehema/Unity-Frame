using Rain.Core;
using UnityEngine;

namespace Rain.Tests
{
    // 红点示例代码
    public class DemoUIRedDot : MonoBehaviour
    {
        // 手动添加
        public const string UIMain = "UIMain";
        public const string UIMain_Intensify = "UIMain_Intensify";
        public const string UIMain_AutoIntensify = "UIMain_AutoIntensify";
        public const string UIMain_AutoIntensify2 = "UIMain_AutoIntensify2";
        public const string UIMain_AutoIntensify3 = "UIMain_AutoIntensify3";
        
        // 初始化
        public void Init()
        {
            // 手动添加
            UIRedDot.Ins.AddRedDotCfg(UIMain);
            UIRedDot.Ins.AddRedDotCfg(UIMain_Intensify, UIMain);
            UIRedDot.Ins.AddRedDotCfg(UIMain_AutoIntensify, UIMain);
            UIRedDot.Ins.AddRedDotCfg(UIMain_AutoIntensify2, UIMain_AutoIntensify);
            UIRedDot.Ins.AddRedDotCfg(UIMain_AutoIntensify3, UIMain_AutoIntensify2);

            UIRedDot.Ins.Init();
        }

        private void Start()
        {
            // 改变布尔状态
            UIRedDot.Ins.Change(DemoUIRedDot.UIMain_AutoIntensify2, true);
            RLog.Log(UIRedDot.Ins.GetState(DemoUIRedDot.UIMain_AutoIntensify3));
            
            // 改变数量状态
            UIRedDot.Ins.Change(DemoUIRedDot.UIMain_AutoIntensify2, 15);
            RLog.Log(UIRedDot.Ins.GetCount(DemoUIRedDot.UIMain_AutoIntensify3));
            
            // 改变文本状态
            UIRedDot.Ins.Change(DemoUIRedDot.UIMain_AutoIntensify2, "空闲");
            RLog.Log(UIRedDot.Ins.GetTextState(DemoUIRedDot.UIMain_AutoIntensify3));
            
            // 绑定，解绑GameObject
            UIRedDot.Ins.Binding(DemoUIRedDot.UIMain_AutoIntensify2, this.gameObject);
            UIRedDot.Ins.UnBinding(DemoUIRedDot.UIMain_AutoIntensify2);
            UIRedDot.Ins.UnBinding(DemoUIRedDot.UIMain_AutoIntensify2, this.gameObject);
            
            // 清空所有红点状态
            UIRedDot.Ins.RemoveAllRed();
        }
    }
}
