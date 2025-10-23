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
        MsgMgr.Ins.DispatchEvent(MsgEvent.City_Camera_Move, mainCamera.transform.position);
    }
}