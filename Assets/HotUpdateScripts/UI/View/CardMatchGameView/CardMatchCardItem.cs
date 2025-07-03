using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rain.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class CardMatchCardItem : BasePoolItem, IPointerClickHandler
{
    enum CardStatus
    {
        Close,
        Open,
        UnLock,
    }
    string imgPath;
    public delegate void OnCardOpen(CardMatchCardItem cardItem);
    public OnCardOpen onCardOpen;
    CardStatus cardStatus = CardStatus.Close;
    public const float cardFlipDuation = 0.3f;

    public string ImgPath { get => imgPath; set => imgPath = value; }

    public override void OnCreate(params object[] _params)
    {
        base.OnCreate(_params);
        ImgPath = _params[0] as string;
        Sprite sprite = Resources.Load<Sprite>("UI/unit/" + ImgPath);
        ui.icon_Image.sprite = sprite;
    }

    public override void OnOpen(params object[] _params)
    {
        base.OnOpen(_params);
        ui.back.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardStatus != CardStatus.Close)
        {
            return;
        }
        onCardOpen(this);
        OpenAnimation();
    }

    void OpenAnimation()
    {
        cardStatus = CardStatus.Open;
        transform.DOScaleX(0, cardFlipDuation).SetEase(Ease.Linear).OnComplete(() =>
        {
            ui.back.SetActive(false);
            transform.DOScale(1.1f, cardFlipDuation).SetEase(Ease.Linear);
        });
    }

    public void CloseAnimation()
    {
        if (IsUnlock())
        {
            return;
        }
        transform.DOKill();
        transform.DOScale(new Vector3(0, 1, 1), cardFlipDuation).SetEase(Ease.Linear).OnComplete(() =>
        {
            ui.back.SetActive(true);
            transform.DOScaleX(1, cardFlipDuation).SetEase(Ease.Linear).OnComplete(() =>
            {
                cardStatus = CardStatus.Close;
            });
        });
    }

    public void UnLockCard()
    {
        cardStatus = CardStatus.UnLock;
    }

    public bool IsUnlock()
    {
        return cardStatus == CardStatus.UnLock;
    }
}
