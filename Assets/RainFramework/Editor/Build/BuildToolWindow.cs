using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 打包工具窗口
/// </summary>
public class BuildToolWindow : EditorWindow
{
    BuildToolConfig config = new BuildToolConfig();
    BuildToolSetting buildToolSetting;
    BuildToolAtlas buildToolAtlas;
    BuildToolAB buildToolAB;
    private Vector2 scrollPosition;
    public static GUIStyle titleStyle;
    public static GUIStyle buttonStyle;


    [MenuItem("开发工具/打包工具 _F5")] // F5快捷键
    public static void ShowWindow()
    {
        BuildToolWindow window = GetWindow<BuildToolWindow>("打包工具", typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        window.minSize = new Vector2(400, 600);
        window.Show();
    }

    private void OnEnable()
    {
        // 加载配置
        config = BuildToolConfig.Load();

        buildToolSetting = new BuildToolSetting(config);
        buildToolAtlas = new BuildToolAtlas(config);
        buildToolAB = new BuildToolAB(config);
    }

    private void OnDestroy()
    {
        config.Save();
    }

    private void OnGUI()
    {
        // 初始化样式
        InitializeStyles();
        // 标题
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("RainFramework 打包工具", titleStyle);

        // 滚动区域 - 只包含配置相关的UI
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        //打包设置
        buildToolSetting?.DrawGUI();

        // 资源准备
        buildToolAtlas?.DrawGUI();

        // AB包设置
        buildToolAB?.DrawGUI();

        EditorGUILayout.EndScrollView();

        // 分隔线
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // 一键打包按钮
        DrawBuildButton();
        EditorGUILayout.Space(10);

        // 重置背景色
        GUI.backgroundColor = Color.white;
    }

    private void InitializeStyles()
    {
        if (titleStyle == null)
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };

        if (buttonStyle == null)
            buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 14, fontStyle = FontStyle.Bold, };
    }

    private void DrawBuildButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("打包", buttonStyle, GUILayout.Height(50), GUILayout.Width(200)))
        {
            StartBuild();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void StartBuild()
    {
        try
        {
            EditorUtility.DisplayProgressBar("打包中", "正在准备打包...", 0f);

            // 创建导出目录
            if (!Directory.Exists(config.exportPath))
            {
                Directory.CreateDirectory(config.exportPath);
            }

            // 自动打包AB包（如果启用）
            if (config.autoBuildAB)
            {
                EditorUtility.DisplayProgressBar("打包中", "自动打包AB包...", 0.25f);
                // TODO: 调用AB包打包功能
            }

            // 开始打包
            EditorUtility.DisplayProgressBar("打包中", "正在打包...", 0.4f);
            BuildPlayer();

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包完成", $"打包完成！\n导出目录: {config.exportPath}", "确定");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包失败", $"打包失败: {e.Message}", "确定");
        }
    }

    private void BuildPlayer()
    {
        string[] scenes = GetBuildScenes();
        string buildPath = GetBuildPath();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = config.buildTarget,
            options = config.developmentBuild ? BuildOptions.Development : BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    private string[] GetBuildScenes()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

    private string GetBuildPath()
    {
        string fileName = GetBuildFileName();
        return Path.Combine(config.exportPath, fileName);
    }

    private string GetBuildFileName()
    {
        string platform = config.buildTarget.ToString();
        string extension = GetBuildExtension();
        return $"{Application.productName}_{platform}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
    }

    private string GetBuildExtension()
    {
        switch (config.buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            case BuildTarget.StandaloneOSX:
                return ".app";
            case BuildTarget.StandaloneLinux64:
                return "";
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.iOS:
                return "";
            default:
                return "";
        }
    }
}
