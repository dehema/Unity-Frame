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
        ArrowAttackStrategy arrowAttackStrategy;


        protected override void Awake()
        {
            base.Awake();
        }

        public override void Init()
        {
            base.Init();
        }

        protected override void InitAttackStrategy()
        {
            // 弓箭手使用远程攻击策略，传入箭矢预制体
            attackStrategy = new ArrowAttackStrategy();
            arrowAttackStrategy = attackStrategy as ArrowAttackStrategy;
            arrowAttackStrategy.SetProjectilePrefabPath(arrowPrefabPath);
        }

        public override void PerformAttackOrder()
        {
            base.PerformAttackOrder();
            if (Data.attackTarget != null && !Data.attackTarget.IsDead && IsEnemy(Data.attackTarget))
            {
                arrowAttackStrategy.Attack(this, Data.attackTarget, bodyPart.shootPos.transform.position);
            }
        }

        protected override void Update()
        {
            base.Update();
        }
    }
}
