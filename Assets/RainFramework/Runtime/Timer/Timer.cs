using System;

namespace Rain.Core
{
    /// <summary>
    /// 计时器类，用于管理基于时间或帧的计时功能
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// 已经过的时间
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
            if (IsPaused)
                return 0;
            
            // 记录本次更新中的触发次数
            int triggerCount = 0;

            if (!isDelayCompleted)
            {
                Delay -= dt;
                if (Delay <= 0f)
                {
                    isDelayCompleted = true;
                    elapsedTime = -Delay; // 保留超出部分时间
                    triggerCount++;
                    Delay = 0f;
                }
                else
                {
                    return triggerCount;
                }
            }
            else
            {
                elapsedTime += dt;
            }

            // 计算需要触发的次数
            if (elapsedTime >= Step)
            {
                float stepsFloat = elapsedTime / Step;
                int steps = UnityEngine.Mathf.FloorToInt(stepsFloat);
                triggerCount += steps;
                elapsedTime -= steps * Step;
            }

            return triggerCount;
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
    }
}