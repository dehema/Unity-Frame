using com.adjust.sdk;
using Newtonsoft.Json;
using System;
using UnityEngine;

/// <summary>
/// ???adjust策略管理器 需要调用三个委托 并根据自身项目需求主动调用A事件
/// </summary>
public class AdjustPolicyMgr : MonoBehaviour
{
    public static AdjustPolicyMgr Ins;
    AdjustPolicyData adjustPolicyData;
    const string adjustPolicyDataField = "adjustPolicyData";
    GameObject adjustScriptGo;
    bool isAdjustEnable = false;
    //获取到线上配置
    bool isOnlineConfig = false;
    /// <summary>
    /// 策略配置
    /// </summary>
    AdjustPolicyConfig adjustPolicyConfig = new AdjustPolicyConfig();
    //策略触发方式
    AdjustPolicyTrigger adjustPolicyTrigger;
    #region Delegate
    /// <summary>
    /// loadingview结束时调用
    /// </summary>
    public Action OnLoadingViewEndDelegate;
    /// <summary>
    /// 游戏开始时
    /// </summary>
    public Action OnGameStartDelegate;
    /// <summary>
    /// 策略A调用
    /// </summary>
    public Action OnPolicyADelegate;
    #endregion

    public void Awake()
    {
        Ins = this;
        InitDele();
        adjustScriptGo = GameObject.FindObjectOfType<Adjust>(true).gameObject;
        if (adjustScriptGo == null)
        {
            adjustPolicyTrigger = AdjustPolicyTrigger.t7;
            return;
        }
        LoadAdjustPolicyData();
        if (adjustPolicyData.adjustPolicyLabel == AdjustPolicyLabel.PolicyEnable)
        {
            adjustPolicyTrigger = AdjustPolicyTrigger.t1;
            RefreshAdjustGoActive();
            return;
        }
        else if (adjustPolicyData.adjustPolicyLabel == AdjustPolicyLabel.PolicyDisable)
        {
            adjustPolicyTrigger = AdjustPolicyTrigger.t2;
            return;
        }
    }

    private void Update()
    {
        if (adjustPolicyData.adjustPolicyLabel == AdjustPolicyLabel.None)
        {
            if (Time.time >= adjustPolicyConfig.judgeBTimeOut)
            {
                Log("超时触发 B事件");
                bool enable = UnityEngine.Random.Range(0, 100) < adjustPolicyConfig.passJudgeBRate;
                Log("是否触发 【触发事件B但是仍然设置为PolicyEnable】事件:" + enable);
                adjustPolicyData.adjustPolicyLabel = enable ? AdjustPolicyLabel.PolicyEnable : AdjustPolicyLabel.PolicyDisable;
                adjustPolicyTrigger = enable ? AdjustPolicyTrigger.t5 : AdjustPolicyTrigger.t6;
                SaveDataRefreshAdjustGoSendEvent();
            }
        }
    }

    /// <summary>
    /// 刷新adjust激活状态
    /// </summary>
    private void RefreshAdjustGoActive()
    {
        if (adjustPolicyData.adjustPolicyLabel == AdjustPolicyLabel.PolicyEnable)
        {
            adjustScriptGo.SetActive(true);
            isAdjustEnable = true;
            Log("修改adjust状态为激活");
        }
    }

    /// <summary>
    /// 只有开启策略流程才注册委托
    /// </summary>
    void InitDele()
    {
        OnLoadingViewEndDelegate += OnLoadingViewEnd;
        OnPolicyADelegate += OnPolicyA;
        OnGameStartDelegate += OnGameStart;
    }

    void ClearDele()
    {
        //OnLoadingViewEndDelegate -= OnLoadingViewEnd;
        //OnPolicyADelegate -= OnPolicyA;
    }

    //loadingview结束时调用
    private void OnLoadingViewEnd()
    {
        if (adjustPolicyData.adjustPolicyLabel != AdjustPolicyLabel.None)
            return;
        if (NetInfoMgr.Ins.ready && !string.IsNullOrEmpty(NetInfoMgr.Ins.NetServerData?.AdjustPolicy))
        {
            //读取到线上配置
            Log("读取到线上配置");
            isOnlineConfig = true;
            adjustPolicyConfig = JsonConvert.DeserializeObject<AdjustPolicyConfig>(NetInfoMgr.Ins.NetServerData.AdjustPolicy);
        }
        else
        {
            //没有读取到线上配置 直接标记为 【PolicyEnable】
            Log("没有读取到线上配置 直接标记为 【PolicyEnable】");
            adjustPolicyData.adjustPolicyLabel = AdjustPolicyLabel.PolicyEnable;
            adjustPolicyTrigger = AdjustPolicyTrigger.t0;
            SaveDataRefreshAdjustGoSendEvent();
        }
    }

    //触发过 OnGameStart
    bool _isOnGameStart = false;
    void OnGameStart()
    {
        if (_isOnGameStart)
            return;
        _isOnGameStart = true;
        if (adjustPolicyData.adjustPolicyLabel == AdjustPolicyLabel.None && !adjustPolicyConfig.strategySwitch)
        {
            Log("没有开启策略，直接标记为【PolicyEnable】");
            adjustPolicyData.adjustPolicyLabel = AdjustPolicyLabel.PolicyEnable;
            adjustPolicyTrigger = AdjustPolicyTrigger.t8;
            SaveDataRefreshAdjustGoSendEvent();
        }
        else if (adjustPolicyTrigger == AdjustPolicyTrigger.t7 || adjustPolicyTrigger == AdjustPolicyTrigger.t1 || adjustPolicyTrigger == AdjustPolicyTrigger.t2)
        {
            SendPostEvent();
        }
    }

