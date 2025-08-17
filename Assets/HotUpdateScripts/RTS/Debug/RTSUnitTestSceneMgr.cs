using System;
using System.Collections;
using System.Collections.Generic;
using Rain.RTS.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class RTSUnitTestSceneMgr : MonoBehaviour
{
    public static RTSUnitTestSceneMgr Ins;

    List<BattleUnit> playerUnits = new List<BattleUnit>();
    NavMeshAgent agent;

    [Header("��������")]
    [SerializeField]
    public Camera mainCamera;     // ���������

    BattleUnit dummy; //����

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
    public void CreatePlayerUnit(UnitConfig unitConfig)
    {
        foreach (var item in playerUnits)
        {
            Destroy(item.gameObject);
        }
        playerUnits.Clear();
        unitConfig.faction = Faction.Player; // ���õ�λ��ӪΪ���
        BattleUnit battleUnit = CreateUnit(unitConfig);
        if (battleUnit == null)
        {
            return;
        }
        playerUnits.Add(battleUnit);
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void CreateDummy()
    {
        if (dummy != null)
        {
            return;
        }
        UnitConfig unitConfig = ConfigMgr.Ins.allUnitConfig.unit.Find((x) => x.ID == 1101);
        unitConfig.hp = 100000;                 // ���ü�������ֵ
        unitConfig.unitType = UnitType.Dummy;   // ���õ�λ����Ϊ����
        unitConfig.faction = Faction.Dummy;   // ���õ�λ����Ϊ����
        dummy = CreateUnit(unitConfig);
        dummy.transform.position = new Vector3(0, 0, -5);
    }

    /// <summary>
    /// ������λ
    /// </summary>
    /// <param name="unitConfig"></param>
    public BattleUnit CreateUnit(UnitConfig unitConfig)
    {
        BattleUnit battleUnit =  BattleMgr.Ins.CreateUnit(unitConfig);
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
            BattleUnit battleUnit = hit.collider.gameObject.GetComponent<BattleUnit>();
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
            BattleMgr.Ins.AllUnitMove(hit.point);
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
