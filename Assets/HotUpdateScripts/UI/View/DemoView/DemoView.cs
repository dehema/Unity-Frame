using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;
using static SettingField;

public partial class DemoView : BaseView
{
    DemoViewParams _viewParams;

    public override void Init(IViewParams viewParams = null)
    {
        base.Init(viewParams);

        //视图参数
        _viewParams = viewParams as DemoViewParams;
        Debug.Log(_viewParams.number);
        Debug.Log(_viewParams.tips);
        _viewParams?.action();
        _viewParams?.actionParams("this is a param!");

        //注册按钮事件
        //ui.btClose_Button.
        ui.btClose_Button.SetButton(() => { UIMgr.Ins.CloseView<DemoView>(); });

        //同步加载
        Sprite sprite = AssetMgr.Ins.Load<Sprite>("beastmen_centigors1");
        ui.imgBuild1_Image.sprite = sprite;

        //异步加载
        BaseLoader baseLoader = AssetMgr.Ins.LoadAsync<Sprite>("beastmen_centigors2", (OnAssetObject) =>
        {
            ui.imgBuild2_Image.sprite = OnAssetObject;
        });
    }

    public override void OnOpen(IViewParams viewParams = null)
    {
        base.OnOpen(viewParams);
        Debug.Log("UI打开");
    }

    /// <summary>
    /// 关闭事件 回调在UIMgr里实现
    /// </summary>
    /// <param name="_cb"></param>
    public override void OnClose(Action _cb)
    {
        Debug.Log("UI关闭");
        base.OnClose(_cb);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AudioMgr.Ins.PlayAudioEffect("Electronic high shot");


        }
    }
}


public class DemoViewParams : IViewParams
{
    public int number;
    public string tips;
    public Action action;
    public Action<string> actionParams;
}
