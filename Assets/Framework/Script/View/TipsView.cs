using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public partial class TipsView : BaseView
{
    ObjPool tipsPool;
    List<GameObject> items = new List<GameObject>();
    /// <summary>
    /// item高度
    /// </summary>
    float itemHeight = 0;
    const float tweenMoveDuration = 0.5f;
    const float tweenFadeDuration = 0.5f;
    const float showDuration = 2f;
    Color initTxtColor = Color.clear;
    Color initBgColor = Color.clear;
    public override void Init(params object[] _params)
    {
        base.Init(_params);
        tipsPool = PoolMgr.Ins.CreatePool(templeteTips);
        itemHeight = templeteTips.GetComponent<RectTransform>().rect.height;
    }

    public override void OnOpen(params object[] _params)
    {
        base.OnOpen(_params);
        commonFloat.SetActive(false);
    }

    /// <summary>
    /// 重新对齐
    /// </summary>
    void ReAlign(int _offset, Action _cb = null)
    {
        if (items.Count == 0)
        {
            _cb?.Invoke();
            return;
        }
        for (int i = 0; i < items.Count; i++)
        {
            Vector2 targetPos = new Vector2(0, itemHeight * (i + _offset));
            items[i].GetComponent<RectTransform>().DOKill();
            items[i].GetComponent<RectTransform>().DOAnchorPos(targetPos, tweenMoveDuration);
        }
        if (_cb != null)
        {
            Timer.Ins.SetTimeOut(ed => { _cb(); }, tweenMoveDuration);
        }
    }

    public void Tips(string _str)
    {
        Func<GameObject> CreateItem = () =>
        {
            GameObject item = tipsPool.Get();
            items.Insert(0, item);
            ReAlign(0);
            item.GetComponent<RectTransform>().DOKill();
            Image imgBg = GetComponentInChildren<Image>("bg", item.transform);
            if (initBgColor == Color.clear)
            {
                initBgColor = imgBg.color;
            }
            else
            {
                imgBg.color = initBgColor;
            }
            Text txt = GetComponentInChildren<Text>("txt", item.transform);
            if (initTxtColor == Color.clear)
            {
                initTxtColor = txt.color;
            }
            else
            {
                txt.color = initTxtColor;
            }
            txt.text = _str;
            item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return item;
        };
        GameObject item = CreateItem();
        Timer.Ins.SetTimeOut(() =>
        {
            Image bg = GetComponentInChildren<Image>("bg", item.transform);
            bg.DOFade(0, tweenFadeDuration);
            Text txt = GetComponentInChildren<Text>("txt", item.transform);
            txt.DOFade(0, tweenFadeDuration).onComplete = () =>
            {
                tipsPool.CollectOne(item);
                items.RemoveAt(items.Count - 1);
            };
        }, showDuration);
    }

    /// <summary>
    /// 根据浮动框选择位置
    /// </summary>
    /// <param name="_rect"></param>
    /// <returns></returns>
    private Vector2 GetFloatTipsPos(RectTransform _rect)
    {
        int xOffset = (Input.mousePosition.x < Screen.width / 2) ? 1 : -1;
        int yOffset = (Input.mousePosition.y < Screen.height / 2) ? 1 : -1;
        Vector2 offset = new Vector2(_rect.rect.width / 2 * xOffset, _rect.rect.height / 2 * yOffset);
        return offset + new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
    }

    /// <summary>
    /// 显示通用浮动信息
    /// </summary>
    public void ShowCommonFloatTips(string _tips)
    {
        if (!commonFloat.activeSelf)
        {
            txtCommonFloat_Text.text = _tips;
            float width = Utility.GetTextWidth(txtCommonFloat_Text, _tips);
            float height = Utility.GetTextHeight(txtCommonFloat_Text, _tips);
            txtCommonFloat_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            txtCommonFloat_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            txtCommonFloat_Rect.anchoredPosition = Vector2.zero;
            commonFloat_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + 50);
            commonFloat_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 50);
            commonFloat.SetActive(true);
        }
        commonFloat_Rect.anchoredPosition = GetFloatTipsPos(commonFloat_Rect);
    }

    /// <summary>
    /// 隐藏通用浮动信息
    /// </summary>
    public void HideCommonFloatTips()
    {
        commonFloat.SetActive(false);
    }
}
