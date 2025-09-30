using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

public class MainCitySceneConfig : MonoBehaviour, ISceneConfigProvider
{
    [SerializeField]
    SceneID sceneID = SceneID.MainCity;
    public void OnSceneLoad(string _sceneName)
    {
        if (sceneID.ToString() != _sceneName)
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
    }

    public void OnSceneUnLoad(string _sceneName)
    {
        if (sceneID.ToString() != _sceneName)
            return;
    }


}
