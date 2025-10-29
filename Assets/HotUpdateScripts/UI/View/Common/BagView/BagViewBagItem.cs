using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class BagViewBagItem : InfiniteScrollItem
{
    BagViewBagItemData itemData;
    public override void UpdateData(InfiniteScrollData _scrollData)
    {
        base.UpdateData(scrollData);
        itemData = _scrollData as BagViewBagItemData;
        ui.content_Rect.SetSizeDeltaWidth(itemData.itemSize);
        ui.content_Rect.SetSizeDeltaHeight(itemData.itemSize);
        ui.num_Text.text = Util.Text.FormatNum(itemData.itemNum);
        ui.param_Text.text = itemData.itemConfig.Param.ToString();
        ui.icon_Image.sprite = AssetMgr.Ins.Load<Sprite>(itemData.itemConfig.IconName);
        ui.board_Image.sprite = PlayerMgr.Ins.GetQualitySprite(itemData.itemConfig.Quality);
    }

    public override void SetActive(bool active, bool notifyEvent = true)
    {
        base.SetActive(active, notifyEvent);
    }
}



public class BagViewBagItemData : UIControlDemo_DynamicContainerItemData
{
    public int itemNum;
    public ItemConfig itemConfig;
}
