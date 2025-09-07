using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 部署界面
/// </summary>
public partial class RTSBattleDeployView : BaseView
{
    // 阵型对象池
    ObjPool poolFormation = null;
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        poolFormation = PoolMgr.Ins.CreatePool(ui.tgFormation);
        RefreshFormation();
    }

    string[] formationTextArr = new[]
    {
        "盾墙推进阵（防御型）",
        "楔形突破阵（进攻型）",
        "雁形狙击阵（远程压制型）",
        "环形防御阵（应急型）",
        "菱形突袭阵（游击型）",
    };

    void RefreshFormation()
    {
        ui.tgFormation_Toggle.SetToggle(OnValueChangeFormation);
        foreach (var _val in formationTextArr)
        {
            GameObject formationItem = poolFormation.Get();
            TextMeshProUGUI text = formationItem.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            text.text = _val;
        }
    }

    /// <summary>
    /// 当阵型切换时
    /// </summary>
    public void OnValueChangeFormation(bool _ison)
    {

    }
}
