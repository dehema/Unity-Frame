/**
 * 
 * 常量配置
 * 
 * 
 * **/
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CConfig
{
    #region 常量字段
    //登录url
    public const string LoginUrl = "/api/client/user/getId?gameCode=";
    //配置url
    public const string ConfigUrl = "/api/client/config?gameCode=";
    //时间戳url
    public const string TimeUrl = "/api/client/common/current_timestamp?gameCode=";
    #endregion

    #region 本地存储的字符串
    /// <summary>
    /// 本地用户id (string)
    /// </summary>
    public const string sv_LocalUserId = "sv_LocalUserId";
    /// <summary>
    /// 本地服务器id (string)
    /// </summary>
    public const string sv_LocalServerId = "sv_LocalServerId";
    /// <summary>
    /// 是否是新用户玩家 (bool)
    /// </summary>
    public const string sv_IsNewPlayer = "sv_IsNewPlayer";

    /// <summary>
    /// 签到次数 (int)
    /// </summary>
    public const string sv_DailyBounsGetCount = "sv_DailyBounsGetCount";
    /// <summary>
    /// 签到最后日期 (int)
    /// </summary>
    public const string sv_DailyBounsDate = "sv_DailyBounsDate";
    /// <summary>
    /// 新手引导完成的步数
    /// </summary>
    public const string sv_NewUserStep = "sv_NewUserStep";
    /// <summary>
    /// 金币余额
    /// </summary>
    public const string sv_GoldCoin = "sv_GoldCoin";
    /// <summary>
    /// 累计金币总数
    /// </summary>
    public const string sv_CumulativeGoldCoin = "sv_CumulativeGoldCoin";
    /// <summary>
    /// 钻石/现金余额
    /// </summary>
    public const string sv_Token = "sv_Token";
    /// <summary>
    /// 累计钻石/现金总数
    /// </summary>
    public const string sv_CumulativeToken = "sv_CumulativeToken";
    /// <summary>
    /// 钻石Amazon
    /// </summary>
    public const string sv_Amazon = "sv_Amazon";
    /// <summary>
    /// 累计Amazon总数
    /// </summary>
    public const string sv_CumulativeAmazon = "sv_CumulativeAmazon";
    /// <summary>
    /// 游戏总时长
    /// </summary>
    public const string sv_TotalGameTime = "sv_TotalGameTime";
    /// <summary>
    /// 第一次获得钻石奖励
    /// </summary>
    public const string sv_FirstGetToken = "sv_FirstGetToken";
    /// <summary>
    /// 是否已显示评级弹框
    /// </summary>
    public const string sv_HasShowRatePanel = "sv_HasShowRatePanel";
    /// <summary>
    /// 累计Roblox奖券总数
    /// </summary>
    public const string sv_CumulativeLottery = "sv_CumulativeLottery";
    /// <summary>
    /// 已经通过一次的关卡(int array)
    /// </summary>
    public const string sv_AlreadyPassLevels = "sv_AlreadyPassLevels";
    /// <summary>
    /// 新手引导
    /// </summary>
    public const string sv_NewUserStepFinish = "sv_NewUserStepFinish";
    public const string sv_task_level_count = "sv_task_level_count";
    // 是否第一次使用过slot
    public const string sv_FirstSlot = "sv_FirstSlot";
    /// <summary>
    /// adjust adid
    /// </summary>
    public const string sv_AdjustAdid = "sv_AdjustAdid";

    #endregion

    #region 监听发送的消息

    /// <summary>
    /// 有窗口打开
    /// </summary>
    public static string mg_WindowOpen = "mg_WindowOpen";
    /// <summary>
    /// 窗口关闭
    /// </summary>
    public static string mg_WindowClose = "mg_WindowClose";
    /// <summary>
    /// 关卡结算时传值
    /// </summary>
    public static string mg_ui_levelcomplete = "mg_ui_levelcomplete";
    /// <summary>
    /// 增加金币
    /// </summary>
    public static string mg_ui_addgold = "mg_ui_addgold";
    /// <summary>
    /// 增加钻石/现金
    /// </summary>
    public static string mg_ui_addtoken = "mg_ui_addtoken";
    /// <summary>
    /// 增加amazon
    /// </summary>
    public static string mg_ui_addamazon = "mg_ui_addamazon";

    /// <summary>
    /// 游戏暂停/继续
    /// </summary>
    public static string mg_GameSuspend = "mg_GameSuspend";

    #endregion

    #region 动态加载资源的路径

    // 金币图片
    public static string path_GoldCoin_Sprite = "Art/Tex/UI/jiangli1";
    // 钻石图片
    public static string path_Token_Sprite_Dimond = "Art/Tex/UI/jiangli4";

    #endregion
}

