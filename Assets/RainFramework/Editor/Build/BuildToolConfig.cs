using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public string version = "1.0.0";
    public string exportPath = "";
    public BuildTarget buildTarget = BuildTarget.Android;
    public bool developmentBuild = false;       //是否是开发版本

    // AB包相关配置
    public string abOutputPath = "";
    public string assetRemoteAddress = ""; //远端ab地址
    public bool autoBuildAB = true;

    // 图集相关配置
    public bool autoPackAtlas = true;
    public string atlasSourcePath = "Assets/AssetBundles/Art/Resources/UI";
    public string atlasOutputPath = "Assets/AssetBundles/Art/Atlas";

    // 其他配置
    public bool showAdvancedOptions = false;
    public bool clearBuildCache = false;

    //ab包地址
    public string ABRemoteAddress => Path.Combine(assetRemoteAddress, "AssetBundles");
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
                config.exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RainBuilds");
                config.buildTarget = EditorUserBuildSettings.activeBuildTarget;
                config.abOutputPath = Path.Combine(Application.persistentDataPath, "AssetBundles");
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
        try
        {
            string json = JsonUtility.ToJson(this, true);
            File.WriteAllText(configPath, json);
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"保存配置失败: {e.Message}");
        }
    }
}