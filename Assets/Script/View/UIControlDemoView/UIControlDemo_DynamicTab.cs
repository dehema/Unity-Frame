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
        //分页标签
        public Tab tabOriginalPrefab;

        UIControlDemo_DynamicTabData tabData = new UIControlDemo_DynamicTabData();

        public void OnClickAddPage()
        {
            UIControlDemo_DynamicTabItem pageItemData = new UIControlDemo_DynamicTabItem();
            pageItemData.MakeRandomItem(30, 50);
            pageItemData.name = $"选项卡{tabData.category.Count + 1}";
            tabData.category.Add(pageItemData);

            SetData(tabData);
            UpdatePage();
        }

        public void OnClickRemovePage()
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

        public void OnClickAddItem()
        {
            int tabIndex = tabController.GetSelectedIndex();
            if (tabIndex < 0 || tabIndex >= tabData.category.Count)
                return;
            UIControlDemo_DynamicTabItem tabItem = tabData.category[tabIndex];
            if (tabItem != null)
            {
                tabItem.AddItem();
            }
            UpdatePage();
        }

        public void OnClickRemoveItem()
        {
            int tabIndex = tabController.GetSelectedIndex();
            if (tabIndex < 0 || tabIndex >= tabData.category.Count)
                return;
            UIControlDemo_DynamicTabItem tabItem = tabData.category[tabIndex];
            if (tabItem != null)
            {
                tabItem.RemoveItem();
            }
            UpdatePage();
        }
    }
}
