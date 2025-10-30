using System;
using System.Collections;
using System.Collections.Generic;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 世界地图HUD
/// </summary>
public partial class WorldMapHudView : BaseView
{
    WorldMapHudViewParam param;
    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        param = _viewParam as WorldMapHudViewParam;
    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
    }
}

public class WorldMapHudViewParam : IViewParam
{

}
