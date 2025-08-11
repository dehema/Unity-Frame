
using UnityEngine;

namespace Rain.Core.RTS
{
    public class HurtState : BaseState
    {
        public override UnitStateType stateType => UnitStateType.Hurt;
        private float _hurtDuration; // 受伤硬直时长
        private float _enterTime;    // 进入状态的时间
        private BaseState _previousState; // 受伤前的状态

        // 构造函数，接收受伤硬直时长和之前的状态
        public HurtState(float duration, BaseState previousState)
        {
            _hurtDuration = duration;
            _previousState = previousState;
        }

        public override void Enter(BattleUnit unit)
        {
            base.Enter(unit);
            _enterTime = Time.time;
            // 播放受伤动画
            unit.PlayHurtAnimation();
            // 受伤时停止移动
            unit.moveController.Stop();
            Debug.Log($"{unit.Data.Name} 进入受伤状态");
        }

        public override void Update()
        {
            // 受伤硬直结束后返回之前的状态
            if (Time.time - _enterTime >= _hurtDuration)
            {
                // 如果之前的状态是死亡状态，保持死亡
                if (_previousState is DieState)
                {
                    unit.stateMachine.ChangeState(_previousState);
                }
                // 如果目标仍在攻击范围内，返回攻击状态
                else if (unit.Data.AttackTarget != null && unit.IsTargetInAttackRange())
                {
                    unit.stateMachine.ChangeState(new AttackState());
                }
                // 如果有移动目标，返回移动状态
                else if (Vector3.Distance(unit.transform.position, unit.attackTarget.transform.position) > unit.Data.attackRange)
                {
                    unit.stateMachine.ChangeState(new MoveState());
                }
                // 否则返回空闲状态
                else
                {
                    unit.stateMachine.ChangeState(_previousState ?? new IdleState());
                }
            }
        }

        public override void Exit()
        {
            Debug.Log($"{unit.Data.Name} 退出受伤状态");
        }

        public override void UpdateState()
        {
        }
    }
}
