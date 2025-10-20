using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using static SettingField;

/// <summary>
/// 登录
/// </summary>
public partial class LoginView : BaseView
{
    LoginViewParam param;
    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        param = _viewParam as LoginViewParam;
        ui.btLogin_Button.SetButton(OnClickLogin);
    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
        ui.progress_Slider.value = 0;
        ui.btLogin_Button.interactable = true;
        StartLoginProgress();
    }

    private void Update()
    {
        ui.progressVal_Text.text = $"{Mathf.Ceil(ui.progress_Slider.value * 100)}%";
    }

    void OnClickLogin()
    {
        StartLoginProgress();
    }

    /// <summary>
    /// 开始模拟登录进度
    /// </summary>
    private void StartLoginProgress()
    {
        SceneMgr.Ins.ChangeScene(SceneID.MainCity, null, true);
        StartCoroutine(SimulateLoginProgress());
    }

    /// <summary>
    /// 模拟登录进度协程
    /// </summary>
    private IEnumerator SimulateLoginProgress()
    {
        ui.btLogin_Button.interactable = false;

        // 根据LoginState的四个状态执行登录流程
        LoginState[] loginStates = { LoginState.InitLocalResources, LoginState.CheckVersion, LoginState.UpdateResources, LoginState.GameLogin };
        string[] stateTexts = { "正在初始化本地资源...", "正在检查版本...", "正在更新资源...", "正在登录游戏..." };
        float[] progressValues = { 0.25f, 0.5f, 0.75f, 1.0f };
        float[] durations = { 0.5f, 0.8f, 1.2f, 0.6f };

        for (int i = 0; i < loginStates.Length; i++)
        {
            LoginState currentState = loginStates[i];
            ui.tips_Text.text = stateTexts[i];

            // 执行当前状态的处理
            yield return StartCoroutine(ProcessLoginState(currentState, progressValues[i], durations[i]));

            // 状态间短暂停顿
            if (i < loginStates.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        // 所有状态完成，执行登录完成回调
        OnLoginComplete();
    }

    /// <summary>
    /// 处理登录状态
    /// </summary>
    private IEnumerator ProcessLoginState(LoginState state, float targetProgress, float duration)
    {
        float startProgress = ui.progress_Slider.value;
        switch (state)
        {
            case LoginState.CheckVersion:
                break;
            case LoginState.InitLocalResources:
            case LoginState.UpdateResources:
            case LoginState.GameLogin:
                yield return ui.progress_Slider.DOValue(targetProgress, duration).WaitForCompletion();
                break;
        }


        // 根据状态执行特定逻辑
        switch (state)
        {
            case LoginState.InitLocalResources:
                // 初始化本地资源的逻辑
                Debug.Log("初始化本地资源完成");
                break;

            case LoginState.CheckVersion:
                {
                    // 检查版本的逻辑
                    Debug.Log("版本检查完成");
                    yield return StartCoroutine(HotUpdateMgr.Ins.InitRemoteVersion());
                    yield return StartCoroutine(HotUpdateMgr.Ins.InitRemoteAssetBundleMap());
                    //检查是否需要更新
                    Tuple<Dictionary<string, string>, long> tuple = HotUpdateMgr.Ins.CheckHotUpdate();
                    float startVal = ui.progress_Slider.value;
                    Debug.Log($"热更新：{tuple.Item1.Count}个文件，总大小{Util.Converter.FormatBytes(tuple.Item2, 1)}");
                    bool complete = false;
                    HotUpdateMgr.Ins.StartHotUpdate(tuple.Item1,
                        completed: () =>
                        {
                            ui.progress_Slider.DOKill();
                            ui.progress_Slider.value = targetProgress;
                            complete = true;
                        },
                        overallProgress: (progress) =>
                        {
                            float _progress = Mathf.Lerp(startVal, targetProgress, progress);
                            ui.progress_Slider.DOValue(_progress, duration);
                        }
                    );
                    while (!complete)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    Util.Log(ResourceMap.Mappings);
                }
                break;

            case LoginState.UpdateResources:
                // 更新资源的逻辑
                Debug.Log("资源更新完成");
                break;

            case LoginState.GameLogin:
                // 游戏登录的逻辑
                Debug.Log("游戏登录完成");
                break;
        }
    }

    /// <summary>
    /// 登录完成
    /// </summary>
    private void OnLoginComplete()
    {
        param?.action?.Invoke();
        Close();
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
    }

    /// <summary>
    /// 登录阶段
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// 初始化本地资源
        /// </summary>
        InitLocalResources = 0,

        /// <summary>
        /// 检查版本
        /// </summary>
        CheckVersion = 1,

        /// <summary>
        /// 更新资源
        /// </summary>
        UpdateResources = 2,

        /// <summary>
        /// 游戏登录
        /// </summary>
        GameLogin = 3
    }
}

public class LoginViewParam : IViewParam
{
    public Action action;
}
