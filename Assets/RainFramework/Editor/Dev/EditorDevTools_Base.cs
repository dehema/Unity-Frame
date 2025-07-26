using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

// 分页模块基类
public abstract class EditorDevTools_Base
{
    /// <summary>
    /// 分页名称
    /// </summary>
    public string pageName;
    /// <summary>
    /// 主窗口
    /// </summary>
    protected EditorWindow mainWindow;
    /// <summary>
    /// 样式
    /// </summary>
    protected EditorDevTools_Style style;
    /// <summary>
    /// 子分页
    /// </summary>
    public List<EditorDevTools_Base> subPages;
    /// <summary>
    /// 父模块引用
    /// </summary>
    public EditorDevTools_Base parent { get; set; }
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool isActive;
    /// <summary>
    /// 层级
    /// </summary>
    public int level;
    /// <summary>
    /// 索引
    /// </summary>
    public int index;

    public EditorDevTools_Base(EditorWindow mainWindow, List<EditorDevTools_Base> subModules = null)
    {
        this.mainWindow = mainWindow;
        this.subPages = subModules ?? new List<EditorDevTools_Base>();
        for (int i = 0; i < subPages.Count; i++)
        {
            EditorDevTools_Base subPage = subPages[i];
            subPage.index = i;
            subPage.parent = this;
        }
        isActive = false;
    }

    /// <summary>
    /// 绘制分页内容的抽象方法，由子类实现
    /// </summary>
    public abstract void DrawContent();

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="_level"></param>
    public void SetLevel(int _level)
    {
        level = _level;
        foreach (var _subPage in subPages)
        {
            _subPage.SetLevel(level + 1);
        }
    }

    public void SetStyle(EditorDevTools_Style _style)
    {
        style = _style;
    }
}
