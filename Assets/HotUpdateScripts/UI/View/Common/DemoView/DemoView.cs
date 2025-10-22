using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public partial class DemoView : BaseView
{
    DemoViewParam _viewParam;

    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);

        //视图参数
        _viewParam = viewParam as DemoViewParam;
        Debug.Log(_viewParam.number);
        Debug.Log(_viewParam.tips);
        _viewParam?.action();
        _viewParam?.actionParams("this is a param!");

        //注册按钮事件
        //ui.btClose_Button.
        ui.btClose_Button.SetButton(Close);
    }

    public override void OnOpen(IViewParam viewParam = null)
    {
        base.OnOpen(viewParam);
        Debug.Log("UI打开");

        //同步加载
        Sprite sprite = AssetMgr.Ins.Load<Sprite>("Btn_Rectangle00_n_Blue");
        ui.imgBuild1_Image.sprite = sprite;

        //异步加载
        BaseLoader baseLoader = AssetMgr.Ins.LoadAsync<Sprite>("Btn_Rectangle00_n_Gray", (OnAssetObject) =>
        {
            ui.imgBuild2_Image.sprite = OnAssetObject;
        });

        //计时器
        TimerMgr.Ins.AddTimer(this, step: 1, field: 0, onSecond: () =>
        {
            Debug.Log("onSecond");
        }, onComplete: () =>
        {
            Debug.Log("onComplete");
        });

        //图集
        SpriteAtlas atlas = AssetMgr.Ins.Load<SpriteAtlas>("atlas_common");
        Sprite targetSprite = atlas?.GetSprite("Btn_Rectangle00_n_Green");
        ui.imgBuild3_Image.sprite = targetSprite;
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
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        AudioMgr.Ins.PlayAudioEffect("Electronic high shot");
        //    }
    }

    public class DemoViewParam : IViewParam
    {
        public int number;
        public string tips;
        public Action action;
        public Action<string> actionParams;
    }
}