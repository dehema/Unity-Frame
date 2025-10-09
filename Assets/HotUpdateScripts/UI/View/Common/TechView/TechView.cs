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
    ObjPool poolTechCategory;
    UIControlDemo_DynamicTableData tableData = new UIControlDemo_DynamicTableData();

    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        InitUI();
    }

    void InitUI()
    {
        ui.btClose_Button.SetButton(Close);
        poolTechCategory = PoolMgr.Ins.CreatePool(ui.tgTechCategory);
        RefreshTgTechCategory();
    }

    void RefreshTgTechCategory()
    {
        poolTechCategory.CollectAll();
        for (int i = 0; i < ConfigMgr.TechCategory.DataList.Count; i++)
        {
            int index = i;
            //UI
            TechCategoryConfig config = ConfigMgr.TechCategory.DataList[index];
            GameObject item = poolTechCategory.Get();
            Toggle toggle = item.GetComponent<Toggle>();
            Tab tab = item.GetComponent<Tab>();

            item.transform.Find("text").GetComponent<TextMeshProUGUI>().text = config.Name;
            item.transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"UI/2D/{config.Icon}");
            item.transform.Find("line").gameObject.SetActive(index != ConfigMgr.TechCategory.DataList.Count - 1);

            //data
            ui.techContent_TabController.AddTab(tab, ui.tabPage_TabPage);
            toggle.SetToggle((_ison) =>
            {
                if (!_ison)
                    return;
                ui.techContent_TabController.GetTab(index).Select();
            });
        }
        SetData();
        GameObject firstTag = poolTechCategory.ShowPool.First();
        firstTag.GetComponent<Toggle>().isOn = true;
        firstTag.GetComponent<Tab>().Select();
    }

    public void SetData()
    {
        int tabCount = ui.techContent_TabController.GetTabCount();
        for (int i = 0; i < tabCount; i++)
        {
            UIControlDemo_DynamicTabData tabData = new UIControlDemo_DynamicTabData();

            TechCategoryConfig techCategoryConfig = ConfigMgr.TechCategory.DataList[i];
            TechCategory techCategory = techCategoryConfig.Category;

            int _level = 1;
            TechRow.TechRowData techRowData = new TechRow.TechRowData();
            techRowData.index = _level;
            foreach (var techData in ConfigMgr.Tech.DataList)
            {
                if (techData.TechCategory != techCategory)
                    continue;
                if (techData.TechLevel != _level || techData.TechLevel == techCategoryConfig.MaxLevel)
                {
                    tabData.AddItem(techRowData);
                    techRowData = new TechRow.TechRowData();
                    _level = techData.TechLevel;
                }
                techRowData.techConfigs.Add(techData);
                techRowData.techCategoryConfig = techCategoryConfig;
            }

            // 添加最后一个techRowData（如果有数据的话）
            if (techRowData.techConfigs.Count > 0)
            {
                tabData.AddItem(techRowData);
            }

            tableData.category.Add(tabData);
            Tab tab = ui.techContent_TabController.GetTab(i);
            tab.SetData(tabData);
        }
    }
}
