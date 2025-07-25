using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Rain.Core
{
    public interface IModule
    {
        /// <summary>
        /// 创建模块
        /// </summary>
        void OnInit(System.Object createParam);

        /// <summary>
        /// 轮询模块
        /// </summary>
        void OnUpdate();

        void OnLateUpdate();

        void OnFixedUpdate();

        /// <summary>
        /// 销毁模块
        /// </summary>
        void OnTermination();

    }
}
