using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.EventSystems;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// SLG类型游戏的相机控制器
/// </summary>
public class CameraController_City : MonoBehaviour
{
    [SerializeField][Header("相机组件")] public Camera mainCamera;                   // 主相机引用
    [Header("【相机设置】")]
    //鼠标专用
    [SerializeField][Header("鼠标移动速度")] public float mousePanSpeed = 1;
    [SerializeField][Header("鼠标缩放速度")] public const float mouseZoomSpeed = 3f;

    //移动
    [SerializeField][Header("移动范围限制")] private Vector4 posLimit = new Vector4(-60, 20, -20, 20);
    [SerializeField][Header("移动平滑度")] private float panDampening = 5f;
    [SerializeField][Header("触摸平移速度")] private float touchPanSpeed = 0.01f;
    [SerializeField][Header("触摸最小距离")] private float touchMinDistance = 10f;

    //缩放
    [SerializeField][Header("缩放默认值")] private float zoomDefaultSize = 15f;
    [SerializeField][Header("缩放最小值")] private float zoomMinSize = 3f;
    [SerializeField][Header("缩放最大值")] private float zoomMaxSize = 20f;
    [SerializeField][Header("触摸缩放速度")] private float touchZoomSpeed = 0.5f;
    [SerializeField][Header("缩放平滑度")] private float zoomDampening = 5f;

    // 当前目标正交尺寸
    private float targetOrthographicSize;
    // 相机目标位置
    private Vector3 targetPosition;
    // 鼠标中键拖动状态
    private bool isMiddleMouseDragging = false;
    private Vector3 lastMousePosition;
    // 触摸输入相关变量
    private bool isTouchPanning = false;
    private Vector2 lastTouchPosition;
    private float lastTouchDistance;
    private bool isTwoFingerZoom = false;

    public Vector4 PosLimit { get => posLimit; set => posLimit = value; }
    public float PanDampening { get => panDampening; set => panDampening = value; }
    public float TouchPanSpeed { get => touchPanSpeed; set => touchPanSpeed = value; }
    public float TouchMinDistance { get => touchMinDistance; set => touchMinDistance = value; }
    public float ZoomDefaultSize { get => zoomDefaultSize; set => zoomDefaultSize = value; }
    public float ZoomMinSize { get => zoomMinSize; set => zoomMinSize = value; }
    public float ZoomMaxSize { get => zoomMaxSize; set => zoomMaxSize = value; }
    public float TouchZoomSpeed { get => touchZoomSpeed; set => touchZoomSpeed = value; }
    public float ZoomDampening { get => zoomDampening; set => zoomDampening = value; }

