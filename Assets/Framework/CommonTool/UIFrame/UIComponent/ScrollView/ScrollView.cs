/**
 * 
 * 支持上下滑动的scroll view
 * 
 * **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollView : MonoBehaviour
{
    //预支单体
    public Item itemCell;
    //scrollview
    public ScrollRect scrollRect;

    //content
    public RectTransform content;
    //间隔
    public float spacing = 10;
    //总的宽
    public float totalWidth;
    //总的高
    public float totalHeight;
    //可见的数量
    public int visibleCount;
    //初始数据完成是否检测计算
    public bool isClac = false;
    //开始的索引
    public int startIndex;
    //结尾的索引
    public int lastIndex;
    //item的高
    public float itemHeight = 50;

    //缓存的itemlist
    public List<Item> itemList;
    //可见的itemList
    public List<Item> visibleList;
    //总共的dataList
    public List<int> allList;

    void Start()
    {
        totalHeight = this.GetComponent<RectTransform>().sizeDelta.y;
        totalWidth = this.GetComponent<RectTransform>().sizeDelta.x;
        content = scrollRect.content;
        InitData();

    }
    //初始化
    public void InitData()
    {
        visibleCount = Mathf.CeilToInt(totalHeight / LineHeight) + 1;
        for (int i = 0; i < visibleCount; i++)
        {
            this.AddItem();
        }
        startIndex = 0;
        lastIndex = 0;
        List<int> numberList = new List<int>();
        //数据长度
        int dataLength = 20;
        for (int i = 0; i < dataLength; i++)
        {
            numberList.Add(i);
        }
        SetData(numberList);
    }
    //设置数据
    void SetData(List<int> list)
    {
        allList = list;
        startIndex = 0;
        if (DataCount <= visibleCount)
        {
            lastIndex = DataCount;
        }
        else
        {
            lastIndex = visibleCount - 1;
        }
        //Debug.Log("ooooooooo"+lastIndex);
        for (int i = startIndex; i < lastIndex; i++)
        {
            Item obj = PopItem();
            if (obj == null)
            {
                Debug.Log("获取item为空");
            }
            else
            {
                obj.gameObject.name = i.ToString();

                obj.gameObject.SetActive(true);
                obj.transform.localPosition = new Vector3(0, -i * LineHeight, 0);
                visibleList.Add(obj);
                UpdateItem(i, obj);
            }

        }
        content.sizeDelta = new Vector2(totalWidth, DataCount * LineHeight - spacing);
        isClac = true;
    }
    //更新item
    public void UpdateItem(int index, Item obj)
    {
        int d = allList[index];
        string str = d.ToString();
        obj.name = str;
        //更新数据 todo
    }
    //从itemlist中取出item
    public Item PopItem()
    {
        Item obj = null;
        if (itemList.Count > 0)
        {
            obj = itemList[0];
            obj.gameObject.SetActive(true);
            itemList.RemoveAt(0);
        }
        else
        {
            Debug.Log("从缓存中取出的是空");
        }
        return obj;
    }
    //item进入itemlist
    public void PushItem(Item obj)
    {
        itemList.Add(obj);
        obj.gameObject.SetActive(false);
    }
    public int DataCount
    {
        get
        {
            return allList.Count;
        }
    }
    //每一行的高
    public float LineHeight
    {
        get
        {
            return itemHeight + spacing;
        }
    }
    //添加item到缓存列表中
    public void AddItem()
    {
        GameObject obj = Instantiate(itemCell.gameObject);
        obj.transform.SetParent(content);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.pivot = new Vector2(0.5f, 1);
        obj.SetActive(false);
        obj.transform.localScale = Vector3.one;
        Item o = obj.GetComponent<Item>();
        itemList.Add(o);
    }



    void Update()
    {
        if (isClac)
        {
            Scroll();
        }
    }
    /// <summary>
    /// 计算滑动支持上下滑动
    /// </summary>
    void Scroll()
    {
        float vy = content.anchoredPosition.y;
        float rollUpTop = (startIndex + 1) * LineHeight;
        float rollUnderTop = startIndex * LineHeight;

        if (vy > rollUpTop && lastIndex < DataCount)
        {
            //上边界移除
            if (visibleList.Count > 0)
            {
                Item obj = visibleList[0];
                visibleList.RemoveAt(0);
                PushItem(obj);
            }
            startIndex++;
        }
        float rollUpBottom = (lastIndex - 1) * LineHeight - spacing;
        if (vy < rollUpBottom - totalHeight && startIndex > 0)
        {
            //下边界减少
            lastIndex--;
            if (visibleList.Count > 0)
            {
                Item obj = visibleList[visibleList.Count - 1];
                visibleList.RemoveAt(visibleList.Count - 1);
                PushItem(obj);
            }

        }
        float rollUnderBottom = lastIndex * LineHeight - spacing;
        if (vy > rollUnderBottom - totalHeight && lastIndex < DataCount)
        {
            //Debug.Log("下边界增加"+vy);
            //下边界增加
            Item go = PopItem();
            visibleList.Add(go);
            go.transform.localPosition = new Vector3(0, -lastIndex * LineHeight);
            UpdateItem(lastIndex, go);
            lastIndex++;
        }


        if (vy < rollUnderTop && startIndex > 0)
        {
            //Debug.Log("上边界增加"+vy);
            //上边界增加
            startIndex--;
            Item go = PopItem();
            visibleList.Insert(0, go);
            UpdateItem(startIndex, go);
            go.transform.localPosition = new Vector3(0, -startIndex * LineHeight);
        }

    }
}
