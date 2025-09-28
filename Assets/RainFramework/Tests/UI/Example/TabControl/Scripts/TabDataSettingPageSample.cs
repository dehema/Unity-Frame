using Rain.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.Core
{
    public class TabDataSettingPageSample : MonoBehaviour
    {
        public Text text;

        public void DataSetting(Tab tab)
        {
            if (tab.IsSelected() == true)
            {
                TabDataSettingSampleData sampleData = (TabDataSettingSampleData)tab.GetData();

                text.text = sampleData.text;
            }
        }
    }
}