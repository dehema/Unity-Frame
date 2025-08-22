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
        public override void Enter(BaseBattleUnit unit)
        {
            base.Enter(unit);
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
                animator.SetBool(_isAttackingHash, true);
                isStartAttack = true;
                return;
            }
        }

        public override void Exit()
        {
            animator.SetBool(_isAttackingHash, false);
            base.Exit();
        }

        public override void UpdateState()
        {
        }
    }
}
