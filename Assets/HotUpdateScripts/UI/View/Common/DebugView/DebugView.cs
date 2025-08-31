using System;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class DebugView : BaseView
{
    ObjPool btUIPool;

    public override void Init(IViewParam viewParams = null)
    {
        base.Init(viewParams);
        InitUIPool();
        ui.viewList_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.pixelRect.height / 2 - 140);
        ui.btStartGame_Button.SetButton(() => { ChangeScene(SceneID.MainScene); });
        ui.tdScene_Button.SetButton(() => { ChangeScene(SceneID.TD); });
        ui.rtsUnitTest_Button.SetButton(() => { ChangeScene(SceneID.RTSUnitDummyTest); });
    }

    private void ChangeScene(SceneID _sceneID, Action _cb = null)
    {
        SceneMgr.Ins.ChangeScene(_sceneID, _cb);
        Close();
    }

    private void InitUIPool()
    {
        btUIPool = PoolMgr.Ins.CreatePool(ui.btUIItem);
        foreach (var uiConfig in ConfigMgr.Ins.GetAllViewConfig())
        {
            string uiName = uiConfig.Key;
            GameObject item = btUIPool.Get();
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (uiName == string.Empty || uiName == string.Empty)
                {
                    UIMgr.Ins.OpenView(uiName);
                }
                else if (uiName == ViewName.DemoView)
                {
                    DemoViewParam viewParams = new DemoViewParam();
                    viewParams.number = 123;
                    viewParams.tips = "hello world";
                    viewParams.action = () =>
                    {
                        Console.WriteLine("trigger action");
                    };
                    viewParams.actionParams = (string text) =>
                    {
                        Console.WriteLine($"trigger action whih params:{text}");
                    };
                    UIMgr.Ins.OpenView(uiName, viewParams);
                }
                else if (uiName == ViewName.CountDownView)
                {
                    CountDownViewParam viewParams = new CountDownViewParam();
                    viewParams.countDown = 5;
                    viewParams.delay = 1;
                    viewParams.cb = () => { Debug.Log("倒计时结束"); };
                    UIMgr.Ins.OpenView(uiName, viewParams);
                }
                else
                {
                    UIMgr.Ins.OpenView(uiName);
                }
                if (ui.closeAfterOpenView_Toggle.isOn)
                {
                    Close();
                }
            });
            GetComponentInChildren<Text>("Text", item.transform).text = GetUINameInChinese(uiConfig);
        }
    }

    private string GetUINameInChinese(KeyValuePair<string, ViewConfig> _config)
    {
        string name = _config.Value.comment;
        if (string.IsNullOrEmpty(name))
        {
            name = _config.Key;
        }
        return name;
    }
}