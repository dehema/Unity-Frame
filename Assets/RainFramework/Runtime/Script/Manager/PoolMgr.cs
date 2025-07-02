using Rain.Core;
using UnityEngine;

/// <summary>
/// 对象池管理器 - 负责创建和管理游戏对象池
/// </summary>
public class PoolMgr : MonoSingleton<PoolMgr>
{
    /// <summary>
    /// 创建一个对象池
    /// </summary>
    /// <param name="_prototype">对象池的原型对象</param>
    /// <returns>创建的对象池实例</returns>
    public ObjPool CreatePool(GameObject _prototype)
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
}
