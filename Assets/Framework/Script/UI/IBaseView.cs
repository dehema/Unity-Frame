using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBaseView
{
    public void OnOpen(params object[] _params);
    public void OnClose(Action action);
}
