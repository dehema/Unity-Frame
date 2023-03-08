using System.Collections;
using UnityEngine;

public class PoolItemBase : BaseUI
{
    public virtual void OnCreate(params object[] _params)
    {
        _LoadUI();
    }

    public virtual void OnCollect()
    {

    }

    internal virtual void _LoadUI()
    {

    }
}
