using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllFactionConfig : ConfigBase
{
    public Dictionary<int, FactionConfig> faction = new Dictionary<int, FactionConfig>();
    public Dictionary<RaceType, RaceConfig> race = new Dictionary<RaceType, RaceConfig>();

    public override void Init()
    {
        base.Init();
        foreach (var item in faction)
        {
            item.Value.Init();
        }
        foreach (var item in race)
        {
            item.Value.Init();
        }
    }
}

public class FactionConfig
{
    /// <summary>
    /// 派系ID
    /// </summary>
    public int ID;
    public string name;
    /// <summary>
    /// 种族ID
    /// </summary>
    public RaceType raceID;
    public string _defaultBuilding;
    public string _initial_Building;
    /// <summary>
    /// 默认被建造的建筑
    /// </summary>
    public List<int> defaultBuilding = new List<int>();
    /// <summary>
    /// 初始可显示的
    /// </summary>
    public List<int> initial_Building = new List<int>();
    /// <summary>
    /// 初始部队单位
    /// </summary>
    public string _init_troop_unit;
    /// <summary>
    /// 初始部队单位
    /// </summary>
    public Dictionary<int, int> init_troop_unit = new Dictionary<int, int>();

    public void Init()
    {
        Array.ForEach(_defaultBuilding.Split(','), val => { defaultBuilding.Add(int.Parse(val)); });
        Array.ForEach(_initial_Building.Split(','), val => { initial_Building.Add(int.Parse(val)); });
        init_troop_unit = JsonConvert.DeserializeObject<Dictionary<int, int>>(_init_troop_unit);
    }
}

/// <summary>
/// 种族建筑
/// </summary>
public class RaceConfig
{
    /// <summary>
    /// 种族类型
    /// </summary>
    public RaceType raceType;
    public string _mainBase;
    public string _military;
    public string _economy;
    /// <summary>
    /// 主要序列
    /// </summary>
    public List<int> mainBase = new List<int>();
    /// <summary>
    /// 军事序列
    /// </summary>
    public List<int> military = new List<int>();
    /// <summary>
    /// 经济序列
    /// </summary>
    public List<int> economy = new List<int>();
    public void Init()
    {
        Array.ForEach(_mainBase.Split(','), val => { mainBase.Add(int.Parse(val)); });
        Array.ForEach(_military.Split(','), val => { military.Add(int.Parse(val)); });
        Array.ForEach(_economy.Split(','), val => { economy.Add(int.Parse(val)); });
    }
}