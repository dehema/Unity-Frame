using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//登录服务器返回数据
public class RootData
{
    public int code { get; set; }
    public string msg { get; set; }
    public ServerData data { get; set; }
}
//用户登录信息
public class ServerUserData
{
    public int code { get; set; }
    public string msg { get; set; }
    public int data { get; set; }
}
//服务器的数据
public class ServerData
{
    public string init { get; set; }
    public string version { get; set; }

    public string apple_pie { get; set; }
    public string inter_b2f_count { get; set; }
    public string inter_b2f_freq { get; set; }
    public string ad_fail_interval { get; set; }
    public string ad_limit_interval { get; set; }
    public string ad_limit_hour { get; set; }
    public string ad_limit { get; set; }
    public string inter_freq { get; set; }
    public string inter_delay { get; set; }
    public string inter_count { get; set; }
    public string relax_interval { get; set; }
    public string trial_MaxNum { get; set; }
    public string nextlevel_interval { get; set; }
    /// <summary>
    /// 调试设备列表
    /// </summary>
    public string DebugDeviceID { get; set; }
    public string Setting { get; set; }
    public string AdjustPolicy { get; set; }
}
public class Init
{
    public double[] cash_random { get; set; }
    public double box_cash_price { get; set; }
    public double box_gold_price { get; set; }
    public double box_amazon_price { get; set; }
    public double box_skill_chance { get; set; }
    public double passlevel_cash_price { get; set; }
    public double small_reward_chance { get; set; }
    public Dictionary<string, int> small_reward_weight_group { get; set; }
    public double small_reward_cash_price { get; set; }
    public double small_reward_gold_price { get; set; }
    public int card_reward_count { get; set; }
    public Dictionary<string, int> card_reward_weight_group { get; set; }
    public double card_reward_cash_price { get; set; }
    public MultiGroup[] cash_group { get; set; }
    public MultiGroup[] gold_group { get; set; }
    public MultiGroup[] amazon_group { get; set; }
    public List<ServerSlotItem> slot_group { get; set; }

    public double HandCardMatchingChance { get; set; }
    public double HeapCardMatchingChance { get; set; }
    public int BonusIntervalTime { get; set; }
    public int BonusIntervelNum { get; set; }
}


public class MultiGroup
{
    public int max { get; set; }
    public int multi { get; set; }
}

public class ServerSlotItem
{
    public int multi { get; set; }
    public int weight { get; set; }
}
