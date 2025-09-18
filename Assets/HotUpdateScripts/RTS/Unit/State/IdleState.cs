using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 空闲状态机
    /// </summary>
    public class IdleState : BaseState
    {
        public override UnitStateType stateType => UnitStateType.Idle;

        public override void Enter(BaseBattleUnit unit, params object[] _param)
        {
            base.Enter(unit, _param);
        }

        public override void Update()
        {
            base.Update();
            // 检查是否需要移动
            if (unit.agent.remainingDistance > unit.agent.stoppingDistance)
            {
                unit.stateMachine.ChangeState(new MoveState());
                return;
            }
            // 检查是否可以攻击
            else if (unit.CanAttack())
            {
                unit.stateMachine.ChangeState(new AttackState());
                return;
            }

            // 自动控制模式下，寻找最近的敌人
            if (unit.ControlMode == UnitControlMode.Auto)
            {
                unit.AutoFindAndAttackNearestEnemy();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void UpdateState()
        {
        }
    }
}
