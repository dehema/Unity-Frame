using System;
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
        LoadPlayerData();
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
        //string data = gameData.ToString();
        //PlayerPrefs.SetString(SaveField.gameData, data);
    }

    /// <summary>
    /// 读取玩家数据
    /// </summary>
    public void LoadPlayerData()
    {
        //玩家数据
        playerData = new PlayerData();
        InitPlayerData();
        SavePlayerData();
    }

    /// <summary>
    /// 保存玩家数据（属性数值）
    /// </summary>
    public void SavePlayerData()
    {
        string str = playerData.ToJson();
        PlayerPrefs.SetString(SaveField.playerData, str);
        PlayerMgr.Ins.playerData.createTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    /// <summary>
    /// 初始化玩家数据
    /// </summary>
    private void InitPlayerData()
    {
        string playerDataStr = PlayerPrefs.GetString(SaveField.playerData);
        bool isNewPlayer = string.IsNullOrEmpty(playerDataStr);
        //读取玩家数据
        if (!isNewPlayer)
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(playerDataStr);
            playerData.SetVal(dict);
        }
        //科技
        foreach (var item in ConfigMgr.Tech.DataMap)
        {
            if (!playerData.techs.ContainsKey(item.Key))
            {
                playerData.techs.Add(item.Key, new TechData(item.Key));
            }
        }
        //建筑
        foreach (var item in ConfigMgr.CityBuildingSlot.DataMap)
        {
            if (!playerData.cityBuildings.ContainsKey(item.Key))
            {
                CityBuildingData newData = new CityBuildingData();
                newData.SlotID = item.Key;
                newData.BuildingType = item.Value.BuildingType;
                playerData.cityBuildings.Add(item.Key, newData);
            }
        }
        //如果是新玩家补初始资源
        if (isNewPlayer)
            SupplyNewPlayerRes();
    }

    //如果是新玩家补初始资源
    public void SupplyNewPlayerRes()
    {
        playerData.food.Value = 1000000;
        playerData.wood.Value = 1000000;
        playerData.gold.Value = 1000000;
        playerData.book.Value = 1000000;
        playerData.ore.Value = 1000000;
        playerData.diamond.Value = 10000;
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
    /// 保存所有数据
    /// </summary>
    public void SaveAllData()
    {
        SaveGameData();
        SavePlayerData();
        SaveSettingData();
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
