using System.Collections;
using System.Collections.Generic;
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
        foreach (var imagePath in imageArray)
        {
            for (int i = 1; i <= 2; i++)
            {
                CardMatchCardItem item = cardPool.Get<CardMatchCardItem>(imagePath);
                cardList.Add(item);
                item.onCardOpen += OnCardOpen;
            }
        }
    }

    public void OnCardOpen(CardMatchCardItem cardItem)
    {
        if (cardItem1 != null)
        {
            cardItem1 = cardItem;
            CheckCard();
        }
        else if (cardItem2 != null)
        {
            cardItem2 = cardItem;
            CheckCard();
        }
    }

    public void CheckCard()
    {
        if (cardItem1.ImgPath == cardItem2.ImgPath)
        {
            cardItem1.UnLockCard();
            cardItem2.UnLockCard();
        }
        bool unlockAll = true;
        foreach (var item in cardList)
        {
            if (!item.IsUnlock())
            {
                break;
            }
        }
        if (unlockAll)
        {
            Debug.Log("全部解锁");
        }
    }
}
