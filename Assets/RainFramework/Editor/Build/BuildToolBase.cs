using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class BuildToolBase
{
    private bool isWindowExpanded = true;     //窗口是否展开
    public BuildToolConfig config;

    protected string pageName = "name";


    public BuildToolBase(BuildToolConfig _config)
    {
        config = _config;
    }

    protected abstract void DrawContent();

    public void DrawGUI()
    {
        // 绘制折叠面板
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        isWindowExpanded = EditorGUILayout.Foldout(isWindowExpanded, $"【{pageName}】", true, EditorStyles.foldoutHeader);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(5);
        if (isWindowExpanded)
            DrawContent();
        // 分隔线
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}