    void SendOnPolicyA()
    {
        string OnPolicyAKey = "OnPolicyAKey";
        Log($"准备发送发送打点e4033");
        bool hasOnPolicyAKey = !string.IsNullOrEmpty(PlayerPrefs.GetString(OnPolicyAKey));
        if (!hasOnPolicyAKey)
        {
            int strategySwitchIndex = adjustPolicyConfig.strategySwitch ? 1 : 0;
            if (!isOnlineConfig)
            {
                //未读到线上配置    
                strategySwitchIndex = 2;
            }
            Log($"发送打点e4033->{strategySwitchIndex}->{adjustPolicyData.adjustPolicyLabel}->{(int)adjustPolicyTrigger}");
            PostEventScript.Ins.SendEvent(PostEventType.e4033, strategySwitchIndex, (int)adjustPolicyData.adjustPolicyLabel, (int)adjustPolicyTrigger);
            PlayerPrefs.SetString(OnPolicyAKey, OnPolicyAKey);
        }
        else
        {
            Log($"之前发送过打点e4033");
        }
    }

    private void OnPolicyA()
    {
        if (adjustPolicyData.adjustPolicyLabel != AdjustPolicyLabel.None)
        {
            SendOnPolicyA();
            return;
        }
        bool enable = UnityEngine.Random.Range(0, 100) < adjustPolicyConfig.passJudgeARate;
        adjustPolicyData.adjustPolicyLabel = enable ? AdjustPolicyLabel.PolicyEnable : AdjustPolicyLabel.PolicyDisable;
        adjustPolicyTrigger = enable ? AdjustPolicyTrigger.t3 : AdjustPolicyTrigger.t4;
        SaveDataRefreshAdjustGoSendEvent();
        SendOnPolicyA();
    }

    private void SendPostEvent()
    {
        int strategySwitchIndex = adjustPolicyConfig.strategySwitch ? 1 : 0;
        if (!isOnlineConfig)
        {
            //未读到线上配置    
            strategySwitchIndex = 2;
        }
        Log($"发送打点,是否开启【{strategySwitchIndex}】,标记字段【{adjustPolicyData.adjustPolicyLabel}】,触发方式{adjustPolicyTrigger}");
        PostEventScript.Ins.SendEvent(PostEventType.e4032, strategySwitchIndex, (int)adjustPolicyData.adjustPolicyLabel, (int)adjustPolicyTrigger);
    }

    void LoadAdjustPolicyData()
    {
        string data = PlayerPrefs.GetString(adjustPolicyDataField);
        if (string.IsNullOrEmpty(data))
        {
            Log("未读取到本地数据");
            adjustPolicyData = new AdjustPolicyData();
        }
        else
        {
            adjustPolicyData = JsonConvert.DeserializeObject<AdjustPolicyData>(data);
            Log("读取到本地数据，标记为" + adjustPolicyData.adjustPolicyLabel.ToString());
        }
    }

    void SaveAdjustPolicyData()
    {
        string data = JsonConvert.SerializeObject(adjustPolicyData);
        PlayerPrefs.SetString(adjustPolicyDataField, data);
    }

    private void Log(string _str)
    {
        Debug.Log("AdjustPolicyMgr->" + _str);
    }

    void SaveDataRefreshAdjustGoSendEvent()
    {
        SaveAdjustPolicyData();
        RefreshAdjustGoActive();
        SendPostEvent();
    }
}

public class AdjustPolicyConfig
{
    /// <summary>
    /// 策略是否启用 【策略启用】进行策略流程    【策略不启用】直接初始化adjust并且将玩家标记为【PolicyEnable】
    /// </summary>
    public bool strategySwitch = false;
    /// <summary>
    /// 超时时间 秒 超时直接判定标记为【PolicyDisable】
    /// </summary>
    public float judgeBTimeOut = 3600;
    /// <summary>
    /// 触发策略A被判定为【PolicyEnable】的几率 百分制30=30%
    /// </summary>
    public int passJudgeARate = 100;
    /// <summary>
    /// 超时后还能被判定为【PolicyEnable】的几率 百分制30=30%
    /// </summary>
    public int passJudgeBRate = 0;
}

public enum AdjustPolicyLabel
{
    None = 0,
    /// <summary>
    /// 策略启用
    /// </summary>
    PolicyEnable = 1,
    /// <summary>
    /// 策略不启用 下次进入不判断后台配置，开启adjust
    /// </summary>
    PolicyDisable = 2,
}

public class AdjustPolicyData
{
    public AdjustPolicyLabel adjustPolicyLabel = AdjustPolicyLabel.None;
}

/// <summary>
/// Adjust策略触发方式
/// </summary>
public enum AdjustPolicyTrigger
{
    /// <summary>
    /// 未读到线上配置，直接启动策略
    /// </summary>
    t0 = 0,
    /// <summary>
    /// 读到玩家数据，启动策略
    /// </summary>
    t1 = 1,
    /// <summary>
    /// 读到玩家数据，不启动策略
    /// </summary>
    t2 = 2,
    /// <summary>
    /// 触发事件A，启动策略
    /// </summary>
    t3 = 3,
    /// <summary>
    /// 触发事件A，但是不启动策略
    /// </summary>
    t4 = 4,
    /// <summary>
    /// 触发事件B，但是启动策略
    /// </summary>
    t5 = 5,
    /// <summary>
    /// 触发事件B，不启动策略
    /// </summary>
    t6 = 6,
    /// <summary>
    /// 没有找到adjust脚本
    /// </summary>
    t7 = 7,
    /// <summary>
    /// 没有开启策略，初始化
    /// </summary>
    t8 = 8
}