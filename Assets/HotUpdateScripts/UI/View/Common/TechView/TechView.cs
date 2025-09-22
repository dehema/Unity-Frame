using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 科技
/// </summary>
public partial class TechView : BaseView
{
    ObjPool poolTeckCategory;
    UIControlDemo_DynamicTabData tabData = new UIControlDemo_DynamicTabData();

    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        InitUI();
        RefreshTgTeckCategory();
    }

    void InitUI()
    {
        ui.btClose_Button.SetButton(Close);
        poolTeckCategory = PoolMgr.Ins.CreatePool(ui.tgTeckCategory);
    }

    void RefreshTgTeckCategory()
    {
        poolTeckCategory.CollectAll();
        for (int i = 0; i < ConfigMgr.TechCategory.DataList.Count; i++)
        {
            int index = i;
            //UI
            TechCategoryConfig config = ConfigMgr.TechCategory.DataList[index];
            GameObject item = poolTeckCategory.Get();
            Toggle toggle = item.GetComponent<Toggle>();
            Tab tab = item.GetComponent<Tab>();
            item.transform.Find("text").GetComponent<TextMeshProUGUI>().text = config.Name;
            item.transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"UI/2D/{config.Icon}");
            item.transform.Find("line").gameObject.SetActive(index != ConfigMgr.TechCategory.DataList.Count - 1);

            //data
            UIControlDemo_DynamicTabItem pageItemData = new UIControlDemo_DynamicTabItem();
            pageItemData.MakeRandomItem(5 + 5 * i, 5 + 5 * i);
            tabData.category.Add(pageItemData);
            ui.techContent_TabController.AddTab(tab, ui.tabPage_TabPage);
            toggle.SetToggle((_ison) =>
            {
                if (!_ison)
                    return;
                ui.techContent_TabController.GetTab(index).Select();
            });
        }
        SetData(tabData);
        GameObject firstTag = poolTeckCategory.ShowPool.First();
        firstTag.GetComponent<Toggle>().isOn = true;
        firstTag.GetComponent<Tab>().Select();
    }

    public void SetData(UIControlDemo_DynamicTabData data)
    {
        for (int i = 0; i < data.category.Count; i++)
        {
            TabPage page = ui.tabPage_TabPage;
            Tab tab = ui.techContent_TabController.GetTab(i);
            tab.SetLinkPage(page);
            tab.SetData(data.category[i]);
        }
    }
}
