using System;
using Rain.UI;
using UnityEngine;

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
        LoadSceneViewParam viewParams = new LoadSceneViewParam();
        viewParams.targetSceneName = _sceneID.ToString();
        targetSceneName = viewParams.targetSceneName;
        viewParams.CloseCB = () =>
        {
            currSceneID = _sceneID;
            OnSceneChangeComplete();
            _changeSuccess?.Invoke();
        };
        OnSceneStartChange();
        UIMgr.Ins.OpenView<LoadSceneView>(viewParams);
    }

    /// <summary>
    /// 场景开始跳转
    /// </summary>
    public void OnSceneStartChange()
    {
        if (currSceneID == SceneID.MainScene)
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

    public bool IsMain
    {
        get
        {
            return currSceneID == SceneID.MainScene;
        }
    }
}