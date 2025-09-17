using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Rain.Core;
using Rain.RTS.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CameraController_RTS))]
[RequireComponent(typeof(BattleSceneRoot))]
public class RTSUnitSceneMgr : MonoBehaviour
{
    public static RTSUnitSceneMgr Ins;

    public CameraController_RTS cameraController;   //相机脚本
    public BattleSceneRoot sceneRoot;               //场景节点脚本

    public Camera MainCamera => cameraController.mainCamera;
    List<BaseBattleUnit> playerUnits = new List<BaseBattleUnit>();
    NavMeshAgent agent;
    BaseBattleUnit dummy;                   // 假人


    void Awake()
    {
        Ins = this;
        cameraController = GetComponent<CameraController_RTS>();
        sceneRoot = GetComponent<BattleSceneRoot>();
        BattleMgr.Ins.unitCamera = MainCamera;
        // 重置镜头位置
        cameraController.ResetCameraToWorldCenter();
        // 打开调试界面
        if (SceneManager.GetActiveScene().name == SceneName.RTSUnitDummyTest)
        {
            UIMgr.Ins.OpenView(ViewName.RTSUnitTestView);
        }
    }

    void Update()
    {
        if (BattleMgr.Ins.battleData.battleType == BattleType.SingleUnitDemo)
        {
            // 检测鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                MovePlayerUnitsToMousePosition();
            }
        }
    }

    /// <summary>
    /// 创建玩家单位
    /// </summary>
    public BaseBattleUnit CreatePlayerUnit(UnitConfig unitConfig)
    {
        foreach (var item in playerUnits)
        {
            Destroy(item.gameObject);
        }
        playerUnits.Clear();
        unitConfig.faction = Faction.Player; // 设置单位阵营为玩家
        BaseBattleUnit battleUnit = CreateUnit(unitConfig);
        if (battleUnit != null)
        {
            playerUnits.Add(battleUnit);
        }
        return battleUnit;
    }

    /// <summary>
    /// 创建假人
    /// </summary>
    public BaseBattleUnit CreateDummy()
    {
        if (dummy != null)
        {
            return null;
        }
        UnitConfig unitConfig = ConfigMgr.Unit.Get(1);
        unitConfig.faction = Faction.Dummy;
        dummy = CreateUnit(unitConfig);
        return dummy;
    }

    /// <summary>
    /// 创建单位
    /// </summary>
    /// <param name="unitConfig"></param>
    public BaseBattleUnit CreateUnit(UnitConfig unitConfig)
    {
        BaseBattleUnit battleUnit = BattleMgr.Ins.CreateUnit(unitConfig);
        return battleUnit;
    }

    /// <summary>
    /// 移动单位到鼠标点击位置
    /// </summary>
    private void MovePlayerUnitsToMousePosition()
    {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (isOverUI)
        {
            return; // 点到UI
        }
        // 从相机发射射线到鼠标位置
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(MainCamera.transform.position, ray.direction, Color.red, 1);
        RaycastHit hit;
        // 如果射线击中了地面
        if (Physics.Raycast(ray, out hit, 1000f, 1 << GameObjectLayer.Scene3D))
        {
            BaseBattleUnit battleUnit = hit.collider.gameObject.GetComponent<BaseBattleUnit>();
            if (battleUnit != null)
            {
                //如果是个单位
                CampRelation relation = BattleMgr.Ins.GetFactionRelation(Faction.Player, battleUnit.Data.faction);
                if (relation == CampRelation.Hostile)
                {
                    BattleMgr.Ins.FactionUnitAttackUnit(Faction.Player, battleUnit);
                    return;
                }
            }

            // 设置导航目标位置
            BattleMgr.Ins.AllPlayerUnitMove(hit.point);
        }
    }

    /// <summary>
    /// 在场景视图中绘制辅助线
    /// </summary>
    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            // 绘制导航路径
            Gizmos.color = Color.blue;
            var path = agent.path;
            Vector3 previousCorner = agent.transform.position;

            // 绘制路径中的每个拐点
            foreach (var corner in path.corners)
            {
                Gizmos.DrawLine(previousCorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                previousCorner = corner;
            }
        }
    }
}
