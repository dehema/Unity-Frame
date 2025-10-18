using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Rain.Core;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 打包工具配置数据
/// </summary>
[System.Serializable]
public class BuildToolConfig
{
    [NonSerialized]
    public const string configPath = "Assets/RainFramework/Editor/Build/BuildToolConfig.json";

    // 基础打包配置
    public string Version = "1.0.0";
    public string PackageExportPath = "";       //安装包导出路径
    public BuildTarget BuildTarget = BuildTarget.Android;
    public bool DevelopmentBuild = false;       //是否是开发版本
    public string GameRemoteAddress;            //远端地址

    // AB包相关配置
    public bool AutoBuildAB = true;
    public string ABOutputPath => Application.persistentDataPath + HotUpdateMgr.HotUpdateDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName();
    public string ABRemoteAddress => GameRemoteAddress + HotUpdateMgr.HotUpdateDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName(); //远端ab地址


    /// <summary>
    /// 加载配置
    /// </summary>
    public static BuildToolConfig Load()
    {
        BuildToolConfig config = new BuildToolConfig();
        try
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonUtility.FromJson<BuildToolConfig>(json);
            }
            else
            {
                // 设置默认值
                config.PackageExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RainBuilds");
                config.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
                config.Save();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载配置失败: {e.Message}");
            config = new BuildToolConfig();
        }
        return config;
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(configPath, json);
        AssetDatabase.Refresh();
    }
}