using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorDevTools_UI : EditorDevTools_Base
{
    public EditorDevTools_UI(EditorWindow mainWindow, EditorDevTools_Style style, List<EditorDevTools_Base> subModules = null)
        : base(mainWindow, style, subModules)
    {
        this.pageName = "UI";
    }

    public override void DrawContent()
    {
        if (!EditorApplication.isPlaying)
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label("\n 请先运行游戏");
            }
            GUILayout.EndVertical();
            return;
        }
    }
}
