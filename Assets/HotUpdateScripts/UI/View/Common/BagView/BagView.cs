using System;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包
/// </summary>
public partial class BagView : BaseView
{
    BagViewParam param;
    ItemType currPageItemType;
    DBBinding handler;
    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        param = _viewParam as BagViewParam;
        InitData();
        InitUI();
    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
        handler = PlayerMgr.Ins.playerData.items.Bind((db) =>
        {
            RefreshBagContent(currPageItemType);
        });
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        handler?.UnBind();
    }

    void InitData()
    {
        currPageItemType = ItemType.Res;
    }

    void InitUI()
    {
        ui.InfiniteScroll_InfiniteScroll.onChangeValue.AddListener(OnChangeValue);
        ui.InfiniteScroll_InfiniteScroll.onChangeActiveItem.AddListener(OnChangeActiveItem);
        ui.InfiniteScroll_InfiniteScroll.onStartLine.AddListener(OnStartLine);
        ui.InfiniteScroll_InfiniteScroll.onEndLine.AddListener(OnEndLine);
        ui.tgItemTypeRes_Toggle.SetToggle((bool ison) => { if (ison) RefreshBagContent(ItemType.Res); });
        ui.tgItemTypeSpeed_Toggle.SetToggle((bool ison) => { if (ison) RefreshBagContent(ItemType.Speed); });
        ui.tgItemTypeBuff_Toggle.SetToggle((bool ison) => { if (ison) RefreshBagContent(ItemType.Buff); });
        ui.tgItemTypeEquip_Toggle.SetToggle((bool ison) => { if (ison) RefreshBagContent(ItemType.Equip); });
        ui.tgItemTypeOther_Toggle.SetToggle((bool ison) => { if (ison) RefreshBagContent(ItemType.Other); });
        ui.btClose_Button.SetButton(Close);
    }

    List<DBItemData> currItemDatas = new List<DBItemData>();
    void RefreshBagContent(ItemType _itemType)
    {
        currPageItemType = _itemType;
        currItemDatas = PlayerMgr.Ins.GetAllItemByType(_itemType);
        ui.InfiniteScroll_InfiniteScroll.ClearData();
        float itemSize = (ui.InfiniteScroll_Rect.rect.width -
            ui.InfiniteScroll_InfiniteScroll.GetPadding().x * 2 -
            (ui.InfiniteScroll_InfiniteScroll.layout.GridCount() - 1) * ui.InfiniteScroll_InfiniteScroll.GetSpace().x) /
            ui.InfiniteScroll_InfiniteScroll.layout.GridCount();
        BagViewBagItemData[] datas = new BagViewBagItemData[currItemDatas.Count];
        for (int index = 0; index < currItemDatas.Count; index++)
        {
            DBItemData itemData = currItemDatas[index];
            BagViewBagItemData data = new BagViewBagItemData();
            data.index = index;
            data.itemConfig = itemData.ItemConfig;
            data.itemNum = itemData.ItemNum.Value;
            data.itemSize = itemSize;
            data.buttonEvent = (index) => { Debug.Log($"点击 {index}"); };
            datas[index] = data;
        }
        ui.InfiniteScroll_InfiniteScroll.InsertData(datas);
    }

    public void OnChangeValue(int firstItemIndex, int lastItemIndex, bool isStartLine, bool isEndLine)
    {
        string str1 = isStartLine ? "到达最上方" : "没有到最上方";
        string str2 = isEndLine ? "到达最下方" : "没有到最下方";
        Debug.Log($"显示第{firstItemIndex}-{lastItemIndex}个元素,{str1},{str2}");
    }

    public void OnChangeActiveItem(int index, bool _isShow)
    {
        if (_isShow)
        {
            Debug.Log($"显示第 {index} 个对象");
        }
        else
        {
            Debug.Log($"隐藏第 {index} 个对象");
        }
    }

    public void OnStartLine(bool isStartLine)
    {
        string str1 = isStartLine ? "到达最上方" : "没有到最上方";
        Debug.Log(str1);
    }

    public void OnEndLine(bool isEndLine)
    {
        string str2 = isEndLine ? "到达最下方" : "没有到最下方";
        Debug.Log(str2);
    }
}

public class BagViewParam : IViewParam
{

}
