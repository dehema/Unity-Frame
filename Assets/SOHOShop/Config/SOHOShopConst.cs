using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHOShopConst
{
    #region 本地存储的字符串
    /// <summary>
    /// 碎片
    /// </summary>
    public const string sv_Puzzle = "sv_Puzzle";
    /// <summary>
    /// 金币 Amazon
    /// </summary>
    public const string sv_GoldAmazonWithdraw = "sv_GoldAmazonWithdraw";
    /// <summary>
    /// 现金提现记录
    /// </summary>
    public const string sv_CashWithdrawList = "sv_CashWithdrawList";
    /// <summary>
    /// 当前现金提现累计余额
    /// </summary>
    public const string sv_CashWithdrawBalance = "sv_CashWithdrawBalance";
    /// <summary>
    /// 用户账户
    /// </summary>
    public const string sv_UserAccount = "sv_UserAccount";
    public const string sv_PersonName = "sv_PersonName";
    public const string sv_PersonAddress = "sv_PersonAddress";
    public const string sv_PersonTel = "sv_PersonTel";
    /// <summary>
    /// 游戏初次激活时间
    /// </summary>
    public const string sv_InitSeconds = "sv_InitSeconds";
    #endregion

    #region 监听发送的消息
    /// <summary>
    /// 刷新现金提现列表
    /// </summary>
    public const string mg_RefreshCashWithdrawList = "mg_RefreshCashWithdrawList";
    /// <summary>
    /// 刷新现金提现用户账户
    /// </summary>
    public const string mg_RefreshCashWithdrawUserAccount = "mg_RefreshCashWithdrawUserAccount";
    /// <summary>
    /// 展示/收起提现详情时，修改高度
    /// </summary>
    public const string mg_RefreshCashWithdrawHeight = "mg_RefreshCashWithdrawHeight";
    /// <summary>
    /// 刷新金币、Amazon列表
    /// </summary>
    public const string mg_RefreshGoldAmazonWithdrawList = "mg_RefreshGoldAmazonWithdrawList";
    /// <summary>
    /// 刷新现金提现倒计时
    /// </summary>
    public const string mg_RefreshCountdown = "mg_RefreshCountdown";
    /// <summary>
    /// 现金商店引导
    /// </summary>
    public static string mg_ShowCashShopHand = "mg_ShowCashShopHand";

    /// <summary>
    /// 历史记录
    /// </summary>
    public static string mg_SOHORecordList = "mg_SOHORecordList";


    #endregion
}
