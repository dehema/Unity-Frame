using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
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
        MsgMgr.Ins.AddEventListener(MsgEvent.WorldMap_SelectTile, OnSelectTile, this);
        ui.selectTile.SetActive(false);
    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
    }

    /// <summary>
    /// 选择地图
    /// </summary>
    /// <param name="_worldPos"></param>
    public void OnSelectTile(params object[] obj)
    {
        if (obj[0] is not Vector2Int tilePos)
            return;
        TileData tileData = WorldMapMgr.Ins.GetTileData(tilePos);
        if (tileData == null)
            return;
        ui.selectTile_WorldMapSelectTileDialog.SetTileData(tileData);
        Debug.Log($"点击地图地块({tilePos.x}, {tilePos.y}), 地形类型为{tileData.Type}");
        ui.selectTile.SetActive(true);
    }
}

public class WorldMapHudViewParam : IViewParam
{

}
