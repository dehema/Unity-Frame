using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class DataMgr : Singleton<DataMgr>
{
    /// <summary>
    /// 玩家存档
    /// </summary>
    public PlayerData playerData;
    bool isLoaded = false;
    public GameData gameData;
    public SettingData settingData;

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
            SavePlayerData();
        }
        else
        {
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(playerDataStr);
            Utility.Dump("-------------------------------玩家数据--------------------------------");
            playerData.SetVal(dict);
        }
        //游戏数据
        gameData = JsonConvert.DeserializeObject<GameData>(PlayerPrefs.GetString(SaveField.gameData));
        if (gameData == null)
        {
            gameData = new GameData();
            SaveGameData();
        }
        //设置
        settingData = JsonConvert.DeserializeObject<SettingData>(PlayerPrefs.GetString(SaveField.settingData));
        if (settingData == null)
        {
            settingData = new SettingData();
            SaveSettingData();
        }
        AudioMgr.Ins.soundVolume = settingData.soundVolume;
        AudioMgr.Ins.musicVolume = settingData.musicVolume;
        isLoaded = true;
        //login
        Login();
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
        settingData.musicVolume = AudioMgr.Ins.musicVolume;
        settingData.soundVolume = AudioMgr.Ins.soundVolume;
        PlayerPrefs.SetString(SaveField.settingData, JsonConvert.SerializeObject(settingData));
    }

    /// <summary>
    /// 玩家登陆
    /// </summary>
    public void Login()
    {
        if (gameData.lastLoginDate.Date != DateTime.Now.Date)
        {
            NewDay();
        }
        gameData.lastLoginDate = DateTime.Now;
        //清空广告计数
        gameData.lastADInterstitialTime = DateTime.Now;
        gameData.lastADRewardTime = DateTime.Now;
        gameData.passRewardVideoTime = 0;
        SaveGameData();
    }

    /// <summary>
    /// 新的登录日
    /// </summary>
    public void NewDay()
    {
        //post event
        PostEventScript.Ins.SendEvent(PostEventType.e4028, Utility.GetIPAdress());
        PostEventScript.Ins.SendEvent(PostEventType.e4029, LangMgr.Ins.currLang);
    }
}

public class SaveField
{
    public const string playerData = "playerData";
    public const string gameData = "gameData";
    public const string settingData = "settingData";
}