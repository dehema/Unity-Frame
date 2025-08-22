using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 弓箭手单位类，使用远程攻击策略
    /// </summary>
    public class ArcherUnit : BaseBattleUnit
    {
        [Header("弓箭手特有属性")]
        const string arrowPrefabPath = "Prefab/Weapon/Arrow";


        protected override void Awake()
        {
            base.Awake();
        }

        protected override void InitAttackStrategy()
        {
            // 弓箭手使用远程攻击策略，传入箭矢预制体
            attackStrategy = new ArrowAttackStrategy();
            attackStrategy.SetProjectilePrefabPath(arrowPrefabPath);
        }

        public override void Attack()
        {
            if (Data.AttackTarget != null && !Data.AttackTarget.IsDead && IsEnemy(Data.AttackTarget))
            {
                attackStrategy.PerformAttack(this, Data.AttackTarget);
            }
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
