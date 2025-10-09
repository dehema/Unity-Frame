using System.Collections;
using System.Collections.Generic;
using Rain.Core;

public partial class BuildingHudItem : BasePoolItem
{
    BuildingController buildingController;
    CityBuildingData buildingData => buildingController.BuildingData;
    DBBinding bind;
    public override void OnCreate(params object[] _param)
    {
        base.OnCreate(_param);
        buildingController = _param[0] as BuildingController;
        buildingData.Level.Bind((dm) =>
        {
            ui.BuildingLevel_Text.text = dm.value.ToString();
        });
        Refresh();
    }


    public void Refresh()
    {
        //名字
        ui.BuildingName_Text.text = buildingData.BuildingConfig.BuildingName;
        //进度
        ui.BuildingProgress.SetActive(buildingData.IsBuilding);
        //气泡
        if (buildingData.BuildingType != CityBuildingType.Tech)
        {
            ui.btTips.SetActive(false);
        }
        //菜单
    }

    public override void OnCollect()
    {
        base.OnCollect();
        bind?.UnBind();
    }
}
