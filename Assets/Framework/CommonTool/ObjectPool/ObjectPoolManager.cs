/*
 * 
 *  管理多个对象池的管理类
 * 
 * **/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    //管理objectpool的字典
    private Dictionary<string, ObjectPool> m_PoolDic;
    private Transform m_RootTransform=null;
    //构造函数
    public ObjectPoolManager()
    {
        m_PoolDic = new Dictionary<string, ObjectPool>();      
    }
    
    //创建一个新的对象池
    public T CreateObjectPool<T>(string poolName) where T : ObjectPool, new()
    {
        if (m_PoolDic.ContainsKey(poolName))
        {
            return m_PoolDic[poolName] as T;
        }
        if (m_RootTransform == null)
        {
            m_RootTransform = this.transform;
        }      
        GameObject obj = new GameObject(poolName);
        obj.transform.SetParent(m_RootTransform);
        T pool = new T();
        pool.Init(poolName, obj.transform);
        m_PoolDic.Add(poolName, pool);
        return pool;
    }
    //取对象
    public GameObject GetGameObject(string poolName)
    {
        if (m_PoolDic.ContainsKey(poolName))
        {
            return m_PoolDic[poolName].Get();
        }
        return null;
    }
    //回收对象
    public void RecycleGameObject(string poolName,GameObject go)
    {
        if (m_PoolDic.ContainsKey(poolName))
        {
            m_PoolDic[poolName].Recycle(go);
        }
    }
    //销毁所有的对象池
    public void OnDestroy()
    {
        m_PoolDic.Clear();
        GameObject.Destroy(m_RootTransform);
    }
    /// <summary>
    /// 查询是否有该对象池
    /// </summary>
    /// <param name="poolName"></param>
    /// <returns></returns>
    public bool QueryPool(string poolName)
    {
        return m_PoolDic.ContainsKey(poolName) ? true : false;
    }
}
