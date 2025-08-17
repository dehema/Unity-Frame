using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.RTS.Core;
using Rain.UI;
using UnityEngine;

public partial class RTSHudView : BaseView
{
    ObjPool stateBarPool;
    Dictionary<string, HudUnitStateBar> pools = new Dictionary<string, HudUnitStateBar>();

    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        stateBarPool = PoolMgr.Ins.CreatePool(ui.hudUnitStateBar);

        MsgMgr.Ins.AddEventListener(MsgEvent.RTSBattleUnitAdd, OnBattleUnitAdd, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.RTSBattleUnitRemove, OnBattleUnitRemove, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.RTSBattleUnitMove, OnBattleUnitMove, this);
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
        if (pools.ContainsKey(battleUnit.Data.unitId))
        {
            stateBarPool.CollectOne(pools[battleUnit.Data.unitId].gameObject);
        }
    }

    public void OnBattleUnitMove(params object[] obj)
    {
        BattleUnit battleUnit = obj[0] as BattleUnit;
        if (pools.ContainsKey(battleUnit.Data.unitId))
        {
            HudUnitStateBar bar = pools[battleUnit.Data.unitId];
            bar.UpdatePos(battleUnit.transform.position);
        }
    }

    public void CreateStateBar(HudUnitStateBarParam param)
    {
        if (pools.ContainsKey(param.unitID))
        {
            return;
        }
        HudUnitStateBar bar = stateBarPool.Get<HudUnitStateBar>(param);
        bar.UpdatePos(param.pos);
        pools[param.unitID] = bar;
    }
}