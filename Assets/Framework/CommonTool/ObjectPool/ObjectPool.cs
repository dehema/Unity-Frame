/*
 *   管理对象的池子
 * 
 * **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool 
{
    private Queue<GameObject> m_PoolQueue;
    //池子名称
    private string m_PoolName;
    //父物体
    protected Transform m_Parent;
    //缓存对象的预制体
    private GameObject prefab;
    //最大容量
    private int m_MaxCount;
    //默认最大容量
    protected const int m_DefaultMaxCount = 20;
    public GameObject Prefab
    {
        get => prefab;set { prefab = value;  }
    }
    //构造函数初始化
    public ObjectPool()
    {
        m_MaxCount = m_DefaultMaxCount;
        m_PoolQueue = new Queue<GameObject>();
    }
    //初始化
    public virtual void Init(string poolName,Transform transform)
    {
        m_PoolName = poolName;
        m_Parent = transform;
    }
    //取对象
    public virtual GameObject Get()
    {
        GameObject obj;
        if (m_PoolQueue.Count > 0)
        {
            obj = m_PoolQueue.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate<GameObject>(prefab);
            obj.transform.SetParent(m_Parent);
            obj.SetActive(false);
        }
        obj.SetActive(true);
        return obj;
    }
    //回收对象
    public virtual void Recycle(GameObject obj)
    {
        if (m_PoolQueue.Contains(obj)) return;
        if (m_PoolQueue.Count >= m_MaxCount)
        {
            GameObject.Destroy(obj);
        }
        else
        {
            m_PoolQueue.Enqueue(obj);
            obj.SetActive(false);
        }
    }
    /// <summary>
    /// 回收所有激活的对象
    /// </summary>
    public virtual void RecycleAll()
    {
        Transform[] child = m_Parent.GetComponentsInChildren<Transform>();
        foreach (Transform item in child)
        {
            if (item == m_Parent)
            {
                continue;
            }
            
            if (item.gameObject.activeSelf)
            {
                Recycle(item.gameObject);
            }
        }
    }
    //销毁
    public virtual void Destroy()
    {
        m_PoolQueue.Clear();
    }
}
