using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;

/// <summary>
/// 世界地图选择地块弹窗
/// </summary>
public partial class WorldMapSelectTileDialog : BaseUI
{
    TileData tileData;

    private void Awake()
    {
        ui.btClose_Button.SetButton(() => { gameObject.SetActive(false); });
    }

    public void SetTileData(TileData _tileData)
    {
        tileData = _tileData;
        ui.pos_Text.text = $"({tileData.Pos.x}，{tileData.Pos.y})";
    }
}
