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
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        MsgMgr.Ins.DispatchEvent(MsgEvent.City_Camera_Move, mainCamera.transform.position);
    }

    // 每帧更新
    protected override void Update()
    {
        base.Update();
    }

    protected override void LoadConfig()
    {
        base.LoadConfig();

        Dictionary<string, GameSettingConfig> config = ConfigMgr.GameSetting.DataMap;

        Setting.PosLimit = ParseVector4(config["camera_city_posLimit"].Val);
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
}
