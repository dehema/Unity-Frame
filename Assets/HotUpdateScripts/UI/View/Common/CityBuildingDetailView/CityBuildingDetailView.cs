using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 城市建筑详情
/// </summary>
public partial class CityBuildingDetailView : BaseView
{
    CityBuildingDetailViewParam param;
    CityBuildingData cityBuildingData => param.cityBuildingData;
    DBBinding dbLevel;

    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        ui.btBuildingLevelUp_Button.SetButton(() =>
        {
            CityBuildingUpgradeViewParam param = new CityBuildingUpgradeViewParam();
            param.cityBuildingData = cityBuildingData;
            UIMgr.Ins.OpenView<CityBuildingUpgradeView>(param);
            Close();
        });
    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
        param = _viewParam as CityBuildingDetailViewParam;
        //UI
        dbLevel = cityBuildingData.Level.Bind((dm) =>
        {
            ui.lbBuildingName_Text.text = $"{cityBuildingData.BuildingConfig.BuildingName}  {dm.value.ToString()}级";
        });
        ui.imgBuildingIcon_Image.sprite = Resources.Load<Sprite>($"UI/Common/{cityBuildingData.BuildingConfig.Icon}");
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        dbLevel.UnBind();
    }
}


public class CityBuildingDetailViewParam : IViewParam
{
    public CityBuildingData cityBuildingData;
}