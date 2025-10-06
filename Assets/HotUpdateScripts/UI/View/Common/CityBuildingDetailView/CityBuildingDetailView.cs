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

    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        param = _viewParams as CityBuildingDetailViewParam;
        param.cityBuildingData.Level.Bind((dm) =>
        {
            ui.lbBuildingName_Text.text = $"{param.cityBuildingData.BuildingConfig.BuildingName}  {dm.value.ToString()}";
        });
        DataMgr.Ins.playerData.cityBuildings[2].Level.Bind((dm) =>
        {
            ui.lbBuildingName_1_Text.text = $"{param.cityBuildingData.BuildingConfig.BuildingName}  {dm.value.ToString()}";
        });
        DataMgr.Ins.playerData.cityBuildings.Bind((dm) =>
        {
            ui.lbBuildingName_Text.text = $"{param.cityBuildingData.BuildingConfig.BuildingName}  {DataMgr.Ins.playerData.cityBuildings[1].Level.Value.ToString()}";
            ui.lbBuildingName_1_Text.text = $"{param.cityBuildingData.BuildingConfig.BuildingName}  {DataMgr.Ins.playerData.cityBuildings[2].Level.Value.ToString()}";
        });
        ui.btBuildingLevelUp_Button.SetButton(() =>
        {
            DataMgr.Ins.playerData.cityBuildings[1].Level.Value++;
        });
        ui.btBuildingLevelUp_1_Button.SetButton(() =>
        {
            DataMgr.Ins.playerData.cityBuildings[2].Level.Value++;
        });
        ui.btBuildingLevelUp_2_Button.SetButton(() =>
        {
            DataMgr.Ins.playerData.cityBuildings[3].Level.Value++;
        });
    }

    public override void OnOpen(IViewParam _viewParams = null)
    {
        base.OnOpen(_viewParams);
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
    }
}


public class CityBuildingDetailViewParam : IViewParam
{
    public CityBuildingData cityBuildingData;
}