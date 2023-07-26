using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ExampleView : ExampleViewParent
{
    private void Awake()
    {
        Debug.Log("Awake");
    }

    private void Start()
    {
        Debug.Log("Start");
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    public override void Init(params object[] _params)
    {
        base.Init(_params);
        Debug.Log("Init");
        btClose_Button.SetButton(Close);
        btButton_Button.SetButton(() => { Debug.Log("debug.log click"); }, AudioSound.Sound_UIButton);
        btAddGold_Button.SetButton(() => { DataMgr.Ins.playerData.gold.Value++; });
        btUnBindAllDataBind_Button.SetButton(() => { UnBindAllDataBind(); });
    }

    public override void OnOpen(params object[] _params)
    {
        base.OnOpen();
        Debug.Log("OnOpen");
        SetTimeOut(th => { Debug.LogError("This is a view timer!"); }, 2);
        Timer.Ins.SetTimeOut(() => { Debug.LogError("This is a common timer!"); }, 2);
        DataBind(DataMgr.Ins.playerData.gold, dm => { goldNum_Text.text = dm.value.ToString(); });
        Utility.Dump(Utility.GetSetting("key1"));
        Utility.Dump(Utility.GetSetting<int>("key2"));
        Utility.Dump(Utility.GetSetting<int[]>("key3"));
        Utility.Dump(Utility.GetSetting<Dictionary<int, int>>("key4"));
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        Timer.Ins.RemoveTimerGroup(Timer.defaultTimerGroup);
    }
}
