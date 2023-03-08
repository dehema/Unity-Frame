using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedeemUtil
{
    // 提现
    public static Dictionary<string, long> getMaxRank(Redeem[] list)
    {
        int maxRank = 0;
        long lastUpdateTime = DateUtil.Current();
        foreach (Redeem item in list)
        {
            if (item.rank > maxRank)
            {
                maxRank = item.rank;
                lastUpdateTime = item.lastUpdateRankTime;
            }
        }
        Dictionary<string, long> res = new Dictionary<string, long>();
        res.Add("maxRank", maxRank);
        res.Add("lastUpdateTime", lastUpdateTime);
        return res;
    }

    // 根据时间间隔，计算排名
    public static int randomRank(long seconds)
    {
        if (seconds < 10 * 60)
        {
            return Random.Range(5, 30);
        }
        else if (seconds < 3 * 60 * 60)
        {
            return Random.Range(100, 500);
        }
        else
        {
            return Random.Range(500, 1500);
        }
    }

    // 需要减少的排名
    public static int getDiffRank(int rank, long lastUpdateTime)
    {
        int diffRank = 0;
        long current = DateUtil.Current();
        float diffTime = (current - lastUpdateTime) / (60f * 60f); // 距离上次修改排名相差的小时数
        if (diffTime <= 0)
        {
            return 0;
        }

        while (diffTime > 0)
        {
            float delTime = 0;
            if (rank > 2000)
            {
                delTime = (rank - 2000) / 20;
                diffRank += (int)(Mathf.Min(delTime, diffTime) * 20);
            }
            else if (rank > 1500 && rank <= 2000)
            {
                delTime = (rank - 1500) / 15;
                diffRank += (int)(Mathf.Min(delTime, diffTime) * 15);
            }
            else if (rank > 1000 && rank <= 1500)
            {
                delTime = (rank - 1000) / 10;
                diffRank += (int)(Mathf.Min(delTime, diffTime) * 10);
            }
            else if (rank > 500 && rank <= 1000)
            {
                delTime = (rank - 500) / 5;
                diffRank += (int)(Mathf.Min(delTime, diffTime) * 5);
            }
            else
            {
                delTime = rank;
                diffRank += (int)(Mathf.Min(delTime, diffTime) * 1);
            }
            diffTime -= delTime;
        }

        return diffRank;
    }

    public static void updateWaitingRank(Redeem[] list)
    {
        long now = DateUtil.Current();
        // 1、找到排名最小的提现记录
        Redeem firstItem = null;
        foreach (Redeem item in list)
        {
            if (item.state == Redeem.RedeemState.Waiting)
            {
                if (firstItem == null || firstItem.rank > item.rank)
                {
                    firstItem = item;
                }
            }
        }

        if (firstItem == null)
        {
            return;
        }
        // 2、计算应减少的排名
        int diffRank = getDiffRank(firstItem.rank, firstItem.lastUpdateRankTime);
        if (diffRank > 0)
        {
            foreach (Redeem item in list)
            {
                if (item.state == Redeem.RedeemState.Waiting)
                {
                    item.rank = item.rank - diffRank;
                    item.lastUpdateRankTime = now;
                    if (item.rank <= 0)
                    {
                        item.state = Redeem.RedeemState.Complete;
                    }
                }
            }
        }
    }
}
