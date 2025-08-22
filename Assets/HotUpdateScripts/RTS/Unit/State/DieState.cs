using UnityEngine;

namespace Rain.RTS.Core
{
    /// <summary>
    /// 死亡状态机
    /// </summary>
    public class DieState : BaseState
    {
        public override UnitStateType stateType => UnitStateType.Die;
        int _deadHash = Animator.StringToHash("IsDead");
        public override void Enter(BaseBattleUnit unit)
        {
            base.Enter(unit);
            unit.Dead();
            animator.SetTrigger(_deadHash);
        }

        public override void Update()
        {
            base.Update();
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
