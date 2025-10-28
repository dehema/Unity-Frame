using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 世界地图的相机控制器
/// </summary>
public class CameraController_WorldMap : CameraController_Base
{

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        MsgMgr.Ins.DispatchEvent(MsgEvent.WorldMap_Camera_Move, mainCamera.transform.position);
    }

    protected override void Update()
    {
        base.Update();
        if (IsCurrentFrameMoving)
        {
            MsgMgr.Ins.DispatchEvent(MsgEvent.WorldMap_Camera_Move, mainCamera.transform.position);
        }
        if (IsCurrentFrameZooming)
        {
            MsgMgr.Ins.DispatchEvent(MsgEvent.WorldMap_Camera_Zoom, mainCamera.orthographicSize);
        }
    }

    protected override void LoadConfig()
    {
        base.LoadConfig();

        Dictionary<string, GameSettingConfig> config = ConfigMgr.GameSetting.DataMap;

        Setting.PosLimit = ParseVector4("-1000, 1000, -1000, 1000");

        Setting.ZoomDefaultSize = float.Parse(config["camera_worldMap_zoomDefaultSize"].Val);
        Setting.ZoomMinSize = float.Parse(config["camera_worldMap_zoomMinSize"].Val);
        Setting.ZoomMaxSize = float.Parse(config["camera_worldMap_zoomMaxSize"].Val);
        Setting.MouseZoomSpeed = float.Parse(config["camera_worldMap_mouseZoomSpeed"].Val);
    }
}