using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rain.UI;
using UnityEngine;

public partial class GameWinView : BaseView
{
    GameWinViewParam viewParam;
    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
        viewParam = null;
        if (_viewParam != null)
        {
            viewParam = _viewParam as GameWinViewParam;
        }
        ui.icon.transform.DOKill();
        ui.icon.transform.localScale = Vector3.zero;
        ui.icon.transform.DOScale(1, 0.7f).SetEase(Ease.OutElastic);
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        viewParam?.closeCB?.Invoke();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Close();
        }
    }
}

public class GameWinViewParam : IViewParam
{
    public Action closeCB;

    public GameWinViewParam(Action _closeCB)
    {
        closeCB = _closeCB;
    }
}