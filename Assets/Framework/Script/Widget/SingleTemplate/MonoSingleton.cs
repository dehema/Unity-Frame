/**
 * 
 * 继承MonoBehaviour 的单例模版
 * 
 * **/
using UnityEngine;
using System.Collections;
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    #region 单例
    private static T instance;
    public static T Ins { get { return GetInstance(); } }
    public static T GetInstance()
    {
        if (instance == null)
        {
            GameObject obj = new GameObject(typeof(T).Name);
            obj.transform.SetParent(MainMgr.Ins.transform);
            instance = obj.AddComponent<T>();
        }
        return instance;
    }
    #endregion
}

