using UnityEngine;

namespace Rain.Core.RTS
{
    public class AttackState : BaseState
    {
        Animator animator;
        private int _isAttackingHash = Animator.StringToHash("IsAttacking");
        public override UnitStateType stateType => UnitStateType.Attack;
        private float _lastAttackTime;
        public override void Enter(BattleUnit unit)
        {
            base.Enter(unit);
            animator = unit.animator;

            unit.moveController.Stop();
            _lastAttackTime = Time.time - unit.Data.attackInterval;
        }

        public override void Update()
        {

            // 如果没有攻击目标，进入空闲状态
            if (unit.Data.attackTarget == null || !unit.Data.attackTarget.IsAlive() || !unit.IsEnemy(unit.Data.attackTarget))
            {
                unit.ClearAttackTarget();
                unit.stateMachine.ChangeState(new IdleState());
                return;
            }

            if (!unit.IsTargetInAttackRange())
            {
                unit.stateMachine.ChangeState(new MoveState());
                return;
            }

            if (unit.CanAttack())
            {
                animator.SetBool(_isAttackingHash, true);
                return;
            }

            // 有移动目标时切换到移动状态
            if (unit.HasMoveTarget)
            {
                unit.stateMachine.ChangeState(new MoveState());
                return;
            }

            // 检查是否在攻击范围内
            float distanceToTarget = Vector3.Distance(
                unit.transform.position,
                unit.Data.attackTarget.transform.position
            );

            if (distanceToTarget > unit.Data.attackRange)
            {
                unit.Data.attackTarget = null;
                unit.stateMachine.ChangeState(new IdleState());
                return;
            }

            // 有移动目标时切换到移动状态
            if (unit.HasMoveTarget)
            {
                unit.stateMachine.ChangeState(new MoveState());
                return;
            }

            // 目标超出攻击范围时进入空闲状态
            if (!unit.IsTargetInAttackRange())
            {
                unit.Data.attackTarget = null;
                unit.stateMachine.ChangeState(new IdleState());
                return;
            }

            // 面向目标
            unit.LookAtTarget();

            // 检查攻击间隔
            if (Time.time - _lastAttackTime >= unit.Data.attackInterval)
            {
                unit.AttackTarget();
                _lastAttackTime = Time.time;
            }
        }

        public override void Exit()
        {
            Debug.Log($"{unit.Data.Name} 退出攻击状态");
        }

        public override void UpdateState()
        {
        }
    }
}
