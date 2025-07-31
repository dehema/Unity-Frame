using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rain.Core.RTS
{
    public abstract class BaseState : IState
    {
        public BattleUnit unit;

        public virtual void Enter(BattleUnit unit)
        {
            this.unit = unit;
        }

        public abstract void Update();

        public abstract void Exit();
    }
}
