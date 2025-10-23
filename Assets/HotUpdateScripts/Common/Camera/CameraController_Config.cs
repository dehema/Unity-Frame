using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机配置 - 只读类，用于存储从配置表读取的初始数据
/// </summary>
[System.Serializable]
public class CameraController_Config
{
    [Header("【相机设置】")]
    //鼠标专用
    [SerializeField][Header("鼠标移动速度")] private readonly float _mousePanSpeed = 1;
    [SerializeField][Header("鼠标缩放速度")] private readonly float _mouseZoomSpeed = 3f;

    //移动
    [SerializeField][Header("移动范围限制")] private readonly Vector4 _posLimit = new Vector4(-60, 20, -20, 20);
    [SerializeField][Header("移动平滑度")] private readonly float _panDampening = 5f;
    [SerializeField][Header("触摸平移速度")] private readonly float _touchPanSpeed = 0.01f;
    [SerializeField][Header("触摸最小距离")] private readonly float _touchMinDistance = 10f;

    //缩放
    [SerializeField][Header("缩放默认值")] private readonly float _zoomDefaultSize = 15f;
    [SerializeField][Header("缩放最小值")] private readonly float _zoomMinSize = 3f;
    [SerializeField][Header("缩放最大值")] private readonly float _zoomMaxSize = 20f;
    [SerializeField][Header("触摸缩放速度")] private readonly float _touchZoomSpeed = 0.5f;
    [SerializeField][Header("缩放平滑度")] private readonly float _zoomDampening = 5f;

    // 只读属性 - 提供对配置数据的只读访问
    public float MousePanSpeed => _mousePanSpeed;
    public float MouseZoomSpeed => _mouseZoomSpeed;
    public Vector4 PosLimit => _posLimit;
    public float PanDampening => _panDampening;
    public float TouchPanSpeed => _touchPanSpeed;
    public float TouchMinDistance => _touchMinDistance;
    public float ZoomDefaultSize => _zoomDefaultSize;
    public float ZoomMinSize => _zoomMinSize;
    public float ZoomMaxSize => _zoomMaxSize;
    public float TouchZoomSpeed => _touchZoomSpeed;
    public float ZoomDampening => _zoomDampening;

    /// <summary>
    /// 从配置表创建配置对象
    /// </summary>
    public static CameraController_Config CreateFromConfigTable()
    {
        return new CameraController_Config
        {
        };
    }
}
