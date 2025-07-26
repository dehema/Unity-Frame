using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RTSUnitTestSceneMgr : MonoBehaviour
{
    [Header("��λ����")]
    GameObject unit;
    NavMeshAgent agent;

    [Header("��������")]
    [SerializeField]
    Camera mainCamera;     // ���������

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// ������λ
    /// </summary>
    /// <param name="unitConfig"></param>
    public void CreateUnit(UnitConfig unitConfig)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefab/Unit/" + unitConfig.fullID);
        if (prefab != null)
        {
            // ʵ������λԤ����
            unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            // ��ȡ�����NavMeshAgent���
            agent = unit.GetComponent<NavMeshAgent>();

            // ����NavMeshAgent��������
            agent.speed = unitConfig.moveSpeed;
            agent.acceleration = 999;
            agent.angularSpeed = unitConfig.angularSpeed;
        }
    }

    void Update()
    {
        // ������������
        if (Input.GetMouseButtonDown(0))
        {
            MoveUnitToMousePosition();
        }
    }

    /// <summary>
    /// �ƶ���λ�������λ��
    /// </summary>
    private void MoveUnitToMousePosition()
    {
        // ȷ����λ�ʹ����������
        if (unit == null || agent == null)
        {
            Debug.LogWarning("��λ��NavMeshAgent��������ڣ����ȴ�����λ");
            return;
        }

        // ������������ߵ����λ��
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mainCamera.transform.position, ray.direction, Color.red);
        RaycastHit hit;
        // ������߻����˵���
        if (Physics.Raycast(ray, out hit, 1000f, 1 << GameObjectLayer.Scene3D))
        {
            // ���õ���Ŀ��λ��
            agent.SetDestination(hit.point);
            Debug.Log("��λ�ƶ���: " + hit.point);
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
