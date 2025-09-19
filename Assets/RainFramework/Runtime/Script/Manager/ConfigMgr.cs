using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Rain.UI;
using SimpleJSON;
using Rain.Core;
using System.IO;


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
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        // 编辑器、Windows、Mac平台：直接使用文件路径
        filePath = Path.Combine(Application.streamingAssetsPath, "Config", "Generate", _tableName + ".json");
        if (File.Exists(filePath))
        {
            // 同步读取（也可使用File.ReadAllTextAsync异步读取）
            result = File.ReadAllText(filePath);
        }
        else
        {
            Debug.LogError($"文件不存在: {filePath}");
        }

#elif UNITY_ANDROID
        // Android平台：需使用WWW或UnityWebRequest读取
        filePath = Path.Combine("jar:file://" + Application.dataPath, "Config", "!/assets/", _tableName);
        using (var www = new WWW(filePath))
        {
            // 等待WWW加载完成（异步）
            await Task.Yield();
            while (!www.isDone)
                await Task.Yield();

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError($"Android读取失败: {www.error}");
            }
            else
            {
                result = www.text;
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
        Utility.Log(config);
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
