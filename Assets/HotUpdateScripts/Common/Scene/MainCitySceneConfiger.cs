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
    }

    public void OnSceneUnLoad(params object[] obj)
    {
        SceneChangeParam sceneChangeParam = obj[0] as SceneChangeParam;
        if (sceneID.ToString() != sceneChangeParam.sceneName)
            return;
    }
}
