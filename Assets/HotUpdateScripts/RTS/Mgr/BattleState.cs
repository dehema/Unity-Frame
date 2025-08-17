namespace Rain.RTS.Core
{
    /// <summary>
    /// 战斗阶段
    /// </summary>
    public enum BattleState
    {
        Prepare,        // 准备阶段
        InProgress,     // 战斗进行中
        Paused,         // 暂停状态
        Victory,        // 胜利
        Defeat          // 失败
    }
}
