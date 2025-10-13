using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rain.Core
{
    /// <summary>
    /// 计时器管理器，负责创建、更新和管理所有计时器
    /// </summary>
    [UpdateRefresh]
    public class TimerMgr : ModuleSingleton<TimerMgr>, IModule
    {
        /// <summary>
        /// 存储计时器的列表
        /// </summary>
        private List<Timer> times = new List<Timer>();

        /// <summary>
        /// 初始化时间（毫秒）
        /// </summary>
        private long initTime;

        /// <summary>
        /// 服务器时间（毫秒）
        /// </summary>
        private long serverTime;

        /// <summary>
        /// 临时时间，用于计算服务器时间差值
        /// </summary>
        private long tempTime;

        /// <summary>
        /// 是否处于焦点状态
        /// </summary>
        private bool isFocus = true;

        /// <summary>
        /// 帧时间，默认为1
        /// </summary>
        private int frameTime = 1;

        /// <summary>
        /// 初始化计时器管理器
        /// </summary>
        /// <param name="createParam">创建参数</param>
        public void OnInit(object createParam)
        {
            initTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            serverTime = 0;
            tempTime = 0;
        }

        /// <summary>
        /// 在Unity的LateUpdate阶段调用
        /// </summary>
        public void OnLateUpdate()
        {

        }

        /// <summary>
        /// 在Unity的FixedUpdate阶段调用
        /// </summary>
        public void OnFixedUpdate()
        {

        }

        /// <summary>
        /// 终止计时器管理器，清理资源
        /// </summary>
        public void OnTermination()
        {
            MsgMgr.Ins.RemoveEventListener(MsgEvent.ApplicationFocus, OnApplicationFocus, this);
            MsgMgr.Ins.RemoveEventListener(MsgEvent.NotApplicationFocus, NotApplicationFocus, this);
            times.Clear();
            base.Destroy();
        }

        /// <summary>
        /// 在Unity的Update阶段调用，更新所有计时器
        /// </summary>
        public void OnUpdate()
        {
            // 如果失去焦点或者计时器数量为0，则返回
            if (isFocus == false || times.Count <= 0)
            {
                return;
            }
            float dt = Time.deltaTime;

            for (int i = 0; i < times.Count; i++)
            {
                Timer timer = times[i];

                if (timer.IsFinish)
                {
                    times.RemoveAt(i);
                    i--;
                    continue;
                }

                // 调用计时器更新方法，根据是否为帧计时器传入不同参数
                int triggerCount = timer.IsFrameTimer ? timer.Update(frameTime) : timer.Update(dt);

                if (triggerCount > 0) // 如果本帧触发次数大于0，执行相关逻辑
                {
                    if (timer.IsFinish || timer.Handle == null || timer.Handle.Equals(null))
                    {
                        timer.IsFinish = true;
                        continue;
                    }

                    int field = timer.Field; // 获取计时器剩余字段值

                    for (int j = 0; j < triggerCount; j++)
                    {
                        field = field > 0 ? field - 1 : field; // 每次减少计时器字段值

                        if (field == 0) // 若字段值为0，触发onSecond事件，并执行OnTimerComplete
                        {
                            timer.Field = field; // 更新计时器剩余字段值
                            timer.OnSecond?.Invoke();
                            OnTimerComplete(timer);
                            break;
                        }
                        else
                        {
                            timer.Field = field; // 更新计时器剩余次数
                            timer.OnSecond?.Invoke();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 处理计时器完成事件
        /// </summary>
        /// <param name="timer">完成的计时器</param>
        private void OnTimerComplete(Timer timer)
        {
            timer.IsFinish = true;
            if (timer.OnComplete is { } onComplete) // 若OnComplete事件存在，触发事件
            {
                onComplete.Invoke();
            }
        }

        /// <summary>
        /// 注册一个基于时间的计时器并返回Timer对象
        /// </summary>
        /// <param name="handle">计时器持有者对象</param>
        /// <param name="step">计时间隔（秒）</param>
        /// <param name="delay">初始延迟（秒）</param>
        /// <param name="field">触发次数，0表示1次</param>
        /// <param name="onSecond">每次触发的回调</param>
        /// <param name="onComplete">完成时的回调</param>
        /// <returns>Timer对象</returns>
        public Timer AddTimer(object handle, float step = 1f, float delay = 0f, int field = 0, Action onSecond = null, Action onComplete = null)
        {
            int id = Guid.NewGuid().GetHashCode(); // 生成一个唯一的ID
            Timer timer = new Timer(handle, id, step, delay, field, onSecond, onComplete, false); // 创建一个计时器对象
            times.Add(timer);
            return timer;
        }

        /// <summary>
        /// 注册一个以帧为单位的计时器并返回Timer对象
        /// </summary>
        /// <param name="handle">计时器持有者对象</param>
        /// <param name="stepFrame">计时间隔（帧）</param>
        /// <param name="delayFrame">初始延迟（帧）</param>
        /// <param name="field">触发次数，0表示1次</param>
        /// <param name="onFrame">每帧触发的回调</param>
        /// <param name="onComplete">完成时的回调</param>
        /// <returns>Timer对象</returns>
        public Timer AddTimerFrame(object handle, float stepFrame = 1f, float delayFrame = 0f, int field = 0, Action onFrame = null, Action onComplete = null)
        {
            int id = Guid.NewGuid().GetHashCode(); // 生成一个唯一的ID
            Timer timer = new Timer(handle, id, stepFrame, delayFrame, field, onFrame, onComplete, true); // 创建一个以帧为单位的计时器对象
            times.Add(timer);
            return timer;
        }

        /// <summary>
        /// 根据ID注销计时器
        /// </summary>
        /// <param name="id">计时器ID</param>
        public void RemoveTimer(int id)
        {
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i].ID == id)
                {
                    times[i].Destroy(); // 调用Timer的Destroy方法
                    times.RemoveAt(i);  // 从列表中移除
                    break;
                }
            }
        }

        /// <summary>
        /// 直接移除Timer对象
        /// </summary>
        /// <param name="timer">要移除的Timer对象</param>
        public void RemoveTimer(Timer timer)
        {
            if (timer != null)
            {
                timer.Destroy(); // 调用Timer的Destroy方法
                times.Remove(timer); // 从列表中移除
            }
        }

        /// <summary>
        /// 获取计时器
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public Timer GetTimer(int _id)
        {
            foreach (var item in times)
            {
                if (item.ID == _id)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置服务器时间
        /// </summary>
        /// <param name="val">服务器时间（毫秒）</param>
        public void SetServerTime(long val)
        {
            // 如果传入的值不为0，则更新服务器时间和临时时间
            if (val != 0)
            {
                serverTime = val;
                tempTime = GetTime();
            }
        }

        /// <summary>
        /// 获取服务器时间（毫秒）
        /// </summary>
        /// <returns>当前服务器时间</returns>
        public long GetServerTime()
        {
            // 返回服务器时间加上当前时间与临时时间之间的差值
            return (serverTime + (GetTime() - tempTime));
        }

        /// <summary>
        /// 获取游戏中的总时长（毫秒）
        /// </summary>
        /// <returns>游戏运行的总时长</returns>
        public long GetTime()
        {
            //可改为Unity启动的总时长
            // float floatValue = Time.time;
            // long longValue = (long)(floatValue * 1000000);
            // return longValue;
            // 返回当前时间与初始化时间的差值
            return GetLocalTime() - initTime;
        }

        /// <summary>
        /// 获取本地时间（毫秒）
        /// </summary>
        /// <returns>当前本地时间的毫秒数</returns>
        public long GetLocalTime()
        {
            // 返回当前UTC时间的毫秒数
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 暂停所有计时器，或指定计时器
        /// </summary>
        /// <param name="id">计时器ID，0表示所有计时器</param>
        public void Pause(int id = 0)
        {
            if (id == 0)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    times[i].IsPaused = true;
                }
            }
            else
            {
                for (int i = 0; i < times.Count; i++)
                {
                    if (times[i].ID == id)
                    {
                        times[i].IsPaused = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 暂停指定的Timer对象
        /// </summary>
        /// <param name="timer">要暂停的Timer对象</param>
        public void Pause(Timer timer)
        {
            if (timer != null)
            {
                timer.IsPaused = true;
            }
        }

        /// <summary>
        /// 恢复所有计时器，或指定计时器
        /// </summary>
        /// <param name="id">计时器ID，0表示所有计时器</param>
        public void Resume(int id = 0)
        {
            if (id == 0)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    times[i].IsPaused = false;
                }
            }
            else
            {
                for (int i = 0; i < times.Count; i++)
                {
                    if (times[i].ID == id)
                    {
                        times[i].IsPaused = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 恢复指定的Timer对象
        /// </summary>
        /// <param name="timer">要恢复的Timer对象</param>
        public void Resume(Timer timer)
        {
            if (timer != null)
            {
                timer.IsPaused = false;
            }
        }

        /// <summary>
        /// 添加应用程序焦点变化的监听
        /// </summary>
        public void AddListenerApplicationFocus()
        {
            MsgMgr.Ins.AddEventListener(MsgEvent.ApplicationFocus, OnApplicationFocus, this);
            MsgMgr.Ins.AddEventListener(MsgEvent.NotApplicationFocus, NotApplicationFocus, this);
        }

        /// <summary>
        /// 当应用程序获得焦点时调用
        /// </summary>
        void OnApplicationFocus()
        {
            isFocus = true;
        }

        /// <summary>
        /// 当应用程序失去焦点时调用
        /// </summary>
        void NotApplicationFocus()
        {
            isFocus = false;
        }

        /// <summary>
        /// 重新启动所有计时器，或指定计时器
        /// </summary>
        /// <param name="id">计时器ID，0表示所有计时器</param>
        public void Restart(int id = 0)
        {
            if (id == 0)
            {
                for (int i = 0; i < times.Count; i++)
                {
                    times[i].Reset();
                }
            }
            else
            {
                for (int i = 0; i < times.Count; i++)
                {
                    if (times[i].ID == id)
                    {
                        times[i].Reset();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 重新启动指定的Timer对象
        /// </summary>
        /// <param name="timer">要重启的Timer对象</param>
        public void Restart(Timer timer)
        {
            if (timer != null)
            {
                timer.Reset();
            }
        }

        /// <summary>
        /// 销毁指定持有者的所有计时器
        /// </summary>
        /// <param name="handle">持有者对象</param>
        public void DestroyTimersByHandle(object handle)
        {
            for (int i = times.Count - 1; i >= 0; i--)
            {
                if (times[i].Handle == handle)
                {
                    times[i].Destroy();
                    times.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 销毁所有计时器
        /// </summary>
        public void DestroyAllTimers()
        {
            for (int i = 0; i < times.Count; i++)
            {
                times[i].Destroy();
            }
            times.Clear();
        }

        /// <summary>
        /// 销毁指定类型的计时器（时间计时器或帧计时器）
        /// </summary>
        /// <param name="isFrameTimer">true为帧计时器，false为时间计时器</param>
        public void DestroyTimersByType(bool isFrameTimer)
        {
            for (int i = times.Count - 1; i >= 0; i--)
            {
                if (times[i].IsFrameTimer == isFrameTimer)
                {
                    times[i].Destroy();
                    times.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取指定持有者的计时器数量
        /// </summary>
        /// <param name="handle">持有者对象</param>
        /// <returns>计时器数量</returns>
        public int GetTimerCountByHandle(object handle)
        {
            int count = 0;
            for (int i = 0; i < times.Count; i++)
            {
                if (times[i].Handle == handle)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取所有计时器数量
        /// </summary>
        /// <returns>计时器总数</returns>
        public int GetTotalTimerCount()
        {
            return times.Count;
        }
    }
}