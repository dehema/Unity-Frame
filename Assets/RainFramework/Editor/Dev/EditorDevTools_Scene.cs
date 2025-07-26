using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorDevTools_Scene : EditorDevTools_Base
{
    // 每行显示的按钮数量
    private const int button_count_row = 4;

    // 按钮之间的间距
    private const float const_button_spacing = 5f;

    // 场景列表
    private List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

    public EditorDevTools_Scene(EditorWindow mainWindow, List<EditorDevTools_Base> subModules = null)
        : base(mainWindow, subModules)
    {
        this.pageName = "Scene";

        buildScenes.AddRange(EditorBuildSettings.scenes);
    }

    public override void DrawContent()
    {
        // 如果没有场景，显示提示信息
        if (buildScenes.Count == 0)
        {
            EditorGUILayout.HelpBox("没有在BuildSettings中找到场景，请先添加场景到BuildSettings中。", MessageType.Info);
            return;
        }

        // 计算需要多少行来显示所有场景
        int rowCount = Mathf.CeilToInt((float)buildScenes.Count / button_count_row);

        // 绘制场景按钮
        for (int row = 0; row < rowCount; row++)
        {
            EditorGUILayout.BeginHorizontal();

            // 计算当前行应该显示多少个按钮
            int buttonsInThisRow = button_count_row;

            // 计算每个按钮的宽度（根据窗口宽度动态计算）
            float windowWidth = EditorGUIUtility.currentViewWidth;
            float buttonWidth = (windowWidth - (button_count_row + 1) * const_button_spacing - 10) / button_count_row;

            // 在当前行绘制按钮
            for (int col = 0; col < buttonsInThisRow; col++)
            {
                int sceneIndex = row * button_count_row + col;
                DrawSceneButton(sceneIndex, buttonWidth);

                // 添加按钮之间的间距（除了最后一个按钮）
                if (col < buttonsInThisRow - 1)
                {
                    GUILayout.Space(const_button_spacing);
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(const_button_spacing); // 行间距
        }
    }

    /// <summary>
    /// 绘制单个场景按钮
    /// </summary>
    private void DrawSceneButton(int sceneIndex, float buttonWidth)
    {
        if (sceneIndex >= buildScenes.Count) return;

        EditorBuildSettingsScene scene = buildScenes[sceneIndex];

        // 获取场景名称（不包含路径和扩展名）
        string sceneName = Path.GetFileNameWithoutExtension(scene.path);

        // 检查场景是否已启用
        bool isEnabled = scene.enabled;
        GUI.enabled = isEnabled;

        // 设置按钮颜色：已启用的场景使用正常颜色，禁用的场景使用灰色
        Color originalColor = GUI.color;
        if (!isEnabled)
        {
            GUI.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        }

        // 检查当前打开的场景
        bool isCurrentScene = false;
        if (EditorSceneManager.sceneCount > 0)
        {
            for (int i = 0; i < EditorSceneManager.sceneCount; i++)
            {
                var openScene = EditorSceneManager.GetSceneAt(i);
                if (openScene.path == scene.path)
                {
                    isCurrentScene = true;
                    break;
                }
            }
        }

        // 当前打开的场景使用不同的按钮样式
        GUIStyle buttonStyle = new GUIStyle(style.btLarge);
        if (isCurrentScene)
        {
            buttonStyle.fontSize = style.btLarge.fontSize + 1;
            buttonStyle.normal.textColor = Color.green;
            buttonStyle.fontStyle = FontStyle.Bold;
        }

        // 绘制按钮
        if (GUILayout.Button(sceneName, buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(30)))
        {
            // 点击按钮时打开场景
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scene.path);
            }
        }

        // 恢复GUI设置
        GUI.color = originalColor;
        GUI.enabled = true;
    }
}
