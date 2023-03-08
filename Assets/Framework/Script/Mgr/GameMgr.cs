using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static MaxSdkCallbacks;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Ins;
    public bool enterGame = false;

    private void Awake()
    {
        Ins = this;
    }

    private void Start()
    {
        ConfigMgr.Ins.Init();
        ConfigMgr.Ins.LoadAllConfig();
        LangMgr.Ins.Init();
        DataMgr.Ins.Load();
        if (Application.isEditor)
        {
            UIMgr.Ins.OpenView<DebugView>();
        }
        else
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        LoadingViewParams viewParams = new LoadingViewParams();
        viewParams.CloseCB = () => { EnterGame(); };
        UIMgr.Ins.OpenView<LoadingView>(viewParams);
        InitSOHOShop();
    }

    /// <summary>
    /// 完成loading进入游戏
    /// </summary>
    public void EnterGame()
    {
        Debug.Log("-----------------------EnterGame-----------------------");
        AdjustPolicyMgr.Ins.OnGameStartDelegate();
        enterGame = true;
        CheckFirstEnterGame();
        ADMgr.Ins.OnGameEnter();
        InitEnterGameTimer();
        PostEventScript.Ins.SendEvent(PostEventType.e1001);
        UIMgr.Ins.OpenView<TopView>();
    }

    /// <summary>
    /// 数据初始化之后 首次进入游戏
    /// </summary>
    private void CheckFirstEnterGame()
    {
        if (DataMgr.Ins.gameData.enterGameTime == 0)
        {
        }
        DataMgr.Ins.gameData.enterGameTime++;
    }

    private void InitSOHOShop()
    {
        // 提现商店初始化
        // 提现商店中的金币、现金和amazon卡均为double类型，参数请根据具体项目自行处理
        SOHOShopManager.Ins.InitSOHOShopAction(
            () => { return DataMgr.Ins.playerData.cash.Value; },
            () => { return DataMgr.Ins.playerData.gold.Value; },
            () => { return DataMgr.Ins.playerData.amazon.Value; },
            (_cash) => { DataMgr.Ins.playerData.cash.Value -= (float)_cash; },
            (_gold) => { DataMgr.Ins.playerData.gold.Value -= (float)_gold; },
            (_amazon) => { DataMgr.Ins.playerData.amazon.Value -= (float)_amazon; }
        );
    }

    private void InitEnterGameTimer()
    {
        //每十秒保存数据 游戏时长
        Timer.Ins.SetInterval(() =>
        {
            DataMgr.Ins.gameData.lastOffLine = DateTime.Now;
            DataMgr.Ins.SaveGameData();
        }, 10);
        Timer.Ins.SetInterval(() =>
        {
            PostEventScript.Ins.SendEvent(PostEventType.e2001, DataMgr.Ins.playerData.gold.Value, DataMgr.Ins.playerData.cash.Value, (int)Time.time);
        }, 120);
        //间隔时间后播放插屏
        if (!Application.isEditor)
        {
            Timer.Ins.SetInterval(() =>
            {
                ADMgr.Ins.ShowInterStitialAd((300).ToString());
            }, int.Parse(NetInfoMgr.Ins.NetServerData.relax_interval));
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && !Application.isEditor)
        {
            DataMgr.Ins.gameData.gamePauseNum++;
            if (DataMgr.Ins.gameData.gamePauseNum == int.Parse(NetInfoMgr.Ins.NetServerData.inter_b2f_count))
            {
                DataMgr.Ins.gameData.gamePauseNum = 0;
                ADMgr.Ins.ShowInterStitialAd(((int)ADInterstitalType.applicationPause).ToString());
            }
            DataMgr.Ins.SaveGameData();
        }
    }
}
