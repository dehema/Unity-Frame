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
        public List<UIControlDemo_DynamicContainerItemData> itemList = new List<UIControlDemo_DynamicContainerItemData>();

        public void MakeRandomItem(int from, int to)
        {
            for (int i = 0; i < Random.Range(from, to); i++)
            {
                AddItem();
            }
        }

        public void AddItem()
        {
            UIControlDemo_DynamicContainerItemData item = new UIControlDemo_DynamicContainerItemData();
            item.name = $"Item {itemList.Count}";
            item.buttonEvent = (i) => { Debug.Log($"onclick {i}"); };
            itemList.Add(item);
        }

        public void RemoveItem()
        {
            if (itemList.Count > 0)
                itemList.RemoveAt(itemList.Count - 1);
        }
    }
}