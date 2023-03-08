using com.adjust.sdk;
using Newtonsoft.Json;
using System;
using UnityEngine;

/// <summary>
/// ???adjust���Թ����� ��Ҫ��������ί�� ������������Ŀ������������A�¼�
/// </summary>
public class AdjustPolicyMgr : MonoBehaviour
{
    public static AdjustPolicyMgr Ins;
    AdjustPolicyData adjustPolicyData;
    const string adjustPolicyDataField = "adjustPolicyData";
    GameObject adjustScriptGo;
    bool isAdjustEnable = false;
    //��ȡ����������
    bool isOnlineConfig = false;
    /// <summary>
    /// ��������
    /// </summary>
    AdjustPolicyConfig adjustPolicyConfig = new AdjustPolicyConfig();
    //���Դ�����ʽ
    AdjustPolicyTrigger adjustPolicyTrigger;
    #region Delegate
    /// <summary>
    /// loadingview����ʱ����
    /// </summary>
    public Action OnLoadingViewEndDelegate;
    /// <summary>
    /// ��Ϸ��ʼʱ
    /// </summary>
    public Action OnGameStartDelegate;
    /// <summary>
    /// ����A����
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
                Log("��ʱ���� B�¼�");
                bool enable = UnityEngine.Random.Range(0, 100) < adjustPolicyConfig.passJudgeBRate;
                Log("�Ƿ񴥷� �������¼�B������Ȼ����ΪPolicyEnable���¼�:" + enable);
                adjustPolicyData.adjustPolicyLabel = enable ? AdjustPolicyLabel.PolicyEnable : AdjustPolicyLabel.PolicyDisable;
                adjustPolicyTrigger = enable ? AdjustPolicyTrigger.t5 : AdjustPolicyTrigger.t6;
                SaveDataRefreshAdjustGoSendEvent();
            }
        }
    }

    /// <summary>
    /// ˢ��adjust����״̬
    /// </summary>
    private void RefreshAdjustGoActive()
    {
        if (adjustPolicyData.adjustPolicyLabel == AdjustPolicyLabel.PolicyEnable)
        {
            adjustScriptGo.SetActive(true);
            isAdjustEnable = true;
            Log("�޸�adjust״̬Ϊ����");
        }
    }

    /// <summary>
    /// ֻ�п����������̲�ע��ί��
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

    //loadingview����ʱ����
    private void OnLoadingViewEnd()
    {
        if (adjustPolicyData.adjustPolicyLabel != AdjustPolicyLabel.None)
            return;
        if (NetInfoMgr.Ins.ready && !string.IsNullOrEmpty(NetInfoMgr.Ins.NetServerData?.AdjustPolicy))
        {
            //��ȡ����������
            Log("��ȡ����������");
            isOnlineConfig = true;
            adjustPolicyConfig = JsonConvert.DeserializeObject<AdjustPolicyConfig>(NetInfoMgr.Ins.NetServerData.AdjustPolicy);
        }
        else
        {
            //û�ж�ȡ���������� ֱ�ӱ��Ϊ ��PolicyEnable��
            Log("û�ж�ȡ���������� ֱ�ӱ��Ϊ ��PolicyEnable��");
            adjustPolicyData.adjustPolicyLabel = AdjustPolicyLabel.PolicyEnable;
            adjustPolicyTrigger = AdjustPolicyTrigger.t0;
            SaveDataRefreshAdjustGoSendEvent();
        }
    }

    //������ OnGameStart
    bool _isOnGameStart = false;
    void OnGameStart()
    {
        if (_isOnGameStart)
            return;
        _isOnGameStart = true;
        if (adjustPolicyData.adjustPolicyLabel == AdjustPolicyLabel.None && !adjustPolicyConfig.strategySwitch)
        {
            Log("û�п������ԣ�ֱ�ӱ��Ϊ��PolicyEnable��");
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
        Log($"׼�����ͷ��ʹ��e4033");
        bool hasOnPolicyAKey = !string.IsNullOrEmpty(PlayerPrefs.GetString(OnPolicyAKey));
        if (!hasOnPolicyAKey)
        {
            int strategySwitchIndex = adjustPolicyConfig.strategySwitch ? 1 : 0;
            if (!isOnlineConfig)
            {
                //δ������������    
                strategySwitchIndex = 2;
            }
            Log($"���ʹ��e4033->{strategySwitchIndex}->{adjustPolicyData.adjustPolicyLabel}->{(int)adjustPolicyTrigger}");
            PostEventScript.Ins.SendEvent(PostEventType.e4033, strategySwitchIndex, (int)adjustPolicyData.adjustPolicyLabel, (int)adjustPolicyTrigger);
            PlayerPrefs.SetString(OnPolicyAKey, OnPolicyAKey);
        }
        else
        {
            Log($"֮ǰ���͹����e4033");
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
            //δ������������    
            strategySwitchIndex = 2;
        }
        Log($"���ʹ��,�Ƿ�����{strategySwitchIndex}��,����ֶΡ�{adjustPolicyData.adjustPolicyLabel}��,������ʽ{adjustPolicyTrigger}");
        PostEventScript.Ins.SendEvent(PostEventType.e4032, strategySwitchIndex, (int)adjustPolicyData.adjustPolicyLabel, (int)adjustPolicyTrigger);
    }

    void LoadAdjustPolicyData()
    {
        string data = PlayerPrefs.GetString(adjustPolicyDataField);
        if (string.IsNullOrEmpty(data))
        {
            Log("δ��ȡ����������");
            adjustPolicyData = new AdjustPolicyData();
        }
        else
        {
            adjustPolicyData = JsonConvert.DeserializeObject<AdjustPolicyData>(data);
            Log("��ȡ���������ݣ����Ϊ" + adjustPolicyData.adjustPolicyLabel.ToString());
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
    /// �����Ƿ����� ���������á����в�������    �����Բ����á�ֱ�ӳ�ʼ��adjust���ҽ���ұ��Ϊ��PolicyEnable��
    /// </summary>
    public bool strategySwitch = false;
    /// <summary>
    /// ��ʱʱ�� �� ��ʱֱ���ж����Ϊ��PolicyDisable��
    /// </summary>
    public float judgeBTimeOut = 3600;
    /// <summary>
    /// ��������A���ж�Ϊ��PolicyEnable���ļ��� �ٷ���30=30%
    /// </summary>
    public int passJudgeARate = 100;
    /// <summary>
    /// ��ʱ���ܱ��ж�Ϊ��PolicyEnable���ļ��� �ٷ���30=30%
    /// </summary>
    public int passJudgeBRate = 0;
}

public enum AdjustPolicyLabel
{
    None = 0,
    /// <summary>
    /// ��������
    /// </summary>
    PolicyEnable = 1,
    /// <summary>
    /// ���Բ����� �´ν��벻�жϺ�̨���ã�����adjust
    /// </summary>
    PolicyDisable = 2,
}

public class AdjustPolicyData
{
    public AdjustPolicyLabel adjustPolicyLabel = AdjustPolicyLabel.None;
}

/// <summary>
/// Adjust���Դ�����ʽ
/// </summary>
public enum AdjustPolicyTrigger
{
    /// <summary>
    /// δ�����������ã�ֱ����������
    /// </summary>
    t0 = 0,
    /// <summary>
    /// ����������ݣ���������
    /// </summary>
    t1 = 1,
    /// <summary>
    /// ����������ݣ�����������
    /// </summary>
    t2 = 2,
    /// <summary>
    /// �����¼�A����������
    /// </summary>
    t3 = 3,
    /// <summary>
    /// �����¼�A�����ǲ���������
    /// </summary>
    t4 = 4,
    /// <summary>
    /// �����¼�B��������������
    /// </summary>
    t5 = 5,
    /// <summary>
    /// �����¼�B������������
    /// </summary>
    t6 = 6,
    /// <summary>
    /// û���ҵ�adjust�ű�
    /// </summary>
    t7 = 7,
    /// <summary>
    /// û�п������ԣ���ʼ��
    /// </summary>
    t8 = 8
}