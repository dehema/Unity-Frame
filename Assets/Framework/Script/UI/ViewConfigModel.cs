using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewConfigModel
{
    public string comment;
    public string layer = "NormalUI";
    public bool hasBg = true;
    public bool bgClose = false;
    /// <summary>
    /// 允许存在在大地图上的UI，如果允许则：屏幕边缘光标改变样式，打开页面不暂停世界时间流速，屏幕不能移动到Game窗口外面
    /// </summary>
    public bool worldAllow = false;
    /// <summary>
    /// 按ESC键可以关闭的页面
    /// </summary>
    public bool escClose = false;
    public string bgColor;
    public ViewShowMethod showMethod;
}