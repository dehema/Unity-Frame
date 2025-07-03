using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池类 - 管理游戏对象的复用和生命周期
/// </summary>
public class ObjPool
{
    /// <summary>
    /// 已激活对象列表 - 当前正在使用的对象
    /// </summary>
    public List<GameObject> activePool = new List<GameObject>();

    /// <summary>
    /// 非激活对象列表 - 当前等待复用的对象
    /// </summary>
    public List<GameObject> inActivePool = new List<GameObject>();

    /// <summary>
    /// 原型对象 - 用于创建新实例的模板
    /// </summary>
    public GameObject prototype;

    /// <summary>
    /// 原型对象的父节点 - 激活对象将被放置在这里
    /// </summary>
    public Transform prototypeParent;

    /// <summary>
    /// 非激活对象的父节点 - 非激活对象将被放置在这里
    /// </summary>
    public Transform inActiveParent;

    /// <summary>
    /// 脚本列表 - 跟踪从池中获取的组件
    /// </summary>
    public List<BasePoolItem> scriptList = new List<BasePoolItem>();

    /// <summary>
    /// 创建一个新的对象池
    /// </summary>
    /// <param name="_prototype">原型对象</param>
    /// <param name="_inActiveParent">非激活对象的父节点</param>
    public ObjPool(GameObject _prototype, Transform _inActiveParent)
    {
        if (_prototype == null || _inActiveParent == null)
        {
            Debug.LogError("[ObjPool] 创建对象池失败：参数为空");
            return;
        }

        // 初始化属性
        prototype = _prototype;
        prototypeParent = _prototype.transform.parent;
        inActiveParent = _inActiveParent;

        // 保存原始缩放比例，确保在父节点变化后保持正确的缩放
        Vector3 scale = _prototype.transform.localScale;

        // 将原型对象移动到非激活父节点下并设置为非激活状态
        _prototype.transform.SetParent(_inActiveParent);
        _prototype.transform.localScale = scale;
        _prototype.SetActive(false);
    }

    /// <summary>
    /// 从对象池中获取一个对象
    /// </summary>
    /// <param name="_params">初始化参数，传递给对象的OnCreate方法</param>
    /// <returns>激活的游戏对象</returns>
    public GameObject Get(params object[] _params)
    {
        GameObject item;
        // 是否创建  不是则为显示
        bool isFirstCreate = false;
        // 如果非激活池中有对象，则复用它
        if (inActivePool.Count > 0)
        {
            // 从非激活池中取出最后一个对象
            item = inActivePool[inActivePool.Count - 1];
            inActivePool.Remove(item);

            // 将对象移动到原型父节点下
            item.transform.SetParent(prototypeParent);
        }
        else
        {
            // 如果没有可复用的对象，则创建新实例
            item = GameObject.Instantiate(prototype, prototypeParent);
            isFirstCreate = true;
        }

        // 将对象添加到激活池中
        activePool.Add(item);

        // 设置对象属性
        item.transform.name = prototype.name;
        item.transform.localScale = prototype.transform.localScale;
        item.SetActive(true);

        // 如果对象包含BasePoolItem组件，调用其OnCreate方法
        BasePoolItem poolItemBase = item.GetComponent<BasePoolItem>();
        if (poolItemBase != null)
        {
            if (isFirstCreate)
                poolItemBase.OnCreate(_params);
            poolItemBase.OnOpen(_params);
            return item;
        }

        // 如果对象包含BasePoolItem3D组件，调用其OnCreate方法
        BasePoolItem3D poolItemBase3D = item.GetComponent<BasePoolItem3D>();
        if (poolItemBase3D != null)
        {
            if (isFirstCreate)
                poolItemBase3D.OnCreate(_params);
            poolItemBase.OnOpen(_params);
        }

        return item;
    }

    /// <summary>
    /// 从对象池中获取一个对象并返回指定类型的组件
    /// </summary>
    /// <typeparam name="T">要获取的组件类型</typeparam>
    /// <param name="_params">初始化参数，传递给对象的OnCreate方法</param>
    /// <returns>对象上的指定类型组件</returns>
    public T Get<T>(params object[] _params) where T : BasePoolItem
    {
        // 先从池中获取游戏对象
        GameObject item = Get(_params);

        // 获取对象上的指定类型组件
        T t = item.GetComponent<T>();

        // 将组件添加到脚本列表中进行跟踪
        if (t != null)
        {
            scriptList.Add(t);
        }
        else
        {
            Debug.LogWarning($"[ObjPool] 对象 {item.name} 上没有找到 {typeof(T).Name} 组件");
        }

        return t;
    }

