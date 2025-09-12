using System.Collections.Generic;
using Rain.Core;
using Rain.RTS.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class RTSUnitTestSceneMgr : MonoBehaviour
{
    public static RTSUnitTestSceneMgr Ins;

    List<BaseBattleUnit> playerUnits = new List<BaseBattleUnit>();
    NavMeshAgent agent;

    [Header("��������")]
    [SerializeField]
    public Camera mainCamera;     // ���������

    BaseBattleUnit dummy; //����

    void Awake()
    {
        Ins = this;
        UIMgr.Ins.OpenView(ViewName.RTSHudView);
        UIMgr.Ins.OpenView(ViewName.RTSUnitTestView);
        mainCamera = GetComponent<Camera>();
        BattleMgr.Ins.unitCamera = mainCamera;
    }

    /// <summary>
    /// ������ҵ�λ
    /// </summary>
    public BaseBattleUnit CreatePlayerUnit(UnitConfig unitConfig)
    {
        foreach (var item in playerUnits)
        {
            Destroy(item.gameObject);
        }
        playerUnits.Clear();
        unitConfig.faction = Faction.Player; // ���õ�λ��ӪΪ���
        BaseBattleUnit battleUnit = CreateUnit(unitConfig);
        if (battleUnit != null)
        {
            playerUnits.Add(battleUnit);
        }
        return battleUnit;
    }

    /// <summary>
    /// ��������
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
    /// ������λ
    /// </summary>
    /// <param name="unitConfig"></param>
    public BaseBattleUnit CreateUnit(UnitConfig unitConfig)
    {
        BaseBattleUnit battleUnit = BattleMgr.Ins.CreateUnit(unitConfig);
        return battleUnit;
    }

    void Update()
    {
        // ������������
        if (Input.GetMouseButtonDown(0))
        {
            MovePlayerUnitsToMousePosition();
        }
    }

    /// <summary>
    /// �ƶ���λ�������λ��
    /// </summary>
    private void MovePlayerUnitsToMousePosition()
    {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (isOverUI)
        {
            return; // �㵽UI
        }
        // ������������ߵ����λ��
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mainCamera.transform.position, ray.direction, Color.red, 1);
        RaycastHit hit;
        // ������߻����˵���
        if (Physics.Raycast(ray, out hit, 1000f, 1 << GameObjectLayer.Scene3D))
        {
            BaseBattleUnit battleUnit = hit.collider.gameObject.GetComponent<BaseBattleUnit>();
            if (battleUnit != null)
            {
                //����Ǹ���λ
                Relation relation = BattleMgr.Ins.GetFactionRelation(Faction.Player, battleUnit.Data.faction);
                if (relation == Relation.Hostile)
                {
                    BattleMgr.Ins.FactionUnitAttackUnit(Faction.Player, battleUnit);
                    return;
                }
            }

            // ���õ���Ŀ��λ��
            BattleMgr.Ins.AllPlayerUnitMove(hit.point);
        }
    }

    /// <summary>
    /// �ڳ�����ͼ�л��Ƹ�����
    /// </summary>
    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            // ���Ƶ���·��
            Gizmos.color = Color.blue;
            var path = agent.path;
            Vector3 previousCorner = agent.transform.position;

            // ����·���е�ÿ���յ�
            foreach (var corner in path.corners)
            {
                Gizmos.DrawLine(previousCorner, corner);
                Gizmos.DrawSphere(corner, 0.1f);
                previousCorner = corner;
            }
        }
    }
}
