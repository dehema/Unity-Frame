namespace Rain.RTS.Core
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
            base.Exit();
        }

        public override void UpdateState()
        {
        }
    }
}
