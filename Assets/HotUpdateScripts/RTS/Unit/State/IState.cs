using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rain.Core.RTS
{
    public interface IState
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="unit"></param>
        void Enter(BattleUnit unit);

        /// <summary>
        /// 状态更新
        /// </summary>
        /// <param name="unit"></param>
        void Update();

        /// <summary>
        /// 退出状态
        /// </summary>
        /// <param name="unit"></param>
        void Exit();
    }
}
