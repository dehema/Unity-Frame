using System;
using Rain.Core;
using UnityEngine;

namespace Rain.UI
{
    public class LogViewer : SingletonMono<LogViewer>
    {
        [Header("按键~号启用")] public bool keyCodeBackQuote = true;
        [Header("5指长按启用")] public bool gestureEnable = true;

        [Space(5)] [Header("发送邮件")]
        public MailData mailSetting = null;
        private Viewer viewer = null;

        protected override void Init()
        {
            Initialize();
        }

        public override void OnQuitGame()
        {
            Clear();
        }

        public void AddCheatKeyCallback(Action<string> callback)
        {
            Function.Ins.AddCheatKeyCallback(callback);
        }

        public void AddCommand(object instance, string methodName)
        {
            Function.Ins.AddCommand(instance, methodName);
        }

        public void Show()
        {
            viewer.Show();
        }

        public string MakeLogWithCategory(string message, string category)
        {
            return Log.Ins.MakeLogMessageWithCategory(message, category);
        }

        private void Initialize()
        {
            Function.Ins.Initialize();

            SetMailData();

            if (viewer == null)
            {
                viewer = transform.GetChild(0).GetComponent<Viewer>();
            }

            viewer.Initialize();
            SetGestureEnable();
            SetKeyCodeEnable();
        }

        private void Clear()
        {
            Function.Ins.Clear();
        }
        
        private void SetGestureEnable()
        {
            viewer.SetGestureEnable(gestureEnable);
        }
        
        private void SetKeyCodeEnable()
        {
            viewer.SetKeyCodeEnable(keyCodeBackQuote);
        }
        
        private void SetMailData()
        {
            Function.Ins.SetMailData(mailSetting);
        }
    }
}