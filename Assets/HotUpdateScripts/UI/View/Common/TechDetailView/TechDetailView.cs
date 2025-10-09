using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 科技详情
/// </summary>
public partial class TechDetailView : BaseView
{
    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        ui.btFinish_Button.SetButton(OnClickFinish);
        ui.btStudy_Button.SetButton(OnClickStudy);
    }

    private void OnClickStudy()
    {
        Close();
    }

    private void OnClickFinish()
    {
        ConfirmDiamondSpeedView.ViewParam viewParam = new ConfirmDiamondSpeedView.ViewParam();
        viewParam.onFinish = () =>
        {
            Close();
        };
        UIMgr.Ins.OpenView<ConfirmDiamondSpeedView>(viewParam);
    }
}
