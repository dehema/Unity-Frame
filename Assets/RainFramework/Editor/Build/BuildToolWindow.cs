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
    BuildToolAtlas buildToolAtlas;
    BuildToolAB buildToolAB;
    private Vector2 scrollPosition;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;


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

        buildToolAtlas = new BuildToolAtlas(config);
        buildToolAB = new BuildToolAB(config);

        // 初始化样式
        InitializeStyles();
    }

    private void OnDestroy()
    {
        config.Save();
    }

    private void InitializeStyles()
    {
        titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
        };
    }

    private void OnGUI()
    {
        // 标题
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("RainFramework 打包工具", titleStyle);
        EditorGUILayout.Space(20);

        // 滚动区域 - 只包含配置相关的UI
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 导出目录设置
        DrawExportPathSection();

        EditorGUILayout.Space(20);

        // 资源准备
        buildToolAtlas?.DrawResourcePreparationSection();

        EditorGUILayout.Space(20);

        // AB包设置
        buildToolAB?.DrawABSettingsSection();

        EditorGUILayout.EndScrollView();

        // 分隔线
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space(10);

        // 固定区域 - 打包和工具相关的UI
        EditorGUILayout.LabelField("【打包操作】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        // 添加一些背景色区分
        GUI.backgroundColor = new Color(0.9f, 0.9f, 1f, 1f);

        // 打包选项
        DrawBuildOptionsSection();

        EditorGUILayout.Space(20);

        // 一键打包按钮
        DrawBuildButton();

        EditorGUILayout.Space(20);

        // 其他工具按钮
        DrawUtilityButtons();

        EditorGUILayout.EndVertical();

        // 重置背景色
        GUI.backgroundColor = Color.white;
    }

    private void DrawExportPathSection()
    {
        EditorGUILayout.LabelField("【导出设置】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("安装包导出目录:", GUILayout.Width(120));
        config.exportPath = EditorGUILayout.TextField(config.exportPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("选择导出目录", config.exportPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                config.exportPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标平台:", GUILayout.Width(80));
        config.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(config.buildTarget);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
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

            // 构建AssetBundle
            if (config.buildAssetBundles)
            {
                EditorUtility.DisplayProgressBar("打包中", "构建AssetBundle...", 0.2f);
                BuildAssetBundles();
            }

            // 自动打包AB包（如果启用）
            if (config.autoBuildAB)
            {
                EditorUtility.DisplayProgressBar("打包中", "自动打包AB包...", 0.25f);
                // TODO: 调用AB包打包功能
            }

            // 复制到StreamingAssets
            if (config.copyToStreamingAssets)
            {
                EditorUtility.DisplayProgressBar("打包中", "复制到StreamingAssets...", 0.3f);
                CopyToStreamingAssets();
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

    private void BuildAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, config.buildTarget);
    }

    private void CopyToStreamingAssets()
    {
        // 这里可以添加复制逻辑
        Debug.Log("复制到StreamingAssets功能待实现");
    }

    private void OpenExportDirectory()
    {
        if (Directory.Exists(config.exportPath))
        {
            EditorUtility.RevealInFinder(config.exportPath);
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "导出目录不存在", "确定");
        }
    }

    private void ClearExportDirectory()
    {
        if (Directory.Exists(config.exportPath))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清理导出目录吗？", "确定", "取消"))
            {
                Directory.Delete(config.exportPath, true);
                Directory.CreateDirectory(config.exportPath);
                Debug.Log("导出目录已清理");
            }
        }
    }


    private void DrawBuildOptionsSection()
    {
        EditorGUILayout.BeginVertical("box");

        // 目标平台
        config.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("目标平台:", config.buildTarget);

        // 开发版本
        config.developmentBuild = EditorGUILayout.Toggle("开发版本:", config.developmentBuild);

        EditorGUILayout.EndVertical();
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

    private void DrawUtilityButtons()
    {
        EditorGUILayout.LabelField("【工具】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("打开导出目录"))
        {
            OpenExportDirectory();
        }

        if (GUILayout.Button("清理导出目录"))
        {
            ClearExportDirectory();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("构建AssetBundle"))
        {
            BuildAssetBundles();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

}
