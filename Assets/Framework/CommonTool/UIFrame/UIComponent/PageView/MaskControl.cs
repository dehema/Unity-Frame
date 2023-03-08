using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MaskControl : MonoBehaviour
{
    public RectTransform mask;
    public PageView mypageview;
    private void Awake()
    {
        mypageview.OnPageChange = pagechange;
    }

    void pagechange(int index)
    {
        if (index >= this.transform.childCount) return;
        Vector3 pos= this.transform.GetChild(index).GetComponent<RectTransform>().position;
        mask.GetComponent<RectTransform>().position = pos;
    }
}
