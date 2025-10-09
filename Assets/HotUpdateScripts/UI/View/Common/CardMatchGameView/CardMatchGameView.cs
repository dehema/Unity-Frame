using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class CardMatchGameView : BaseView
{
    ObjPool cardPool;
    string[] imageArray = { "人族剑士", "人族魔法师", "兽人士兵", "哥布林", "巨魔", "怨灵", "森林精灵", "美人鱼" };
    CardMatchCardItem cardItem1;
    CardMatchCardItem cardItem2;
    List<CardMatchCardItem> cardList = new List<CardMatchCardItem>();
    const int gameTime = 60; //游戏限时
    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        cardPool = PoolMgr.Ins.CreatePool(ui.cardMatchCardItem);
        ui.btClose_Button.SetButton(Close);
        ui.btSuccess_Button.SetDebugButton(DebugGameWin);
        ui.btFail_Button.SetDebugButton(DebugGameOver);
    }

    public override void OnOpen(IViewParam viewParam = null)
    {
        base.OnOpen(viewParam);
        cardItem1 = null;
        cardItem2 = null;
        //倒计时界面
        CountDownViewParam viewParmas = new CountDownViewParam
        {
            countDown = 3,
            delay = 0f,
            cb = GameStart
        };
        UIMgr.Ins.OpenView<CountDownView>(viewParmas);
        //倒计时滑块
        ui.slider_SliderCountDown.Init(gameTime, () =>
        {
            if (!IsAllUnlock())
            {
                GameOver();
            }
        });
        InitCard();
    }

    void InitCard()
    {
        //生成卡牌
        cardList.Clear();
        List<string> cardPath = new List<string>();
        foreach (var imagePath in imageArray)
        {
            for (int i = 1; i <= 2; i++)
            {
                cardPath.Add(imagePath);
            }
        }
        //cardPath.Shuffle();
        foreach (var imagePath in cardPath)
        {
            CardMatchCardItem item = cardPool.Get<CardMatchCardItem>(imagePath);
            cardList.Add(item);
            item.onCardOpen += OnCardOpen;
        }
    }

    void GameStart()
    {
        SetBlockVisible(false);
        ui.slider_SliderCountDown.Play();
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        foreach (var card in cardList)
        {
            card.onCardOpen -= OnCardOpen;
        }
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
            CheckAllUnlock();
        }
        else
        {
            TimerMgr.Ins.AddTimer(this, delay: CardMatchCardItem.cardFlipDuation * 3, onComplete: () =>
            {
                ClearCardData();
            });
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
        return unlockAll;
    }

    void CheckAllUnlock()
    {
        bool unlockAll = IsAllUnlock();
        Debug.Log("CheckAllUnlock" + unlockAll);
        if (unlockAll)
        {
            ui.slider_SliderCountDown.Stop();
            TimerMgr.Ins.AddTimer(this, delay: CardMatchCardItem.cardFlipDuation * 2, onComplete: () =>
            {
                GameWin();
            });
            Debug.Log("全部解锁");
        }
    }

    void ClearCardData()
    {
        SetBlockVisible(false);
        cardItem1?.CloseAnimation();
        cardItem2?.CloseAnimation();
        cardItem1 = null;
        cardItem2 = null;
    }

    private void SetBlockVisible(bool _show)
    {
        ui.block_Image.raycastTarget = _show;
    }

    void DebugGameWin()
    {
        foreach (CardMatchCardItem item in cardList)
        {
            item.OpenAnimation();
            item.UnLockCard();
        }
        CheckAllUnlock();
    }

    void DebugGameOver()
    {
        GameOver();
    }

    void GameWin()
    {
        Debug.Log("GameWin");
        GameWinViewParam viewParam = new GameWinViewParam(() =>
        {
            Close();
        });
        UIMgr.Ins.OpenView<GameWinView>(viewParam);
    }

    void GameOver()
    {
        Debug.Log("GameOver");
        ui.slider_SliderCountDown.Stop();
        GameOverViewParam viewParam = new GameOverViewParam(() =>
        {
            Close();
        });
        UIMgr.Ins.OpenView<GameOverView>(viewParam);
    }
}
