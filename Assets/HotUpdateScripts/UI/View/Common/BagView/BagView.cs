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
    private InfiniteScroll infinite { get { return ui.InfiniteScroll_InfiniteScroll; } }
    private BagViewBagItem currSelectItem;
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
        OnSelectItem(null);
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
        infinite.onChangeValue.AddListener(OnChangeValue);
        infinite.onChangeActiveItem.AddListener(OnChangeActiveItem);
        infinite.onStartLine.AddListener(OnStartLine);
        infinite.onEndLine.AddListener(OnEndLine);
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
        currSelectItem = null;
        currPageItemType = _itemType;
        currItemDatas = PlayerMgr.Ins.GetAllItemByType(_itemType);
        infinite.ClearData();
        float itemSize = (ui.InfiniteScroll_Rect.rect.width - infinite.GetPadding().x * 2 - (infinite.layout.GridCount() - 1) * infinite.GetSpace().x)
            / infinite.layout.GridCount();
        BagViewBagItemData[] datas = new BagViewBagItemData[currItemDatas.Count];
        for (int index = 0; index < currItemDatas.Count; index++)
        {
            DBItemData itemData = currItemDatas[index];
            BagViewBagItemData data = new BagViewBagItemData();
            data.index = index;
            data.itemConfig = itemData.ItemConfig;
            data.itemNum = itemData.ItemNum.Value;
            data.itemSize = itemSize;
            data.OnSelectItem = OnSelectItem;
            datas[index] = data;
        }
        infinite.InsertData(datas);
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

    /// <summary>
    /// 当选择对象
    /// </summary>
    private void OnSelectItem(BagViewBagItem _item)
    {
        if (currSelectItem == _item)
            currSelectItem = null;
        else
            currSelectItem = _item;
        foreach (var item in infinite.GetAllItem())
        {
            if (item != currSelectItem)
            {
                (item as BagViewBagItem).SetOnSelect(false);
            }
        }
        currSelectItem?.SetOnSelect(true);
        RefreshSelItemDialog();
    }

    void RefreshSelItemDialog()
    {
        ui.selItemDialog.SetActive(currSelectItem != null);
        ui.selItemDialog_BagViewSelItemDialog.SetBagViewBagItem(currSelectItem);
    }
}

public class BagViewParam : IViewParam
{

}
