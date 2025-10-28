using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// ��������
/// </summary>
[Serializable]
public class DBItemData : DBClass
{
    /// <summary>
    /// ����ID
    /// </summary>
    public string ItemID;
    /// <summary>
    /// ��������
    /// </summary>
    public DBInt ItemNum;


    /// <summary>
    /// ��ȡ��������
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
