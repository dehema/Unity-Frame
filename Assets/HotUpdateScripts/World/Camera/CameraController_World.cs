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
public class CameraController_World : CameraController_Base
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
    }

    protected override void LoadConfig()
    {
        base.LoadConfig();

        Setting.PosLimit = ParseVector4("-1000, 1000, -1000, 1000");
    }
}