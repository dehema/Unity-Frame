using System.Collections;
using System.Collections.Generic;
using F8Framework.Core;
using UnityEngine;

namespace F8Framework.Launcher
{
    public static class FF8
    {
        // ȫ����Ϣ
        private static MessageManager _message;

        // ��Ϸʱ�����-->ʹ������Ϣģ��
        private static TimerManager _timer;


        public static MessageManager Message
        {
            get
            {
                if (_message == null)
                    _message = ModuleCenter.CreateModule<MessageManager>();
                return _message;
            }
            set
            {
                if (_message == null)
                    _message = value;
            }
        }

        public static TimerManager Timer
        {
            get
            {
                if (_timer == null)
                    _timer = ModuleCenter.CreateModule<TimerManager>();
                return _timer;
            }
            set
            {
                if (_timer == null)
                    _timer = value;
            }
        }
    }
}
