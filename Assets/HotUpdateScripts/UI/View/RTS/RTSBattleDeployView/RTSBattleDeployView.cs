using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������
/// </summary>
public partial class RTSBattleDeployView : BaseView
{
    // ���Ͷ����
    ObjPool poolFormation = null;
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        poolFormation = PoolMgr.Ins.CreatePool(ui.tgFormation);
        RefreshFormation();
    }

    string[] formationTextArr = new[]
    {
        "��ǽ�ƽ��󣨷����ͣ�",
        "Ш��ͻ���󣨽����ͣ�",
        "���ξѻ���Զ��ѹ���ͣ�",
        "���η�����Ӧ���ͣ�",
        "����ͻϮ���λ��ͣ�",
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
    /// �������л�ʱ
    /// </summary>
    public void OnValueChangeFormation(bool _ison)
    {

    }
}
