using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class HudUnitStateBar : BasePoolItem
{
    float rectWidth;
    float rectHeight;
    RectTransform rect;
    public override void OnCreate(params object[] _params)
    {
        base.OnCreate(_params);
        rect = GetComponent<RectTransform>();
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



    public void Update()
    {
        //��Ļ����
        //ģ�������ڽ��� + ��� + 0.2ƫ��ֵ
        //Vector3 sPos = BattleSceneMgr.Ins.battleFieldCamera.WorldToScreenPoint(unitBase.transform.position + new Vector3(0, unitBase.unitConfig.height + 0.2f));
        ////Debug.LogError($"X:{sPos.y}  Y:{sPos.y}");
        ////
        //Vector2 uiPos = new Vector3(sPos.x - Screen.width / 2, sPos.y - Screen.height / 2);
        //GetComponent<RectTransform>().anchoredPosition = uiPos;
    }
}
