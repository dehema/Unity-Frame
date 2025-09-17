using System.Collections.Generic;
using Rain.Core;
using Rain.RTS.Core;
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
    // 当前显示的阵型
    GameObject currShowFormation;
    int formationID;
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
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
        if (poolFormation.activePool.Count > 0)
        {
            poolFormation.activePool.First().GetComponent<Toggle>().isOn = true;
        }
    }

    /// <summary>
    /// 当阵型切换时
    /// </summary>
    public void OnValueChangeFormation(bool _ison)
    {
        //EventSystem.current.currentSelectedGameObject
        foreach (var item in poolFormation.activePool)
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
            text.color = tg.isOn ? Utility.ColorHexToRGB("#0073CC") : Color.white;
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

    public void OnClickStartBattle()
    {
        StartBattleParam startBattleParam = new StartBattleParam();
        StartBattleArmyParam playerArmyParam = new StartBattleArmyParam(Faction.Player);
        playerArmyParam.initUnitNum = new Dictionary<int, int> { { 1101, 10 }, { 1201, 10 }, { 1301, 10 }, { 1401, 10 } };
        playerArmyParam.formationID = formationID;
        startBattleParam.startBattleArmyParams.Add(playerArmyParam);

        StartBattleArmyParam enemyArmyParam = new StartBattleArmyParam(Faction.Enemy);
        enemyArmyParam.initUnitNum = new Dictionary<int, int> { { 1101, 10 }, { 1201, 10 }, { 1301, 10 }, { 1401, 10 } };
        enemyArmyParam.formationID = formationID;
        startBattleParam.startBattleArmyParams.Add(enemyArmyParam);

        SceneMgr.Ins.ChangeScene(SceneID.RTSBattle, () =>
        {
            BattleMgr.Ins.InitBattle(startBattleParam);
        });
        Close();
    }
}
