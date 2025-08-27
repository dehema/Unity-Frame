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
