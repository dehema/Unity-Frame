using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 近战攻击策略，适用于步兵等近战单位
    /// </summary>
    public class InfantryAttackStrategy : IAttackStrategy
    {
        public void PerformAttack(BaseBattleUnit attacker, BaseBattleUnit target)
        {
            // 近战攻击逻辑 - 直接造成伤害
            if (target != null && !target.IsDead)
            {
                target.Hurt(attacker.Data.attack);
                Debug.Log($"{attacker.UnitName} 近战攻击了 {target.UnitName}，造成 {attacker.Data.attack} 点伤害");
            }
        }
    }
}
