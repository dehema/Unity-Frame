using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UIControlDemo;
using UnityEngine;
using UnityEngine.UIElements;

public class UIControlDemo_DynamicTab_Page : MonoBehaviour
{
    public InfiniteScroll scroll;

    public void OnNotify(Tab tab)
    {
        if (tab.IsSelected() == true)
        {
            //记录滚动条的位置
            Vector2 scrollPos = scroll.GetScrollPosition();
            UIControlDemo_DynamicTabItem data = (UIControlDemo_DynamicTabItem)tab.GetData();

            scroll.ClearData();

            for (int index = 0; index < data.itemList.Count; index++)
            {
                AddData(data.itemList[index]);
            }
            //scroll.MoveToLastData();
            //移动滚动条的位置，避免出现滚动条闪现的情况
            scroll.SetScrollPosition(scrollPos);

            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void AddData(UIControlDemo_DynamicContainerItemData itemData)
    {
        UIControlDemo_DynamicContainerItemData scrollData = new UIControlDemo_DynamicContainerItemData();
        scrollData.name = itemData.name;
        scrollData.buttonEnabled = true;
        scrollData.buttonEvent = itemData.buttonEvent;

        scroll.InsertData(scrollData);
    }

    public void RemoveData(int index)
    {
        scroll.RemoveData(index);
    }
}

