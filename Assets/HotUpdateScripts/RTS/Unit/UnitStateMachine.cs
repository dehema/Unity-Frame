//为什么不能继承？
//职责混淆
//状态机的核心功能是 "管理状态"，而IState的功能是 "实现具体状态行为"，继承会导致状态机同时承担两种不相关的职责。
//逻辑矛盾
//如果UnitStateMachine继承IState，它将需要实现Enter()/Update()/Exit()方法，但状态机本身并不对应任何具体状态，这些方法会变得毫无意义。
//扩展性问题
//当需要新增状态时，继承方式会迫使状态机修改自身逻辑，违反 "开闭原则"。

//┌─────────────┐       管理        ┌────────────┐
//│                          │  ───────>  │                        │
//│     UnitStateMachine     │                   │       IState(接口)     │               
//│                          │  <───────  │                        │
//└─────────────┘       被管理      └────────────┘
//                                                              ↑
//                                                             实现
//                             ┌─────────┐ ┌─────────┐ ┌─────────┐
//                             │   AttackState    │ │    MoveState     │ │ AttackState      │
//                             └─────────┘ └─────────┘ └─────────┘

using UnityEngine;

namespace Rain.RTS.Core
{
    public class UnitStateMachine
    {
        public BaseState currentState { get; private set; }
        private BaseBattleUnit _unit;

        public UnitStateMachine(BaseBattleUnit unit)
        {
            _unit = unit;
        }

        // 切换状态
        public void ChangeState(BaseState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter(_unit);
        }

        // 每帧更新状态
        public void Update()
        {
            currentState?.Update();
        }

        public void UpdateState()
        {
            currentState?.UpdateState();
        }
    }
}
