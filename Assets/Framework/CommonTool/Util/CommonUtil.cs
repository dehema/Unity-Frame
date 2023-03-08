using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonUtil
{
    /// <summary>
    /// 是否隐藏现金元素  "apple"不显示现金元素，"pad"显示现金元素
    /// </summary>
    /// <returns></returns>
    public static bool IsApple()
    {
        return NetInfoMgr.Ins.NetServerData.apple_pie == "apple";
    }

    public static bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
            return false;
#endif
    }

    /// <summary>
    /// 是否为竖屏
    /// </summary>
    /// <returns></returns>
    public static bool IsPortrait()
    {
        return Screen.height > Screen.width;
    }
}
