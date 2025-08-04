using UnityEngine;
using UnityEngine.AI;

namespace Rain.Core.RTS
{
    public class MoveState : BaseState
    {
        public override UnitStateType stateType => UnitStateType.Move;

        private int _speedHash = Animator.StringToHash("Speed");
        private int _isAttackingHash;
        MovementSubState moveState;
        NavMeshAgent agent;
        Animator animator;

        // 构造函数初始化动画哈希和参数
        public MoveState()
        {
            _isAttackingHash = Animator.StringToHash("IsAttacking");
        }

        public override void Enter(BattleUnit unit)
        {
            base.Enter(unit);
            agent = unit.moveController.agent;
            animator = unit.animator;
            // 进入移动状态时启动导航
            unit.moveController.MoveTo(unit.MoveTarget);
        }

        public override void Update()
        {
            // 检查是否已到达目的地
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 切换到空闲状态
                unit.stateMachine.ChangeState(new IdleState());
                animator.SetFloat(_speedHash, 0);
                return;
            }

            // 计算动画速度并设置
            float animationSpeed = 1;
            animator.SetFloat(_speedHash, animationSpeed);
            agent.speed = unit.Data.moveSpeed;

            // 面向移动方向
            RotateTowards(unit.transform, agent.velocity.normalized);

            // 检查是否可以攻击
            if (unit.CanAttack())
            {
                // 切换到攻击状态
                unit.stateMachine.ChangeState(new AttackState());
                animator.SetBool(_isAttackingHash, true);
                agent.ResetPath(); // 停止移动
                return;
            }
        }

        public override void Exit()
        {
            // 退出移动状态时的清理
            Debug.Log($"{unit.Data.Name} 退出移动状态");
        }

        // 面向移动方向（复用原有逻辑）
        private void RotateTowards(Transform transform, Vector3 direction)
        {
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * 10f
                );
            }
        }

        public override void UpdateState()
        {
            unit.moveController.MoveTo(unit.MoveTarget);
        }
    }

    // 新增移动子状态枚举（区分走/跑）
    public enum MovementSubState
    {
        Walk,
        Run
    }
}