using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 系统定义类（包含框架中使用到的枚举类型、委托事件、系统常量、接口等）
/// </summary>
//ui窗体类型
public enum UIFormType
{
    //普通窗体
    Normal,
    //固定窗体
    Fixed,
    //弹出窗体
    PopUp,
    //UI特效窗体 置于最顶层
    Top
}
//ui窗体的显示类型
public enum UIFormShowMode
{
    //普通  “普通显示”模式允许多个窗体同时显示
    Normal,
    //反向切换  “反向切换”模式类型，一般都大量引用于“弹出窗体”
    //显示弹出窗体时不完全覆盖底层窗体，一般在屏幕的四周会露出底层窗体
    ReverseChange,
    //隐藏其他  “隐藏其他界面” 模式一般应用于全局性的窗体 
    //为了减少UI渲染压力、提高Unity渲染效率，则设置被覆盖的窗体为“不可见”状态
    HideOther,
    //等待上一个弹窗关闭
    Wait
}
//ui窗体透明度类型
public enum UIFormLucenyType
{
    //完全透明，不能穿透
    Lucency,
    //半透明，不能穿透
    Translucence,
    //低透明度，不能穿透
    ImPenetrable,
    //可以穿透
    Penetrable,
    NoMask
}
//弹窗出现消失动画类型
public enum UIFormShowAnimationType
{
    none,
    //缩放动画
    scale,
    //下拉动画
    moveDown

}
//常量字段
public class SysDefine:MonoBehaviour
{
    /*路径常量*/
    public const string SYS_PATH_CANVAS = "Canvas";
    public const string SYS_PATH_UIFORMS_CONFIG_INFO = "UIFormsConfigInfo";
    /*标签常量*/
    public const string SYS_TAG_CANVAS = "Canvas";
    /* 节点常量*/
    public const string SYS_Pool_NODE = "Pool";
    public const string SYS_NORMAL_NODE = "Normal";
    public const string SYS_FIXED_NODE = "Fixed";
    public const string SYS_POPUP_NODE = "PopUp";
    public const string SYS_TOP_NODE = "Top"; 
    public const string SYS_SCRIPTMANAGER_NODE = "_UIScripts";
    /*遮罩管理器中，透明度常量*/
    /*摄像机层深的常量*/
}
