using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEditor.Build;
using UnityEngine;

public partial class CardMatchGameView : BaseView
{
    ObjPool cardPool;
    string[] imageArray = { "人族剑士", "人族魔法师", "兽人士兵", "哥布林", "巨魔", "怨灵", "森林精灵", "美人鱼" };
    CardMatchCardItem cardItem1;
    CardMatchCardItem cardItem2;
    List<CardMatchCardItem> cardList = new List<CardMatchCardItem>();
    public override void Init(IViewParams viewParams = null)
    {
        base.Init(viewParams);
        cardPool = PoolMgr.Ins.CreatePool(ui.cardMatchCardItem);
        ui.btClose_Button.SetButton(Close);
        
    }

    public override void OnOpen(IViewParams viewParams = null)
    {
        base.OnOpen(viewParams);
        //倒计时界面
        CountDownViewParams viewParmas = new CountDownViewParams
        {
            countDown = 3,
            delay = 0f,
            cb = GameStart
        };
        UIMgr.Ins.OpenView<CountDownView>(viewParmas);
        //倒计时滑块
        ui.slider_SliderCountDown.Init(30, () =>
        {
            if (IsAllUnlock())
            {
                GameWin();
            }
            else
            {
                GameOver();
            }
        });
    }

    void GameStart()
    {
        SetBlockVisible(false);
        ui.slider_SliderCountDown.Play();
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
        ui.slider_SliderCountDown.Stop();
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
        if (IsAllUnlock())
        {
            GameWin();
        }
    }

    bool IsAllUnlock()
    {
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
            GameWin();
            Debug.Log("全部解锁");
            return unlockAll;
        }
        return unlockAll;
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

    void GameWin()
    {
        Debug.Log("GameWin");
    }

    void GameOver()
    {
        Debug.Log("GameOver");
    }
}
