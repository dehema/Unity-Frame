using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

[RequireComponent(typeof(MainCityCreator))]
public class MainCitySceneConfig : MonoBehaviour, ISceneConfigProvider
{
    SceneID sceneID = SceneID.MainCity;
    MainCityCreator mainCityCreator;

    private void Awake()
    {
        mainCityCreator = GetComponent<MainCityCreator>();
        MsgMgr.Ins.AddEventListener(MsgEvent.SceneLoaded, OnSceneLoad, this);
        MsgMgr.Ins.AddEventListener(MsgEvent.SceneUnload, OnSceneUnLoad, this);
    }

    public void OnSceneLoad(params object[] obj)
    {
        // ≈‰÷√
        SceneChangeParam sceneChangeParam = obj[0] as SceneChangeParam;
        if (sceneID.ToString() != sceneChangeParam.sceneName)
            return;
        Dictionary<string, GameSettingConfig> config = ConfigMgr.GameSetting.DataMap;
        CameraController_City camera = FindFirstObjectByType<CameraController_City>();
        string[] posLimit = config["mainCity_Camera_PosLimit"].Val.Split(",");
        camera.PosLimit = new Vector4(float.Parse(posLimit[0]), float.Parse(posLimit[1]), float.Parse(posLimit[2]), float.Parse(posLimit[3]));
        camera.PanDampening = float.Parse(config["mainCity_Camera_panDampening"].Val);
        camera.TouchPanSpeed = float.Parse(config["mainCity_Camera_touchPanSpeed"].Val);
        camera.TouchMinDistance = float.Parse(config["mainCity_Camera_touchMinDistance"].Val);
        camera.ZoomDefaultSize = float.Parse(config["mainCity_Camera_zoomDefaultSize"].Val);
        camera.ZoomMinSize = float.Parse(config["mainCity_Camera_zoomMinSize"].Val);
        camera.ZoomMaxSize = float.Parse(config["mainCity_Camera_zoomMaxSize"].Val);
        camera.TouchZoomSpeed = float.Parse(config["mainCity_Camera_touchZoomSpeed"].Val);
        camera.ZoomDampening = float.Parse(config["mainCity_Camera_zoomDampening"].Val);

        //
    }

    public void OnSceneUnLoad(params object[] obj)
    {
        SceneChangeParam sceneChangeParam = obj[0] as SceneChangeParam;
        if (sceneID.ToString() != sceneChangeParam.sceneName)
            return;
    }
}
