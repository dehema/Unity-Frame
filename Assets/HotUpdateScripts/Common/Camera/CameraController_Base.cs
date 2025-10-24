using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 相机控制器基类 - 管理可修改的运行时数据和只读的配置数据
/// </summary>
public class CameraController_Base : MonoBehaviour
{
    [Header("相机组件")]
    public Camera mainCamera;

    [SerializeField] private CameraController_Setting _setting;
    protected CameraController_Setting Setting => _setting;

    // 当前目标正交尺寸
    protected float targetOrthographicSize;
    // 相机目标位置
    protected Vector3 targetPosition;
    // 鼠标中键拖动状态
    protected bool isMiddleMouseDragging = false;
    protected Vector3 lastMousePosition;
    // 触摸输入相关变量
    protected Vector2 lastTouchPosition;
    protected float lastTouchDistance;
    //当前帧是否移动
    protected bool isTouchPanning = false;
    //当前帧是否缩放
    protected bool isTwoFingerZoom = false;

    protected virtual void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponent<Camera>();
        }

        SceneMgr.Ins.Camera = mainCamera;
    }

    protected virtual void Start()
    {
        LoadConfig();
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

        SetTargetOrthographicSize(Setting.ZoomDefaultSize);

        // 初始化目标位置为当前位置并进行边界限制
        targetPosition = ClampPosition(mainCamera.transform.position);
        mainCamera.transform.position = targetPosition;
    }

    // 每帧更新
    protected virtual void Update()
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
    /// 处理相机缩放
    /// </summary>
    private void HandleZoom()
    {
        // 处理鼠标滚轮缩放（PC端）
#if UNITY_STANDALONE || UNITY_EDITOR
        HandleMouseZoom();
#endif

        // 处理触摸缩放（移动端）
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchZoom();
#endif

        if (mainCamera.orthographicSize == targetOrthographicSize)
        {
            return;
        }

        // 平滑过渡到目标正交尺寸
        float sizeDifference = Mathf.Abs(mainCamera.orthographicSize - targetOrthographicSize);
        if (sizeDifference > 0.01f)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetOrthographicSize, Time.deltaTime * Setting.ZoomDampening);
        }
        else
        {
            mainCamera.orthographicSize = targetOrthographicSize;
        }
        float zoomRatio = Mathf.InverseLerp(Setting.ZoomMaxSize, Setting.ZoomMinSize, mainCamera.orthographicSize);
        MsgMgr.Ins.DispatchEvent(MsgEvent.City_Camera_Zoom, mainCamera.orthographicSize, zoomRatio);
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
            targetOrthographicSize -= scrollInput * Setting.MouseZoomSpeed * zoomFactor;

            // 限制缩放范围
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, Setting.ZoomMinSize, Setting.ZoomMaxSize);
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
                if (Mathf.Abs(distanceDelta) > Setting.TouchMinDistance * 0.1f)
                {
                    // 计算缩放因子
                    float zoomFactor = Mathf.Max(mainCamera.orthographicSize * 0.1f, 0.5f);

                    // 应用缩放（距离增加时缩小视角，距离减小时放大视角）
                    targetOrthographicSize -= distanceDelta * Setting.TouchZoomSpeed * zoomFactor * 0.01f;

                    // 限制缩放范围
                    targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, Setting.ZoomMinSize, Setting.ZoomMaxSize);
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
#if UNITY_STANDALONE || UNITY_EDITOR
        HandleMousePanning();
#endif

        // 处理触摸平移（移动端）
#if UNITY_ANDROID || UNITY_IOS
        HandleTouchPanning();
#endif

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
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * Setting.PanDampening);
        }
        else
        {
            // 当距离很小时，直接设置为目标位置，避免无限插值
            mainCamera.transform.position = targetPosition;
        }
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

            // 根据屏幕尺寸和相机正交尺寸计算移动比例
            // 屏幕像素到世界坐标的转换比例
            float screenToWorldRatio = (mainCamera.orthographicSize * 2) / Screen.height;

            // 计算相机移动方向和距离
            // 注意：鼠标向右移动，相机应该向左移动，所以使用负值
            Vector3 moveDirection = new Vector3(
                -mouseDelta.x * screenToWorldRatio * Setting.MousePanSpeed,
                0,
                -mouseDelta.y * screenToWorldRatio * Setting.MousePanSpeed
            );

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

                // 根据屏幕尺寸和相机正交尺寸计算移动比例
                // 屏幕像素到世界坐标的转换比例
                float screenToWorldRatio = (mainCamera.orthographicSize * 2) / Screen.height;

                // 计算相机移动方向和距离
                // 注意：触摸向右移动，相机应该向左移动，所以使用负值
                Vector3 moveDirection = new Vector3(
                    -touchDelta.x * screenToWorldRatio * Setting.TouchPanSpeed,
                    0,
                    -touchDelta.y * screenToWorldRatio * Setting.TouchPanSpeed
                );

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
    protected virtual void HandleRaycast()
    {

    }

    protected virtual GameObject GetHandleRaycast()
    {
        bool isOverUI = EventSystem.current.IsPointerOverGameObject();
        if (isOverUI)
        {
            return null; // 点到UI
        }
        // 从相机位置发射射线到鼠标位置
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 只检测Layer 7的对象
        int layerMask = 1 << 7; // Layer 7

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.collider.gameObject;
        }
        return null;
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

    protected virtual void LoadConfig()
    {
        Dictionary<string, GameSettingConfig> config = ConfigMgr.GameSetting.DataMap;

        Setting.MousePanSpeed = float.Parse(config["camera_mousePanSpeed"].Val);
        Setting.MouseZoomSpeed = float.Parse(config["camera_mouseZoomSpeed"].Val);

        Setting.PanDampening = float.Parse(config["camera_panDampening"].Val);
        Setting.TouchPanSpeed = float.Parse(config["camera_touchPanSpeed"].Val);
        Setting.TouchMinDistance = float.Parse(config["camera_touchMinDistance"].Val);

        Setting.ZoomDefaultSize = float.Parse(config["camera_zoomDefaultSize"].Val);
        Setting.ZoomMinSize = float.Parse(config["camera_zoomMinSize"].Val);
        Setting.ZoomMaxSize = float.Parse(config["camera_zoomMaxSize"].Val);
        Setting.TouchZoomSpeed = float.Parse(config["camera_touchZoomSpeed"].Val);
        Setting.ZoomDampening = float.Parse(config["camera_zoomDampening"].Val);
    }

    protected Vector4 ParseVector4(string value)
    {
        string[] parts = value.Split(',');
        return new Vector4(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2]),
            float.Parse(parts[3])
        );
    }

    /// <summary>
    /// 设置正交尺寸
    /// </summary>
    protected void SetTargetOrthographicSize(float _size)
    {
        mainCamera.orthographicSize = _size;
        targetOrthographicSize = _size;
    }

    // 将位置限制在 posLimit 指定的范围内（X: xMin~xMax, Z: zMin~zMax）
    protected Vector3 ClampPosition(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, Setting.PosLimit.x, Setting.PosLimit.y);
        float clampedZ = Mathf.Clamp(position.z, Setting.PosLimit.z, Setting.PosLimit.w);
        return new Vector3(clampedX, position.y, clampedZ);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraController_Base))]
public class CameraController_Base_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector界面
        DrawDefaultInspector();

        // 添加分隔线
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("编辑器工具", EditorStyles.boldLabel);

        // 获取目标组件
        CameraController_Base cameraController = (CameraController_Base)target;

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
