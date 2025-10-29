using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

namespace Rain.UI
{
    public class UIControlDemo_DynamicTableData : ITabData
    {
        public string name;
        public List<UIControlDemo_DynamicTabData> category = new List<UIControlDemo_DynamicTabData>();
    }

    public class UIControlDemo_DynamicTabData : ITabData
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
            UIControlDemo_DynamicContainerItemData data = new UIControlDemo_DynamicContainerItemData();
            data.name = $"Item {itemList.Count}";
            data.OnSelectItemIndex = (i) => { Debug.Log($"onclick {i}"); };
            itemList.Add(data);
        }

        public void AddItem(UIControlDemo_DynamicContainerItemData _data)
        {
            itemList.Add(_data);
        }

        public void RemoveItem()
        {
            if (itemList.Count > 0)
                itemList.RemoveAt(itemList.Count - 1);
        }
    }
}