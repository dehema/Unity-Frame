using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPool
{
    /// <summary>
    /// 激活池
    /// </summary>
    public List<GameObject> activePool = new List<GameObject>();
    /// <summary>
    /// 非激活池
    /// </summary>
    public List<GameObject> inActivePool = new List<GameObject>();
    public GameObject prototype;
    public Transform prototypeParent;
    public Transform inActiveParent;
    public List<MonoBehaviour> scriptList = new List<MonoBehaviour>();

    public ObjPool(GameObject _prototype, Transform _inActiveParent)
    {
        prototype = _prototype;
        prototypeParent = _prototype.transform.parent;
        inActiveParent = _inActiveParent;
        Vector3 scale = _prototype.transform.localScale;
        _prototype.transform.SetParent(_inActiveParent);
        _prototype.transform.localScale = scale;
        _prototype.SetActive(false);
    }

    public GameObject Get(params object[] _params)
    {
        GameObject item;
        if (inActivePool.Count > 0)
        {
            item = inActivePool[inActivePool.Count - 1];
            inActivePool.Remove(item);
            item.transform.SetParent(prototypeParent);
        }
        else
        {
            item = GameObject.Instantiate<GameObject>(prototype, prototypeParent);
        }
        activePool.Add(item);
        item.transform.name = prototype.name;
        item.transform.localScale = prototype.transform.localScale;
        item.SetActive(true);
        PoolItemBase poolItemBase = item.GetComponent<PoolItemBase>();
        if (poolItemBase != null)
        {
            poolItemBase.OnCreate(_params);
        }
        PoolItemBase3D poolItemBase3D = item.GetComponent<PoolItemBase3D>();
        if (poolItemBase3D != null)
        {
            poolItemBase3D.OnCreate(_params);
        }
        return item;
    }

    public T Get<T>(params object[] _params) where T : MonoBehaviour
    {
        GameObject item = Get(_params);
        T t = item.GetComponent<T>();
        scriptList.Add(t);
        return t;
    }

    public GameObject GetItemByIndex(int _index)
    {
        if (_index < activePool.Count)
        {
            return activePool[_index];
        }
        return null;
    }
    public T GetItemByIndex<T>(int _index)
    {
        GameObject item = GetItemByIndex(_index);
        if (item != null)
        {
            return item.GetComponent<T>();
        }
        return default(T);
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="go"></param>
    public void CollectOne(GameObject go)
    {
        if (!activePool.Contains(go))
        {
            return;
        }
        for (int i = 0; i < scriptList.Count; i++)
        {
            if (scriptList[i].gameObject == go)
            {
                scriptList.Remove(scriptList[i]);
                break;
            }
        }
        activePool.Remove(go);
        inActivePool.Add(go);
        go.transform.SetParent(inActiveParent);

        PoolItemBase poolItemBase = go.GetComponent<PoolItemBase>();
        if (poolItemBase != null)
        {
            poolItemBase.OnCollect();
        }
        PoolItemBase3D poolItemBase3D = go.GetComponent<PoolItemBase3D>();
        if (poolItemBase3D != null)
        {
            poolItemBase3D.OnCollect();
        }
    }

    /// <summary>
    /// 回收所有对象
    /// </summary>
    public void CollectAll()
    {
        for (int i = activePool.Count - 1; i >= 0; i--)
        {
            CollectOne(activePool[i]);
        }
    }

    /// <summary>
    /// 获取item对象的数量
    /// </summary>
    /// <returns></returns>
    public int ItemNum
    {
        get { return activePool.Count; }
    }
}
