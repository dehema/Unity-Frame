using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �������
/// </summary>
public partial class RTSBattleDeployView : BaseView
{
    // ���Ͷ����
    ObjPool poolFormation = null;
    // ��ǰ��ʾ������
    GameObject currShowFormation;
    int formationID;
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        poolFormation = PoolMgr.Ins.CreatePool(ui.tgFormation);
        RefreshFormation();
    }

    void RefreshFormation()
    {
        int index = 1;
        foreach (var item in ConfigMgr.DeployFormation.DataList)
        {
            GameObject formationItem = poolFormation.Get();
            formationItem.name = index.ToString();
            Toggle toggle = formationItem.GetComponent<Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            toggle.SetToggle(OnValueChangeFormation);
            TextMeshProUGUI text = formationItem.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            text.text = item.Name;
            index++;
        }
        if (poolFormation.activePool.Count > 0)
        {
            poolFormation.activePool.First().GetComponent<Toggle>().isOn = true;
        }
    }

    /// <summary>
    /// �������л�ʱ
    /// </summary>
    public void OnValueChangeFormation(bool _ison)
    {
        if (_ison)
        {
            if (int.TryParse(EventSystem.current.currentSelectedGameObject.name, out formationID))
            {
                RefreshShowFormation();
            }
        }
    }

    public void RefreshShowFormation()
    {
        GameObject go = Resources.Load<GameObject>($"Prefab/Formation/{formationID}");
        if (go == null)
        {
            return;
        }
        currShowFormation?.DestroyNow();
        currShowFormation = Instantiate(go, ui.sandBox.transform);
        currShowFormation.transform.localPosition = Vector3.zero;

    }
}
