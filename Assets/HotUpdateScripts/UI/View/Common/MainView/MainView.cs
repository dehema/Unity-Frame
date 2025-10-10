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
        ui.btChatPanel_Button.SetButton(() => { UIMgr.Ins.OpenView<ChatView>(); });
        MsgMgr.Ins.AddEventListener(MsgEvent.SelectCityBuilding, OnSelectBuilding, this);
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
}
