using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public static class DOTweenExtensions
{
    public static void DoWidth(this RectTransform @this, float endValue, float duration, bool snapping = false)
    {
        @this.DOSizeDelta(new Vector2(endValue, @this.rect.height), duration, snapping);
    }

    public static void DoHeight(this RectTransform @this, float endValue, float duration, bool snapping = false)
    {
        @this.DOSizeDelta(new Vector2(@this.rect.width, endValue), duration, snapping);
    }
}
