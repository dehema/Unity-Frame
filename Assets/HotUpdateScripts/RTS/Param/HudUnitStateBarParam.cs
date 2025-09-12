using Rain.RTS.Core;
using UnityEngine;


/// <summary>
/// ��λѪ��
/// </summary>
public class HudUnitStateBarParam
{
    /// <summary>
    /// ��λID
    /// </summary>
    public string unitID;
    /// <summary>
    /// ��λ�߶�
    /// </summary>
    public float height;
    /// <summary>
    /// ��λλ��
    /// </summary>
    public Vector3 pos;
    /// <summary>
    /// �������
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

