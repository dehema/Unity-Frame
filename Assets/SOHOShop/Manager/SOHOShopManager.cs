using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHOShopManager : MonoBehaviour
{
    public static SOHOShopManager Ins;

    public Func<double> getCashAction;
    public Func<double> getGoldAction;
    public Func<double> getAmazonAction;
    public Action<double> subCashAction;
    public Action<double> subGoldAction;
    public Action<double> subAmazonAction;

    private bool isReady;

    private void Awake()
    {
        Ins = this;

        isReady = false;
    }

    private void Start()
    {
        InitSOHOShop();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    // 增加看广告次数
        //    AddTaskValue();
        //}
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && isReady)
        {
            // 进入前台，重新开始倒计时
            CashRedeemManager.instance.InitCashWithdraw();
        }
    }

    private void InitSOHOShop()
    {
        SOHOShopDataManager.instance.InitShopData();

        isReady = true;
    }

    public void InitSOHOShopAction(Func<double> _getCashAction, Func<double> _getGoldAction, Func<double> _getAmazonAction, 
        Action<double> _subCashAction, Action<double> _subGoldAction, Action<double> _subAmazonAction)
    {
        getCashAction = _getCashAction;
        getGoldAction = _getGoldAction;
        getAmazonAction = _getAmazonAction;
        subCashAction = _subCashAction;
        subGoldAction = _subGoldAction;
        subAmazonAction = _subAmazonAction;
    }

    /// <summary>
    /// 增加现金
    /// </summary>
    /// <param name="num"></param>
    public void AddCash(double num)
    {
        if (num > 0)
        {
            CashRedeemManager.instance.AddCashBalance(num);
        }
    }


    /// <summary>
    /// 完成提现任务
    /// </summary>
    /// <param name="taskId"></param>
    public void AddTaskValue()
    {
        CashRedeemManager.instance.AddTaskValue();
    }

    /// <summary>
    /// 打开提现商店panel
    /// </summary>
    public void ShowRedeemPanel()
    {
        UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("RedeemPanel"));

        // 打点
        //PostEventScript.GetInstance().SendEvent("1301", NumberUtil.DoubleToStr(getCashAction()));
    }

    /// <summary>
    /// 金币/Amazon提现Panel
    /// </summary>
    public void ShowGoldAmazonRedeemPanel()
    {
        UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("GoldAmazonRedeemPanel"));

        // 打点
        //PostEventScript.GetInstance().SendEvent("1302", NumberUtil.DoubleToStr(getGoldAction()), NumberUtil.DoubleToStr(getAmazonAction()));
    }

    /// <summary>
    /// 碎片兑换panel
    /// </summary>
    public void ShowRedeemGiftPanel()
    {
        UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("RedeemGiftPanel"));

        // 打点
        PostEventScript.GetInstance().SendEvent("1303");
    }

    /// <summary>
    /// 随机获得一个碎片奖励
    /// </summary>
    /// <returns></returns>
    public Puzzle GetRewardPuzzle()
    {
        return SOHOPuzzleManager.instance.getUnachievedPuzzle();
    }

    /// <summary>
    /// 获取碎片
    /// </summary>
    public void AddRewardPuzzle(Puzzle puzzle)
    {
        // 领取碎片动画
        SOHOPuzzleManager.instance.addPuzzle(puzzle);
    }

    /// <summary>
    /// 现金提现倒计时
    /// </summary>
    /// <returns></returns>
    public long GetCashWithdrawCountDown()
    {
        return isReady ? CashRedeemManager.instance.getCurrentCountDown() : 0;
    }

    /// <summary>
    /// 是否有未提现的现金提现记录
    /// </summary>
    /// <returns></returns>
    public bool HasUnchckedCashWithdraw()
    {
        return CashRedeemManager.instance.hasUncheckedItem();
    }

    /// <summary>
    /// 倒计时结束时，修改提现状态
    /// </summary>
    public void CashWithdrawFinishCountdown()
    {
        CashRedeemManager.instance.FinishInitCountDown(null);
    }
}
