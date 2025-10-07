using System;
using System.Collections;
using System.Collections.Generic;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 聊天
/// </summary>
public partial class ChatView : BaseView
{
    ChatViewParam param;
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        param = _viewParams as ChatViewParam;
    }

    public override void OnOpen(IViewParam _viewParams = null)
    {
        base.OnOpen(_viewParams);
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
    }
}

public class ChatViewParam : IViewParam
{

}
