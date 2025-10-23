using System.Collections.Generic;
using Rain.Core;
using Rain.RTS.Core;
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
    // ��ǰ��ʾ������
    GameObject currShowFormation;
    int formationID;
    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        poolFormation = PoolMgr.Ins.CreatePool(ui.tgFormation);
        ui.btBattle_Button.SetButton(OnClickStartBattle);
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
        if (poolFormation.ShowPool.Count > 0)
        {
            poolFormation.ShowPool.First().GetComponent<Toggle>().isOn = true;
        }
    }

    /// <summary>
    /// �������л�ʱ
    /// </summary>
    public void OnValueChangeFormation(bool _ison)
    {
        //EventSystem.current.currentSelectedGameObject
        foreach (var item in poolFormation.ShowPool)
        {
            Toggle tg = item.GetComponent<Toggle>();
            if (tg.isOn)
            {
                if (int.TryParse(item.name, out formationID))
                {
                    RefreshShowFormation();
                }
            }

            TextMeshProUGUI text = item.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            text.color = tg.isOn ? Util.Common.ColorHexToRGB("#0073CC") : Color.white;
        }
    }

    public void RefreshShowFormation()
    {
        GameObject go = AssetMgr.Ins.Load<GameObject>($"RTS_Formation_{formationID}");
        if (go == null)
        {
            return;
        }
        currShowFormation?.DestroyNow();
        currShowFormation = Instantiate(go, ui.sandBox.transform);
        currShowFormation.transform.localPosition = Vector3.zero;

    }

    public void OnClickStartBattle()
    {
        StartBattleParam startBattleParam = new StartBattleParam();
        Dictionary<int, int> initUnitNum = new Dictionary<int, int> { { 1101, 6 }, { 1201, 4 }, { 1301, 2 }, { 1401, 2 } };

        //���
        StartBattleArmyParam playerArmyParam = new StartBattleArmyParam(Faction.Player);
        playerArmyParam.initUnitNum = initUnitNum;
        playerArmyParam.formationID = formationID;
        startBattleParam.startBattleArmyParams.Add(playerArmyParam);

        //����
        StartBattleArmyParam enemyArmyParam = new StartBattleArmyParam(Faction.Enemy);
        enemyArmyParam.initUnitNum = initUnitNum;
        startBattleParam.startBattleArmyParams.Add(enemyArmyParam);

        SceneMgr.Ins.ChangeScene(SceneID.RTSBattle, () =>
        {
            BattleMgr.Ins.InitBattle(startBattleParam);
        });
        Close();
    }
}
