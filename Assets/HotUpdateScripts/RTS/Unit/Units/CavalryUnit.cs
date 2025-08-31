using System.Collections;
using System.Collections.Generic;
using Rain.RTS.Core;
using UnityEngine;

/// <summary>
/// �����λ�࣬ʹ�ý�ս��������
/// </summary>
public class CavalryUnit : BaseBattleUnit
{
    [Header("������������")]
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
        // ����ʹ�ý�ս��������
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
