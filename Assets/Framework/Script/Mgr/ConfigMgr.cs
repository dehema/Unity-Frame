using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using System;

public class ConfigMgr : MonoSingleton<ConfigMgr>
{
    const string configPath = "Json/";
    public ImageTextMixConfig imageTextMix;
    public GameSettingConfig settingConfig;

    public void Init()
    {
    }

    public void LoadAllConfig(bool _localConfig = true)
    {
        imageTextMix = LoadConfig<ImageTextMixConfig>("ImageTextMix");
        settingConfig = LoadConfig<GameSettingConfig>("Setting", _localConfig ? String.Empty : NetInfoMgr.Ins.NetServerData?.Setting);
        AllLoadComplete();
    }

    private void AllLoadComplete()
    {

    }

    /// <summary>
    /// ??ȡ????
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
            Debug.Log("??ȡ???????ļ?" + filePath + "\n" + _config);
        }
        else
        {
            Debug.Log("??ȡ??Զ??????" + typeof(T).ToString() + "\n" + _config);
        }
        T t = JsonConvert.DeserializeObject<T>(_config);
        ConfigBase configBase = t as ConfigBase;
        configBase.Init();
        return t;
    }

    /// <summary>
    /// ??ȡUIView????Yaml?ļ?·??
    /// </summary>
    /// <returns></returns>
    public string GetUIViewConfigPath()
    {
        return "Config/UIView";
    }

    /// <summary>
    /// ??ȡUI????
    /// </summary>
    /// <returns></returns>
    public UIViewConfig LoadUIConfig()
    {
        Utility.Log("??ʼ??ȡUI????");
        string configPath = GetUIViewConfigPath();
        string config = Resources.Load<TextAsset>(configPath).text;
        var deserializer = new DeserializerBuilder()
              .WithNamingConvention(CamelCaseNamingConvention.Instance)
              .Build();
        Utility.Dump(config);
        UIViewConfig UIViewConfig = deserializer.Deserialize<UIViewConfig>(config);
        return UIViewConfig;
    }
}
