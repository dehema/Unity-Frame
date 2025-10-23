using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机控制器基类 - 管理可修改的运行时数据和只读的配置数据
/// </summary>
public class CameraController_Base : MonoBehaviour
{
    [Header("相机组件")]
    public Camera mainCamera;
    
    [Header("配置数据（只读）")]
    [SerializeField] private CameraController_Config _configData; // 只读配置数据，作为源数据
    
    [Header("运行时数据（可修改）")]
    // 当前目标正交尺寸
    public float targetOrthographicSize;
    // 相机目标位置
    public Vector3 targetPosition;
    // 鼠标中键拖动状态
    public bool isMiddleMouseDragging = false;
    public Vector3 lastMousePosition;
    // 触摸输入相关变量
    public Vector2 lastTouchPosition;
    public float lastTouchDistance;
    //当前帧是否移动
    public bool isTouchPanning = false;
    //当前帧是否缩放
    public bool isTwoFingerZoom = false;

    // 运行时可修改的数据
    private float _currentMousePanSpeed;
    private Vector4 _currentPosLimit;
    private float _currentPanDampening;
    private float _currentTouchPanSpeed;
    private float _currentTouchMinDistance;
    private float _currentZoomDefaultSize;
    private float _currentZoomMinSize;
    private float _currentZoomMaxSize;
    private float _currentTouchZoomSpeed;
    private float _currentZoomDampening;

    // 初始化方法 - 从配置表加载数据
    protected virtual void InitializeFromConfig()
    {
        if (_configData == null)
        {
            Debug.LogError("CameraController_Base: 配置数据未设置！");
            return;
        }

        // 从配置数据初始化运行时数据
        _currentMousePanSpeed = _configData.MousePanSpeed;
        _currentPosLimit = _configData.PosLimit;
        _currentPanDampening = _configData.PanDampening;
        _currentTouchPanSpeed = _configData.TouchPanSpeed;
        _currentTouchMinDistance = _configData.TouchMinDistance;
        _currentZoomDefaultSize = _configData.ZoomDefaultSize;
        _currentZoomMinSize = _configData.ZoomMinSize;
        _currentZoomMaxSize = _configData.ZoomMaxSize;
        _currentTouchZoomSpeed = _configData.TouchZoomSpeed;
        _currentZoomDampening = _configData.ZoomDampening;
    }

    // 重置到配置数据
    public void ResetToConfig()
    {
        InitializeFromConfig();
    }

    // 只读属性 - 从配置数据读取
    public float MouseZoomSpeed => _configData?.MouseZoomSpeed ?? 3f;

    // 可修改属性 - 操作运行时数据
    public float MousePanSpeed 
    { 
        get => _currentMousePanSpeed; 
        set => _currentMousePanSpeed = value; 
    }
    
    public Vector4 PosLimit 
    { 
        get => _currentPosLimit; 
        set => _currentPosLimit = value; 
    }
    
    public float PanDampening 
    { 
        get => _currentPanDampening; 
        set => _currentPanDampening = value; 
    }
    
    public float TouchPanSpeed 
    { 
        get => _currentTouchPanSpeed; 
        set => _currentTouchPanSpeed = value; 
    }
    
    public float TouchMinDistance 
    { 
        get => _currentTouchMinDistance; 
        set => _currentTouchMinDistance = value; 
    }
    
    public float ZoomDefaultSize 
    { 
        get => _currentZoomDefaultSize; 
        set => _currentZoomDefaultSize = value; 
    }
    
    public float ZoomMinSize 
    { 
        get => _currentZoomMinSize; 
        set => _currentZoomMinSize = value; 
    }
    
    public float ZoomMaxSize 
    { 
        get => _currentZoomMaxSize; 
        set => _currentZoomMaxSize = value; 
    }
    
    public float TouchZoomSpeed 
    { 
        get => _currentTouchZoomSpeed; 
        set => _currentTouchZoomSpeed = value; 
    }
    
    public float ZoomDampening 
    { 
        get => _currentZoomDampening; 
        set => _currentZoomDampening = value; 
    }

    // 设置配置数据
    public void SetConfigData(CameraController_Config configData)
    {
        _configData = configData;
        InitializeFromConfig();
    }

    // 获取配置数据（只读）
    public CameraController_Config GetConfigData()
    {
        return _configData;
    }
}