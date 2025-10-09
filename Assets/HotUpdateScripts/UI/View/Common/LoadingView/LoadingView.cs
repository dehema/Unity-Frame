using System;
using DG.Tweening;
using Rain.UI;
using UnityEngine;

public partial class LoadingView : BaseView
{
    //data
    LoadingViewParam _viewParams;

    public override void OnOpen(IViewParam viewParam = null)
    {
        base.OnOpen();
        if (viewParam != null)
        {
            _viewParams = viewParam as LoadingViewParam;
            imgSlider_Image.fillAmount = 0;
            float tweenTime1 = 1.2f;
            float tweenTime2 = 0.5f;
            float tweenTime3 = 1.8f;
            //imgSlider_Image.DOFillAmount(0.5f, tweenTime1).SetEase(Ease.InOutCubic).onComplete = () =>
            //{
            //    imgSlider_Image.DOFillAmount(0.6f, tweenTime2).SetEase(Ease.Linear).SetDelay(0.2f).onComplete = () =>
            //    {
            //        imgSlider_Image.DOFillAmount(0.99f, tweenTime3).onComplete = () =>
            //        {
            //            Close();
            //        };
            //    };
            //};
        }
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        _viewParams?.CloseCB?.Invoke();
    }

    private void Update()
    {
        txtProgress_Text.text = (int)(imgSlider_Image.fillAmount * 100) + "%";
        string loadingStr = "Loading";
        for (int i = 0; i <= Time.time % 3; i++)
        {
            loadingStr += ".";
        }
        txtLoading_Text.text = loadingStr;
    }
}

public class LoadingViewParam
{
    public Action CloseCB;
}
