using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoSingleton<SceneMgr>
{
    //要切换的场景名称
    string targetSceneName;
    SceneID currSceneID;
    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="_sceneID"></param>
    public void ChangeScene(SceneID _sceneID, Action _changeSuccess = null)
    {
        LoadSceneViewParams viewParams = new LoadSceneViewParams();
        viewParams.targetSceneName = _sceneID.ToString();
        targetSceneName = viewParams.targetSceneName;
        viewParams.CloseCB = () =>
        {
            currSceneID = _sceneID;
            _changeSuccess?.Invoke();
            Debug.Log("切换至场景" + viewParams.targetSceneName);
            OnSceneChangeComplete();
        };
        OnSceneStartChange();
        UIMgr.Ins.OpenView<LoadSceneView>(viewParams);
    }

    /// <summary>
    /// 场景开始跳转
    /// </summary>
    public void OnSceneStartChange()
    {
        if (currSceneID == SceneID.WorldMap)
        {
        }
    }

    /// <summary>
    /// 场景跳转完成
    /// </summary>
    void OnSceneChangeComplete()
    {
        Debug.Log("跳转到场景:" + targetSceneName);
    }

    public bool IsWorld
    {
        get
        {
            return currSceneID == SceneID.WorldMap;
        }
    }

    public bool IsBattleField
    {
        get
        {
            return currSceneID == SceneID.BattleField;
        }
    }
}