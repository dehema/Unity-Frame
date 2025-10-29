using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;

/// <summary>
/// 背包界面选择物品弹窗
/// </summary>
public partial class BagViewSelItemDialog : BaseUI
{
    BagViewBagItem currSelectItem;
    int useItemNum = 1;
    int maxUseItemNum = 1;

    private void Start()
    {
        ui.sliderSelItemNum_Slider.onValueChanged.AddListener(OnChangeSelItemNumSlider);
        ui.inputSelItemNum_Input.onEndEdit.AddListener(OnEndEditSelItemNumInput);
        ui.btAddSelNum_Button.SetButton(() =>
        {
            useItemNum = Mathf.Clamp(useItemNum + 1, 1, maxUseItemNum);
            RefreshNum();
        });
        ui.btReduceSelNum_Button.SetButton(() =>
        {
            useItemNum = Mathf.Clamp(useItemNum - 1, 1, maxUseItemNum);
            RefreshNum();
        });
        ui.btMaxSelNum_Button.SetButton(() =>
        {
            useItemNum = maxUseItemNum;
            RefreshNum();
        });
    }

    public void SetBagViewBagItem(BagViewBagItem _item)
    {
        if (currSelectItem != _item)
        {
            currSelectItem = _item;
            useItemNum = 1;
        }
        RefreshData();
        RefreshUseItem();
    }

    void RefreshData()
    {
        maxUseItemNum = PlayerMgr.Ins.GetItemNum(currSelectItem.ItemData.itemConfig.ItemID);
    }

    void RefreshUI()
    {
        RefreshUseItem();
        RefreshNum();
    }

    void RefreshUseItem()
    {
        if (currSelectItem == null)
        {
            return;
        }
        ui.selItemName_Text.text = currSelectItem.ItemData.itemConfig.ItemName;
        ui.selItemDesc_Text.text = currSelectItem.ItemData.itemConfig.ItemDescription;
    }

    void RefreshNum()
    {
        ui.sliderSelItemNum_Slider.value = (float)useItemNum / maxUseItemNum;
        ui.inputSelItemNum_Input.text = useItemNum.ToString();
    }

    void OnChangeSelItemNumSlider(float _sliderVal)
    {
        useItemNum = Mathf.Clamp(Mathf.CeilToInt(_sliderVal * maxUseItemNum), 1, maxUseItemNum);
    }

    void OnEndEditSelItemNumInput(string _inputVal)
    {
        if (int.TryParse(_inputVal, out int val))
        {
            useItemNum = Mathf.Clamp(val, 1, maxUseItemNum);
            RefreshNum();
        }
        else
        {
            RefreshNum();
        }
    }
}
