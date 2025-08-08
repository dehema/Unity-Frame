using System.Collections.Generic;
using Newtonsoft.Json;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class ConfigMgr : MonoSingleton<ConfigMgr>
{
    const string configPath = "Json/";
    public ImageTextMixConfig imageTextMix;
    public GameSettingConfig settingConfig;
    public AllUnitConfig allUnitConfig;
    public AllCityConfig allCityConfig;
    public WorldConfig worldConfig;
    private UIViewConfig uiViewConfig;
    public UIViewConfig UIViewConfig => uiViewConfig;


    public void Init()
    {
    }

    public void LoadAllConfig(bool _localConfig = true)
    {
        uiViewConfig = LoadViewConfig(); ;
        //imageTextMix = LoadConfig<ImageTextMixConfig>("ImageTextMix");
        //settingConfig = LoadConfig<GameSettingConfig>("Setting");
        allUnitConfig = LoadConfig<AllUnitConfig>("Unit");
        //allCityConfig = LoadConfig<AllCityConfig>("City");
        //worldConfig = LoadConfig<WorldConfig>("World");
        AllLoadComplete();
    }

    private void AllLoadComplete()
    {
    }

    /// <summary>
    /// 读取配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="_config"></param>
    /// <returns></returns>
    private T LoadConfig<T>(string key, string _config = "")
    {
        if (string.IsNullOrEmpty(_config))
        {
            string filePath = configPath + key;
            _config = Resources.Load<TextAsset>(filePath).text;
            Debug.Log("读取到配置文件" + filePath + "\n" + _config);
        }
        else
        {
            Debug.Log("读取到远端配置" + typeof(T).ToString() + "\n" + _config);
        }
        T t = JsonConvert.DeserializeObject<T>(_config);
        ConfigBase configBase = t as ConfigBase;
        configBase.OnLoadComplete();
        return t;
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

    /// <summary>
    /// 读取城镇配置
    /// </summary>
    /// <returns></returns>
    public CityConfig GetCityConfig(int _cityID)
    {
        return allCityConfig.city[_cityID];
    }

    /// <summary>
    /// 读取单位配置
    /// </summary>
    /// <returns></returns>
    public UnitConfig GetUnitConfig(int _unitID)
    {
        return allUnitConfig.unit[_unitID];
    }
}
