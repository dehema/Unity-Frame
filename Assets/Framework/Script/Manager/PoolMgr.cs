using System.Collections;
using UnityEngine;

public class PoolMgr : MonoSingleton<PoolMgr>
{
    public ObjPool CreatePool(GameObject _prototype)
    {
        Transform inActiveParent = CreatePoolParent(_prototype);
        ObjPool objPool = new ObjPool(_prototype, inActiveParent);
        return objPool;
    }

    private Transform CreatePoolParent(GameObject _prototype)
    {
        GameObject parent;
        if (_prototype.GetComponentInParent<Canvas>() != null)
        {
            parent = Tools.Ins.Create2DGo("pool_" + _prototype.name, _prototype.GetComponentInParent<Canvas>().transform);
        }
        else
        {
            parent = Tools.Ins.Create3DGo("pool_" + _prototype.name, _prototype.transform.parent);
        }
        parent.SetActive(false);
        return parent.transform;
    }
}