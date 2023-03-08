using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class TopView : BaseView
{
    //data
    bool isShow = true;
    const float showDuration = 0.5f;

    public override void Init(params object[] _params)
    {
        base.Init(_params);
        //top
        btGold_Button.onClick.AddListener(() =>
        {
            SOHOShopManager.Ins.ShowGoldAmazonRedeemPanel();
        });
        btCash_Button.onClick.AddListener(() =>
        {
            SOHOShopManager.Ins.ShowRedeemPanel();
        });
        btCash.gameObject.SetActive(!CommonUtil.IsApple());
        btAmazon_Button.onClick.AddListener(() =>
        {
            SOHOShopManager.Ins.ShowGoldAmazonRedeemPanel();
        });
        btAmazon.gameObject.SetActive(!CommonUtil.IsApple());
        //tween
        InitBinding();
    }

    void InitBinding()
    {
        DataMgr.Ins.playerData.gold.Bind((m) =>
        {
            goldNum_Text.text = m.value.ToString();
            DataMgr.Ins.SavePlayerData();
        });
        DataMgr.Ins.playerData.cash.Bind((m) =>
        {
            cashNum_Text.text = m.value.ToString();
            DataMgr.Ins.SavePlayerData();
        });
        DataMgr.Ins.playerData.amazon.Bind((m) =>
        {
            amazonNum_Text.text = m.value.ToString();
            DataMgr.Ins.SavePlayerData();
        });
    }

    public void SetShow(bool _isShow, Action _closeCB = null)
    {
        isShow = _isShow;
        if (isShow)
        {
            top_Rect.DOAnchorPos(Vector2.zero, showDuration);
        }
        else
        {
            top_Rect.DOAnchorPos(new Vector2(0, top_Rect.rect.height), showDuration).onComplete = () =>
            {
                _closeCB?.Invoke();
            };
        }
    }
}
