using System;

namespace Rain.Core
{
    /// <summary>
    /// 计时器类，用于管理基于时间或帧的计时功能
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// 计时器及计算后的盈余时间（仅限内部计算用）
        /// </summary>
        private float elapsedTime = 0f;
        /// <summary>
        /// 延迟是否已完成
        /// </summary>
        private bool isDelayCompleted = false;
        /// <summary>
        /// 计时器持有者对象
        /// </summary>
        public object Handle = null;
        /// <summary>
        /// 计时器唯一标识符
        /// </summary>
        public int ID = 0;
        /// <summary>
        /// 计时器步长（秒或帧）
        /// </summary>
        public float Step = 1f;
        /// <summary>
        /// 计时器初始延迟（秒或帧）
        /// </summary>
        public float Delay = 0f;
        /// <summary>
        /// 计时器剩余触发次数，0表示无限次
        /// </summary>
        public int Field = 0;
        /// <summary>
        /// 每次计时器触发时调用的回调
        /// </summary>
        public Action OnSecond = null;
        /// <summary>
        /// 计时器完成时调用的回调
        /// </summary>
        public Action OnComplete = null;
        /// <summary>
        /// 计时器是否已完成
        /// </summary>
        public bool IsFinish = false;
        /// <summary>
        /// 是否为帧计时器（而非时间计时器）
        /// </summary>
        public bool IsFrameTimer = false;
        /// <summary>
        /// 计时器是否已暂停
        /// </summary>
        public bool IsPaused = false;

        /// <summary>
        /// 存储初始值以便重置
        /// </summary>
        private readonly float _initialStep;
        private readonly float _initialDelay;
        private readonly int _initialField;

        /// <summary>
        /// 创建一个新的计时器实例
        /// </summary>
        /// <param name="handle">计时器持有者对象</param>
        /// <param name="id">计时器唯一标识符</param>
        /// <param name="step">计时器步长（秒或帧）</param>
        /// <param name="delay">初始延迟（秒或帧）</param>
        /// <param name="field">触发次数，0表示无限</param>
        /// <param name="onSecond">每次触发的回调</param>
        /// <param name="onComplete">完成时的回调</param>
        /// <param name="isFrameTimer">是否为帧计时器</param>
        public Timer(object handle, int id, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null, bool isFrameTimer = false)
        {
            Handle = handle;
            ID = id;
            Step = step;
            Delay = delay;
            Field = field;
            OnSecond = onSecond;
            OnComplete = onComplete;
            IsFrameTimer = isFrameTimer;
            // 保存初始值
            _initialStep = step;
            _initialDelay = delay;
            _initialField = field;
        }

        /// <summary>
        /// 更新计时器状态
        /// </summary>
        /// <param name="dt">时间增量（秒或帧）</param>
        /// <returns>本次更新触发的次数</returns>
        public int Update(float dt)
        {
            // 如果计时器已暂停，直接返回0
            if (IsPaused)
                return 0;

            // 记录本次更新中的触发次数
            int triggerCount = 0;

            // 阶段1：处理初始延迟
            if (!isDelayCompleted)
            {
                // 减少延迟时间
                Delay -= dt;
                
                // 检查延迟是否完成
                if (Delay <= 0f)
                {
                    // 延迟完成，标记状态
                    isDelayCompleted = true;
                    // 保留超出延迟的时间到elapsedTime中，避免时间丢失
                    elapsedTime = -Delay; 
                    // 延迟完成时立即触发一次
                    triggerCount++;
                    Delay = 0f;
                }
                else
                {
                    // 延迟未完成，直接返回，不进行后续计算
                    return triggerCount;
                }
            }
            else
            {
                // 阶段2：延迟已完成，累计经过的时间
                elapsedTime += dt;
            }

            // 阶段3：计算基于步长的触发次数
            if (elapsedTime >= Step)
            {
                // 计算可以触发多少次（向下取整）
                float stepsFloat = elapsedTime / Step;
                int steps = UnityEngine.Mathf.FloorToInt(stepsFloat);
                
                // 累加触发次数
                triggerCount += steps;
                
                // 扣除已触发的时间，保留余数
                elapsedTime -= steps * Step;
            }

            return triggerCount;
        }

        /// <summary>
        /// 获取计时器剩余时间
        /// </summary>
        /// <returns>剩余时间（秒或帧），如果计时器已完成返回0</returns>
        public float GetRemainingTime()
        {
            // 如果计时器已完成，返回0
            if (IsFinish)
                return 0f;

            // 如果还在延迟阶段，返回剩余延迟时间
            if (!isDelayCompleted)
            {
                return Delay;
            }

            // 如果延迟已完成，计算到下次触发还需要多少时间
            // 剩余时间 = 步长 - 当前已累计的时间
            float remainingTime = Step - elapsedTime;
            
            // 确保返回值不为负数
            return Math.Max(0f, remainingTime);
        }

        /// <summary>
        /// 获取计时器总剩余时间（包括延迟和步长）
        /// </summary>
        /// <returns>总剩余时间（秒或帧）</returns>
        public float GetTotalRemainingTime()
        {
            // 如果计时器已完成，返回0
            if (IsFinish)
                return 0f;

            // 如果还在延迟阶段，返回剩余延迟时间
            if (!isDelayCompleted)
            {
                return Delay;
            }

            // 如果延迟已完成，计算总剩余时间
            // 总剩余时间 = 剩余步长时间
            return GetRemainingTime();
        }

        /// <summary>
        /// 重置计时器到初始状态
        /// </summary>
        public void Reset()
        {
            IsPaused = false;
            elapsedTime = 0f;
            IsFinish = false;
            isDelayCompleted = false;

            // 恢复初始值
            Step = _initialStep;
            Delay = _initialDelay;
            Field = _initialField;
        }

        /// <summary>
        /// 销毁计时器，清理所有资源和引用
        /// </summary>
        public void Destroy()
        {
            // 停止计时
            IsFinish = true;
            
            // 清理回调函数引用，避免内存泄漏
            OnSecond = null;
            OnComplete = null;
            
            // 清理持有者引用
            Handle = null;
            
            // 重置所有状态
            IsPaused = false;
            elapsedTime = 0f;
            isDelayCompleted = false;
            
            // 重置参数到初始状态
            Step = _initialStep;
            Delay = _initialDelay;
            Field = _initialField;
        }

        /// <summary>
        /// 检查计时器是否已被销毁
        /// </summary>
        public bool IsDestroyed => IsFinish && OnSecond == null && OnComplete == null;

        /// <summary>
        /// 安全地调用回调函数（检查是否已销毁）
        /// </summary>
        private void SafeInvoke(Action callback)
        {
            if (callback != null && !IsDestroyed)
            {
                callback.Invoke();
            }
        }
    }
}