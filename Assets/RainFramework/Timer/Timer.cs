using System;

namespace F8Framework.Core
{
    /// <summary>
    /// 计时器类，用于管理基于时间或帧的计时功能
    /// </summary>
    public class Timer
    {
        // 计时器内部状态
        private float elapsedTime = 0f;      // 当前步长周期内已经过的时间
        private bool isDelayCompleted = false;  // 初始延迟是否已完成
        
        // 计时器基本属性
        public object Handle = null;         // 计时器持有者，用于标识计时器归属
        public int ID = 0;                   // 计时器唯一标识，用于在TimerMgr中查找和管理
        public float Step = 1f;              // 计时步长：时间计时器-每隔多少秒触发一次；帧计时器-每隔多少帧触发一次
        public float Delay = 0f;             // 初始延迟：时间计时器-延迟多少秒开始计时；帧计时器-延迟多少帧开始计时
        public int Field = 0;                // 计时器字段值：当大于0时表示计时器需要触发的总次数，减至0时完成并触发OnComplete；当等于0时不会触发完成事件
        
        // 回调函数
        public Action OnSecond = null;       // 周期触发回调：每次计时周期结束时触发（对于帧计时器，实际是OnFrame的功能）
        public Action OnComplete = null;     // 完成触发回调：计时器完成全部计时后触发
        
        // 计时器状态标记
        public bool IsFinish = false;        // 是否已完成：标记计时器是否已经完成全部计时
        public bool IsFrameTimer = false;    // 是否为帧计时器：标记计时器是基于时间还是基于帧
        public bool IsPaused = false;        // 是否暂停：标记计时器当前是否处于暂停状态

        // 存储初始值以便重置
        private readonly float _initialStep;    // 初始步长值
        private readonly float _initialDelay;   // 初始延迟值
        private readonly int _initialField;     // 初始字段值

        /// <summary>
        /// 计时器构造函数
        /// </summary>
        /// <param name="handle">计时器持有者</param>
        /// <param name="id">计时器唯一ID</param>
        /// <param name="step">计时步长(秒/帧)</param>
        /// <param name="delay">初始延迟(秒/帧)</param>
        /// <param name="field">可触发次数</param>
        /// <param name="onSecond">周期触发回调</param>
        /// <param name="onComplete">完成触发回调</param>
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
        /// <param name="dt">时间增量(秒)或帧增量</param>
        /// <returns>当前帧触发的次数</returns>
        public int Update(float dt)
        {
            // 如果计时器暂停，不进行任何计时
            if (IsPaused)
                return 0;

            int triggerCount = 0; // 记录触发次数

            // 处理初始延迟
            if (!isDelayCompleted)
            {
                Delay -= dt;  // 减少延迟时间/帧数
                if (Delay <= 0f)  // 延迟结束
                {
                    isDelayCompleted = true;
                    elapsedTime = -Delay; // 保留超出部分时间/帧数，确保精确计时
                    triggerCount++;       // 延迟结束后立即触发一次
                    Delay = 0f;
                }
                else
                {
                    return triggerCount;  // 延迟未结束，直接返回
                }
            }
            else
            {
                elapsedTime += dt;  // 累加经过的时间/帧数
            }

            // 计算需要触发的次数
            if (elapsedTime >= Step)
            {
                float stepsFloat = elapsedTime / Step;  // 计算经过了多少个完整周期
                int steps = UnityEngine.Mathf.FloorToInt(stepsFloat);  // 向下取整，得到完整周期数
                triggerCount += steps;  // 增加触发次数
                elapsedTime -= steps * Step;  // 减去已计算的时间/帧数，保留剩余部分
            }

            return triggerCount;  // 返回本次更新触发的次数
        }

        /// <summary>
        /// 重置计时器到初始状态
        /// </summary>
        public void Reset()
        {
            IsPaused = false;          // 取消暂停状态
            elapsedTime = 0f;          // 清空已经过的时间/帧数
            IsFinish = false;          // 重置完成标记
            isDelayCompleted = false;  // 重置延迟完成标记

            // 恢复所有初始配置值
            Step = _initialStep;       // 恢复初始步长
            Delay = _initialDelay;     // 恢复初始延迟
            Field = _initialField;     // 恢复初始字段值(触发次数)
        }
    }
}