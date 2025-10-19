using System;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Rain.UI;
using SimpleJSON;
using Rain.Core;
using System.IO;
#if UNITY_ANDROID
using UnityEngine.Networking;
#endif

public class ConfigMgr : MonoSingleton<ConfigMgr>
{
    private static Tables cfg;

    private UIViewConfig uiViewConfig;
    public UIViewConfig UIViewConfig => uiViewConfig;

    public static TbGameSetting GameSetting => cfg.TbGameSetting;
    public static TbSLGSetting SLGSetting => cfg.TbSLGSetting;
    public static TbImageTextMix ImageTextMix => cfg.TbImageTextMix;
    public static TbUnit Unit => cfg.TbUnit;
    public static TbDeployFormation DeployFormation => cfg.TbDeployFormation;
    public static TbTech Tech => cfg.TbTech;
    public static TbTechCategory TechCategory => cfg.TbTechCategory;
    public static TbCityBuilding CityBuilding => cfg.TbCityBuilding;
    public static TbCityBuildingLevel CityBuildingLevel => cfg.TbCityBuildingLevel;
    public static TbCityBuildingSlot CityBuildingSlot => cfg.TbCityBuildingSlot;



    public void Init()
    {
        //deployFormation = new DeployFormation();
    }

    public void LoadAllConfig(bool _localConfig = true)
    {
        uiViewConfig = LoadViewConfig();

        cfg = new Tables(LoadJson);
        AllLoadComplete();
    }

    private JSONNode LoadJson(string _tableName)
    {
        string filePath = "";
        string result = "";

#if  UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        // 编辑器、Windows、Mac平台：同步读取
        filePath = Path.Combine(Application.streamingAssetsPath, "Config", "Generate", _tableName + ".json");
        if (File.Exists(filePath))
        {
            result = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError($"文件不存在: {filePath}");
            return null;
        }

#elif UNITY_ANDROID
        // Android平台使用UnityWebRequest读取
        string url = Path.Combine(Application.streamingAssetsPath, "Config", "Generate", _tableName + ".json");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 发送同步请求（如果需要异步可改为SendWebRequest().WaitForCompletion()或异步等待）
            UnityWebRequestAsyncOperation asyncOp = webRequest.SendWebRequest();
            // 等待请求完成
            while (!asyncOp.isDone)
            {
                // 可以在这里添加进度更新逻辑
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                result = webRequest.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"Android加载失败: {webRequest.error}，路径: {url}");
                return null;
            }
        }
#endif

        return JSONNode.Parse(result);
    }

    private void AllLoadComplete()
    {
    }

    /// <summary>
    /// 获取UIView配置Json文件路径
    /// </summary>
    /// <returns></returns>
    public string UIViewConfigPath
    {
        get { return "Config/UIViewConfig"; }
    }

    /// <summary>
    /// 读取UI配置
    /// </summary>
    /// <returns></returns>
    private UIViewConfig LoadViewConfig()
    {
        if (uiViewConfig != null)
        {
            return uiViewConfig;
        }
        Debug.Log("开始读取UI配置");
        string configPath = UIViewConfigPath;
        string config = Resources.Load<TextAsset>(configPath).text;
        var deserializer = new DeserializerBuilder()
              .WithNamingConvention(CamelCaseNamingConvention.Instance)
              .Build();
        Util.Log(config);
        uiViewConfig = deserializer.Deserialize<UIViewConfig>(config);
        foreach (var group in uiViewConfig.view)
        {
            foreach (var view in group.Value)
            {
                view.Value.viewName = view.Key;
                view.Value.group = group.Key;
                uiViewConfig.allViewConfig.Add(view.Key, view.Value);
            }
        }
        return uiViewConfig;
    }

    /// <summary>
    /// 获取所有UI配置
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, ViewConfig> GetAllViewConfig()
    {
        return uiViewConfig.allViewConfig;
    }
}
