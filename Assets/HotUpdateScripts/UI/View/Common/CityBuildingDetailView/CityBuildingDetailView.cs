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
    DBHandler.Binding dbLevel;

    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
    }

    public override void OnOpen(IViewParam _viewParams = null)
    {
        base.OnOpen(_viewParams);
        param = _viewParams as CityBuildingDetailViewParam;
        //data
        dbLevel = cityBuildingData.Level.Bind((dm) =>
        {
            ui.lbBuildingName_Text.text = $"{cityBuildingData.BuildingConfig.BuildingName}  {dm.value.ToString()}级";
        });
        ui.btBuildingLevelUp_Button.SetButton(() =>
        {
            cityBuildingData.Level.Value++;
        });
        //UI
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