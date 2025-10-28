using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// 道具数据
/// </summary>
[Serializable]
public class DBItemData : DBClass
{
    /// <summary>
    /// 道具ID
    /// </summary>
    public string ItemID;
    /// <summary>
    /// 道具数量
    /// </summary>
    public DBInt ItemNum;


    /// <summary>
    /// 获取道具配置
    /// </summary>
    public ItemConfig ItemConfig
    {
        get
        {
            return ConfigMgr.Item.Get(ItemID);
        }
    }

    public bool IsRes => ItemConfig.ItemType == ItemType.Res;
    public bool IsSpeed => ItemConfig.ItemType == ItemType.Speed;
    public bool IsBuff => ItemConfig.ItemType == ItemType.Buff;
    public bool IsEquip => ItemConfig.ItemType == ItemType.Equip;
    public bool IsOther => ItemConfig.ItemType == ItemType.Other;

    public DBItemData() { }
    public DBItemData(string _itemID, int _itemNum = 0)
    {
        ItemID = _itemID;
        ItemNum = new DBInt(_itemNum);
    }
}
