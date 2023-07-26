using DG.Tweening;
using UnityEngine;
using System;

public partial class LoadingView : BaseView
{
    //data
    LoadingViewParams viewParams;

    public override void OnOpen(params object[] _params)
    {
        base.OnOpen();
        viewParams = _params.Length > 0 ? (LoadingViewParams)_params[0] : null;
        imgSlider_Image.fillAmount = 0;
        float tweenTime1 = 1.2f;
        float tweenTime2 = 0.5f;
        float tweenTime3 = 1.8f;
        imgSlider_Image.DOFillAmount(0.5f, tweenTime1).SetEase(Ease.InOutCubic).onComplete = () =>
        {
            imgSlider_Image.DOFillAmount(0.6f, tweenTime2).SetEase(Ease.Linear).SetDelay(0.2f).onComplete = () =>
            {
                imgSlider_Image.DOFillAmount(0.99f, tweenTime3).onComplete = () =>
                {
                    Close();
                };
            };
        };
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        viewParams?.CloseCB?.Invoke();
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

public class LoadingViewParams
{
    public Action CloseCB;
}