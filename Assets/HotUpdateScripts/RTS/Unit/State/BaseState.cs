using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rain.Core.RTS
{
    public abstract class BaseState : IState
    {
        public BattleUnit unit;
        public abstract UnitStateType stateType { get; }

        public virtual void Enter(BattleUnit unit)
        {
            this.unit = unit;
            Debug.Log($"[ {unit.Data.Name} ] 进入【 {stateType} 】状态 ");
        }

        public abstract void Update();

        public virtual void Exit()
        {

            Debug.Log($"[ {unit.Data.Name} ] 退出【 {stateType} 】状态 ");
        }

        public abstract void UpdateState();
    }
}
