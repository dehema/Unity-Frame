using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHODateUtil
{
    
    public static long[] StartAndEndPointTime(DateTime now)
    {
        long nowTicks = DateUtil.TicksToSeconds(now.Ticks);  // 当前时间
        long startTicks = DateUtil.TicksToSeconds(DateTime.Today.Ticks); // 当日零点时间

        int initSeconds = (int)(nowTicks - startTicks);
        if (PlayerPrefs.HasKey(SOHOShopConst.sv_InitSeconds))
        {
            initSeconds = SaveDataManager.GetInt(SOHOShopConst.sv_InitSeconds);
        }
        else
        {
            SaveDataManager.SetInt(SOHOShopConst.sv_InitSeconds, initSeconds);
        }
        
        
        DateTime today = DateTime.Today.AddDays(-1).AddSeconds(initSeconds);
        startTicks = DateUtil.TicksToSeconds(today.Ticks);
        
        int interval = SOHOShopDataManager.instance.shopJson.cash_withdraw_time;
        int pointCount = 2 * 24 * 60 * 60 / interval;
        for (int i = 0; i < pointCount; i++)
        {
            if (nowTicks >= startTicks + interval * i && nowTicks < startTicks + interval * (i + 1))
            {
                return new long[] { startTicks + interval * i, startTicks + interval * (i + 1) };
            }
        }
        return new long[] { startTicks, startTicks + interval };
    }

    public static long[] StartAndEndPointTimeOfNow()
    {
        return StartAndEndPointTime(DateTime.Now);
    }

}
