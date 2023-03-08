using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashRedeemManager : MonoBehaviour
{
    public static CashRedeemManager instance;
    
    public List<CashRedeem> CashRedeemList; // 提现记录

    private void Awake()
    {
        instance = this;
    }

    public void InitCashWithdraw()
    {
        // 提现列表
        if (CashRedeemList == null || CashRedeemList.Count == 0)
        {
            CashRedeemList = new List<CashRedeem>();
            string[] stringList = SaveDataManager.GetStringArray(SOHOShopConst.sv_CashWithdrawList);
            foreach (string item in stringList)
            {
                CashRedeemList.Add(JsonMapper.ToObject<CashRedeem>(item));
            }
        }

        updateCashWithdraw();
        updateWaitingRank();

    }

    // 检查更新绿币提现记录
    private void updateCashWithdraw()
    {
        DateTime now = DateTime.Now;
        long nowTicks = now.Ticks / 10000000;
        long[] timePoints = SOHODateUtil.StartAndEndPointTime(now);
        long startTime = CashRedeemList.Count == 0 ? nowTicks : timePoints[0];
        long endTime = startTime + SOHOShopDataManager.instance.shopJson.cash_withdraw_time;

        double cashBalance = SaveDataManager.GetDouble(SOHOShopConst.sv_CashWithdrawBalance);
        // 判断当前时间点是否已有记录
        bool flag = false;
        foreach(CashRedeem item in CashRedeemList)
        {
            if (item.state == Redeem.RedeemState.Init)
            {
                // 如果当前时间已经有记录，则跳过；否则修改已有记录状态
                if (item.startTime <= nowTicks && item.endTime > nowTicks)
                {
                    flag = true;
                    item.cashout = cashBalance;
                }
                else
                {
                    if (cashBalance == 0)
                    {
                        // 如果当前的记录余额为0，则重新开始倒计时
                        flag = true;
                        item.startTime = startTime;
                        item.endTime = endTime;
                    }
                    else
                    {
                        item.state = Redeem.RedeemState.Unchecked;
                        item.cashout = cashBalance;
                        cashBalance = 0;
                        SaveDataManager.SetString(SOHOShopConst.sv_CashWithdrawBalance, cashBalance.ToString());

                        // 打点
                        PostEventScript.GetInstance().SendEvent("1304", NumberUtil.DoubleToStr(item.cashout));
                    }
                }
            }
        }
        if (!flag)
        {
            // 创建新记录
            CashRedeem newCashWighdraw = new CashRedeem();
            newCashWighdraw.id = CashRedeemList.Count;
            newCashWighdraw.cashout = cashBalance;
            newCashWighdraw.state = Redeem.RedeemState.Init;
            newCashWighdraw.startTime = startTime;
            newCashWighdraw.endTime = endTime;
            newCashWighdraw.userAccount = SOHOShopDataManager.instance.currentUserAccount;
            CashRedeemList.Add(newCashWighdraw);
        }
        // 保存提现记录
        saveCashWitrdrawsList();

        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_RefreshCountdown);
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_ShowCashShopHand, new MessageData(false));
    }

    /// <summary>
    /// 保存提现记录
    /// </summary>
    private void saveCashWitrdrawsList()
    {
        List<string> strings = new List<string>();
        foreach(CashRedeem item in CashRedeemList)
        {
            strings.Add(JsonMapper.ToJson(item));
        }
        SaveDataManager.SetStringArray(SOHOShopConst.sv_CashWithdrawList, strings.ToArray());
        // 刷新列表
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_RefreshCashWithdrawList);
    }

    /// <summary>
    /// 增加现金余额
    /// </summary>
    /// <param name="num"></param>
    public void AddCashBalance(double num)
    {
        double balance = SaveDataManager.GetDouble(SOHOShopConst.sv_CashWithdrawBalance);
        balance += num;
        SaveDataManager.SetDouble(SOHOShopConst.sv_CashWithdrawBalance, balance);
        updateCashWithdraw();
    }

    /// <summary>
    /// 是否有未提现的记录
    /// </summary>
    /// <returns></returns>
    public bool HasUncheckedStateCashWithdraw()
    {
        foreach(CashRedeem item in CashRedeemList)
        {
            if (item.state == Redeem.RedeemState.Unchecked)
            {
                return true;
            }
        }
        return false;
    }

    // 完成一个任务
    public void AddTaskValue()
    {
        Dictionary<string, long> maxRankRes = RedeemUtil.getMaxRank(CashRedeemList.ToArray());
        int maxRank = (int)maxRankRes["maxRank"];
        long lastUpdateTime = maxRankRes["lastUpdateTime"];

        long now = DateUtil.Current();
        foreach (CashRedeem item in CashRedeemList)
        {
            if (item.state == Redeem.RedeemState.Checked)
            {
                item.taskValue++;
                // 如果该提现记录完成了所有任务，进入排队阶段
                if (item.taskValue >= SOHOShopConfig.TaskTotalNum)
                {
                    item.state = Redeem.RedeemState.Waiting;
                    maxRank = maxRank == 0 ? UnityEngine.Random.Range(2000, 4000) : maxRank + RedeemUtil.randomRank(now - lastUpdateTime);
                    lastUpdateTime = now;
                    item.rank = maxRank;
                    item.lastUpdateRankTime = now;

                    // 打点
                    PostEventScript.GetInstance().SendEvent("1306");
                }
            }
        }
        saveCashWitrdrawsList();
    }

    // 提现
    public void CheckCashWithdraw(int id)
    {
        if (SOHOShopDataManager.instance.currentUserAccount == null)
        {
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("WithdrawPanel"));
            return;
        }

        CashRedeem selectedCashWithdraw = null;
        foreach(CashRedeem item in CashRedeemList)
        {
            if (item.id == id)
            {
                selectedCashWithdraw = item;
                break;
            }
        }

        if (selectedCashWithdraw == null || selectedCashWithdraw.state != Redeem.RedeemState.Unchecked)
        {
            return;
        }
        // 修改记录状态
        selectedCashWithdraw.state = Redeem.RedeemState.Checked;
        selectedCashWithdraw.taskValue = 0;
        // 扣除游戏现金余额
        SOHOShopManager.Ins.subCashAction(selectedCashWithdraw.cashout);
        saveCashWitrdrawsList();
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_ShowCashShopHand, new MessageData(false));

        // 打点
        PostEventScript.GetInstance().SendEvent("1305");
    }

    // 到达可提现时间
    public void FinishInitCountDown(CashRedeem obj)
    {
        updateCashWithdraw();
    }

    // 每隔一段时间，减少提现排队当前排名
    public void updateWaitingRank()
    {
        RedeemUtil.updateWaitingRank(CashRedeemList.ToArray());
        saveCashWitrdrawsList();
    }


    /// <summary>
    /// 修改用户账户
    /// </summary>
    public void ChangeUserAccount()
    {
        foreach (CashRedeem item in CashRedeemList)
        {
            if ((item.userAccount == null || item.state == Redeem.RedeemState.Init || item.state == Redeem.RedeemState.Unchecked) 
                && SOHOShopDataManager.instance.currentUserAccount != null)
            {
                item.userAccount = SOHOShopDataManager.instance.currentUserAccount;
            }
        }
        // 保存提现记录
        saveCashWitrdrawsList();
        // 刷新页面
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_RefreshCashWithdrawList);
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_RefreshCashWithdrawUserAccount);
    }

    // 计算init状态的记录倒计时
    public long getCurrentCountDown() 
    {
        long countdown = 0;
        foreach(CashRedeem item in CashRedeemList)
        {
            if (item.state == Redeem.RedeemState.Init)
            {
                countdown = item.endTime - DateUtil.Current();
                break;
            }
        }

        return countdown;
    }

    // 是否有未点击提现的记录
    public bool hasUncheckedItem()
    {
        foreach(CashRedeem item in CashRedeemList)
        {
            if(item.state == Redeem.RedeemState.Unchecked)
            {
                return true;
            }
        }
        return false;
    }
}
