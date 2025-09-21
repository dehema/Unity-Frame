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

    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);
        InitUI();
        RefreshTgTeckCategory();
    }

    void InitUI()
    {
        poolTeckCategory = PoolMgr.Ins.CreatePool(ui.tgTeckCategory);
    }

    void RefreshTgTeckCategory()
    {
        poolTeckCategory.CollectAll();
        for (int i = 0; i < ConfigMgr.TechCategory.DataList.Count; i++)
        {
            TechCategoryConfig config = ConfigMgr.TechCategory.DataList[i];
            GameObject item = poolTeckCategory.Get();
            item.transform.Find("text").GetComponent<TextMeshProUGUI>().text = config.Name;
            item.transform.Find("icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"UI/2D/{config.Icon}");
            item.transform.Find("line").gameObject.SetActive(i != ConfigMgr.TechCategory.DataList.Count - 1);
        }
        poolTeckCategory.ShowPool.First().GetComponent<Toggle>().isOn = true;
    }


}
