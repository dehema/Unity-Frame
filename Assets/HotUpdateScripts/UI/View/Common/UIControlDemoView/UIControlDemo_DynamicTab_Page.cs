using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

namespace Rain.UI
{

    public class UIControlDemo_DynamicTab_Page : MonoBehaviour
    {
        public InfiniteScroll scroll;

        public void OnNotify(Tab tab)
        {
            if (tab.IsSelected() == true)
            {
                //��¼��������λ��
                Vector2 scrollPos = scroll.GetScrollPosition();
                ITabData _ITabData = tab.GetData();
                UIControlDemo_DynamicTabData data = (UIControlDemo_DynamicTabData)_ITabData;

                scroll.ClearData();

                for (int index = 0; index < data.itemList.Count; index++)
                {
                    AddData(data.itemList[index]);
                }
                //scroll.MoveToLastData();
                //�ƶ���������λ�ã�������ֹ��������ֵ����
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
            //UIControlDemo_DynamicContainerItemData scrollData = new UIControlDemo_DynamicContainerItemData();
            //scrollData.name = itemData.name;
            //scrollData.buttonEnabled = true;
            //scrollData.buttonEvent = itemData.buttonEvent;
            //scroll.InsertData(scrollData);

            scroll.InsertData(itemData);
        }

        public void RemoveData(int index)
        {
            scroll.RemoveData(index);
        }
    }
}