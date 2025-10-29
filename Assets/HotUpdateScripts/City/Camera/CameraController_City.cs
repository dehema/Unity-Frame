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
public class CameraController_City : CameraController_Base
{
    public static CameraController_City Ins { get; private set; }
    float buildingZoom; //建筑选中缩放值
    float defaultZoom;  //默认缩放值

    protected override void Awake()
    {
        base.Awake();
        Ins = this;
    }

    protected override void Start()
    {
        base.Start();
        MsgMgr.Ins.DispatchEvent(MsgEvent.City_Camera_Move, mainCamera.transform.position);
    }

    protected override void Update()
    {
        base.Update();
        if (IsCurrentFrameMoving)
        {
            MsgMgr.Ins.DispatchEvent(MsgEvent.City_Camera_Move, mainCamera.transform.position);
        }
        if (IsCurrentFrameZooming)
        {
            MsgMgr.Ins.DispatchEvent(MsgEvent.City_Camera_Zoom, mainCamera.orthographicSize);
        }
    }

    protected override void LoadConfig()
    {
        base.LoadConfig();

        Dictionary<string, GameSettingConfig> config = ConfigMgr.GameSetting.DataMap;

        Setting.PosLimit = ParseVector4(config["camera_city_posLimit"].Val);
        buildingZoom = float.Parse(config["camera_city_buildingZoom"].Val);
        defaultZoom = float.Parse(config["camera_city_defaultZoom"].Val);
    }

    GameObject _pressedTarget = null;
    Vector3 _pressMousePos;
    protected override void HandleRaycast()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            _pressedTarget = GetHandleRaycast();
            _pressMousePos = Input.mousePosition;
        }
        else if (_pressedTarget != null && Input.mousePosition == _pressMousePos && Input.GetMouseButtonUp(0))
        {
            if (_pressedTarget == GetHandleRaycast())
            {
                // 检查命中的对象是否有BuildingController脚本
                BuildingController buildingController = _pressedTarget.GetComponent<BuildingController>();
                if (buildingController != null)
                {
                    buildingController.OnSelect();
                }
            }
        }
    }

    protected override GameObject GetHandleRaycast()
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

    #region 缩放
    public void SetCameraZoomToObject(float _size = 10, bool _tween = true)
    {
        base.SetCameraZoom(buildingZoom, _tween);
    }

    public void SetCameraZoomToDefault(float _size = 15, bool _tween = true)
    {
        base.SetCameraZoom(defaultZoom, _tween);
    }
    #endregion
}
