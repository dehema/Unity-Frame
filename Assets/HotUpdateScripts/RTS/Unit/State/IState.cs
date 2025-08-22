using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rain.RTS.Core
{
    public interface IState
    {
        /// <summary>
        /// 进入状态
        /// </summary>
        /// <param name="unit"></param>
        void Enter(BaseBattleUnit unit);

        /// <summary>
        /// 每帧更新
        /// </summary>
        /// <param name="unit"></param>
        void Update();

        /// <summary>
        /// 退出状态
        /// </summary>
        /// <param name="unit"></param>
        void Exit();

        /// <summary>
        /// 更新状态逻辑
        /// </summary>
        void UpdateState();
    }
}
