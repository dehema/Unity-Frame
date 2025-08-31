using System;
using Rain.Core;
using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 攻击状态机
    /// </summary>
    public class AttackState : BaseState
    {
        Animator animator;
        private int _isAttackingHash = Animator.StringToHash("IsAttacking");
        public override UnitStateType stateType => UnitStateType.Attack;
        AnimatorStateInfo stateInfo;
        bool isStartAttack = false;     // 是否开始攻击
        AttackStateParam attackStateParam;

        public override void Enter(BaseBattleUnit unit, params object[] _param)
        {
            base.Enter(unit, _param);
            if (_param.Length > 0)
            {
                attackStateParam = _param[0] as AttackStateParam;
            }
            animator = unit.animator;

            unit.moveController.Stop();
        }

        public override void Update()
        {
            base.Update();
            // 面向目标
            unit.LookAtTarget();

            //  攻击动画切换完毕后并切换到Idle
            if (isStartAttack)
            {
                stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.normalizedTime >= 1f && stateInfo.shortNameHash == Animator.StringToHash("Attack"))
                {
                    unit.stateMachine.ChangeState(new IdleState());
                }
                return;
            }
            else
            {
                // 如果没有攻击目标，进入空闲状态
                // 如果目标已经死亡
                // 如果对方不是敌人
                if (unit.Data.AttackTarget == null || !unit.Data.AttackTarget.IsAlive() || !unit.IsEnemy(unit.Data.AttackTarget))
                {
                    unit.ClearAttackTarget();
                    unit.stateMachine.ChangeState(new IdleState());
                }

                //  如果不在攻击范围内则继续追击
                if (!unit.IsTargetInAttackRange())
                {
                    unit.stateMachine.ChangeState(new MoveState());
                }
            }

            // 是否可以攻击
            if (!isStartAttack && unit.CanAttack())
            {
                Attack();
                return;
            }
        }

        // 触发攻击函数计时器ID
        int attackTimerID;
        void Attack()
        {
            animator.SetBool(_isAttackingHash, true);
            isStartAttack = true;

            // 设置攻击伤害系数
            if (attackStateParam != null)
            {
                unit.SetDamageFactor(attackStateParam.attackFactor);
            }
            else
            {
                unit.SetDamageFactor();
            }


            Action action = () =>
            {
                unit.PerformAttackOrder();
            };
            attackTimerID = TimerMgr.Ins.AddTimer(this, delay: GetAttackDelay(), onComplete: action);
        }

        float GetAttackDelay()
        {
            switch (unit.Data.unitType)
            {
                case UnitType.Infantry:
                default:
                    return 0.15f;
                case UnitType.Archer:
                    return 0.14f;
                case UnitType.Cavalry:
                    return 0.12f;
            }
        }

        public override void Exit()
        {
            base.Exit();
            animator.SetBool(_isAttackingHash, false);
            TimerMgr.Ins.RemoveTimer(attackTimerID);
        }

        public override void UpdateState()
        {
        }
    }

    /// <summary>
    /// 攻击状态参数
    /// </summary>
    public class AttackStateParam : IBaseStateParam
    {
        //public AttackStateParam(BaseBattleUnit _unit)
        //{
        //    this.unit = _unit;
        //}

        //private BaseBattleUnit _unit;

        //public override BaseBattleUnit unit
        //{
        //    get => _unit;
        //    set => _unit = value;
        //}

        /// <summary>
        /// 攻击力系数
        /// </summary>
        public float attackFactor = 1;
    }
}
