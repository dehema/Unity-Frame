using DG.Tweening;
using Rain.UI;
using UnityEngine;


public partial class HudUnitStateBar : BasePoolItem
{
    float rectWidth;
    float rectHeight;
    HudUnitStateBarParam param;
    float cameraOffsetY = 0;
    public Vector3 lastPos;

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

        // 血条尺寸动画
        ui.greenHp_Rect.DoWidth(hpNow * rectWidth, 0.3f);

        // 星际争霸2风格的血条颜色变化
        Color targetColor = GetHealthBarColor(hpNow);
        ui.greenHp_Image.DOColor(targetColor, 0.3f);
    }

    /// <summary>
    /// 根据血量百分比获取血条颜色（星际争霸2风格）
    /// </summary>
    /// <param name="hpPercent">血量百分比 (0-1)</param>
    /// <returns>对应的颜色</returns>
    private Color GetHealthBarColor(float hpPercent)
    {
        Color color;
        if (hpPercent > 0.3f)
        {
            // 线性映射公式：(当前值 - 原最小值) / (原最大值 - 原最小值) * (目标最大值 - 目标最小值) + 目标最小值
            color = new Color(1 - (hpPercent - 0.3f) / 0.7f, hpPercent, 0);
        }
        else
        {
            color = new Color(0.7f + hpPercent, 0, 0);
        }
        return color;
    }

    public void UpdatePos(Vector3 _pos)
    {
        lastPos = _pos;
        // 将3D世界坐标转换为屏幕UI坐标
        Vector3 screenPos = param.camera.WorldToScreenPoint(_pos + new Vector3(0, cameraOffsetY, 0));
        screenPos += new Vector3(0, 20, 0);
        //
        // UI坐标需转换为RectTransform的局部坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(param.canvasRect, screenPos, UIMgr.Ins.Camera, out Vector2 localPos))
        {
            rect.localPosition = localPos;
        }
    }
}