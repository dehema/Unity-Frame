using System;

/// <summary>
/// 绿币提现记录
/// </summary>
public class CashRedeem : Redeem
{
    public long startTime;          // 本次提现开始时间
    public long endTime;            // 本次提现结束时间
    public int taskValue;           // 提现任务完成数量
}
