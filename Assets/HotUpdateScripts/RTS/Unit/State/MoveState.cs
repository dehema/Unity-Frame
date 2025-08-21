using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 移动状态机
    /// </summary>
    public class MoveState : BaseState
    {
        public override UnitStateType stateType => UnitStateType.Move;

        private int _speedHash = Animator.StringToHash("Speed");
        NavMeshAgent agent;
        MoveStateType moveStateType = MoveStateType.Move;

        // 构造函数初始化动画哈希和参数
        public MoveState()
        {
        }

        public override void Enter(BattleUnit unit)
        {
            base.Enter(unit);
            agent = unit.moveController.agent;

            // 进入移动状态时启动导航
            if (unit.MovePos != null)
            {
                unit.moveController.MoveTo((Vector3)unit.MovePos);
            }
            else if (unit.AttackTarget != null)
            {
                unit.moveController.MoveTo(unit.Data.AttackTarget);
                moveStateType = MoveStateType.MoveAndAttack;
            }

            // 计算动画速度并设置
            float animationSpeed = 1;
            animator.SetFloat(_speedHash, animationSpeed);
            agent.speed = unit.Data.moveSpeed;
        }

        public override void Update()
        {
            base.Update();
            if (agent.pathPending)
            {
                //等待路径计算完成
                return;
            }
            // 检查是否已到达目的地
            if (moveStateType == MoveStateType.Move)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    unit.stateMachine.ChangeState(new IdleState());
                    return;
                }
            }
            else if (moveStateType == MoveStateType.MoveAndAttack)
            {
                //到达攻击范围
                if (unit.IsTargetInAttackRange())
                {
                    unit.stateMachine.ChangeState(new AttackState());
                    return;
                }
                //到目的地但是敌人逃出攻击范围
                else if (agent.remainingDistance <= agent.stoppingDistance && !unit.IsTargetInAttackRange())
                {
                    if (unit.MovePos != null)
                    {
                        unit.moveController.MoveTo((Vector3)unit.MovePos);
                        return;
                    }
                }
            }
            // 面向移动方向
            RotateTowards(unit.transform, agent.velocity.normalized);
        }

        public override void Exit()
        {
            base.Exit();
            animator.SetFloat(_speedHash, 0);
            agent.ResetPath(); // 停止移动
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
            if (unit.MovePos != null)
            {
                unit.moveController.MoveTo((Vector3)unit.MovePos);
            }
        }

        public void MoveTo(Vector3 targetPos)
        {
            unit.moveController.MoveTo(targetPos);
        }
    }

    // 新增移动子状态枚举（区分走/跑）
    public enum MovementSubState
    {
        Walk,
        Run
    }

    public enum MoveStateType
    {
        Move,
        MoveAndAttack,
    }
}