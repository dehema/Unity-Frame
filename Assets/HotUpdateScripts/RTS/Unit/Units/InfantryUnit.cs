using System;
using Rain.Core;
using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 步兵单位类，使用近战攻击策略
    /// </summary>
    public class InfantryUnit : BaseBattleUnit
    {
        [Header("步兵特有属性")]
        InfantryAttackStrategy infantryAttackStrategy;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void InitAttackStrategy()
        {
            attackStrategy = new InfantryAttackStrategy();
            infantryAttackStrategy = attackStrategy as InfantryAttackStrategy;
        }

        public override void PerformAttackOrder()
        {
            base.PerformAttackOrder();
            if (Data.attackTarget != null && !Data.attackTarget.IsDead && IsEnemy(Data.attackTarget))
            {
                infantryAttackStrategy.Attack(this, Data.attackTarget);
            }
        }
    }
}
