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
                data.buttonEvent = (index) => { Debug.Log($"点击 {index}"); };
                infiniteScroll.InsertData(data);
                index++;
            }
        }

        public void OnChangeValue(int firstItemIndex, int lastItemIndex, bool isStartLine, bool isEndLine)
        {
            string str1 = isStartLine ? "到达最上方" : "没有到最上方";
            string str2 = isEndLine ? "到达最下方" : "没有到最下方";
            Debug.Log($"显示第{firstItemIndex}-{lastItemIndex}个元素,{str1},{str2}");
        }

        public void OnChangeActiveItem(int index, bool _isShow)
        {
            if (_isShow)
            {
                Debug.Log($"显示第 {index} 个对象");
            }
            else
            {
                Debug.Log($"隐藏第 {index} 个对象");
            }
        }

        public void OnStartLine(bool isStartLine)
        {
            string str1 = isStartLine ? "到达最上方" : "没有到最上方";
            Debug.Log(str1);
        }

        public void OnEndLine(bool isEndLine)
        {
            string str2 = isEndLine ? "到达最下方" : "没有到最下方";
            Debug.Log(str2);
        }
    }
}
