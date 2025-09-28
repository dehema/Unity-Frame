using Rain.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.Core
{
    public class TabDataSettingButtonSample : MonoBehaviour
    {
        public Text title;

        public void DataSetting(ITabData data)
        {
            TabDataSettingSampleData sampleData = (TabDataSettingSampleData)data;

            title.text = sampleData.name;
        }
    }
}