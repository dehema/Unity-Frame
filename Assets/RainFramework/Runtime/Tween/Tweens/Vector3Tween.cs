﻿using UnityEngine;

namespace Rain.Core
{
    public class Vector3Tween : BaseTween
    {
        private Vector3 from = Vector3.zero;
        private Vector3 to = Vector3.zero;
        private Vector3 tempValue = Vector3.zero;
        
        public Vector3Tween(Vector3 from, Vector3 to, float t, int id)
        {
            Init(from, to, t);
            this.id = id;
        }

        internal void Init(Vector3 from, Vector3 to, float t)
        {
            this.from = from;
            this.to = to;
            this.duration = t;
            this.PauseReset = () => this.Init(from, to, t);
        }

        /// <summary>
        /// 每帧执行的更新逻辑
        /// </summary>
        public override void Update(float deltaTime)
        {
            if(isPause || IsComplete || IsRecycle)
                return;

            // 处理启动延迟
            if (delay > 0.0f)
            {
                delay -= deltaTime;
                return;
            }
            
            base.Update(deltaTime);

            currentTime += deltaTime;
            
            // 检查是否完成当前周期
            if (currentTime >= duration)
            {
                if (onUpdateVector3 != null)
                    onUpdateVector3(to);

                if (onUpdateVector2 != null)
                    onUpdateVector2(to);
                
                bool shouldComplete = !HandleLoop();
                if (shouldComplete)
                    onComplete();
            }
            
            float normalizedProgress = currentTime / duration;
            // 通过曲线函数计算缓动进度
            float curveProgress = GetCurveProgress(normalizedProgress);
            
            // 基于缓动算法计算当前值
            EasingFunctions.ChangeVector(from, to, curveProgress, ease, ref tempValue);
            
            // 触发值更新回调
            if (onUpdateVector3 != null)
                onUpdateVector3(tempValue);

            if (onUpdateVector2 != null)
                onUpdateVector2(tempValue);
        }

        public override void Reset()
        {
            base.Reset();
            from = Vector3.zero;
            to = Vector3.zero;
        }

        public override void ReplayReset()
        {
            base.ReplayReset();
            Init(from, to, duration);
        }
        
        private float GetCurveProgress(float normalizedProgress)
        {
            switch (loopType)
            {
                case LoopType.Yoyo:
                    // 使用平滑的往返曲线 (0→1→0)
                    return Mathf.PingPong(normalizedProgress * 2, 1);
                default:
                    return normalizedProgress;
            }
        }
        
        private bool HandleLoop()
        {
            if (this.loopType == LoopType.None || this.tempLoopCount == 0)
            {
                return false;
            }
            else
            {
                if (this.tempLoopCount > 0)
                {
                    this.tempLoopCount -= 1;
                }
                switch (this.loopType)
                {
                    case LoopType.Restart:
                        break;
                    case LoopType.Flip:
                        (from, to) = (to, from);
                        break;
                    case LoopType.Incremental:
                    {
                        var delta = to - from;
                        from = to;
                        to += delta;
                        break;
                    }
                    case LoopType.Yoyo:
                        break;
                }
                this.ReplayReset();
                return this.tempLoopCount > 0;
            }
        }
    }
}
