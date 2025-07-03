using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class CardMatchGameView : BaseView
{
    ObjPool cardPool;
    string[] imageArray = { "哥布林战士", "骷髅战士", "皮卡丘", "人类士兵", "史莱姆_冰", "史莱姆_草", "史莱姆_火", "史莱姆_雷" };
    CardMatchCardItem cardItem1;
    CardMatchCardItem cardItem2;
    List<CardMatchCardItem> cardList = new List<CardMatchCardItem>();
    public override void Init(IViewParams viewParams = null)
    {
        base.Init(viewParams);
        cardPool = PoolMgr.Ins.CreatePool(ui.cardMatchCardItem);
    }

    public override void OnOpen(IViewParams viewParams = null)
    {
        base.OnOpen(viewParams);
        SetBlockVisible(false);
        List<string> strings = new List<string>();
        foreach (var imagePath in imageArray)
        {
            for (int i = 1; i <= 2; i++)
            {
                strings.Add(imagePath);
            }
        }
        strings.Shuffle();
        foreach (var imagePath in strings)
        {
            CardMatchCardItem item = cardPool.Get<CardMatchCardItem>(imagePath);
            cardList.Add(item);
            item.onCardOpen += OnCardOpen;
        }
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        cardPool.CollectAll();
    }

    public void OnCardOpen(CardMatchCardItem cardItem)
    {

        if (cardItem1 == null)
        {
            cardItem1 = cardItem;
        }
        else
        {
            cardItem2 = cardItem;
            CheckCard();
        }
    }

    public void CheckCard()
    {
        SetBlockVisible(true);
        if (cardItem1.ImgPath == cardItem2.ImgPath)
        {
            cardItem1.UnLockCard();
            cardItem2.UnLockCard();
            ClearCardData();
        }
        else
        {
            TimerMgr.Ins.AddTimer(this, delay: CardMatchCardItem.cardFlipDuation * 2, onComplete: () =>
            {
                ClearCardData();
            });
        }
        bool unlockAll = true;
        foreach (var item in cardList)
        {
            if (!item.IsUnlock())
            {
                unlockAll = false;
                break;
            }
        }
        if (unlockAll)
        {
            Debug.Log("全部解锁");
            return;
        }
    }

    void ClearCardData()
    {
        SetBlockVisible(false);
        cardItem1.CloseAnimation();
        cardItem2.CloseAnimation();
        cardItem1 = null;
        cardItem2 = null;
    }

    private void SetBlockVisible(bool _show)
    {
        ui.block_Image.raycastTarget = _show;
    }
}
