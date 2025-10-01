using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 钻石加速
/// </summary>
public partial class ConfirmDiamondSpeedView : BaseView
{
    ViewParam viewParam;
    override public void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        viewParam = _viewParams as ViewParam;
        ui.btClose_Button.SetButton(() =>
        {
            viewParam.onCancel?.Invoke();
            Close();
        });
        ui.btFinish_Button.SetButton(OnClickFinish);
    }

    private void OnClickFinish()
    {
        viewParam.onFinish?.Invoke();
        Close();
    }

    public class ViewParam : IViewParam
    {
        public Action onCancel;
        public Action onFinish;

    }
}
