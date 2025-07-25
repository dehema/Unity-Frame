using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SLG类型游戏的相机控制器
/// 实现类似文明6的相机效果，包括滚轮缩放
/// </summary>
public class CameraController_SLG : MonoBehaviour
{
    [Header("相机设置")]
    [Tooltip("相机组件")]
    public Camera mainCamera;                   // 主相机引用

    [Tooltip("缩放平滑度")]
    private float defaultOrthographicSize = 8f; // 默认正交尺寸（初始视角大小）
    [Header("缩放设置")]
    [Tooltip("最小正交尺寸")]
    public float minOrthographicSize = 3f;      // 最小缩放值（放大极限）
    [Tooltip("最大正交尺寸")]
    public float maxOrthographicSize = 15f;     // 最大缩放值（缩小极限）
    [Tooltip("缩放速度")]
    public const float zoomSpeed = 3f;          // 缩放速度
    [Tooltip("缩放平滑度")]
    public float zoomDampening = 5f;           // 缩放平滑度


    // 当前目标正交尺寸
    private float targetOrthographicSize;

    // 初始化
    void Start()
    {
        // 如果没有指定相机，则使用当前物体上的相机组件
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }

        // 确保相机为正交投影
        if (mainCamera != null && !mainCamera.orthographic)
        {
            Debug.LogWarning("相机不是正交投影模式，已自动切换为正交投影模式");
            mainCamera.orthographic = true;
        }

        // 初始化目标正交尺寸为当前尺寸
        if (mainCamera != null)
        {
            targetOrthographicSize = mainCamera.orthographicSize;
        }

        SetTargetOrthographicSize(defaultOrthographicSize);
    }

    // 每帧更新
    void Update()
    {
        if (mainCamera == null) return;

        // 处理相机缩放
        HandleZoom();
    }

    /// <summary>
    /// 设置正交尺寸
    /// </summary>
    private void SetTargetOrthographicSize(float _size)
    {
        mainCamera.orthographicSize = _size;
        targetOrthographicSize = _size;
    }

    /// <summary>
    /// 处理相机缩放
    /// </summary>
    private void HandleZoom()
    {
        // 获取鼠标滚轮输入（正值为向上滚动，负值为向下滚动）
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // 根据滚轮输入调整目标正交尺寸
        // 向上滚动（正值）减小正交尺寸（放大视角）
        // 向下滚动（负值）增加正交尺寸（缩小视角）
        if (scrollInput != 0)
        {
            // 计算缩放因子，根据当前正交尺寸动态调整缩放速度
            // 当视角放大时，缩放速度减小；当视角缩小时，缩放速度增加
            float zoomFactor = Mathf.Max(mainCamera.orthographicSize * 0.1f, 0.5f);

            // 应用缩放
            targetOrthographicSize -= scrollInput * zoomSpeed * zoomFactor;

            // 限制缩放范围
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);
        }

        // 平滑过渡到目标正交尺寸
        if (mainCamera.orthographicSize != targetOrthographicSize)
        {
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                targetOrthographicSize,
                Time.deltaTime * zoomDampening
            );
        }
    }
}
