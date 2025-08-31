using System.Collections;
using System.Collections.Generic;
using Rain.RTS.Core;
using UnityEngine;

/// <summary>
/// 骑兵单位类，使用近战攻击策略
/// </summary>
public class CavalryUnit : BaseBattleUnit
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
        // 步兵使用近战攻击策略
        attackStrategy = new InfantryAttackStrategy();
        infantryAttackStrategy = attackStrategy as InfantryAttackStrategy;
    }

    public override void PerformAttackOrder()
    {
        base.PerformAttackOrder();
        if (Data.AttackTarget != null && !Data.AttackTarget.IsDead && IsEnemy(Data.AttackTarget))
        {
            infantryAttackStrategy.Attack(this, Data.AttackTarget);
        }
    }
}
