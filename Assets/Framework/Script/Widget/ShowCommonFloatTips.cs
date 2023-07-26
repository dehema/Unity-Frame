using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowCommonFloatTips : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    string tips;
    bool enable = false;
    Vector3 lastMousePos;

    public void SetTips(string _tips)
    {
        tips = _tips;
    }

    private void Update()
    {
        if (enable)
        {
            if (lastMousePos != Input.mousePosition)
            {
                UIMgr.Ins.OpenView<TipsView>().ShowCommonFloatTips(tips);
                lastMousePos = Input.mousePosition;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        enable = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        enable = false;
        UIMgr.Ins.OpenView<TipsView>().HideCommonFloatTips();
    }
}
