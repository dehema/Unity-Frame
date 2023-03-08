using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHOShopUtil
{
    public static string PanelName(string name)
    {
        return CommonUtil.IsPortrait() ? "Portrait" + name : "Landscape" + name;
    }
}
