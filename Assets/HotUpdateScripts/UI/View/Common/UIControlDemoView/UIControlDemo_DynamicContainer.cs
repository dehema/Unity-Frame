using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.UI;
using Resources = UnityEngine.Resources;

namespace Rain.UI
{
    public class UIControlDemo_DynamicContainer : MonoBehaviour
    {
        [SerializeField]
        InfiniteScroll infiniteScroll;

        private void Start()
        {
            InitData();
        }

        private void InitData()
        {
            Object[] objects = Resources.LoadAll<Sprite>("UI/Common");
            int index = 0;
            foreach (Object obj in objects)
            {
                Sprite sprite = obj as Sprite;
                UIControlDemo_DynamicContainerItemData data = new UIControlDemo_DynamicContainerItemData();
                data.index = index;
                data.name = sprite.name;
                data.icon = sprite;
                data.buttonEnabled = true;
                data.buttonEvent = (index) => { Debug.Log($"��� {index}"); };
                infiniteScroll.InsertData(data);
                index++;
            }
        }

        public void OnChangeValue(int firstItemIndex, int lastItemIndex, bool isStartLine, bool isEndLine)
        {
            string str1 = isStartLine ? "�������Ϸ�" : "û�е����Ϸ�";
            string str2 = isEndLine ? "�������·�" : "û�е����·�";
            Debug.Log($"��ʾ��{firstItemIndex}-{lastItemIndex}��Ԫ��,{str1},{str2}");
        }

        public void OnChangeActiveItem(int index, bool _isShow)
        {
            if (_isShow)
            {
                Debug.Log($"��ʾ�� {index} ������");
            }
            else
            {
                Debug.Log($"���ص� {index} ������");
            }
        }

        public void OnStartLine(bool isStartLine)
        {
            string str1 = isStartLine ? "�������Ϸ�" : "û�е����Ϸ�";
            Debug.Log(str1);
        }

        public void OnEndLine(bool isEndLine)
        {
            string str2 = isEndLine ? "�������·�" : "û�е����·�";
            Debug.Log(str2);
        }
    }
}
