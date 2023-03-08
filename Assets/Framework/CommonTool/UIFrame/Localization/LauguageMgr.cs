/*
 * 
 * 多语言
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauguageMgr 
{
    public static LauguageMgr _Instatnce;
    //语言翻译的缓存集合
    private Dictionary<string, string> _DicLauguageCache;

    private LauguageMgr()
    {
        _DicLauguageCache = new Dictionary<string, string>();
        //初始化语言缓存集合
        InitLauguageCache();
    }

    /// <summary>
    /// 获取实例
    /// </summary>
    /// <returns></returns>
    public static LauguageMgr GetInstance()
    {
        if (_Instatnce == null)
        {
            _Instatnce = new LauguageMgr();
        }
        return _Instatnce;
    }

    /// <summary>
    /// 得到显示文本信息
    /// </summary>
    /// <param name="lauguageId">语言id</param>
    /// <returns></returns>
    public string ShowText(string lauguageId)
    {
        string strQueryResult = string.Empty;
        if (string.IsNullOrEmpty(lauguageId)) return null;
        //查询处理
        if(_DicLauguageCache!=null && _DicLauguageCache.Count >= 1)
        {
            _DicLauguageCache.TryGetValue(lauguageId, out strQueryResult);
            if (!string.IsNullOrEmpty(strQueryResult))
            {
                return strQueryResult;
            }
        }
        Debug.Log(GetType() + "/ShowText()/ Query is Null!  Parameter lauguageID: " + lauguageId);
        return null;
    }

    /// <summary>
    /// 初始化语言缓存集合
    /// </summary>
    private void InitLauguageCache()
    {
        //LauguageJSONConfig_En
        //LauguageJSONConfig
        IConfigManager config = new ConfigManagerByJson("LauguageJSONConfig");
        if (config != null)
        {
            _DicLauguageCache = config.AppSetting;
        }
    }
}
