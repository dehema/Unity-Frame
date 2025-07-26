using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SLG类型游戏的相机控制器
/// 实现类似文明6的相机效果，包括滚轮缩放
/// </summary>
public class CameraController_RTS : MonoBehaviour
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
    
    [Header("平移设置")]
    [Tooltip("相机移动速度")]
    public float panSpeed = 10f;               // 相机平移速度
    [Tooltip("相机移动平滑度")]
    public float panDampening = 2f;            // 相机平移平滑度


    // 当前目标正交尺寸
    private float targetOrthographicSize;
    
    // 相机目标位置
    private Vector3 targetPosition;
    
    // 鼠标中键拖动状态
    private bool isMiddleMouseDragging = false;
    private Vector3 lastMousePosition;

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
        
        // 初始化目标位置为当前位置
        targetPosition = transform.position;
    }

    // 每帧更新
    void Update()
    {
        if (mainCamera == null) return;

        // 处理相机缩放
        HandleZoom();
        
        // 处理相机平移
        HandlePanning();
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
    
    /// <summary>
    /// 处理相机平移（鼠标中键拖动）
    /// </summary>
    private void HandlePanning()
    {
        // 检测鼠标中键按下状态
        if (Input.GetMouseButtonDown(2)) // 鼠标中键按下
        {
            isMiddleMouseDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(2)) // 鼠标中键释放
        {
            isMiddleMouseDragging = false;
        }
        
        // 处理鼠标中键拖动
        if (isMiddleMouseDragging)
        {
            // 计算鼠标位置差值
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            
            // 根据相机正交尺寸调整移动速度（缩放越大，移动速度越快）
            float moveSpeedFactor = mainCamera.orthographicSize / defaultOrthographicSize;
            
            // 计算相机移动方向和距离
            // 注意：鼠标向右移动，相机应该向左移动，所以使用负值
            Vector3 moveDirection = new Vector3(
                -mouseDelta.x * moveSpeedFactor * panSpeed * Time.deltaTime,
                -mouseDelta.y * moveSpeedFactor * panSpeed * Time.deltaTime,
                0
            );
            
            // 将屏幕空间移动转换为世界空间移动
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.z = 0; // 确保相机只在XY平面移动
            
            // 更新目标位置
            targetPosition += moveDirection;
            
            // 更新鼠标位置记录
            lastMousePosition = Input.mousePosition;
        }
        
        // 平滑过渡到目标位置
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * panDampening
            );
        }
    }
}
