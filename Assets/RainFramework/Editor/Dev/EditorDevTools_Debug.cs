using System.Collections;
using System.Collections.Generic;
using Rain.UI;
using UnityEditor;
using UnityEngine;


public class EditorDevTools_Debug : EditorDevTools_Base
{
    // 道具设置相关字段
    private const int titleWidth = 120;
    private string itemId = "10001";
    private int itemCount = 100;

    public EditorDevTools_Debug(EditorWindow mainWindow, List<EditorDevTools_Base> subModules = null) : base(mainWindow, subModules)
    {
        this.pageName = "调试";
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
        else
        {
        }
        DrawGUI();
    }

    void DrawGUI()
    {
        GUILayout.BeginVertical();

        // 道具设置区域
        GUILayout.Label("道具设置", EditorStyles.boldLabel);

        //设置某种道具数量
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("设置某种道具数量", GUILayout.Width(titleWidth));

            GUILayout.Label("道具ID:", GUILayout.Width(60));
            itemId = EditorGUILayout.TextField(itemId, GUILayout.Width(100));

            GUILayout.Label("道具数量:", GUILayout.Width(60));
            itemCount = EditorGUILayout.IntField(itemCount, GUILayout.Width(80));

            if (GUILayout.Button("设置", GUILayout.Width(60)))
            {
                SetItemCount();
            }
        }
        GUILayout.EndHorizontal();


        //设置所有道具数量
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("设置所有道具数量", GUILayout.Width(titleWidth));

            GUILayout.Label("道具数量:", GUILayout.Width(60));
            itemCount = EditorGUILayout.IntField(itemCount, GUILayout.Width(80));

            if (GUILayout.Button("设置", GUILayout.Width(60)))
            {
                SetAllItemCount();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    /// <summary>
    /// 设置道具数量
    /// </summary>
    private void SetItemCount()
    {
        PlayerMgr.Ins.SetItemNum(itemId, itemCount);
    }

    /// <summary>
    /// 设置道具数量
    /// </summary>
    private void SetAllItemCount()
    {
        foreach (var item in ConfigMgr.Item.DataMap)
        {
            PlayerMgr.Ins.SetItemNum(item.Key, itemCount);
        }
    }
}
