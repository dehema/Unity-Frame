using System.Collections.Generic;
using Newtonsoft.Json;
using Rain.Core;
using UnityEngine;

public class DataMgr : ModuleSingleton<DataMgr>, IModule
{
    /// <summary>
    /// 玩家存档
    /// </summary>
    public PlayerData playerData;
    bool isLoaded = false;
    public GameData gameData;
    public SettingData settingData;
    /// <summary>
    /// 玩家派系
    /// </summary>
    public const int playerFactionID = 11;

    public void Load()
    {
        if (isLoaded)
        {
            return;
        }
        //玩家数据
        playerData = new PlayerData();
        string playerDataStr = PlayerPrefs.GetString(SaveField.playerData);
        if (string.IsNullOrEmpty(playerDataStr))
        {
            playerData.playerName.Value = "dehema";
            SavePlayerData();
        }
        else
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(playerDataStr);
            Debug.Log("-------------------------------玩家数据--------------------------------");
            playerData.SetVal(dict);
        }
        InitGameData();
        //设置
        settingData = JsonConvert.DeserializeObject<SettingData>(PlayerPrefs.GetString(SaveField.settingData));
        if (settingData == null)
        {
            settingData = new SettingData();
            if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
            {
                settingData.language = SystemLanguage.Chinese;
            }
            else
            {
                settingData.language = SystemLanguage.English;
            }
            SaveSettingData();
        }
        //AudioMgr.Ins.soundVolume = settingData.soundVolume;
        //AudioMgr.Ins.musicVolume = settingData.musicVolume;
        isLoaded = true;
        //login
        Login();
    }

    /// <summary>
    /// 初始化游戏数据
    /// </summary>
    void InitGameData()
    {
        SaveGameData();
    }

    /// <summary>
    /// 保存游戏数据 （设置、签到等）
    /// </summary>
    public void SaveGameData()
    {
        string data = JsonConvert.SerializeObject(gameData);
        PlayerPrefs.SetString(SaveField.gameData, data);
    }

    /// <summary>
    /// 保存玩家数据（属性数值）
    /// </summary>
    public void SavePlayerData()
    {
        string str = playerData.ToJson();
        PlayerPrefs.SetString(SaveField.playerData, str);
    }

    /// <summary>
    /// 保存设置数据
    /// </summary>
    public void SaveSettingData()
    {
        //settingData.musicVolume = AudioMgr.Ins.musicVolume;
        //settingData.soundVolume = AudioMgr.Ins.soundVolume;
        PlayerPrefs.SetString(SaveField.settingData, JsonConvert.SerializeObject(settingData));
    }

    /// <summary>
    /// 玩家登陆
    /// </summary>
    public void Login()
    {
        SaveGameData();
    }

    /// <summary>
    /// 新的登录日
    /// </summary>
    public void NewDay()
    {

    }

    /// <summary>
    /// 保存所有数据
    /// </summary>
    public void SaveAllData()
    {
        DataMgr.Ins.SaveGameData();
        DataMgr.Ins.SavePlayerData();
        DataMgr.Ins.SaveSettingData();
    }

    public void OnInit(object createParam)
    {
    }

    public void OnUpdate()
    {
    }

    public void OnLateUpdate()
    {
    }

    public void OnFixedUpdate()
    {
    }

    public void OnTermination()
    {
    }
}

public class SaveField
{
    public const string playerData = "playerData";
    public const string gameData = "gameData";
    public const string settingData = "settingData";
}
