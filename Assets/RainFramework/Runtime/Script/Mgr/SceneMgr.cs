using System;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneMgr : MonoSingleton<SceneMgr>
{
    //要切换的场景名称
    string targetSceneName;
    public SceneID currSceneID;
    public Camera Camera { get; set; }

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
        if (scene.name == SceneName.MainCity)
        {
            UIMgr.Ins.OpenView(ViewName.MainView);
            UIMgr.Ins.OpenView(ViewName.CityHUDView);
        }
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
        if (scene.name == SceneName.MainCity)
        {
            UIMgr.Ins.CloseView(ViewName.CityHUDView);
        }
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    /// <param name="_sceneID"></param>
    public void ChangeScene(SceneID _sceneID, Action _changeSuccess = null, bool showView = true)
    {
        OnSceneStartChange();
        LoadSceneViewParam viewParam = new LoadSceneViewParam();
        viewParam.targetSceneName = _sceneID.ToString();
        targetSceneName = viewParam.targetSceneName;
        LoadSceneView view = null;
        if (showView)
            view = UIMgr.Ins.OpenView<LoadSceneView>(viewParam);
        StartCoroutine(LoadScene(viewParam.targetSceneName, (progress) =>
        {
            view?.SetViewProgress(progress);
        }, () =>
        {
            view?.Close();
            OnSceneChangeComplete();
        }));

    }

    float currProgress = 0;
    IEnumerator LoadScene(string _targetSceneName, Action<float> _onLoadUpdate, Action _onLoadComplete)
    {
        currProgress = 0;
        _onLoadUpdate?.Invoke(currProgress);
        AsyncOperation operation = SceneManager.LoadSceneAsync(_targetSceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            currProgress = operation.progress;
            _onLoadUpdate?.Invoke(currProgress);
            if (currProgress >= 0.9F)
            {
                operation.allowSceneActivation = true;
                _onLoadUpdate?.Invoke(1);
                _onLoadComplete?.Invoke();
            }
            yield return null;
        }

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