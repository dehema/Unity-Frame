using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 主UI
/// </summary>
public partial class MainView : BaseView
{
    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        //msg
        MsgMgr.Ins.AddEventListener(MsgEvent.SelectCityBuilding, OnSelectBuilding, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.SceneLoaded, OnSceneLoaded, this);
        //bind
        PlayerMgr.Ins.GetResDBField(ResType.Food).Bind((dm) =>
        {
            ui.lbFoodNum_Text.text = Util.Common.FormatResourceNumber((long)dm.value);
        });
        PlayerMgr.Ins.GetResDBField(ResType.Wood).Bind((dm) =>
        {
            ui.lbWoodNum_Text.text = Util.Common.FormatResourceNumber((long)dm.value);
        });
        PlayerMgr.Ins.GetResDBField(ResType.Gold).Bind((dm) =>
        {
            ui.lbGoldNum_Text.text = Util.Common.FormatResourceNumber((long)dm.value);
        });
        PlayerMgr.Ins.playerData.diamond.Bind((dm) =>
        {
            ui.lbDiamondNum_Text.text = Util.Common.FormatResourceNumber((long)dm.value);
        });
        //UI
        ui.btChatPanel_Button.SetButton(() => { UIMgr.Ins.OpenView<ChatView>(); });
        ui.btMainCity_Button.SetButton(OnClickMainCity);
        ui.btWorldMap_Button.SetButton(OnClickWorldMap);
    }

    /// <summary>
    /// 点击世界地图
    /// </summary>
    void OnClickMainCity()
    {
        SceneMgr.Ins.ChangeScene(SceneID.MainCity, showView: false);
    }

    /// <summary>
    /// 点击世界地图
    /// </summary>
    void OnClickWorldMap()
    {
        SceneMgr.Ins.ChangeScene(SceneID.WorldMap, showView: false);
    }

    private void OnSelectBuilding(object[] objs)
    {
        CityBuildingData cityBuildingData = objs[0] as CityBuildingData;
        //科研
        //if (cityBuildingData.BuildingType == CityBuildingType.Tech && cityBuildingData.Level.Value > 0)
        //{
        //    UIMgr.Ins.OpenView<TechView>();
        //}
        //else
        {
            CityBuildingDetailViewParam param = new CityBuildingDetailViewParam();
            param.cityBuildingData = cityBuildingData;
            UIMgr.Ins.OpenView<CityBuildingDetailView>(param);
        }
    }

    /// <summary>
    /// 场景加载
    /// </summary>
    private void OnSceneLoaded(object[] param)
    {
        SceneChangeParam sceneChangeParam = param.Length > 0 ? param[0] as SceneChangeParam : null;
        if (sceneChangeParam != null)
        {
            if (sceneChangeParam.sceneID == SceneID.MainCity)
            {
                ui.btMainCity.SetActive(sceneChangeParam.sceneID != SceneID.MainCity);
                ui.btWorldMap.SetActive(sceneChangeParam.sceneID == SceneID.MainCity);
            }
            else if (sceneChangeParam.sceneID == SceneID.WorldMap)
            {
                ui.btMainCity.SetActive(sceneChangeParam.sceneID == SceneID.WorldMap);
                ui.btWorldMap.SetActive(sceneChangeParam.sceneID != SceneID.WorldMap);
            }
        }
    }
}
