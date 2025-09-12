using Rain.RTS.Core;
using UnityEngine;


/// <summary>
/// 单位血条
/// </summary>
public class HudUnitStateBarParam
{
    /// <summary>
    /// 单位ID
    /// </summary>
    public string unitID;
    /// <summary>
    /// 单位高度
    /// </summary>
    public float height;
    /// <summary>
    /// 单位位置
    /// </summary>
    public Vector3 pos;
    /// <summary>
    /// 相机引用
    /// </summary>
    public Camera camera;
    public RectTransform canvasRect;
    public HudUnitStateBarParam() { }
    public HudUnitStateBarParam(BaseBattleUnit _battleUnit, Camera _camera, RectTransform _canvasRect)
    {

        unitID = _battleUnit.Data.unitId;
        height = _battleUnit.Data.UnitConfig.Height;
        pos = _battleUnit.transform.position;
        camera = _camera;
        canvasRect = _canvasRect;
    }
}

