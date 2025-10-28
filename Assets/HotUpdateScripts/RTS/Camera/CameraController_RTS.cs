using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// SLG类型游戏的相机控制器
/// </summary>
public class CameraController_RTS : MonoBehaviour
{
    [Header("相机设置")]
    [Tooltip("相机组件")]
    public Camera mainCamera;                   // 主相机引用

    [Tooltip("默认尺寸")]
    public float defaultOrthographicSize = 8f; // 默认正交尺寸（初始视角大小）
    [Header("缩放设置")]
    [Tooltip("最小正交尺寸")]
    public float minOrthographicSize = 3f;      // 最小缩放值（放大极限）
    [Tooltip("最大正交尺寸")]
    public float maxOrthographicSize = 15f;     // 最大缩放值（缩小极限）
    [Tooltip("缩放速度")]
    public const float zoomSpeed = 3f;          // 缩放速度
    [Tooltip("缩放平滑度")]
    public float zoomDampening = 5f;            // 缩放平滑度

    [Header("平移设置")]
    [Tooltip("相机移动速度")]
    public float panSpeed = 1;                  // 相机平移速度
    [Tooltip("相机移动平滑度")]
    public float panDampening = 2f;             // 相机平移平滑度


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
        panSpeed = 1;
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
        mainCamera.nearClipPlane = -20;

        // 初始化目标正交尺寸为当前尺寸
        if (mainCamera != null)
        {
            targetOrthographicSize = mainCamera.orthographicSize;
        }

        SetTargetOrthographicSize(defaultOrthographicSize);

        // 初始化目标位置为当前位置
        targetPosition = mainCamera.transform.position;
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

        if (mainCamera.orthographicSize == targetOrthographicSize)
        {
            return;
        }

        // 平滑过渡到目标正交尺寸
        float sizeDifference = Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize);
        if (sizeDifference > 0.1f)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, Time.deltaTime * zoomDampening);
        }
        else
        {
            mainCamera.orthographicSize = targetOrthographicSize;
        }
        float zoomRatio = Mathf.InverseLerp(maxOrthographicSize, minOrthographicSize, mainCamera.orthographicSize);
        MsgMgr.Ins.DispatchEvent(MsgEvent.RTS_Camera_Zoom, mainCamera.orthographicSize, zoomRatio);
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
            //float moveSpeedFactor = mainCamera.orthographicSize / defaultOrthographicSize;
            float moveSpeedFactor = 1;
            float aspectRatio = mainCamera.aspect;

            // 计算相机移动方向和距离
            // 注意：鼠标向右移动，相机应该向左移动，所以使用负值
            // 使用世界坐标系的XZ平面移动
            Vector3 moveDirection = new Vector3(
                -mouseDelta.x * moveSpeedFactor * panSpeed * Time.deltaTime,
                0,
                -mouseDelta.y * moveSpeedFactor * aspectRatio * panSpeed * Time.deltaTime
            );

            // 直接在世界坐标系中移动，不需要转换
            // 考虑相机的旋转角度，确保移动方向正确
            moveDirection = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * moveDirection;
            moveDirection.y = 0; // 确保Y轴不变

            // 更新目标位置
            targetPosition += moveDirection;

            // 更新鼠标位置记录
            lastMousePosition = Input.mousePosition;
        }

        // 平滑过渡到目标位置
        if (mainCamera.transform.position != targetPosition)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                Time.deltaTime * panDampening
            );
        }
    }

    /// <summary>
    /// 调整相机位置使其视野中心对准世界原点(0,0,0)（仅修改X和Z坐标）
    /// </summary>
    public void ResetCameraToWorldCenter()
    {
        if (mainCamera != null)
        {
            Vector3 currentPosition = mainCamera.transform.position;

            // 计算从相机位置到世界原点的射线
            Vector3 worldCenter = Vector3.zero;

            // 对于正交相机，我们需要计算相机应该移动到哪里才能让视野中心对准世界原点
            // 使用相机的前向向量投影到XZ平面上
            Vector3 cameraForward = mainCamera.transform.forward;

            // 如果相机是俯视角度，计算相机在XZ平面上应该的位置
            if (Mathf.Abs(cameraForward.y) > 0.01f) // 确保不是完全水平的相机
            {
                // 计算从当前Y高度到地面(Y=0)的距离比例
                float distanceToGround = currentPosition.y / Mathf.Abs(cameraForward.y);

                // 计算相机在XZ平面上的偏移量，使得视线中心指向世界原点
                Vector3 offsetOnGround = new Vector3(cameraForward.x, 0, cameraForward.z).normalized * distanceToGround;

                // 计算新的相机位置（只修改X和Z）
                Vector3 newPosition = new Vector3(
                    worldCenter.x - offsetOnGround.x,
                    currentPosition.y, // 保持Y坐标不变
                    worldCenter.z - offsetOnGround.z
                );

                mainCamera.transform.position = newPosition;
                targetPosition = newPosition;
            }
            else
            {
                // 如果是水平相机，直接将XZ坐标设为0
                Vector3 newPosition = new Vector3(0f, currentPosition.y, 0f);
                mainCamera.transform.position = newPosition;
                targetPosition = newPosition;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraController_RTS))]
public class CameraController_RTSEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector界面
        DrawDefaultInspector();

        // 添加分隔线
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("编辑器工具", EditorStyles.boldLabel);

        // 获取目标组件
        CameraController_RTS cameraController = (CameraController_RTS)target;

        // 添加重置相机位置按钮
        if (GUILayout.Button("调整相机视野中心到世界原点 (0,0,0)"))
        {
            cameraController.ResetCameraToWorldCenter();

            // 标记场景为已修改（如果在编辑模式下）
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(cameraController);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(cameraController.gameObject.scene);
            }
        }

        // 显示当前相机位置信息
        if (cameraController.mainCamera != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("当前相机信息", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"位置: {cameraController.mainCamera.transform.position}");
            EditorGUILayout.LabelField($"角度: {cameraController.mainCamera.transform.eulerAngles}");
            EditorGUILayout.LabelField($"正交尺寸: {cameraController.mainCamera.orthographicSize:F2}");
        }
    }
}
#endif
