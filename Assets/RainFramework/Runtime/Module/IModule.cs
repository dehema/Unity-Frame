using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace F8Framework.Core
{
    public interface IModule
    {
        /// <summary>
        /// ����ģ��
        /// </summary>
        void OnInit(System.Object createParam);

        /// <summary>
        /// ��ѯģ��
        /// </summary>
        void OnUpdate();

        void OnLateUpdate();

        void OnFixedUpdate();

        /// <summary>
        /// ����ģ��
        /// </summary>
        void OnTermination();

    }
}
