using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.UIControlDemo
{
    public class UIControlDemo_DynamicTab : MonoBehaviour
    {
        public TabController tabController;
        public TabPage horizontalScrollPage;
        public Tab tabOriginalPrefab;

        UIControlDemo_DynamicTabData tabData = new UIControlDemo_DynamicTabData();

        public void OnClickCreateCategory()
        {
            UIControlDemo_DynamicTabItem pageItemData = new UIControlDemo_DynamicTabItem();
            pageItemData.name = "ѡ�";
            tabData.category.Add(pageItemData);

            SetData(tabData);
            UpdatePage();
        }

        public void OnClickRemoveCategory()
        {
            int index = tabController.GetSelectedIndex();
            if (index >= 0 && index < tabData.category.Count)
            {
                tabData.category.RemoveAt(index);
            }

            SetData(tabData);
            UpdatePage();
        }

        public void SetData(UIControlDemo_DynamicTabData data)
        {
            for (int i = data.category.Count; i < tabController.GetTabCount(); i++)
            {
                Tab tab = tabController.GetTab(i);
                tab.SetActive(false);
                tab.NotifyPage();
            }

            for (int i = 0; i < data.category.Count; i++)
            {
                Tab tab = null;
                TabPage page = null;
                page = horizontalScrollPage;
                //page = verticalScrollPage;

                if (i < tabController.GetTabCount())
                {
                    tab = tabController.GetTab(i);
                    tab.SetLinkPage(page);
                }
                else
                {
                    tab = Instantiate<Tab>(tabOriginalPrefab, tabOriginalPrefab.transform.parent);
                    tabController.AddTab(tab, page);
                }

                tab.SetData(data.category[i]);
                tab.SetActive(true);
            }
        }

        public void UpdatePage()
        {
            Tab selectedTab = tabController.GetSelectedTab();
            if (selectedTab != null)
            {
                selectedTab.NotifyPage();

                if (selectedTab.GetActive() == false)
                {
                    tabController.SelectFirstTab();
                }
            }
            else
            {
                tabController.SelectFirstTab();
            }
        }
    }
}
