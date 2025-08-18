using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.RTS.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class RTSHudView : BaseView
{
    CanvasScaler canvasScaler;
    ObjPool stateBarPool;
    Dictionary<string, HudUnitStateBar> barPools = new Dictionary<string, HudUnitStateBar>();

    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        canvasScaler = GetComponent<CanvasScaler>();
        stateBarPool = PoolMgr.Ins.CreatePool(ui.hudUnitStateBar);

        MsgMgr.Ins.AddEventListener(MsgEvent.RTSBattleUnitAdd, OnBattleUnitAdd, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.RTSBattleUnitRemove, OnBattleUnitRemove, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.RTSBattleUnitMove, OnBattleUnitMove, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.CameraZoomRatioChange, OnCameraZoomRatioChange, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.RTSUnitHPChange, OnRTSUnitHPChange, this);
    }

    public override void OnOpen(IViewParam _viewParams = null)
    {
        base.OnOpen(_viewParams);
    }

    public void OnBattleUnitAdd(params object[] obj)
    {
        BattleUnit battleUnit = obj[0] as BattleUnit;
        Camera camera = obj[1] as Camera;
        HudUnitStateBarParam param = new HudUnitStateBarParam(battleUnit, camera, rect);
        CreateStateBar(param);
    }

    public void OnBattleUnitRemove(params object[] obj)
    {
        BattleUnit battleUnit = obj[0] as BattleUnit;
        if (barPools.ContainsKey(battleUnit.Data.unitId))
        {
            stateBarPool.CollectOne(barPools[battleUnit.Data.unitId].gameObject);
        }
    }

    public void OnBattleUnitMove(params object[] obj)
    {
        BattleUnit battleUnit = obj[0] as BattleUnit;
        if (barPools.ContainsKey(battleUnit.Data.unitId))
        {
            HudUnitStateBar bar = barPools[battleUnit.Data.unitId];
            bar.UpdatePos(battleUnit.transform.position);
        }
    }

    public void OnCameraZoomRatioChange(params object[] obj)
    {
        float zoomRatio = (float)(obj[0] as float?);

        // 如果是固定像素大小模式，调整scaleFactor
        canvasScaler.scaleFactor = ((12 + zoomRatio) / 12);

        // 更新所有状态栏的位置和大小
        foreach (var pair in barPools)
        {
            if (pair.Value != null)
            {
                pair.Value.UpdatePos(pair.Value.lastPos);
            }
        }
    }

    private void OnRTSUnitHPChange(params object[] obj)
    {
        BattleUnit battleUnit = obj[0] as BattleUnit;
        float lastHp = (float)(obj[1] as float?);
        if (!barPools.ContainsKey(battleUnit.Data.unitId))
            return;
        HudUnitStateBar bar = barPools[battleUnit.Data.unitId];
        bar.ShowTween(battleUnit.Data.hp, lastHp, battleUnit.Data.maxHealth);
    }

    public void CreateStateBar(HudUnitStateBarParam param)
    {
        if (barPools.ContainsKey(param.unitID))
        {
            return;
        }
        HudUnitStateBar bar = stateBarPool.Get<HudUnitStateBar>(param);
        bar.UpdatePos(param.pos);
        barPools[param.unitID] = bar;
    }
}