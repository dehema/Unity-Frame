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
    public static CameraController_WorldMap Ins { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        Ins = this;
        MsgMgr.Ins.AddEventListener(MsgEvent.SceneLoaded, OnSceneLoaded, this);
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

    /// <summary>
    /// 场景加载
    /// </summary>
    private void OnSceneLoaded(object[] param)
    {
        SceneChangeParam sceneChangeParam = param.Length > 0 ? param[0] as SceneChangeParam : null;
        if (sceneChangeParam != null)
        {
            if (sceneChangeParam.sceneID == SceneID.WorldMap)
            {
                LookAtTile(WorldMapMgr.Ins.WorldMapData.cityPos, false);
            }
        }
    }

    /// <summary>
    /// 看向地块
    /// </summary>
    /// <param name="_tilePos"></param>
    public void LookAtTile(Vector2Int _tilePos, bool _isTween = false)
    {
        Vector3 worldPos = WorldMapMgr.Ins.TilePosToWorldPos(_tilePos);
        SetCameraPosLookAtPos(worldPos, _isTween);
        WorldMapMgr.Ins.CheckAndLoadAreas();
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

    #region 射线
    Vector3 _pressMousePos;
    protected override void HandleRaycast()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            _pressMousePos = Input.mousePosition;
        }
        else if (Input.mousePosition == _pressMousePos && Input.GetMouseButtonUp(0))
        {
            if (GetHandleRaycast())
            {
                OnSelectTile(RaycastHit.point);
            }
        }
    }

    /// <summary>
    /// 选择地图
    /// </summary>
    /// <param name="_worldPos"></param>
    public void OnSelectTile(Vector3 _worldPos)
    {
        WorldMapMgr.Ins.SelectTile(_worldPos);
    }

    #endregion
}