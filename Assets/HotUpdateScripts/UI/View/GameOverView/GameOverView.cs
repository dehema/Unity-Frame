using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rain.UI;
using UnityEngine;

public partial class GameOverView : BaseView
{
    GameOverViewParams viewParams;
    public override void OnOpen(IViewParams _viewParams = null)
    {
        base.OnOpen(_viewParams);
        viewParams = null;
        if (_viewParams != null)
        {
            viewParams = _viewParams as GameOverViewParams;
        }
        ui.icon.transform.DOKill();
        ui.icon.transform.localScale = Vector3.zero;
        ui.icon.transform.DOScale(1, 0.7f).SetEase(Ease.OutElastic);
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        viewParams?.closeCB?.Invoke();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Close();
        }
    }
}


public class GameOverViewParams : IViewParams
{
    public Action closeCB;

    public GameOverViewParams(Action _closeCB)
    {
        closeCB = _closeCB;
    }
}