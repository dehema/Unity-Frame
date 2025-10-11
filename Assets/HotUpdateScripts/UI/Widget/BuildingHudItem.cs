using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;

public partial class BuildingHudItem : BasePoolItem
{
    BuildingController buildingController;
    CityBuildingData buildingData => buildingController.BuildingData;
    DBBinding bind;
    public override void OnCreate(params object[] _param)
    {
        base.OnCreate(_param);
        buildingController = _param[0] as BuildingController;
        ui.btTips_Button.SetButton(OnClickTips);
        gameObject.name = buildingData.BuildingConfig.BuildingName;
        buildingData.Level.Bind((dm) =>
        {
            Refresh();
        });
        buildingData.State.Bind((dm) =>
        {
            Refresh();
        });
    }


    public void Refresh()
    {
        //名字
        ui.BuildingLevel_Text.text = buildingData.Level.Value.ToString();
        ui.BuildingName_Text.text = buildingData.BuildingConfig.BuildingName;
        //进度
        ui.BuildingProgress.SetActive(buildingData.IsBuilding);
        //气泡
        if (buildingData.BuildingType == CityBuildingType.Tech && buildingData.Level.Value > 0)
        {
            ui.btTips.SetActive(true);
        }
        else
        {
            ui.btTips.SetActive(false);
        }
        //菜单
    }

    void OnClickTips()
    {
        if (buildingData.BuildingType == CityBuildingType.Tech)
        {
            UIMgr.Ins.OpenView(ViewName.TechView);
        }
    }

    public override void OnCollect()
    {
        base.OnCollect();
        bind?.UnBind();
    }
}
