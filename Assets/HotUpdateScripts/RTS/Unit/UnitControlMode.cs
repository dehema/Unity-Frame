namespace Rain.RTS.Core
{
    /// <summary>
    /// 单位控制模式
    /// </summary>
    public enum UnitControlMode
    {
        /// <summary>
        /// 手动控制 - 需要玩家点击才会有行为
        /// </summary>
        Manual,
        
        /// <summary>
        /// 自动控制 - 在idle状态下会自动寻找最近的敌人攻击
        /// </summary>
        Auto
    }
}
