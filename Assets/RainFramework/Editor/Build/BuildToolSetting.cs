using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Rain.Core;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 打包设置
/// </summary>
public class BuildToolSetting : BuildToolBase
{
    public BuildToolSetting(BuildToolConfig _config) : base(_config)
    {
        pageName = "打包设置";
    }

    protected override void DrawContent()
    {
        // 版本号设置
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("版本号:", GUILayout.Width(100));
        config.Version = EditorGUILayout.TextField(config.Version);
        EditorGUILayout.EndHorizontal();

        // 安装包导出目录
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("安装包导出目录:", GUILayout.Width(100));
        config.PackageExportPath = EditorGUILayout.TextField(config.PackageExportPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("选择导出目录", config.PackageExportPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                config.PackageExportPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        //版本号设置
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("远端地址:", GUILayout.Width(100));
        config.GameRemoteAddress = EditorGUILayout.TextField(config.GameRemoteAddress);
        if (GUILayout.Button("打开", GUILayout.Width(60)))
            EditorUtility.RevealInFinder(config.GameRemoteAddress + "/");
        EditorGUILayout.EndHorizontal();

        //目标平台
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标平台:", GUILayout.Width(100));
        config.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(config.BuildTarget);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("构建游戏版本文件", GUILayout.Height(30)))
        {
            BuildLocalGameVersionFile();
            BuildRemoteGameVersionFile();
        }
        if (GUILayout.Button("打开导出目录", GUILayout.Height(30)))
        {
            OpenPackageExportDirectory();
        }
        if (GUILayout.Button("清理导出目录", GUILayout.Height(30)))
        {
            ClearPackageExportDirectory();
        }
        EditorGUILayout.EndHorizontal();

    }

    private void OpenPackageExportDirectory()
    {
        if (Directory.Exists(config.PackageExportPath))
        {
            EditorUtility.RevealInFinder(config.PackageExportPath + "/");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "导出目录不存在", "确定");
        }
    }

    private void ClearPackageExportDirectory()
    {
        if (Directory.Exists(config.PackageExportPath))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清理导出目录吗？", "确定", "取消"))
            {
                Directory.Delete(config.PackageExportPath, true);
                Directory.CreateDirectory(config.PackageExportPath);
                Debug.Log("导出目录已清理");
            }
        }
    }

    /// <summary>
    /// 构建本地游戏版本配置
    /// </summary>
    private void BuildLocalGameVersionFile()
    {
        GameVersion gameVersion = new GameVersion();
        gameVersion.Version = "0.0.1";
        gameVersion.AssetRemoteAddress = config.ABRemoteAddress;
        string jsonPath = Path.Combine(Application.dataPath, "Resources", $"{nameof(GameVersion)}.json");
        string json = JsonConvert.SerializeObject(gameVersion, Formatting.Indented);
        File.WriteAllText(jsonPath, json);
    }

    /// <summary>
    /// 构建远端游戏版本配置
    /// </summary>

    private void BuildRemoteGameVersionFile()
    {
        GameVersion gameVersion = new GameVersion();
        gameVersion.Version = config.Version;
        gameVersion.AssetRemoteAddress = config.GameRemoteAddress;
        string jsonPath = Path.Combine(config.GameRemoteAddress, $"{nameof(GameVersion)}.json");
        string json = JsonConvert.SerializeObject(gameVersion, Formatting.Indented);
        File.WriteAllText(jsonPath, json);
    }
}
