using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

namespace Rain.UIControlDemo
{

    public class UIControlDemo_DynamicTabData : ITabData
    {
        public string name;
        public List<UIControlDemo_DynamicTabItem> category = new List<UIControlDemo_DynamicTabItem>();
    }

    public class UIControlDemo_DynamicTabItem : ITabData
    {
        public string name;
        public bool isVertical = true;
        public List<UIControlDemo_DynamicTabPageItemData> itemList = new List<UIControlDemo_DynamicTabPageItemData>();

        public void MakeRandomItem(int from, int to)
        {
            isVertical = Random.Range(0, 2) == 0;
            for (int i = 0; i < Random.Range(from, to); i++)
            {
                UIControlDemo_DynamicTabPageItemData item = new UIControlDemo_DynamicTabPageItemData();
                item.name = "Item " + i;
                item.description = "desc " + i;

                item.buttonText = i.ToString();

                itemList.Add(item);
            }
        }
    }

    public class UIControlDemo_DynamicTabPageItemData
    {
        public string name;
        public string description;
        public string buttonText;
        public System.Action<int> buttonEvent;
    }
}