using Rain.Core;
using Rain.UIControlDemo;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.UIControlDemo
{
    public class UIControlDemo_DynamicTab_Tag : Tab
    {
        public Text titleText;

        public GameObject[] activeObjs;
        public GameObject[] deactiveObjs;

        public override void OnUpdateData(ITabData data)
        {
            base.OnUpdateData(data);

            UIControlDemo_DynamicTabItem sampleData = (UIControlDemo_DynamicTabItem)data;

            titleText.text = sampleData.name;
        }

        public override void OnChangeValue(bool active)
        {
            base.OnChangeValue(active);

            if (activeObjs != null)
            {
                for (int i = 0; i < activeObjs.Length; i++)
                {
                    activeObjs[i].SetActive(active == true);
                }
            }

            if (deactiveObjs != null)
            {
                for (int i = 0; i < deactiveObjs.Length; i++)
                {
                    deactiveObjs[i].SetActive(active == false);
                }
            }
        }
    }
}