    /// <summary>
    /// 根据索引获取激活池中的对象
    /// </summary>
    /// <param name="_index">对象索引</param>
    /// <returns>指定索引处的游戏对象，如果索引无效则返回null</returns>
    public GameObject GetItemByIndex(int _index)
    {
        // 检查索引是否有效
        if (_index >= 0 && _index < activePool.Count)
        {
            return activePool[_index];
        }

        Debug.LogWarning($"[ObjPool] 索引 {_index} 超出范围，当前激活对象数量：{activePool.Count}");
        return null;
    }

    /// <summary>
    /// 根据索引获取激活池中对象的指定类型组件
    /// </summary>
    /// <typeparam name="T">要获取的组件类型</typeparam>
    /// <param name="_index">对象索引</param>
    /// <returns>指定索引处对象上的组件，如果无效则返回默认值</returns>
    public T GetItemByIndex<T>(int _index)
    {
        // 获取指定索引的游戏对象
        GameObject item = GetItemByIndex(_index);

        // 如果对象存在，则获取其组件
        if (item != null)
        {
            T component = item.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning($"[ObjPool] 对象 {item.name} 上没有找到 {typeof(T).Name} 组件");
            }
            return component;
        }

        return default(T);
    }

    /// <summary>
    /// 回收单个游戏对象到对象池
    /// </summary>
    /// <param name="go">要回收的游戏对象</param>
    public void CollectOne(GameObject go)
    {
        // 检查对象是否有效且在激活池中
        if (go == null || !activePool.Contains(go))
        {
            Debug.LogWarning($"[ObjPool] 无法回收对象：对象为空或不在激活池中");
            return;
        }

        // 从脚本列表中移除相关组件
        for (int i = 0; i < scriptList.Count; i++)
        {
            if (scriptList[i] != null && scriptList[i].gameObject == go)
            {
                scriptList.Remove(scriptList[i]);
                break;
            }
        }

        // 从激活池移动到非激活池
        activePool.Remove(go);
        inActivePool.Add(go);

        // 将对象移动到非激活父节点下
        go.transform.SetParent(inActiveParent);

        // 调用对象上 BasePoolItem 组件的 OnCollect 方法
        BasePoolItem poolItemBase = go.GetComponent<BasePoolItem>();
        if (poolItemBase != null)
        {
            poolItemBase.OnCollect();
        }

        // 调用对象上 BasePoolItem3D 组件的 OnCollect 方法
        BasePoolItem3D poolItemBase3D = go.GetComponent<BasePoolItem3D>();
        if (poolItemBase3D != null)
        {
            poolItemBase3D.OnCollect();
        }

        // 设置对象为非激活状态
        go.SetActive(false);
    }

    /// <summary>
    /// 回收所有激活对象到对象池
    /// </summary>
    public void CollectAll()
    {
        // 从后向前遍历，避免在循环中修改集合导致的问题
        for (int i = activePool.Count - 1; i >= 0; i--)
        {
            if (i < activePool.Count) // 再次检查索引有效性，防止在循环中集合大小变化
            {
                CollectOne(activePool[i]);
            }
        }
    }

    /// <summary>
    /// 清空对象池，销毁所有对象
    /// </summary>
    public void Clear()
    {
        // 先回收所有激活对象
        CollectAll();

        // 销毁所有非激活对象
        foreach (GameObject obj in inActivePool)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }

        // 清空列表
        inActivePool.Clear();
        activePool.Clear();
        scriptList.Clear();
    }

    /// <summary>
    /// 获取当前激活对象的数量
    /// </summary>
    public int ItemNum
    {
        get { return activePool.Count; }
    }

    /// <summary>
    /// 获取当前非激活对象的数量
    /// </summary>
    public int InactiveItemNum
    {
        get { return inActivePool.Count; }
    }

    /// <summary>
    /// 获取对象池中对象总数（激活 + 非激活）
    /// </summary>
    public int TotalItemNum
    {
        get { return activePool.Count + inActivePool.Count; }
    }
}
