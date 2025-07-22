using System.Collections;
using System.Collections.Generic;
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
    public List<EditorDevTools_Base> subModules;
    /// <summary>
    /// 父模块引用
    /// </summary>
    public EditorDevTools_Base Parent { get; set; }
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool isActive;

    public EditorDevTools_Base(EditorWindow mainWindow, EditorDevTools_Style style,  List<EditorDevTools_Base> subModules = null)
    {
        this.mainWindow = mainWindow;
        this.style = style;
        this.subModules = subModules ?? new List<EditorDevTools_Base>();
        // 设置子模块的父引用
        foreach (var sub in this.subModules)
        {
            sub.Parent = this;
        }
        isActive = false;
    }

    /// <summary>
    /// 绘制分页内容的抽象方法，由子类实现
    /// </summary>
    public abstract void DrawContent();
}
