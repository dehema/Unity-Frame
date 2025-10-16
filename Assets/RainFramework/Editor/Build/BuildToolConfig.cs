using System.Collections;
using System.Collections.Generic;
using UnityEditor;


/// <summary>
/// 打包工具配置数据
/// </summary>
[System.Serializable]
public class BuildToolConfig
{
    // 基础打包配置
    public string exportPath = "";
    public BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
    public bool developmentBuild = false;
    public bool copyToStreamingAssets = false;
    public bool buildAssetBundles = false;

    // AB包相关配置
    public string abOutputPath = "";
    public string onlineABUrl = "";
    public bool autoBuildAB = true;

    // 图集相关配置
    public bool autoPackAtlas = true;
    public string atlasSourcePath = "Assets/AssetBundles/Art/Resources/UI";
    public string atlasOutputPath = "Assets/AssetBundles/Art/Atlas";

    // 其他配置
    public bool showAdvancedOptions = false;
    public bool clearBuildCache = false;
}