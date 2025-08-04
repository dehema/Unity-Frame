namespace Rain.Core.RTS
{
    public class DieState : BaseState
    {
        public override UnitStateType stateType => UnitStateType.Die;
        public override void Enter(BattleUnit unit)
        {
            base.Enter(unit);
        }

        public override void Update()
        {
        }

        public override void Exit()
        {
        }

        public override void UpdateState()
        {
        }
    }
}
