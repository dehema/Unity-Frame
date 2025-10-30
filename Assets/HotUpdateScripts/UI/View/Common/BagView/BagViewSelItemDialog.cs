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
            RefreshNumUI();
        });
        ui.btReduceSelNum_Button.SetButton(() =>
        {
            useItemNum = Mathf.Clamp(useItemNum - 1, 1, maxUseItemNum);
            RefreshNumUI();
        });
        ui.btMaxSelNum_Button.SetButton(() =>
        {
            useItemNum = maxUseItemNum;
            RefreshNumUI();
        });
    }

    public void SetBagViewBagItem(BagViewBagItem _item)
    {
        if (_item == null)
            return;
        if (currSelectItem != _item)
        {
            currSelectItem = _item;
            useItemNum = 1;
        }
        RefreshData();
        RefreshUI();
        Repos();
    }


    /// <summary>
    /// 重新设置选择物品的弹窗的位置
    /// 根据currSelectItem的位置动态调整对话框显示位置
    /// </summary>
    void Repos()
    {
        if (currSelectItem == null)
        {
            return;
        }

        // 获取当前选中物品的RectTransform
        RectTransform itemRect = currSelectItem.rect;
        if (itemRect == null)
        {
            return;
        }

        // 获取对话框的RectTransform
        RectTransform dialogRect = rect;
        if (dialogRect == null)
        {
            return;
        }

        // 获取Canvas，用于坐标转换
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            return;
        }

        // 将物品的世界坐标转换为屏幕坐标
        Vector3 itemWorldPos = itemRect.position;
        Vector2 itemScreenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, itemWorldPos);

        bool isInUpperHalf = itemScreenPos.y > Screen.height / 2;
        float dialogPosY;
        if (isInUpperHalf)
        {
            dialogPosY = itemScreenPos.y - itemRect.rect.height - rect.rect.height - ui.arrow_Rect.rect.height;
            ui.arrow_Rect.anchorMin = new Vector2(0, 1);
            ui.arrow_Rect.anchorMax = new Vector2(0, 1);
            ui.arrow.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            dialogPosY = itemScreenPos.y + ui.arrow_Rect.rect.height;
            ui.arrow_Rect.anchorMin = new Vector2(0, 0);
            ui.arrow_Rect.anchorMax = new Vector2(0, 0);
            ui.arrow.transform.localScale = new Vector3(1, -1, 1);
        }
        dialogRect.anchoredPosition = new Vector2(0, dialogPosY);
        ui.arrow_Rect.anchoredPosition = new Vector2(itemScreenPos.x + itemRect.rect.width / 2, 0);
    }

    void RefreshData()
    {
        maxUseItemNum = PlayerMgr.Ins.GetItemNum(currSelectItem.ItemData.itemConfig.ItemID);
    }

    void RefreshUI()
    {
        RefreshUseItem();
        RefreshNumUI();
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

    void RefreshNumUI()
    {
        ui.sliderSelItemNum_Slider.value = (float)useItemNum / maxUseItemNum;
        ui.inputSelItemNum_Input.text = useItemNum.ToString();
    }

    void OnChangeSelItemNumSlider(float _sliderVal)
    {
        useItemNum = Mathf.Clamp(Mathf.CeilToInt(_sliderVal * maxUseItemNum), 1, maxUseItemNum);
        RefreshNumUI();
    }

    void OnEndEditSelItemNumInput(string _inputVal)
    {
        if (int.TryParse(_inputVal, out int val))
        {
            useItemNum = Mathf.Clamp(val, 1, maxUseItemNum);
            RefreshNumUI();
        }
        else
        {
            RefreshNumUI();
        }
    }
}
