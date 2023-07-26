using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AllCityConfig : ConfigBase
{
    public Dictionary<int, CityConfig> city = new Dictionary<int, CityConfig>();
    public Dictionary<int, BuildingConfig> building = new Dictionary<int, BuildingConfig>();
    public Dictionary<int, RecruitDailyConfig> recruitDaily = new Dictionary<int, RecruitDailyConfig>();

    public override void Init()
    {
        base.Init();
        foreach (var item in building)
        {
            item.Value.Init();
        }
        foreach (var item in recruitDaily)
        {
            item.Value.Init();
        }
    }
}

public class CityConfig
{
    public int ID;
    public string name;
    /// <summary>
    /// X坐标
    /// </summary>
    public float posX;
    /// <summary>
    /// Y坐标
    /// </summary>
    public float posY;
    public string icon;
    /// <summary>
    /// 派系ID
    /// </summary>
    public int factionID;
    /// <summary>
    /// 商队数量
    /// </summary>
    public int tradeCaravan_num;
    /// <summary>
    /// 商队初始资金
    /// </summary>
    public int tradeCaravan_gold;
}

public class BuildingConfig
{
    public int ID;
    /// <summary>
    /// 建筑ID
    /// </summary>
    public string buildingID;
    /// <summary>
    /// 种族
    /// </summary>
    public RaceType raceType;
    /// <summary>
    /// 建筑类型
    /// </summary>
    public BuildingType buildingType;
    /// <summary>
    /// 建筑二级类型
    /// </summary>
    public BuildingSubType buildingSubType;
    /// <summary>
    /// 建筑名称
    /// </summary>
    public string name;
    /// <summary>
    /// 前置建筑
    /// </summary>
    public string _preBuildingIDs;
    /// <summary>
    /// 升级后的建筑ID
    /// </summary>
    public int upgradeBuildingID = 0;
    /// <summary>
    /// 图标
    /// </summary>
    public string icon;
    /// <summary>
    /// 花费金钱
    /// </summary>
    public int costGold;
    /// <summary>
    /// 升级耗时
    /// </summary>
    public int costHour;
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool enable;
    /// <summary>
    /// 每日工资
    /// </summary>
    public int dailySalary;

    /// <summary>
    /// 前置建筑
    /// </summary>
    public List<int> preBuildingIDs = new List<int>();

    public void Init()
    {
        if (!string.IsNullOrEmpty(_preBuildingIDs))
            Array.ForEach(_preBuildingIDs.Split(','), val => { preBuildingIDs.Add(int.Parse(val)); });
    }
}

/// <summary>
/// 军事设施每日招募设置
/// </summary>
public class RecruitDailyConfig
{
    public int ID;
    /// <summary>
    /// 生成总数
    /// </summary>
    public int totalNum;
    public string _recruitNum;
    /// <summary>
    /// 招募
    /// </summary>
    public Dictionary<int, int> recruitNum;

    public void Init()
    {
        recruitNum = JsonConvert.DeserializeObject<Dictionary<int, int>>(_recruitNum);
    }
}