﻿using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.Core
{
    public class ConsoleLogView : LogViewBase
    {
        public Dropdown categoryDropdown = null;
        public Transform addonMenuRoot = null;
        public LogTypeView logTypeView = null;
        public LogList logList = null;
        public Text stackTrace = null;
        public Transform selectLogAddonMenuRoot = null;

        private int showLogCount = 0;
        private int categoryCount = 0;
        private bool showPlayTime = true;
        private bool showSceneName = true;
        private MultiLayout multiLayout = null;

        public interface IConsolMenuAddon
        {
            GameObject CreateUI(Transform parent);
        }

        public interface ISelectLogAddon
        {
            GameObject CreateUI(Transform parent, System.Func<string> getStackTrace);
        }


        public override void InitializeView()
        {
            multiLayout = GetComponent<MultiLayout>();
            logList.AddSelectCallback(UpdateStackTrace);

            selectLogAddonMenuRoot.gameObject.SetActive(false);

            ResetCategory();

            InitializeAddon();
        }

        public override void SetOrientation(ScreenOrientation orientation)
        {
            if (orientation == ScreenOrientation.Portrait || orientation == ScreenOrientation.PortraitUpsideDown)
            {
                multiLayout.SelectLayout(1);
            }
            else
            {
                multiLayout.SelectLayout(0);
            }

            logList.Resize();
        }

        public override void UpdateResolution()
        {
            logList.Resize();
        }

        public void SelectLogType(bool show)
        {
            logTypeView.SetTypeEnable(LogType.Log, show);
            Log.Ins.SetLogTypeEnable(LogType.Log, show);

            ResetShowLog();
        }

        public void SelectWarningType(bool show)
        {
            logTypeView.SetTypeEnable(LogType.Warning, show);
            Log.Ins.SetLogTypeEnable(LogType.Warning, show);

            ResetShowLog();
        }

        public void SelectErrorType(bool show)
        {
            logTypeView.SetTypeEnable(LogType.Error, show);
            Log.Ins.SetLogTypeEnable(LogType.Error, show);
            Log.Ins.SetLogTypeEnable(LogType.Exception, show);
            Log.Ins.SetLogTypeEnable(LogType.Assert, show);

            ResetShowLog();
        }

        public void SendMail()
        {
            WaitUi.Ins.ShowUi(true);
            WaitUi.Ins.SetMessage(ConsoleViewConst.SEND_MAIL_SENDING);
            
            Log.Ins.SendFullLogToMail((object sender, AsyncCompletedEventArgs e) =>
            {
                WaitUi.Ins.ShowUi(false);
                
                if (e.Cancelled == true)
                {
                    RLog.Log(ConsoleViewConst.SEND_MAIL_CANCELED);
                    Popup.Ins.ShowPopup(ConsoleViewConst.SEND_MAIL_CANCELED);
                }
                else if (e.Error != null)
                {
                    string message = string.Format(ConsoleViewConst.SEND_MAIL_FAILED, e.Error.Message);
                    RLog.LogError(message);
                    Popup.Ins.ShowPopup(message, "ERROR");
                }
                else
                {
                    RLog.Log(ConsoleViewConst.SEND_MAIL_SUCCEEDED);
                    Popup.Ins.ShowPopup(ConsoleViewConst.SEND_MAIL_SUCCEEDED);
                }
                
                logList.MoveToBottom();
            });
        }

        public void SelectCategory()
        {
            int index = categoryDropdown.value;
            if (index == 0)
            {
                Log.Ins.SetCurrentCategory(LogConst.DEFAULT_CATEGORY_NAME);
            }
            else
            {
                Log.Ins.SetCurrentCategory(index - 1);
            }

            ResetShowLog();
        }

        public void ChangeFilterIgnoreCase(bool ignore)
        {
            Log.Ins.SetFilterIgnoreCase(ignore);

            ResetShowLog();
        }

        public void ShowPlayTime(bool show)
        {
            showPlayTime = show;
            logList.ShowPlayTime(showPlayTime);
        }

        public void ShowSceneName(bool show)
        {
            showSceneName = show;
            logList.ShowSceneName(showSceneName);
        }

        public void SetFilter(string filter)
        {
            Log.Ins.SetFilter(filter);
            ResetShowLog();
        }

        public void Clear()
        {
            Log.Ins.ClearLog();
            ResetShowLog();
            ResetCategory();
        }

        private void Update()
        {
            UpdateShowLog();
            UpdateCategory();
            UpdateLogCount();
        }

        private void UpdateLogCount()
        {
            logTypeView.SetLogCount(LogType.Log, Log.Ins.GetLogCount(LogType.Log));
            logTypeView.SetLogCount(LogType.Warning, Log.Ins.GetLogCount(LogType.Warning));
            logTypeView.SetLogCount(LogType.Error,
                Log.Ins.GetLogCount(LogType.Error) + Log.Ins.GetLogCount(LogType.Exception) +
                Log.Ins.GetLogCount(LogType.Assert));
        }

        private void UpdateCategory()
        {
            string[] categories = Log.Ins.GetCategories();

            if (categoryCount - 1 < categories.Length)
            {
                for (int index = categoryCount - 1; index < categories.Length; ++index)
                {
                    AddCategory(categories[index]);
                }
            }
        }

        private void AddCategory(string category)
        {
            categoryDropdown.options.Add(new Dropdown.OptionData(category));
            categoryCount = categoryDropdown.options.Count;
        }

        private void UpdateShowLog()
        {
            List<Log.LogData> logs = Log.Ins.GetCurrentLogs();

            if (showLogCount < logs.Count)
            {
                for (int index = showLogCount; index < logs.Count; ++index)
                {
                    logList.Insert(logs[index], showPlayTime, showSceneName);
                    ++showLogCount;
                }
            }
        }

        private void UpdateStackTrace(Log.LogData data)
        {
            stackTrace.text = string.Format("{0}\n{1}", data.message, data.stackTrace);
            selectLogAddonMenuRoot.gameObject.SetActive(true);
        }

        private void ResetShowLog()
        {
            showLogCount = 0;
            logList.ClearList();
            stackTrace.text = string.Empty;
            selectLogAddonMenuRoot.gameObject.SetActive(false);
        }

        private void InitializeAddon()
        {
            var adapters = Function.Ins.GetAdapters();

            foreach (var adapter in adapters)
            {
                if (adapter is IConsolMenuAddon)
                {
                    var addOn = adapter as IConsolMenuAddon;

                    addOn.CreateUI(addonMenuRoot);
                }

                if (adapter is ISelectLogAddon)
                {
                    var addOn = adapter as ISelectLogAddon;

                    addOn.CreateUI(selectLogAddonMenuRoot, GetStackTrace);
                }
            }
        }

        private void ResetCategory()
        {
            categoryDropdown.options.Clear();
            AddCategory(LogConst.DEFAULT_CATEGORY_NAME);
        }

        public void ListMoveToBottom()
        {
            logList.MoveToBottom();
        }

        public string GetStackTrace()
        {
            return stackTrace.text;
        }

        public void CopyStackTrace()
        {
            GUIUtility.systemCopyBuffer = stackTrace.text;
            Popup.Ins.ShowPopup(ConsoleViewConst.STACKTRACE_COPY_MESSAGE, ConsoleViewConst.STACKTRACE_COPY_TITLE);
        }
    }
}