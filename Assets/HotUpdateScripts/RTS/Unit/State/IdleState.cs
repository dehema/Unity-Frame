using UnityEngine;

namespace Rain.RTS.Core
{
    public class IdleState : BaseState
    {
        public override UnitStateType stateType => UnitStateType.Idle;
        public override void Enter(BattleUnit unit)
        {
            base.Enter(unit);
        }

        public override void Update()
        {
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
