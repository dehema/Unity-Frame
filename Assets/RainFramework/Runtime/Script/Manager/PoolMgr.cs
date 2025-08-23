using Rain.Core;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池管理器 - 负责创建和管理游戏对象池
/// </summary>
public class PoolMgr : MonoSingleton<PoolMgr>
{
    GameObject poolParent;
    
    // 存储所有创建的对象池，键为预制体路径或名称
    private Dictionary<string, ObjPool> poolDictionary = new Dictionary<string, ObjPool>();
    
    // 存储对象池与句柄的关系，用于自动销毁
    private Dictionary<GameObject, List<string>> handleToPoolsMap = new Dictionary<GameObject, List<string>>();

    /// <summary>
    /// 创建一个对象池
    /// </summary>
    /// <param name="_prototype">对象池的原型对象</param>
    /// <param name="handle">句柄对象，当此对象被销毁时，对象池也会被自动销毁</param>
    /// <returns>创建的对象池实例</returns>
    public ObjPool CreatePool(GameObject _prototype, GameObject handle = null)
    {
        if (_prototype == null)
        {
            Debug.LogError("[PoolMgr] 创建对象池失败：原型对象为空");
            return null;
        }
        //对象可能只是空物体
        //if (_prototype.GetComponent<BasePoolItem>() == null)
        //{
        //    Debug.LogError("[PoolMgr] 源对象必须添加继承[BasePoolItem]的组件");
        //    return null;
        //}

        // 创建非活跃对象的父节点
        Transform inActiveParent = CreatePoolParent(_prototype);
        // 使用原型对象和父节点创建对象池
        ObjPool objPool = new ObjPool(_prototype, inActiveParent);
        _prototype.SetActive(false);
        
        // 将对象池添加到字典中，使用对象名称作为键
        string key = _prototype.name;
        poolDictionary[key] = objPool;
        
        // 如果提供了句柄对象，则建立关联
        if (handle != null)
        {
            RegisterPoolWithHandle(key, handle);
        }
        
        return objPool;
    }

    /// <summary>
    /// 创建对象池的父节点
    /// </summary>
    /// <param name="_prototype">对象池的原型对象</param>
    /// <returns>创建的父节点Transform</returns>
    private Transform CreatePoolParent(GameObject _prototype)
    {
        GameObject parent;
        string poolName = "pool_" + _prototype.name;

        // 根据对象类型创建不同的父节点
        if (_prototype.GetComponentInParent<Canvas>() != null)
        {
            // UI对象，父节点创建在Canvas下
            parent = Tools.Ins.Create2DGo(poolName, _prototype.GetComponentInParent<Canvas>().transform);
        }
        else
        {
            // 3D对象，父节点创建在原对象的父节点下
            parent = Tools.Ins.Create3DGo(poolName, _prototype.transform.parent);
        }

        // 设置父节点为非激活状态，隐藏对象池中的非活跃对象
        parent.SetActive(false);
        return parent.transform;
    }
    
    /// <summary>
    /// 根据预制体路径创建对象池
    /// </summary>
    /// <param name="prefabPath">预制体资源路径</param>
    /// <param name="handle">句柄对象，当此对象被销毁时，对象池也会被自动销毁</param>
    /// <returns>创建的对象池实例</returns>
    public ObjPool CreatePoolFromPath(string prefabPath, GameObject handle = null)
    {
        if (string.IsNullOrEmpty(prefabPath))
        {
            Debug.LogError("[PoolMgr] 创建对象池失败：预制体路径为空");
            return null;
        }
        
        // 检查是否已经创建了该路径的对象池
        if (poolDictionary.ContainsKey(prefabPath))
        {
            Debug.Log($"[PoolMgr] 对象池已存在：{prefabPath}");
            return poolDictionary[prefabPath];
        }
        
        // 加载预制体
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[PoolMgr] 创建对象池失败：无法加载预制体 {prefabPath}");
            return null;
        }
        
        // 实例化预制体作为原型对象
        GameObject prototype = GameObject.Instantiate(prefab);
        prototype.name = System.IO.Path.GetFileNameWithoutExtension(prefabPath);
        
        // 创建对象池
        Transform inActiveParent = CreatePoolParent(prototype);
        ObjPool objPool = new ObjPool(prototype, inActiveParent);
        prototype.SetActive(false);
        
        // 将对象池添加到字典中，使用路径作为键
        poolDictionary[prefabPath] = objPool;
        
        // 如果提供了句柄对象，则建立关联
        if (handle != null)
        {
            RegisterPoolWithHandle(prefabPath, handle);
        }
        
        return objPool;
    }
    
    /// <summary>
    /// 获取对象池，如果不存在则创建
    /// </summary>
    /// <param name="prefabPath">预制体资源路径</param>
    /// <param name="handle">句柄对象</param>
    /// <returns>对象池实例</returns>
    public ObjPool GetOrCreatePool(string prefabPath, GameObject handle = null)
    {
        if (poolDictionary.TryGetValue(prefabPath, out ObjPool existingPool))
        {
            return existingPool;
        }
        
        return CreatePoolFromPath(prefabPath, handle);
    }
    
    /// <summary>
    /// 注册对象池与句柄的关联
    /// </summary>
    /// <param name="poolKey">对象池的键（路径或名称）</param>
    /// <param name="handle">句柄对象</param>
    private void RegisterPoolWithHandle(string poolKey, GameObject handle)
    {
        if (handle == null) return;
        
        // 添加到句柄映射表
        if (!handleToPoolsMap.ContainsKey(handle))
        {
            handleToPoolsMap[handle] = new List<string>();
            
            // 添加销毁监听器组件
            PoolHandleDestroyer destroyer = handle.GetComponent<PoolHandleDestroyer>();
            if (destroyer == null)
            {
                destroyer = handle.AddComponent<PoolHandleDestroyer>();
                destroyer.Initialize(this);
            }
        }
        
        // 添加关联
        if (!handleToPoolsMap[handle].Contains(poolKey))
        {
            handleToPoolsMap[handle].Add(poolKey);
        }
    }
    
    /// <summary>
    /// 当句柄被销毁时调用，清理相关的对象池
    /// </summary>
    /// <param name="handle">被销毁的句柄对象</param>
    public void OnHandleDestroyed(GameObject handle)
    {
        if (handleToPoolsMap.TryGetValue(handle, out List<string> poolKeys))
        {
            foreach (string key in poolKeys)
            {
                if (poolDictionary.TryGetValue(key, out ObjPool pool))
                {
                    // 清理对象池
                    pool.Clear();
                    poolDictionary.Remove(key);
                    Debug.Log($"[PoolMgr] 句柄 {handle.name} 被销毁，自动清理对象池：{key}");
                }
            }
            
            // 移除句柄映射
            handleToPoolsMap.Remove(handle);
        }
    }
    
    /// <summary>
    /// 手动注销对象池
    /// </summary>
    /// <param name="poolKey">对象池的键（路径或名称）</param>
    public void DestroyPool(string poolKey)
    {
        if (poolDictionary.TryGetValue(poolKey, out ObjPool pool))
        {
            // 清理对象池
            pool.Clear();
            poolDictionary.Remove(poolKey);
            
            // 从所有句柄映射中移除
            foreach (var kvp in handleToPoolsMap)
            {
                kvp.Value.Remove(poolKey);
            }
            
            Debug.Log($"[PoolMgr] 手动注销对象池：{poolKey}");
        }
    }
}
