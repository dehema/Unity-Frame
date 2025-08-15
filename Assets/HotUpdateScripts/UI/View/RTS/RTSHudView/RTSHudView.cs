using System.Collections;
using System.Collections.Generic;
using Rain.UI;
using UnityEngine;

public partial class RTSHudView : BaseView
{
    ObjPool stateBarPool;
    Dictionary<int, BasePoolItem> pools = new Dictionary<int, BasePoolItem>();

    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        stateBarPool = PoolMgr.Ins.CreatePool(ui.hudUnitStateBar);
    }

    public override void OnOpen(IViewParam _viewParams = null)
    {
        base.OnOpen(_viewParams);
    }

    public void CreateStateBar(int unitID, float height, Vector3 pos)
    {
        if (pools.ContainsKey(unitID))
        {
            return;
        }
        HudUnitStateBarParam param = new HudUnitStateBarParam();
        param.camera = RTSUnitTestSceneMgr.Ins.mainCamera;
        param.unitHeight = height;
        stateBarPool.Get<HudUnitStateBar>(param);
    }

    public void RemoveStateBar(int unitID)
    {
    }
}
