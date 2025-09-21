using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.UI
{
    public class UIControlDemo_DynamicContainerItem : InfiniteScrollItem
    {
        private UIControlDemo_DynamicContainerItemData itemData;

        public Text itemIndex = null;
        public Text itemName = null;
        public Image itemImage = null;

        public Button button = null;
        public System.Action<int> buttonEvent;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            itemData = scrollData as UIControlDemo_DynamicContainerItemData;

            itemIndex.text = itemData.index;
            itemName.text = itemData.name;
            itemImage.sprite = itemData.icon;

            button.interactable = itemData.buttonEnabled;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClickEvent);
        }

        private void OnClickEvent()
        {
            if (itemData.buttonEvent != null)
            {
                itemData.buttonEvent(GetDataIndex());
            }
        }
    }

    public class UIControlDemo_DynamicContainerItemData : InfiniteScrollData
    {
        public string index = string.Empty;
        public string name = string.Empty;
        public Sprite icon;

        public bool buttonEnabled = false;
        public System.Action<int> buttonEvent;
    }
}
