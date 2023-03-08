using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 现状图分段组件
/// </summary>
public class SliderSubsection : BaseUI
{
    const float tweenDuration = 0.8f;
    public Image imgSlider;
    public float value, min, max;
    List<float> subsectionTargets;

    public void Init(float _value, float _min, float _max)
    {
        value = _value;
        min = _min;
        max = _max;
        RefreshSlider(false);
    }


    public void SetVal(float _val, bool _isTween = true, Action _cb = null)
    {
        if (_val != value)
        {
            value = _val;
        }
        RefreshSlider(_isTween, _cb);
    }

    public void RefreshSlider(bool _isTween = true, Action _cb = null)
    {
        float val = GetSliderVal();
        if (imgSlider)
        {
            if (_isTween)
            {
                imgSlider.DOFillAmount(val, tweenDuration).SetEase(Ease.Linear).onComplete = () =>
                {
                    if (_cb != null)
                    {
                        _cb();
                    }
                };
            }
            else
            {
                imgSlider.fillAmount = val;
            }
        }
    }

    /// <summary>
    /// 获得分段位置 世界坐标
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetSubsectionPos(List<float> _subsectionTargets)
    {
        if (max == 0)
        {
            max = _subsectionTargets[_subsectionTargets.Count - 1];
        }
        subsectionTargets = _subsectionTargets;
        int _subsectionNum = _subsectionTargets.Count;
        //先计算分段之后的目标值 比如第一段UI宽度之占了三分之一 但目标值是最小的只有最大值的十分之一
        List<Vector2> anchoredPos = new List<Vector2>();
        RectTransform rect = imgSlider.rectTransform;
        float _subsectionWidth = rect.rect.width / _subsectionNum;
        for (int i = 1; i <= _subsectionNum; i++)
        {
            anchoredPos.Add(new Vector2(_subsectionWidth * i - rect.rect.width / 2, 0));
        }
        List<Vector3> worldPos = new List<Vector3>();
        foreach (var item in anchoredPos)
        {
            worldPos.Add(transform.position + AnchorPosToWorld(item));
        }
        return worldPos;
    }

    public float GetSliderVal()
    {
        if (subsectionTargets == null)
        {
            return (value - min) / (max - min);
        }
        float currVal = value;
        float val = 0;
        for (int i = 0; i < subsectionTargets.Count; i++)
        {
            float target = subsectionTargets[i];
            float lastTarget = 0;
            if (i > 0)
            {
                lastTarget = subsectionTargets[i - 1];
            }
            if (currVal >= target)
            {
                val += 1f / subsectionTargets.Count;
                currVal -= (target - (i == 0 ? 0 : subsectionTargets[i - 1]));
                if (currVal == 0)
                {
                    break;
                }
            }
            else
            {
                val += 1f / subsectionTargets.Count * (currVal / (target - lastTarget));
                break;
            }
        }
        return val;
    }
}
