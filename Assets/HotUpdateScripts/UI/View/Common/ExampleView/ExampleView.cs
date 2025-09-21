using System;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
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

    public override void Init(IViewParam viewParams = null)
    {
        base.Init(viewParams);
        Debug.Log("Init");
        //btClose_Button.SetButton(Close);
        //btButton_Button.SetButton(() => { Debug.Log("debug.log click"); });
        //btAddGold_Button.SetButton(() => { DataMgr.Ins.playerData.gold.Value++; });
        //btUnBindAllDataBind_Button.SetButton(() => { UnBindAllDataBind(); });
    }

    public override void OnOpen(IViewParam viewParams = null)
    {
        base.OnOpen();
        Debug.Log("OnOpen");
        //SetTimeOut(th => { Debug.LogError("This is a view timer!"); }, 2);
        //Timer.Ins.SetTimeOut(() => { Debug.LogError("This is a common timer!"); }, 2);
        //DataBind(DataMgr.Ins.playerData.gold, dm => { goldNum_Text.text = dm.value.ToString(); });
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        //Timer.Ins.RemoveTimerGroup(Timer.defaultTimerGroup);
    }
}
