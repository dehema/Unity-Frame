using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldRedeemManager : MonoBehaviour
{
    public static GoldRedeemManager instance;

    public GoldRedeem[] goldRedeemGroup;

    private void Awake()
    {
        instance = this;
    }

    public void initGoldAmazonRedeemList()
    {
        string[] withdrawList = SaveDataManager.GetStringArray(SOHOShopConst.sv_GoldAmazonWithdraw);
        if (withdrawList.Length == 0)
        {
            goldRedeemGroup = SOHOShopDataManager.instance.shopJson.withdraw_group;
            for (int i = 0; i < withdrawList.Length; i++)
            {
                goldRedeemGroup[i].state = Redeem.RedeemState.Init;
                goldRedeemGroup[i].id = i + 1;
            }
        }
        else
        {
            goldRedeemGroup = new GoldRedeem[withdrawList.Length];
            for (int i = 0; i < withdrawList.Length; i++)
            {
                GoldRedeem item = JsonMapper.ToObject<GoldRedeem>(withdrawList[i]);
                if (item.id == 0)
                {
                    item.id = i + 1;
                }
                goldRedeemGroup[i] = item;

            }
        }

        updateWaitingRank();
    }

    // 金币、Amazon提现
    public void CashOutGoldRedeem(int id)
    {
        int index = -1;
        for (int i = 0; i < goldRedeemGroup.Length; i++)
        {
            GoldRedeem item = goldRedeemGroup[i];
            if (item.id == id)
            {
                index = i;
                item.state = Redeem.RedeemState.Waiting;
                if (item.type == "gold")
                {
                    SOHOShopManager.Ins.subGoldAction(item.num);
                }
                else
                {
                    SOHOShopManager.Ins.subAmazonAction(item.num);
                }
            }
        }

        Dictionary<string, long> maxRankRes = RedeemUtil.getMaxRank(goldRedeemGroup);
        int maxRank = (int)maxRankRes["maxRank"];
        long lastUpdateTime = maxRankRes["lastUpdateTime"];
        long now = DateUtil.Current();
        maxRank = maxRank == 0 ? Random.Range(2000, 4000) : maxRank + RedeemUtil.randomRank(now - lastUpdateTime);
        goldRedeemGroup[index].rank = maxRank;
        goldRedeemGroup[index].lastUpdateRankTime = now;

        saveGoldRedeemList();
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_RefreshGoldAmazonWithdrawList);
    }
    

    private void saveGoldRedeemList()
    {
        List<string> strings = new List<string>();
        foreach (GoldRedeem item in goldRedeemGroup)
        {
            strings.Add(JsonMapper.ToJson(item));
        }
        SaveDataManager.SetStringArray(SOHOShopConst.sv_GoldAmazonWithdraw, strings.ToArray());
    }

    // 修改提现排名
    public void updateWaitingRank()
    {
        RedeemUtil.updateWaitingRank(goldRedeemGroup);
        saveGoldRedeemList();
    }

    // 修改用户提现账户
    public void ChangeUserAccount()
    {
        foreach (GoldRedeem item in goldRedeemGroup)
        {
            if (item.state == Redeem.RedeemState.Init)
            {
                item.userAccount = SOHOShopDataManager.instance.currentUserAccount;
            }
        }
        saveGoldRedeemList();
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_RefreshGoldAmazonWithdrawList);
    }
}
