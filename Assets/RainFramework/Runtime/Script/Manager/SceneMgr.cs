using System;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoSingleton<SceneMgr>
{
    //要切换的场景名称
    string targetSceneName;
    SceneID currSceneID;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneChangeParam param = new SceneChangeParam();
        param.sceneName = scene.name;
        MsgMgr.Ins.DispatchEvent(MsgEvent.SceneLoaded, param);
        // 直接触发当前场景内所有 ISceneConfigProvider 的 OnSceneLoad（避免业务层自己再绑一次）
        //var providers = GameObject.FindObjectsOfType<MonoBehaviour>(true);
        //for (int i = 0; i < providers.Length; i++)
        //{
        //    if (providers[i] is ISceneConfigProvider p)
        //    {
        //        p.OnSceneLoad(scene.name);
        //    }
        //}
    }

    private void OnSceneUnloaded(Scene scene)
    {
        SceneChangeParam param = new SceneChangeParam();
        param.sceneName = scene.name;
        MsgMgr.Ins.DispatchEvent(MsgEvent.SceneUnload, param);
        // 直接触发当前场景内所有 ISceneConfigProvider 的 OnSceneLoad（避免业务层自己再绑一次）
        //var providers = GameObject.FindObjectsOfType<MonoBehaviour>(true);
        //for (int i = 0; i < providers.Length; i++)
        //{
        //    if (providers[i] is ISceneConfigProvider p)
        //    {
        //        p.OnSceneUnLoad(scene.name);
        //    }
        //}
    }

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
    }

    /// <summary>
    /// 场景跳转完成
    /// </summary>
    void OnSceneChangeComplete()
    {
        Debug.Log("跳转到场景:" + targetSceneName);
    }
}

/// <summary>
/// 场景切换参数
/// </summary>
public class SceneChangeParam
{
    public string sceneName;
}