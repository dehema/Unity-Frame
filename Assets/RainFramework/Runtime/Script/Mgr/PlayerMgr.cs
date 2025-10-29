using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

public class PlayerMgr : MonoSingleton<PlayerMgr>
{
    public PlayerData playerData => DataMgr.Ins.playerData;

    /// <summary>
    /// 读取品质图片
    /// </summary>
    /// <param name="_quality"></param>
    /// <returns></returns>
    public Sprite GetQualitySprite(int _quality)
    {
        string spriteName = GetQualitySpriteName(_quality);
        return AssetMgr.Ins.Load<Sprite>(spriteName);
    }

    /// <summary>
    /// 品质图片名字
    /// </summary>
    /// <param name="_quality"></param>
    /// <returns></returns>
    public string GetQualitySpriteName(int _quality)
    {
        switch (_quality)
        {
            case 1:
            default:
                return "Btn_Rectangle00_n_Gray";
            case 2:
                return "Btn_Rectangle00_n_Green";
            case 3:
                return "Btn_Rectangle00_n_Blue";
            case 4:
                return "Btn_Rectangle00_n_Purple";
            case 5:
                return "Btn_Rectangle00_n_Orange";

        }
    }


    /// <summary>
    /// 获得该类型所有道具
    /// </summary>
    /// <param name="itemType"></param>
    public List<DBItemData> GetAllItemByType(ItemType _itemType)
    {
        List<DBItemData> itemDatas = new List<DBItemData>();
        foreach (var item in playerData.items)
        {
            if (item.Value.ItemConfig.ItemType == _itemType)
            {
                itemDatas.Add(item.Value);
            }
        }
        return itemDatas;
    }

    /// <summary>
    /// 获取道具数量
    /// </summary>
    /// <param name="_itemID"></param>
    /// <returns></returns>
    public int GetItemNum(string _itemID)
    {
        playerData.items.TryGetValue(_itemID, out DBItemData _itemData);
        return _itemData == null ? 0 : _itemData.ItemNum.Value;
    }

    /// <summary>
    /// 获取道具字段
    /// </summary>
    /// <param name="_itemID"></param>
    /// <returns></returns>
    public DBItemData GetResDBField(string _itemID)
    {
        playerData.items.TryGetValue(_itemID, out DBItemData _itemData);
        return _itemData;
    }

    /// <summary>
    /// 设置道具数量（调试用）
    /// </summary>
    /// <param name="_itemID"></param>
    /// <param name="_num"></param>
    public void SetItemNum(string _itemID, int _num)
    {
        playerData.items.TryGetValue(_itemID, out DBItemData _itemData);
        if (_itemData == null)
        {
            DBItemData itemData = new DBItemData(_itemID);
            itemData.ItemNum.SetVal(_num);
            playerData.items[_itemID] = itemData;
        }
        else
        {
            _itemData.ItemNum.Value = _num;
        }
    }

    /// <summary>
    /// 获取资源数据字段
    /// </summary>
    /// <param name="_resType"></param>
    /// <returns></returns>
    public DBLong GetResDBField(ResType _resType)
    {
        switch (_resType)
        {
            case ResType.Food:
                return DataMgr.Ins.playerData.food;
            case ResType.Wood:
                return DataMgr.Ins.playerData.wood;
            case ResType.Gold:
                return DataMgr.Ins.playerData.gold;
            case ResType.Book:
                return DataMgr.Ins.playerData.book;
            case ResType.Ore:
                return DataMgr.Ins.playerData.ore;
        }
        return null;
    }

    /// <summary>
    /// 获取资源数量
    /// </summary>
    /// <param name="_resType"></param>
    /// <returns></returns>
    public long GetResType(ResType _resType)
    {
        DBLong field = GetResDBField(_resType);
        return field != null ? field.Value : 0;

    }

    /// <summary>
    /// 增加资源数量
    /// </summary>
    /// <param name="_resType"></param>
    /// <param name="_addNum"></param>
    public void AddResNum(ResType _resType, long _addNum)
    {
        DBLong field = GetResDBField(_resType);
        if (field != null)
        {
            field.Value += _addNum;
        }
    }

    /// <summary>
    /// 增加资源数量
    /// </summary>
    /// <param name="_addNum"></param>
    public void AddResNum(Dictionary<ResType, long> _addNum)
    {
        foreach (var item in _addNum)
        {
            AddResNum(item.Key, item.Value);
        }
    }

    /// <summary>
    /// 资源是否足够
    /// </summary>
    /// <param name="_resType"></param>
    /// <param name="_needNum"></param>
    /// <returns></returns>
    public bool IsResEnough(ResType _resType, long _needNum)
    {
        long num = GetResType(_resType);
        bool res = num >= _needNum;
        return res;
    }

    /// <summary>
    /// 资源是否足够
    /// </summary>
    /// <param name="_costDic"></param>
    /// <returns></returns>
    public bool IsResEnough(Dictionary<ResType, long> _costDic)
    {
        foreach (var item in _costDic)
        {
            if (!IsResEnough(item.Key, item.Value))
                return false;
        }
        return true;
    }

    /// <summary>
    /// 获取科技数据
    /// </summary>
    /// <param name="_techID"></param>
    /// <returns></returns>
    public TechData GetTechData(string _techID)
    {
        DataMgr.Ins.playerData.techs.TryGetValue(_techID, out TechData techData);
        return techData;
    }

    /// <summary>
    /// 获取科技状态
    /// </summary>
    /// <returns></returns>
    public TechState GetTechState(string _techID)
    {
        TechData techData = GetTechData(_techID);
        return (TechState)techData.state.Value;
    }
}
