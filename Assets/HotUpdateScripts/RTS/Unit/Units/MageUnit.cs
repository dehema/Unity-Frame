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
        const string trackPrefabPath = "FX/LightGlow";
        const string hitEffectPrefabPath = "FX/Hit Fire";

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
            mageAttackStrategy.SetTrackPrefabPath(trackPrefabPath);
            mageAttackStrategy.SetHitEffectPrefabPath(hitEffectPrefabPath);
        }

        public override void PerformAttackOrder()
        {
            base.PerformAttackOrder();
            if (Data.attackTarget != null && !Data.attackTarget.IsDead && IsEnemy(Data.attackTarget))
            {
                mageAttackStrategy.Attack(this, Data.attackTarget, bodyPart.shootPos.transform.position);
            }
        }
    }
}
