using System;
using DG.Tweening;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 滑动条倒计时组件
/// 用于显示倒计时进度条效果，支持播放、暂停和结束操作
/// </summary>
public partial class SliderCountDown : BaseUI
{
    // 倒计时结束时的回调函数
    private Action _onFinish;

    // 倒计时总时长（秒）
    private float _duration = 1f;

    // 当前是否正在播放
    private bool _isPlaying = false;

    // 当前剩余时间
    private float _remainingTime;
    private int remainingNumber;

    // 动画Tween引用，用于控制和取消动画
    private Tween _fillTween;

    /// <summary>
    /// 初始化倒计时组件
    /// </summary>
    /// <param name="duration">倒计时总时长（秒）</param>
    /// <param name="onFinish">倒计时结束时的回调函数</param>
    public void Init(float duration, Action onFinish)
    {
        if (duration <= 0f)
        {
            Debug.LogWarning("倒计时时长应大于0秒");
            duration = 0.1f; // 设置一个最小值
        }
        Stop();
        _onFinish = onFinish;
        _duration = duration;
        _remainingTime = _duration;

        // 初始化进度条为满
        ui.imgFill_Image.fillAmount = 1f;


    }

    void RefreshCountDownText()
    {
        ui.countDown_Text.text = (remainingNumber).ToString();
    }

    /// <summary>
    /// 每帧更新倒计时时间
    /// </summary>
    private void Update()
    {
        if (_isPlaying)
        {
            // 更新剩余时间
            _remainingTime -= Time.deltaTime;
            if (remainingNumber != Mathf.FloorToInt(_remainingTime))
            {
                remainingNumber = Mathf.FloorToInt(_remainingTime);
                RefreshCountDownText();
            }
            // 检查是否倒计时结束
            if (_remainingTime <= 0f)
            {
                Finish();
            }
        }
    }

    /// <summary>
    /// 开始或继续播放倒计时
    /// </summary>
    public void Play()
    {
        // 如果已经在播放，则不重复操作
        if (_isPlaying)
            return;
        _isPlaying = true;

        // 取消之前的动画（如果有）
        if (_fillTween != null)
        {
            _fillTween.Kill();
        }

        // 创建新的填充动画，从当前位置到0
        _fillTween = ui.imgFill_Image.DOFillAmount(0, _remainingTime)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 动画完成时调用Finish
                if (_isPlaying) // 确保没有被手动暂停
                    Finish();
            });
    }

    /// <summary>
    /// 结束倒计时并触发回调
    /// </summary>
    /// <returns>剩余的时间（秒）</returns>
    public float Finish()
    {
        Stop();
        // 调用结束回调
        _onFinish?.Invoke();

        return _remainingTime;
    }

    /// <summary>
    /// 重置倒计时到初始状态
    /// </summary>
    public void Stop()
    {
        // 停止当前动画
        if (_fillTween != null)
        {
            _fillTween.Kill();
            _fillTween = null;
        }

        _isPlaying = false;
    }

    /// <summary>
    /// 获取当前剩余时间
    /// </summary>
    /// <returns>剩余时间（秒）</returns>
    public float GetRemainingTime()
    {
        return _remainingTime;
    }

    /// <summary>
    /// 获取总时长
    /// </summary>
    /// <returns>总时长（秒）</returns>
    public float GetDuration()
    {
        return _duration;
    }

    /// <summary>
    /// 组件销毁时确保清理资源
    /// </summary>
    private void OnDestroy()
    {
        Stop();
    }
}
