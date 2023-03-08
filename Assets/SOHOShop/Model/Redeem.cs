using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redeem
{
    // 提现状态 初始化、未提现、任务、排队、完成
    public enum RedeemState { Init, Unchecked, Checked, Waiting, Complete };

    public int id;
    public double cashout;          // 提现金额
    public RedeemState state;     // 状态
    public int rank;                // 排队顺序
    public long lastUpdateRankTime; // 上次修改排队时间
    public UserAccount userAccount; // 用户账户
}
