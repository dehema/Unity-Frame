using System.Collections;
using System.Collections.Generic;
using UnityEditor;


/// <summary>
/// ���������������
/// </summary>
[System.Serializable]
public class BuildToolConfig
{
    // �����������
    public string exportPath = "";
    public BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
    public bool developmentBuild = false;
    public bool copyToStreamingAssets = false;
    public bool buildAssetBundles = false;

    // AB���������
    public string abOutputPath = "";
    public string onlineABUrl = "";
    public bool autoBuildAB = true;

    // ͼ���������
    public bool autoPackAtlas = true;
    public string atlasSourcePath = "Assets/AssetBundles/Art/Resources/UI";
    public string atlasOutputPath = "Assets/AssetBundles/Art/Atlas";

    // ��������
    public bool showAdvancedOptions = false;
    public bool clearBuildCache = false;
}