using DG.Tweening;
using Rain.RTS.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public partial class HudUnitStateBar : BasePoolItem
{
    float rectWidth;
    float rectHeight;
    HudUnitStateBarParam param;
    float cameraOffsetY = 0;
    public Vector3 lastPos;
    float lastHp = 0;

    public override void OnCreate(params object[] _params)
    {

        base.OnCreate(_params);
        param = _params[0] as HudUnitStateBarParam;
        rectWidth = rect.rect.width;
        rectHeight = rect.rect.height;
        cameraOffsetY = Mathf.Cos(param.camera.transform.localEulerAngles.z) * param.height;
    }

    /// <summary>
    /// 生命值动画
    /// </summary>
    /// <param name="_realHp">被攻击后的生命值</param>
    /// <param name="_lastHp">被攻击前的生命值</param>
    /// <param name="_max">生命最大值</param>
    public void ShowTween(float _realHp, float _lastHp, float _max)
    {
        float lastHp = _lastHp / _max;
        float hpNow = _realHp / _max;
        //ui.redHp_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lastHp * rectWidth);
        //ui.greenHp_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lastHp * rectWidth);
        ui.greenHp_Rect.DOSizeDelta(new Vector2(hpNow * rectWidth, rectHeight), 0.5f);
    }

    public void UpdatePos(Vector3 _pos)
    {
        lastPos = _pos;
        // 将3D世界坐标转换为屏幕UI坐标
        Vector3 screenPos = param.camera.WorldToScreenPoint(_pos + new Vector3(0, cameraOffsetY, 0));
        screenPos += new Vector3(0, 20, 0);
        //
        // UI坐标需转换为RectTransform的局部坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(param.canvasRect, screenPos, Camera.main, out Vector2 localPos))
        {
            rect.localPosition = localPos;
        }
    }
}