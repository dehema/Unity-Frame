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
    private string exportPath = "";
    private BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
    private bool developmentBuild = false;
    private bool copyToStreamingAssets = false;
    private bool buildAssetBundles = false;

    private Vector2 scrollPosition;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;

    [MenuItem("开发工具/打包工具 _F5")] // F5快捷键
    public static void ShowWindow()
    {
        BuildToolWindow window = GetWindow<BuildToolWindow>("打包工具");
        window.minSize = new Vector2(400, 600);
        window.Show();
    }

    private void OnEnable()
    {
        // 初始化导出路径 - 默认桌面
        if (string.IsNullOrEmpty(exportPath))
        {
            exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RainBuilds");
        }

        // 初始化目标平台 - 默认当前平台
        buildTarget = EditorUserBuildSettings.activeBuildTarget;

        // 初始化样式
        InitializeStyles();
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
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 标题
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("RainFramework 打包工具", titleStyle);
        EditorGUILayout.Space(20);

        // 导出目录设置
        DrawExportPathSection();

        EditorGUILayout.Space(20);

        // 资源准备
        BuildToolAtlas.DrawResourcePreparationSection();

        EditorGUILayout.Space(20);

        // 打包选项
        DrawBuildOptionsSection();

        EditorGUILayout.Space(20);

        // 一键打包按钮
        DrawBuildButton();

        EditorGUILayout.Space(20);

        // 其他工具按钮
        DrawUtilityButtons();

        EditorGUILayout.EndScrollView();
    }

    private void DrawExportPathSection()
    {
        EditorGUILayout.LabelField("【导出设置】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("安装包导出目录:", GUILayout.Width(120));
        exportPath = EditorGUILayout.TextField(exportPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("选择导出目录", exportPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                exportPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }



    private void DrawBuildOptionsSection()
    {
        EditorGUILayout.LabelField("【打包选项】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        // 目标平台
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("目标平台:", buildTarget);

        // 开发版本
        developmentBuild = EditorGUILayout.Toggle("开发版本:", developmentBuild);

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

    private void StartBuild()
    {
        try
        {
            EditorUtility.DisplayProgressBar("打包中", "正在准备打包...", 0f);

            // 创建导出目录
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            // 构建AssetBundle
            if (buildAssetBundles)
            {
                EditorUtility.DisplayProgressBar("打包中", "构建AssetBundle...", 0.2f);
                BuildAssetBundles();
            }

            // 复制到StreamingAssets
            if (copyToStreamingAssets)
            {
                EditorUtility.DisplayProgressBar("打包中", "复制到StreamingAssets...", 0.3f);
                CopyToStreamingAssets();
            }

            // 开始打包
            EditorUtility.DisplayProgressBar("打包中", "正在打包...", 0.4f);
            BuildPlayer();

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包完成", $"打包完成！\n导出目录: {exportPath}", "确定");
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
            target = buildTarget,
            options = developmentBuild ? BuildOptions.Development : BuildOptions.None
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
        return Path.Combine(exportPath, fileName);
    }

    private string GetBuildFileName()
    {
        string platform = buildTarget.ToString();
        string extension = GetBuildExtension();
        return $"{Application.productName}_{platform}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
    }

    private string GetBuildExtension()
    {
        switch (buildTarget)
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

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, buildTarget);
    }

    private void CopyToStreamingAssets()
    {
        // 这里可以添加复制逻辑
        Debug.Log("复制到StreamingAssets功能待实现");
    }

    private void OpenExportDirectory()
    {
        if (Directory.Exists(exportPath))
        {
            EditorUtility.RevealInFinder(exportPath);
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "导出目录不存在", "确定");
        }
    }

    private void ClearExportDirectory()
    {
        if (Directory.Exists(exportPath))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清理导出目录吗？", "确定", "取消"))
            {
                Directory.Delete(exportPath, true);
                Directory.CreateDirectory(exportPath);
                Debug.Log("导出目录已清理");
            }
        }
    }

}
