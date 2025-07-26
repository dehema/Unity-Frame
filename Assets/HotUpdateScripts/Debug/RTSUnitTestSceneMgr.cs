using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RTSUnitTestSceneMgr : MonoBehaviour
{
    [Header("单位设置")]
    GameObject unit;
    NavMeshAgent agent;

    [Header("射线设置")]
    [SerializeField]
    Camera mainCamera;     // 主相机引用

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    /// <summary>
    /// 创建单位
    /// </summary>
    /// <param name="unitConfig"></param>
    public void CreateUnit(UnitConfig unitConfig)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefab/Unit/" + unitConfig.fullID);
        if (prefab != null)
        {
            // 实例化单位预制体
            unit = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            // 获取或添加NavMeshAgent组件
            agent = unit.GetComponent<NavMeshAgent>();

            // 配置NavMeshAgent基本属性
            agent.speed = unitConfig.moveSpeed;
            agent.acceleration = 999;
            agent.angularSpeed = unitConfig.angularSpeed;
        }
    }

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            MoveUnitToMousePosition();
        }
    }

    /// <summary>
    /// 移动单位到鼠标点击位置
    /// </summary>
    private void MoveUnitToMousePosition()
    {
        // 确保单位和代理组件存在
        if (unit == null || agent == null)
        {
            Debug.LogWarning("单位或NavMeshAgent组件不存在，请先创建单位");
            return;
        }

        // 从相机发射射线到鼠标位置
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(mainCamera.transform.position, ray.direction, Color.red);
        RaycastHit hit;
        // 如果射线击中了地面
        if (Physics.Raycast(ray, out hit, 1000f, 1 << GameObjectLayer.Scene3D))
        {
            // 设置导航目标位置
            agent.SetDestination(hit.point);
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
