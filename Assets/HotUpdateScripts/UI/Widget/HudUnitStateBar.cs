using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class HudUnitStateBar : BasePoolItem
{
    float rectWidth;
    float rectHeight;
    Camera unitCamera;

    /// <summary>
    /// ��λ�߶�
    /// </summary>
    float unitHeight = 0;
    public override void OnCreate(params object[] _params)
    {
        base.OnCreate(_params);
        HudUnitStateBarParam param = _params[0] as HudUnitStateBarParam;
        unitCamera = param.camera;
        unitHeight = param.unitHeight;
        rectWidth = rect.rect.width;
        rectHeight = rect.rect.height;
    }

    /// <summary>
    /// ����ֵ����
    /// </summary>
    /// <param name="_baseUnit">��ע�ĵ�λ</param>
    /// <param name="_realHp">�������������ֵ</param>
    /// <param name="_hpJust">������ǰ������ֵ</param>
    /// <param name="_max">�������ֵ</param>
    public void ShowTween(float _realHp, float _hpJust, float _max)
    {
        float hpJust = _hpJust / _max;
        float hpNow = _realHp / _max;
        ui.redHp_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hpJust * rectWidth);
        ui.greenHp_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, hpJust * rectWidth);
        ui.greenHp_Rect.DOSizeDelta(new Vector2(hpNow * rectWidth, rectHeight), 0.5f);
    }



    public void UpdatePos(Vector3 _pos)
    {
        //��Ļ����
        //ģ�������ڽ��� + ��� + 0.2ƫ��ֵ
        Vector3 sPos = unitCamera.WorldToScreenPoint(_pos + new Vector3(0, unitHeight + 0.2f));
        //Debug.LogError($"X:{sPos.y}  Y:{sPos.y}");
        //
        Vector2 uiPos = new Vector3(sPos.x - Screen.width / 2, sPos.y - Screen.height / 2);
        GetComponent<RectTransform>().anchoredPosition = uiPos;
    }
}

public class HudUnitStateBarParam
{
    public Camera camera;
    public float unitHeight;
}
