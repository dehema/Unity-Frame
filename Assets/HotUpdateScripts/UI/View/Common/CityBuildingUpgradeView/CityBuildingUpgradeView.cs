using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 城市建筑升级
/// </summary>
public partial class CityBuildingUpgradeView : BaseView
{
    CityBuildingUpgradeViewParam param;
    CityBuildingData cityBuildingData => param.cityBuildingData;
    DBHandler.Binding dbLevel;
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        param = _viewParams as CityBuildingUpgradeViewParam;

        ui.btBuildingLevelUp_Button.SetButton(() =>
        {
            cityBuildingData.Level.Value++;
            Close();
        });
    }

    public override void OnOpen(IViewParam _viewParams = null)
    {
        base.OnOpen(_viewParams);
        param = _viewParams as CityBuildingUpgradeViewParam;
        //UI
        dbLevel = cityBuildingData.Level.Bind((dm) =>
        {
            ui.lbBuildingName_Text.text = $"{cityBuildingData.BuildingConfig.BuildingName}  {dm.value.ToString()}级";
        });
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        dbLevel.UnBind();
    }
}

public class CityBuildingUpgradeViewParam : IViewParam
{
    public CityBuildingData cityBuildingData;
}
