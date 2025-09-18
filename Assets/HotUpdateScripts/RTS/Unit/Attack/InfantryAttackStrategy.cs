using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 近战攻击策略，适用于步兵等近战单位
    /// </summary>
    public class InfantryAttackStrategy : IAttackStrategy
    {
        public void Attack(BaseBattleUnit _attacker, BaseBattleUnit _target)
        {
            // 近战攻击逻辑 - 直接造成伤害
            if (_target != null && !_target.IsDead)
            {
                float hurtVal = _attacker.Data.attack * _attacker.Data.damageFactor;
                _target.Hurt(hurtVal);
                //Debug.Log($"[{_attacker.UnitName}] 近战攻击了 [{_target.UnitName}]，造成 [{hurtVal}] 点伤害 (攻击力加成x{_attacker.Data.damageFactor})");
            }
        }
    }
}
