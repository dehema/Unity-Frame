using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 攻击策略接口，用于定义不同兵种的攻击行为
    /// </summary>
    public interface IAttackStrategy
    {
        /// <summary>
        /// 执行攻击
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="target">目标</param>
        void PerformAttack(BaseBattleUnit attacker, BaseBattleUnit target);
    }
}
