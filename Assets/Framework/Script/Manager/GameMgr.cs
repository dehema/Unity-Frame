﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Ins;
    public bool enterGame = false;

    private void Awake()
    {
        Ins = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        ConfigMgr.Ins.Init();
        ConfigMgr.Ins.LoadAllConfig();
        DataMgr.Ins.Load();
        LangMgr.Ins.Init();
        if (Application.isEditor)
        {
            Application.runInBackground = true;
        }
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
        SceneMgr.Ins.ChangeScene(SceneID.FPSDemo, () => { EnterGame(); });
    }

    /// <summary>
    /// 完成loading进入游戏
    /// </summary>
    public void EnterGame()
    {
        Debug.Log("-----------------------EnterGame-----------------------");
        enterGame = true;
        CheckFirstEnterGame();
        InitEnterGameTimer();
    }

    /// <summary>
    /// 数据初始化之后 首次进入游戏
    /// </summary>
    private void CheckFirstEnterGame()
    {
    }

    private void InitEnterGameTimer()
    {
        //每十秒保存数据 游戏时长
        //Timer.Ins.SetInterval(() =>
        //{
        //    DataMgr.Ins.gameData.lastOffLine = DateTime.Now;
        //    DataMgr.Ins.SaveGameData();
        //}, 10);
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && !Application.isEditor)
        {
            DataMgr.Ins.SaveGameData();
        }
    }
}
