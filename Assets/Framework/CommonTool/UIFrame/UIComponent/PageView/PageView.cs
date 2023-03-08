/**
 * 
 * 左右滑动的页面视图
 * 
 * ***/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PageView : MonoBehaviour,IBeginDragHandler,IEndDragHandler
{
    //scrollview
    public ScrollRect rect;
    //求出每页的临界角，页索引从0开始
    List<float> posList = new List<float>();
    //是否拖拽结束
    public bool isDrag = false;
    bool stopMove = true;
    //滑动的起始坐标  
    float targetHorizontal = 0;
    float startDragHorizontal;
    float startTime = 0f;
    //滑动速度  
    public float smooting = 1f;
    public float sensitivity = 0.3f;
    //页面改变
    public Action<int> OnPageChange;
    //当前页面下标
    int currentPageIndex = -1;
    void Start()
    {
        rect = this.GetComponent<ScrollRect>();
        float horizontalLength = rect.content.rect.width - this.GetComponent<RectTransform>().rect.width;
        posList.Add(0);
        for(int i = 1; i < rect.content.childCount - 1; i++)
        {
            posList.Add(GetComponent<RectTransform>().rect.width * i / horizontalLength);
        }
        posList.Add(1);
    }

    
    void Update()
    {
        if(!isDrag && !stopMove)
        {
            startTime += Time.deltaTime;
            float t = startTime * smooting;
            rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targetHorizontal, t);
            if (t >= 1)
            {
                stopMove = true;
            }
        }
        
    }
    /// <summary>
    /// 设置页面的index下标
    /// </summary>
    /// <param name="index"></param>
    void SetPageIndex(int index)
    {
        if (currentPageIndex != index)
        {
            currentPageIndex = index;
            if (OnPageChange != null)
            {
                OnPageChange(index);
            }
        }
    }
    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        startDragHorizontal = rect.horizontalNormalizedPosition;
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        float posX = rect.horizontalNormalizedPosition;
        posX += ((posX - startDragHorizontal) * sensitivity);
        posX = posX < 1 ? posX : 1;
        posX = posX > 0 ? posX : 0;
        int index = 0;
        float offset = Mathf.Abs(posList[index] - posX);
        for(int i = 0; i < posList.Count; i++)
        {
            float temp = Mathf.Abs(posList[i] - posX);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }
        SetPageIndex(index);
        targetHorizontal = posList[index];
        isDrag = false;
        startTime = 0f;
        stopMove = false;
    }
}