    // 将位置限制在 posLimit 指定的范围内（X: xMin~xMax, Z: zMin~zMax）
    private Vector3 ClampPosition(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, PosLimit.x, PosLimit.y);
        float clampedZ = Mathf.Clamp(position.z, PosLimit.z, PosLimit.w);
        return new Vector3(clampedX, position.y, clampedZ);
    }

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }

        SceneMgr.Ins.Camera = mainCamera;
    }

    void Start()
    {
        mousePanSpeed = 1;

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

        SetTargetOrthographicSize(ZoomDefaultSize);

        // 初始化目标位置为当前位置并进行边界限制
        targetPosition = ClampPosition(mainCamera.transform.position);
        mainCamera.transform.position = targetPosition;
        MsgMgr.Ins.DispatchEvent(MsgEvent.CameraMove, mainCamera.transform.position);
    }

    // 每帧更新
    void Update()
    {
        if (mainCamera == null) return;

        // 处理相机缩放
        HandleZoom();

        // 处理相机平移
        HandlePanning();

        // 处理射线检测
        HandleRaycast();

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
        // 处理鼠标滚轮缩放（PC端）
        HandleMouseZoom();

        // 处理触摸缩放（移动端）
        HandleTouchZoom();

        if (mainCamera.orthographicSize == targetOrthographicSize)
        {
            return;
        }

        // 平滑过渡到目标正交尺寸
        float sizeDifference = Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize);
        if (sizeDifference > 0.01f)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, Time.deltaTime * ZoomDampening);
        }
        else
        {
            mainCamera.orthographicSize = targetOrthographicSize;
        }
        float zoomRatio = Mathf.InverseLerp(ZoomMaxSize, ZoomMinSize, mainCamera.orthographicSize);
        MsgMgr.Ins.DispatchEvent(MsgEvent.CameraZoomRatioChange, mainCamera.orthographicSize, zoomRatio);
    }

    /// <summary>
    /// 处理鼠标滚轮缩放
    /// </summary>
    private void HandleMouseZoom()
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
            targetOrthographicSize -= scrollInput * mouseZoomSpeed * zoomFactor;

            // 限制缩放范围
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, ZoomMinSize, ZoomMaxSize);
        }
    }

    /// <summary>
    /// 处理触摸缩放（两指缩放）
    /// </summary>
    private void HandleTouchZoom()
    {
        // 检查是否有两个触摸点
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // 计算两指之间的距离
            float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);

            // 如果是第一次检测到两指触摸，记录初始距离
            if (!isTwoFingerZoom)
            {
                lastTouchDistance = currentTouchDistance;
                isTwoFingerZoom = true;
            }
            else
            {
                // 计算距离变化
                float distanceDelta = currentTouchDistance - lastTouchDistance;

                // 只有当距离变化足够大时才进行缩放
                if (Mathf.Abs(distanceDelta) > TouchMinDistance * 0.1f)
                {
                    // 计算缩放因子
                    float zoomFactor = Mathf.Max(mainCamera.orthographicSize * 0.1f, 0.5f);

                    // 应用缩放（距离增加时缩小视角，距离减小时放大视角）
                    targetOrthographicSize -= distanceDelta * TouchZoomSpeed * zoomFactor * 0.01f;

                    // 限制缩放范围
                    targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, ZoomMinSize, ZoomMaxSize);
                }

                // 更新上次触摸距离
                lastTouchDistance = currentTouchDistance;
            }
        }
        else
        {
            // 没有两指触摸时，重置两指缩放状态
            isTwoFingerZoom = false;
        }
    }

    /// <summary>
    /// 处理相机平移（鼠标拖动和触摸拖动）
    /// </summary>
    private void HandlePanning()
    {
        // 处理鼠标平移（PC端）
        HandleMousePanning();

        // 处理触摸平移（移动端）
        HandleTouchPanning();

        if (mainCamera.transform.position == targetPosition)
        {
            return;
        }

        // 平滑过渡到目标位置（应用边界限制）
        Vector3 positionDifference = mainCamera.transform.position - targetPosition;
        float distance = positionDifference.magnitude;
        if (distance > 0.01f) // 当距离大于0.01时才继续插值
        {
            // 先对目标位置进行边界限制
            targetPosition = ClampPosition(targetPosition);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * PanDampening);
        }
        else
        {
            // 当距离很小时，直接设置为目标位置，避免无限插值
            mainCamera.transform.position = targetPosition;
        }
        MsgMgr.Ins.DispatchEvent(MsgEvent.CameraMove, mainCamera.transform.position);
    }

    /// <summary>
    /// 处理鼠标平移
    /// </summary>
    private void HandleMousePanning()
    {
        // 检测鼠标左键按下状态
        if (Input.GetMouseButtonDown(0)) // 鼠标左键按下
        {
            isMiddleMouseDragging = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0)) // 鼠标左键释放
        {
            isMiddleMouseDragging = false;
        }

        // 处理鼠标拖动
        if (isMiddleMouseDragging)
        {
            // 计算鼠标位置差值
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // 根据相机正交尺寸调整移动速度（缩放越大，移动速度越快）
            //float moveSpeedFactor = mainCamera.orthographicSize / defaultOrthographicSize;
            float moveSpeedFactor = 1;

            // 计算相机移动方向和距离
            // 注意：鼠标向右移动，相机应该向左移动，所以使用负值
            // 使用世界坐标系的XZ平面移动
            Vector3 moveDirection = new Vector3(-mouseDelta.x * moveSpeedFactor * mousePanSpeed * Time.deltaTime, 0, -mouseDelta.y * moveSpeedFactor * mousePanSpeed * Time.deltaTime);

            // 直接在世界坐标系中移动，不需要转换
            // 考虑相机的旋转角度，确保移动方向正确
            moveDirection = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * moveDirection;
            moveDirection.y = 0; // 确保Y轴不变

            // 更新目标位置并进行边界限制
            targetPosition = ClampPosition(targetPosition + moveDirection);

            // 更新鼠标位置记录
            lastMousePosition = Input.mousePosition;
        }
    }

    /// <summary>
    /// 处理触摸平移（单指拖动）
    /// </summary>
    private void HandleTouchPanning()
    {
        // 只有在单指触摸且不是两指缩放时才处理平移
        if (Input.touchCount == 1 && !isTwoFingerZoom)
        {
            Touch touch = Input.GetTouch(0);

            // 检测触摸开始
            if (touch.phase == TouchPhase.Began)
            {
                isTouchPanning = true;
                lastTouchPosition = touch.position;
            }
            // 检测触摸移动
            else if (touch.phase == TouchPhase.Moved && isTouchPanning)
            {
                // 计算触摸位置差值
                Vector2 touchDelta = touch.position - lastTouchPosition;

                // 根据相机正交尺寸调整移动速度
                float moveSpeedFactor = 1;

                // 计算相机移动方向和距离
                // 注意：触摸向右移动，相机应该向左移动，所以使用负值
                float factor = moveSpeedFactor * TouchPanSpeed;
                Vector3 moveDirection = new Vector3(-touchDelta.x * factor, 0, -touchDelta.y * factor);

                // 考虑相机的旋转角度，确保移动方向正确
                moveDirection = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * moveDirection;
                moveDirection.y = 0; // 确保Y轴不变

                // 更新目标位置并进行边界限制
                targetPosition = ClampPosition(targetPosition + moveDirection);

                // 更新触摸位置记录
                lastTouchPosition = touch.position;
            }
            // 检测触摸结束
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouchPanning = false;
            }
        }
        else if (Input.touchCount == 0)
        {
            // 没有触摸时，重置触摸平移状态
            isTouchPanning = false;
        }
    }

    /// <summary>
    /// 处理射线检测
    /// </summary>
    private void HandleRaycast()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            bool isOverUI = EventSystem.current.IsPointerOverGameObject();
            if (isOverUI)
            {
                return; // 点到UI
            }
            // 从相机位置发射射线到鼠标位置
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 只检测Layer 7的对象
            int layerMask = 1 << 7; // Layer 7

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // 检查命中的对象是否有BuildingController脚本
                BuildingController buildingController = hit.collider.GetComponent<BuildingController>();
                if (buildingController != null)
                {
                    // 触发BuildingController的SelectBuilding函数
                    buildingController.OnSelect();
                }
            }
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

                // 应用边界限制
                newPosition = ClampPosition(newPosition);
                mainCamera.transform.position = newPosition;
                targetPosition = newPosition;
            }
            else
            {
                // 如果是水平相机，直接将XZ坐标设为0
                Vector3 newPosition = new Vector3(0f, currentPosition.y, 0f);
                // 应用边界限制
                newPosition = ClampPosition(newPosition);
                mainCamera.transform.position = newPosition;
                targetPosition = newPosition;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraController_City))]
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
        CameraController_City cameraController = (CameraController_City)target;

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
