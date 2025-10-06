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
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        MsgMgr.Ins.AddEventListener(MsgEvent.SelectCityBuilding, (object[] objs) =>
        {
            CityBuildingDetailViewParam param = new CityBuildingDetailViewParam();
            param.cityBuildingData = objs[0] as CityBuildingData;
            UIMgr.Ins.OpenView<CityBuildingDetailView>(param);
        }, this);

    }
}
