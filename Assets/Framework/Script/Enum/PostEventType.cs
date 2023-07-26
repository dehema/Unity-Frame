
public enum PostEventType
{
    /// <summary>
    ///  启动游戏进入前台，每隔两分钟发送一次
    /// </summary>
    e2001 = 2001,
    /// <summary>
    /// 启动游戏（加载完成进入主界面）
    /// </summary>
    e1001 = 1001,
    /// <summary>
    /// 完成新手引导
    /// </summary>
    e1002 = 1002,
    /// <summary>
    /// 审核开关关闭
    /// </summary>
    e1003 = 1003,
    /// <summary>
    /// 每累积完成30次spin
    /// </summary>
    e4001 = 4001,
    /// <summary>
    /// 点击增加spin次数按钮
    /// </summary>
    e4002 = 4002,
    /// <summary>
    /// 点击spin次数中的奖励图标
    /// </summary>
    e4003 = 4003,
    /// <summary>
    /// spin结果有现金
    /// </summary>
    e4004 = 4004,
    /// <summary>
    /// spin结果有金币大奖
    /// </summary>
    e4005 = 4005,
    /// <summary>
    /// spin结果触发freespin模式
    /// </summary>
    e4006 = 4006,
    /// <summary>
    /// spin结果触发bonus
    /// </summary>
    e4007 = 4007,
    /// <summary>
    /// 点击左下角现金按钮
    /// </summary>
    e4008 = 4008,
    /// <summary>
    /// 触发autospin功能
    /// </summary>
    e4009 = 4009,
    /// <summary>
    /// 点击home按钮
    /// </summary>
    e4010 = 4010,
    /// <summary>
    /// 点击设置按钮
    /// </summary>
    e4011 = 4011,
    /// <summary>
    /// 点击金币按钮
    /// </summary>
    e4012 = 4012,
    /// <summary>
    /// 点击现金按钮
    /// </summary>
    e4013 = 4013,
    /// <summary>
    /// 点击解锁按钮
    /// </summary>
    e4014 = 4014,
    /// <summary>
    /// 点击机台说明
    /// </summary>
    e4015 = 4015,
    /// <summary>
    /// 点击激活的存钱罐
    /// </summary>
    e4016 = 4016,
    /// <summary>
    /// 点击限时额外图标
    /// </summary>
    e4017 = 4017,
    /// <summary>
    /// 点击luckybox按钮
    /// </summary>
    e4018 = 4018,
    /// <summary>
    /// Freespin结算奖励
    /// </summary>
    e4019 = 4019,
    /// <summary>
    /// 小游戏结算奖励
    /// </summary>
    e4020 = 4020,
    /// <summary>
    /// 解锁第二个机台
    /// </summary>
    e4021 = 4021,
    /// <summary>
    /// 解锁第三个机台
    /// </summary>
    e4022 = 4022,
    /// <summary>
    /// 后台切回前台
    /// </summary>
    e4023 = 4023,
    /// <summary>
    /// 成功提交提现信息
    /// </summary>
    e4024 = 4024,
    /// <summary>
    /// 完成提现任务
    /// </summary>
    e4025 = 4025,
    /// <summary>
    /// 完成提现等待
    /// </summary>
    e4026 = 4026,
    /// <summary>
    /// 完成提现加速
    /// </summary>
    e4027 = 4027,
    /// <summary>
    /// 获取ip（每日首次启动）
    /// </summary>
    e4028 = 4028,
    /// <summary>
    /// 获取语言（每日首次启动）
    /// </summary>
    e4029 = 4029,
    /// <summary>
    /// 评分弹窗
    /// </summary>
    e4030 = 4030,
    /// <summary>
    /// 触发disco机台boost累积满额效果
    /// </summary>
    e4031 = 4031,
    /// <summary>
    /// 开始播放激励视频
    /// </summary>
    e9001 = 9001,
    /// <summary>
    /// 激励视频播放成功
    /// </summary>
    e9002 = 9002,
    /// <summary>
    /// 激励视频播放失败
    /// </summary>
    e9003 = 9003,
    /// <summary>
    /// 开始播放插屏
    /// </summary>
    e9101 = 9101,
    /// <summary>
    /// 插屏播放成功
    /// </summary>
    e9102 = 9102,
    /// <summary>
    /// 插屏播放失败
    /// </summary>
    e9103 = 9103,
}
