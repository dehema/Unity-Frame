using UnityEngine;
using UnityEngine.AI;

namespace Rain.Core.RTS
{
    public class MoveState : BaseState
    {
        private int _speedHash = Animator.StringToHash("Speed");
        private int _isAttackingHash;
        private float _runAnimationSpeed;
        MovementSubState moveState;

        // 构造函数初始化动画哈希和参数
        public MoveState()
        {
            _isAttackingHash = Animator.StringToHash("IsAttacking");
        }

        public override void Enter(BattleUnit unit)
        {
            base.Enter(unit);
            // 进入移动状态时启动导航
            unit.moveController.MoveTo(unit.moveController.MoveTarget);
            Debug.Log($"{unit.Data.Name} 进入移动状态");
        }

        public override void Update()
        {
            NavMeshAgent agent = unit.moveController.agent;
            Animator animator = unit.animator;

            // 检查是否已到达目的地
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 切换到空闲状态
                unit.stateMachine.ChangeState(new IdleState());
                animator.SetFloat(_speedHash, 0);
                return;
            }

            // 计算动画速度并设置
            float animationSpeed = GetAnimationSpeed(agent.remainingDistance);
            animator.SetFloat(_speedHash, animationSpeed);

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

            // 切换走/跑状态（通过修改移动速度实现）
            if (moveState == MovementSubState.Walk && animationSpeed > _runAnimationSpeed)
            {
                moveState = MovementSubState.Run;
                agent.speed = unit.Data.runSpeed;
            }
            else if (moveState == MovementSubState.Run && animationSpeed <= _runAnimationSpeed)
            {
                moveState = MovementSubState.Walk;
                agent.speed = unit.Data.moveSpeed;
            }
        }

        public override void Exit()
        {
            // 退出移动状态时的清理
            Debug.Log($"{unit.Data.Name} 退出移动状态");
        }

        // 计算动画速度（复用原有逻辑）
        private float GetAnimationSpeed(float remainingDistance)
        {
            // 这里复用你原有的动画速度计算逻辑
            // 示例：根据剩余距离返回0-1之间的动画速度值
            return Mathf.Clamp01(1 - (remainingDistance / 10f));
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
    }

    // 新增移动子状态枚举（区分走/跑）
    public enum MovementSubState
    {
        Walk,
        Run
    }
}