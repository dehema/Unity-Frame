using DB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 绑定事件分发器
/// </summary>
public class DBDispatcher
{
    private struct DispatcherItem
    {
        public DBModify modify;
        public DBKeyModify keyModify;
        public Action<DBModify> action;
        public Action<DBKeyModify> keyAction;
    }

    private HashSet<DBObject> objs = new HashSet<DBObject>();
    private List<DispatcherItem> items = new List<DispatcherItem>();


    public void Add(DBObject obj, Action<DBModify> action, DBModify modify)
    {
        this.objs.Add(obj);
        this.items.Add(new DispatcherItem
        {
            action = action,
            modify = modify,
        }); ;
    }

    public void Add(DBObject obj, Action<DBKeyModify> action, DBKeyModify modify)
    {
        this.objs.Add(obj);
        this.items.Add(new DispatcherItem
        {
            action = null,
            modify = null,
            keyAction = action,
            keyModify = modify
        });
    }

    public void Dispatch()
    {
        foreach (var item in items)
        {
            if (item.action != null)
            {
                item.action(item.modify);
            }
            if (item.keyAction != null)
            {
                item.keyAction(item.keyModify);
            }
        }
        this.objs.Clear();
        this.items.Clear();
    }
}
