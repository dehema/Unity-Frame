using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core.RTS;
using Rain.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class RTSUnitTestSceneMgr : MonoBehaviour
{
    public static RTSUnitTestSceneMgr Ins;

    List<BattleUnit> playerUnits = new List<BattleUnit>();
    NavMeshAgent agent;

    [Header("射线设置")]
    [SerializeField]
    Camera mainCamera;     // 主相机引用

    BattleUnit dummy; //假人

    void Awake()
    {
        Ins = this;
        UIMgr.Ins.OpenView(ViewName.RTSUnitTestView);
        mainCamera = GetComponent<Camera>();
        //UIMgr.Ins.OpenView<RTSUnitTestView>(new RTSUnitTestViewParams()
        //{
        //    actionCreateUnit = CreateUnit
        //});

    }

    /// <summary>
    /// 创建玩家单位
    /// </summary>
    public void CreatePlayerUnit(UnitConfig unitConfig)
    {
        foreach (var item in playerUnits)
        {
            Destroy(item.gameObject);
        }
        playerUnits.Clear();
        unitConfig.faction = Faction.Player; // 设置单位阵营为玩家
        BattleUnit battleUnit = CreateUnit(unitConfig);
        if (battleUnit == null)
        {
            return;
        }
        playerUnits.Add(battleUnit);
    }

    /// <summary>
    /// 创建假人
    /// </summary>
    public void CreateDummy()
    {
        if (dummy != null)
        {
            return;
        }
        UnitConfig unitConfig = ConfigMgr.Ins.allUnitConfig.unit.Find((x) => x.ID == 1101);
        unitConfig.hp = 100000;                 // 设置假人生命值
        unitConfig.unitType = UnitType.Dummy;   // 设置单位类型为假人
        unitConfig.faction = Faction.Dummy;   // 设置单位类型为假人
        dummy = CreateUnit(unitConfig);
        dummy.transform.position = new Vector3(0, 0, -5);
    }

    /// <summary>
    /// 创建单位
    /// </summary>
    /// <param name="unitConfig"></param>
    public BattleUnit CreateUnit(UnitConfig unitConfig)
    {
        BattleUnit battleUnit = null;
        GameObject prefab = Resources.Load<GameObject>("Prefab/Unit/" + unitConfig.fullID);
        if (prefab != null)
        {
            // 实例化单位预制体
            GameObject item = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            battleUnit = item.GetComponent<BattleUnit>();
            battleUnit.InitData(unitConfig);
        }
        return battleUnit;
    }

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            MovePlayerUnitsToMousePosition();
        }
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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mainCamera.transform.position, ray.direction, Color.red);
        RaycastHit hit;
        // 如果射线击中了地面
        if (Physics.Raycast(ray, out hit, 1000f, 1 << GameObjectLayer.Scene3D))
        {
            // 设置导航目标位置
            BattleMgr.Ins.AllUnitMove(hit.point);
            Debug.Log("单位移动到: " + hit.point);
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
