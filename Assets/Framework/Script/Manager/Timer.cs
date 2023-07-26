using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// 计时器 触发事件
/// </summary>
public class Timer : MonoSingleton<Timer>
{
    /// <summary>
    /// 所有的计时器处理器 默认common分组  可以通过分组统一删除计时器
    /// </summary>
    Dictionary<string, TimerGroup> tgGroup = new Dictionary<string, TimerGroup>();
    public const string defaultTimerGroup = "common";

    void Update()
    {
        //for (int i = tgGroup.Count - 1; i >= 0; i--)
        //{
        //    var tg = tgGroup.ElementAt(i);
        //    if (tg.Value.isRelease)
        //    {
        //        tgGroup.Remove(tg.Key);
        //    }
        //}
        lock (tgGroup)
        {
            foreach (var item in tgGroup.ToList())
            {
                List<TimerHandler> thList = item.Value.timerHandlers;
                for (int i = 0; i < thList.Count; i++)
                {
                    TimerHandler th = thList[i];
                    if (!th.enable)
                    {
                        //没有开启
                        continue;
                    }
                    th.__passTime += Time.deltaTime;
                    if (th.__passTime >= th.totalTime && th.timerType != TimerType.CountDown)
                    {
                        //计时结束
                        RemoveTimerHandler(th, true);
                        i--;
                        continue;
                    }
                    else
                    {
                        if (th.timerType == TimerType.TimeOut)
                        {
                        }
                        else if (th.timerType == TimerType.Interval)
                        {
                            th.__interval += Time.deltaTime;
                            if (th.__interval >= th.interval)
                            {
                                th.__interval -= th.interval;
                                th.action(GetTimerDispatcher(th, TimerCBType.interval));
                            }
                        }
                        else if (th.timerType == TimerType.CountDown)
                        {
                            if (th.__passTime >= th.totalTime)
                            {
                                RemoveTimerHandler(th, true);
                                i--;
                                continue;
                            }
                            else if (Mathf.FloorToInt(th.__passTime) > Mathf.FloorToInt(th.__lastTotalTime))
                            {
                                th.action(GetTimerDispatcher(th, TimerCBType.second));
                                th.__lastTotalTime = th.__passTime;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 将处理器添加到容器列表中
    /// </summary>
    /// <param name="_th"></param>
    /// <param name="_group"></param>
    public void AddTimerHandler(TimerHandler _th)
    {
        string group = _th.timerGroup;
        if (GetTimerListByGroup(group) == null)
            tgGroup.Add(group, new TimerGroup());
        tgGroup[group].timerHandlers.Add(_th);
    }

    /// <summary>
    /// 移除一个计时器
    /// </summary>
    /// <param name="_th"></param>
    /// <param name="_complete">是否触发回调</param>
    /// <param name="_group"></param>
    public void RemoveTimerHandler(TimerHandler _th, bool _complete = false)
    {
        if (_th == null)
        {
            return;
        }
        string group = _th.timerGroup;
        if (!tgGroup.ContainsKey(group))
        {
            return;
        }
        List<TimerHandler> timerList = tgGroup[group].timerHandlers;
        if (!timerList.Contains(_th))
        {
            return;
        }
        if (_complete)
        {
            _th.action(GetTimerDispatcher(_th, TimerCBType.timeOver));
        }
        timerList.Remove(_th);
    }

    /// <summary>
    /// 获取一组计时器
    /// </summary>
    /// <param name="_group"></param>
    /// <returns></returns>
    public TimerGroup GetTimerListByGroup(string _group)
    {
        if (!tgGroup.ContainsKey(_group))
        {
            return null;
        }
        return tgGroup[_group];
    }

    /// <summary>
    /// 清除一组数据
    /// </summary>
    public void RemoveTimerGroup(string _group, bool _complete = false)
    {
        TimerGroup timerList = GetTimerListByGroup(_group);
        if (timerList == null)
        {
            return;
        }
        for (int i = timerList.timerHandlers.Count - 1; i >= 0; i--)
        {
            timerList.timerHandlers[i].Remove(_complete);
        }
        timerList.isRelease = true;
        tgGroup.Remove(_group);
    }

    /// <summary>
    /// 获取报名 
    /// </summary>
    public string GetGroupName(string _name = defaultTimerGroup, params object[] _params)
    {
        if (_params.Length > 0)
        {
            return _name + _params[0].ToString();
        }
        return _name;
    }

    /// <summary>
    /// 计时器
    /// </summary>
    /// <param name="_action"></param>
    /// <param name="_totalTime"></param>
    /// <returns></returns>
    public TimerHandler SetTimeOut(Action _action, float _totalTime, string _group = defaultTimerGroup)
    {
        return SetTimeOut((Nullable) => { _action(); }, _totalTime, _group);
    }

    /// <summary>
    /// 计时器
    /// </summary>
    /// <param name="_action"></param>
    /// <param name="_totalTime"></param>
    public TimerHandler SetTimeOut(Action<TimerDispatcher> _action, float _totalTime, string _group = defaultTimerGroup)
    {
        TimerHandler th = new TimerHandler(_action, TimerType.TimeOut, _group);
        th.totalTime = _totalTime;
        AddTimerHandler(th);
        return th;
    }

    /// <summary>
    /// 定时器
    /// </summary>
    /// <param name="_action"></param>
    /// <param name="_interval"></param>
    /// <param name="_totalTime"></param>
    /// <param name="_group"></param>
    /// <returns></returns>
    public TimerHandler SetInterval(Action _action, float _interval, float _totalTime = int.MaxValue, string _group = defaultTimerGroup)
    {
        return SetInterval((Nullable) => { _action(); }, _interval, _totalTime, _group);
    }

    /// <summary>
    /// 定时器
    /// </summary>
    public TimerHandler SetInterval(Action<TimerDispatcher> _action, float _interval, float _totalTime = int.MaxValue, string _group = defaultTimerGroup)
    {
        TimerHandler th = new TimerHandler(_action, TimerType.Interval, _group);
        th.totalTime = _totalTime;
        th.interval = _interval;
        AddTimerHandler(th);
        return th;
    }

    /// <summary>
    /// 倒计时
    /// </summary>
    /// <param name="_action"></param>
    /// <param name="_totalTime"></param>
    /// <param name="_startTime"></param>
    /// <param name="_group"></param>
    /// <returns></returns>
    public TimerHandler SetCountDown(Action<TimerDispatcher> _action, float _totalTime, float _startTime = 0, string _group = defaultTimerGroup)
    {
        TimerHandler th = new TimerHandler(_action, TimerType.CountDown);
        th.totalTime = _totalTime;
        th.__interval = _startTime;
        AddTimerHandler(th);
        th.action(GetTimerDispatcher(th, TimerCBType.init));
        return th;
    }

    public TimerDispatcher GetTimerDispatcher(TimerHandler _th, TimerCBType _cbType)
    {
        TimerDispatcher td = new TimerDispatcher(_cbType, _th.__passTime);
        td.totalTime = _th.totalTime;
        return td;
    }

    public string GetCountDownTimeStr(float _second, bool _incloudMin = true, bool _incloudHour = false)
    {
        return GetCountDownTimeStr((int)_second, _incloudMin, _incloudHour);
    }

    /// <summary>
    /// 获取倒计时的文本
    /// </summary>
    /// <param name="_second"></param>
    /// <returns></returns>
    public string GetCountDownTimeStr(int _second, bool _incloudMin = true, bool _incloudHour = false)
    {
        string secondStr = (_second % 60).ToString().PadLeft(2, '0');
        string str = secondStr;
        int hour = 0;
        if (_incloudHour)
        {
            hour = _second / 60 / 60;
            _second -= hour * 60 * 60;
        }
        if (_incloudMin)
        {
            int minute = _second / 60;
            string minuteStr = minute.ToString().PadLeft(2, '0');
            str = minuteStr + ":" + str;
        }
        if (_incloudHour)
        {
            string hourStr = hour.ToString().PadLeft(2, '0');
            str = hourStr + ":" + str;
        }
        return str;
    }
}

/// <summary>
/// 计时器处理器
/// </summary>
public class TimerHandler
{
    /// <summary>
    /// 回调
    /// </summary>
    public Action<TimerDispatcher> action;
    public TimerType timerType;
    /// <summary>
    /// 总计时
    /// </summary>
    public float totalTime;
    /// <summary>
    /// 倒计时间隔
    /// </summary>
    public float interval;
    /// <summary>
    /// 临时间隔 当临时间隔大于interval时则触发时间并清空__interval
    /// </summary>
    public float __interval = 0;
    /// <summary>
    /// 经过的时间
    /// </summary>
    public float __passTime = 0;
    /// <summary>
    /// 上次触发的当前时间
    /// </summary>
    public float __lastTotalTime = 0;
    public bool enable = true;
    /// <summary>
    /// 计时器分组
    /// </summary>
    public string timerGroup = Timer.defaultTimerGroup;

    public TimerHandler()
    {
    }

    public TimerHandler(Action<TimerDispatcher> _action, TimerType _timerType, string _group = Timer.defaultTimerGroup)
    {
        action = _action;
        timerType = _timerType;
        timerGroup = string.IsNullOrEmpty(_group) ? Timer.defaultTimerGroup : _group;
    }

    /// <summary>
    /// 是否开启计时器，false为暂停
    /// </summary>
    /// <param name="_enable"></param>
    public void SetEnable(bool _enable)
    {
        enable = _enable;
    }

    /// <summary>
    /// 移除计时器
    /// </summary>
    public void Remove(bool _complete = false)
    {
        Timer.Ins.RemoveTimerHandler(this, _complete);
    }
}

public class TimerDispatcher
{
    public TimerCBType type;
    public float totalTime;
    public float currTime;
    /// <summary>
    /// 当前的秒数
    /// </summary>
    public int currSecond;
    public int countdown { get { return Mathf.FloorToInt(totalTime - currTime); } }

    public TimerDispatcher(TimerCBType _cbType)
    {
        type = _cbType;
    }

    public TimerDispatcher(TimerCBType _cbType, float _currTimer)
    {
        type = _cbType;
        currTime = _currTimer;
        currSecond = (int)currTime;
    }
}

/// <summary>
/// 计时器回调类型
/// </summary>
public enum TimerCBType
{
    /// <summary>
    /// 计时结束
    /// </summary>
    timeOver,
    /// <summary>
    /// 间隔
    /// </summary>
    interval,
    second,
    update,
    init
}

/// <summary>
/// 计时类型
/// </summary>
public enum TimerType
{
    /// <summary>
    /// 计时
    /// </summary>
    TimeOut,
    /// <summary>
    /// 间隔计时
    /// </summary>
    Interval,
    /// <summary>
    /// 倒计时
    /// </summary>
    CountDown,
}


public class TimerGroup
{
    public List<TimerHandler> timerHandlers = new List<TimerHandler>();
    public bool isRelease = false;
}