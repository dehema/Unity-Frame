using Rain.Core;
using UnityEngine;
using UnityEngine.AI;


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
        // 移动速度系数
        float moveSpeedFactor = 1;
        // 移动速度系数最大值
        float moveSpeedFactorMax = 1;
        // 移动动画系数
        float moveAnimationFactor = 1;

        // 构造函数初始化动画哈希和参数
        public MoveState()
        {
        }

        public override void Enter(BaseBattleUnit unit, params object[] _param)
        {
            base.Enter(unit, _param);
            agent = unit.moveController.navAgent;

            // 进入移动状态时启动导航
            if (unit.MovePos != null)
            {
                MoveTo((Vector3)unit.MovePos);
            }
            else if (unit.AttackTarget != null)
            {
                MoveAndAttack(unit.Data.attackTarget);
            }

            // 计算动画速度并设置

            if (unit.Data.unitConfig.UnitType == UnitType.Cavalry)
            {
                moveSpeedFactor = 0.4f;
                moveAnimationFactor = 1.2f;
            }
            else
            {
                moveSpeedFactor = 1;
            }
            animator.SetFloat(_speedHash, moveSpeedFactor);
            agent.speed = unit.Data.moveSpeed * moveSpeedFactor;
        }

        const float SetAttackNavTargetInterval = 0.5f;  //设置攻击寻路目标的间隔
        float _tempAttackNavTarget = 0f;                //设置攻击寻路目标的计时
        public override void Update()
        {
            base.Update();
            if (unit.Data.unitConfig.UnitType == UnitType.Cavalry)
            {
                if (moveSpeedFactor < moveSpeedFactorMax)
                {
                    moveSpeedFactor += 0.4f * Time.deltaTime;
                    moveSpeedFactor = Mathf.Clamp(moveSpeedFactor, 0, moveSpeedFactorMax);
                    animator.SetFloat(_speedHash, moveSpeedFactor / moveSpeedFactorMax * moveAnimationFactor);
                    agent.speed = unit.Data.moveSpeed * moveSpeedFactor;
                }
            }
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
                if (!unit.AttackTarget.IsAlive())
                {
                    unit.AutoFindAndAttackNearestEnemy();
                    return;
                }
                //到达攻击范围
                if (unit.IsTargetInAttackRange())
                {
                    Attack();
                    return;
                }
                else
                {
                    //不在攻击范围内  且敌人在移动
                    if (unit.AttackTarget.IsMoveThisFrame)
                    {
                        _tempAttackNavTarget += Time.deltaTime;
                        if (_tempAttackNavTarget >= SetAttackNavTargetInterval)
                        {
                            unit.moveController.MoveToAttack(unit.AttackTarget);
                            _tempAttackNavTarget = 0;
                        }
                    }
                }
            }
            // 面向移动方向
            RotateTowards(unit.transform, agent.velocity.normalized);
        }

        void Attack()
        {
            AttackStateParam attackStateParam = new AttackStateParam();
            if (unit.Data.unitConfig.UnitType == UnitType.Cavalry)
            {
                attackStateParam.attackFactor = 1 + moveSpeedFactor;
            }
            else
            {
                attackStateParam.attackFactor = moveSpeedFactor;
            }
            unit.stateMachine.ChangeState(new AttackState(), attackStateParam);
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

        /// <summary>
        /// 重新设置移动目标
        /// </summary>
        /// <param name="targetPos"></param>
        public void MoveTo(Vector3 targetPos)
        {
            moveStateType = MoveStateType.Move;
            unit.moveController.MoveTo(targetPos);
        }

        /// <summary>
        /// 设置攻击目标
        /// </summary>
        public void MoveAndAttack(BaseBattleUnit _target)
        {
            moveStateType = MoveStateType.MoveAndAttack;
            unit.moveController.MoveToAttack(_target);
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