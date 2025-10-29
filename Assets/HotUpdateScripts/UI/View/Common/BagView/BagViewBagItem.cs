using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class BagViewBagItem : InfiniteScrollItem
{
    public BagViewBagItemData ItemData;
    public override void UpdateData(InfiniteScrollData _scrollData)
    {
        base.UpdateData(scrollData);
        ItemData = _scrollData as BagViewBagItemData;
        ui.content_Rect.SetSizeDeltaWidth(ItemData.itemSize);
        ui.content_Rect.SetSizeDeltaHeight(ItemData.itemSize);
        ui.num_Text.text = Util.Text.FormatNum(ItemData.itemNum);
        ui.param_Text.text = ItemData.itemConfig.Param.ToString();
        ui.icon_Image.sprite = AssetMgr.Ins.Load<Sprite>(ItemData.itemConfig.IconName);
        ui.board_Image.sprite = PlayerMgr.Ins.GetQualitySprite(ItemData.itemConfig.Quality);
        ui.board_Button.SetButton(() =>
        {
            ItemData.OnSelectItem?.Invoke(this);
        });
    }

    public override void SetActive(bool active, bool notifyEvent = true)
    {
        base.SetActive(active, notifyEvent);
    }

    /// <summary>
    /// ÊÇ·ñ±»Ñ¡Ôñ
    /// </summary>
    /// <param name="_isOn"></param>
    public void SetOnSelect(bool _isOn)
    {
        ui.onSel.SetActive(_isOn);
    }
}



public class BagViewBagItemData : UIControlDemo_DynamicContainerItemData
{
    public int itemNum;
    public ItemConfig itemConfig;
    public System.Action<BagViewBagItem> OnSelectItem;
}
