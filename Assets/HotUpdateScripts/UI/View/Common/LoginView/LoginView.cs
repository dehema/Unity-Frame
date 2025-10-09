using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rain.UI;
using UnityEngine;

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
    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
        ui.progress_Slider.value = 0;

        // 开始模拟登录进度
        StartLoginProgress();
    }

    private void Update()
    {
        ui.progressVal_Text.text = $"{Mathf.Ceil(ui.progress_Slider.value * 100)}%";
    }

    /// <summary>
    /// 开始模拟登录进度
    /// </summary>
    private void StartLoginProgress()
    {
        // 随机生成2-4秒的登录时长
        float loginDuration = UnityEngine.Random.Range(2f, 4f);

        // 创建分段进度模拟真实登录过程
        StartCoroutine(SimulateLoginProgress(loginDuration));
    }

    /// <summary>
    /// 模拟登录进度协程
    /// </summary>
    /// <param name="totalDuration">总时长</param>
    private IEnumerator SimulateLoginProgress(float totalDuration)
    {
        // 第一阶段：初始化 (0-20%) - 0.3-0.6秒
        float initDuration = UnityEngine.Random.Range(0.3f, 0.6f);
        yield return ui.progress_Slider.DOValue(0.2f, initDuration).WaitForCompletion();

        // 短暂停顿
        yield return new WaitForSeconds(0.1f);

        // 第二阶段：连接服务器 (20-60%) - 0.8-1.5秒
        float connectDuration = UnityEngine.Random.Range(0.8f, 1.5f);
        yield return ui.progress_Slider.DOValue(0.6f, connectDuration).WaitForCompletion();

        // 短暂停顿
        yield return new WaitForSeconds(0.1f);

        // 第三阶段：验证数据 (60-85%) - 0.5-1.0秒
        float verifyDuration = UnityEngine.Random.Range(0.5f, 1.0f);
        yield return ui.progress_Slider.DOValue(0.85f, verifyDuration).WaitForCompletion();

        // 短暂停顿
        yield return new WaitForSeconds(0.1f);

        // 第四阶段：完成登录 (85-100%) - 剩余时间
        float remainingTime = totalDuration - initDuration - connectDuration - verifyDuration - 0.3f;
        if (remainingTime > 0)
        {
            yield return ui.progress_Slider.DOValue(1f, remainingTime).WaitForCompletion();
        }
        else
        {
            // 如果时间不够，直接完成
            yield return ui.progress_Slider.DOValue(1f, 0.2f).WaitForCompletion();
        }

        // 登录完成，执行回调
        OnLoginComplete();
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
}

public class LoginViewParam : IViewParam
{
    public Action action;
}
