using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rain.RTS.Core
{
    public abstract class BaseState : IState
    {
        public BaseBattleUnit unit;
        protected Animator animator;
        public abstract UnitStateType stateType { get; }

        public virtual void Enter(params object[] _param)
        {
            unit = _param[0] as BaseBattleUnit;
            animator = unit.animator;
            //Debug.Log($"[ {unit.Data.Name} ] 进入【 {stateType} 】状态 ");
        }

        public virtual void Update()
        {

        }

        public virtual void Exit()
        {
            //Debug.Log($"[ {unit.Data.Name} ] 退出【 {stateType} 】状态 ");
        }

        public abstract void UpdateState();
    }

    public abstract class IBaseStateParam
    {
        //public abstract BaseBattleUnit unit { get; set; }
    }
}
