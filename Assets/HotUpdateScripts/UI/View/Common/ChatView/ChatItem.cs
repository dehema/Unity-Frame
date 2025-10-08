using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using static TechRow;

public partial class ChatItem : InfiniteScrollItem
{
    ChatItemData chatItemData;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);
        chatItemData = scrollData as ChatItemData;
        ui.lbContent_Text.text = chatItemData.msg;
    }
}

/// <summary>
/// 科技行数据
/// </summary>
public class ChatItemData : UIControlDemo_DynamicContainerItemData
{
    public string playerIconUrl;
    public string msg;
}