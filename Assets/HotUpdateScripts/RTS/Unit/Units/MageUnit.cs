using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 步兵单位类，使用近战攻击策略
    /// </summary>
    public class MageUnit : BaseBattleUnit
    {
        [Header("步兵特有属性")]
        MageAttackStrategy mageAttackStrategy;

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
            attackStrategy = new MageAttackStrategy();
            mageAttackStrategy = attackStrategy as MageAttackStrategy;
        }

        public override void Attack()
        {
            base.Attack();
            if (Data.AttackTarget != null && !Data.AttackTarget.IsDead && IsEnemy(Data.AttackTarget))
            {
                mageAttackStrategy.Attack(this, Data.AttackTarget, bodyPart.shootPos.transform.position);
            }
        }
    }
}
