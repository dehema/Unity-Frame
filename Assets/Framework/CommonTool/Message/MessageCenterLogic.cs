using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 消息管理器
/// </summary>
public class MessageCenterLogic:MonoSingleton<MessageCenterLogic>
{
    //保存所有消息事件的字典
    //key使用字符串保存消息的名称
    //value使用一个带自定义参数的事件，用来调用所有注册的消息
    private Dictionary<string, Action<MessageData>> dictionaryMessage;

    /// <summary>
    /// 私有构造函数
    /// </summary>
    private MessageCenterLogic()
    {
        InitData();
    }

    private void InitData()
    {
        //初始化消息字典
        dictionaryMessage = new Dictionary<string, Action<MessageData>>();
    }

    /// <summary>

    /// 注册消息事件
    /// </summary>
    /// <param name="key">消息名</param>
    /// <param name="action">消息事件</param>
    public void Register(string key, Action<MessageData> action)
    {
        if (!dictionaryMessage.ContainsKey(key))
        {
            dictionaryMessage.Add(key, null);
        }
        dictionaryMessage[key] += action;
    }



    /// <summary>
    /// 注销消息事件
    /// </summary>
    /// <param name="key">消息名</param>
    /// <param name="action">消息事件</param>
    public void Remove(string key, Action<MessageData> action)
    {
        if (dictionaryMessage.ContainsKey(key) && dictionaryMessage[key] != null)
        {
            dictionaryMessage[key] -= action;
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="key">消息名</param>
    /// <param name="data">消息传递数据，可以不传</param>
    public void Send(string key, MessageData data = null)
    {
        if (dictionaryMessage.ContainsKey(key) && dictionaryMessage[key] != null)
        {
            dictionaryMessage[key](data);
        }
    }

    /// <summary>
    /// 清空所有消息
    /// </summary>
    public void Clear()
    {
        dictionaryMessage.Clear();
    }
}
