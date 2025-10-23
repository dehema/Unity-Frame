using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机配置 - 只读类，用于存储从配置表读取的初始数据
/// </summary>
[System.Serializable]
public class CameraController_Setting
{
    [Header("【相机设置】")]
    //鼠标专用
    [SerializeField][Header("鼠标移动速度")] public float MousePanSpeed = 1;
    [SerializeField][Header("鼠标缩放速度")] public float MouseZoomSpeed = 3f;

    //移动
    [SerializeField][Header("移动范围限制")] public Vector4 PosLimit = new Vector4(-60, 20, -20, 20);
    [SerializeField][Header("移动平滑度")] public float PanDampening = 5f;
    [SerializeField][Header("触摸平移速度")] public float TouchPanSpeed = 0.01f;
    [SerializeField][Header("触摸最小距离")] public float TouchMinDistance = 10f;

    //缩放
    [SerializeField][Header("缩放默认值")] public float ZoomDefaultSize = 15f;
    [SerializeField][Header("缩放最小值")] public float ZoomMinSize = 3f;
    [SerializeField][Header("缩放最大值")] public float ZoomMaxSize = 20f;
    [SerializeField][Header("触摸缩放速度")] public float TouchZoomSpeed = 0.5f;
    [SerializeField][Header("缩放平滑度")] public float ZoomDampening = 5f;

}
