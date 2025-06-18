using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Rain.Core
{
    public static class RLog
    {
        private static StringBuilder sb = new StringBuilder();

        // ��ɫ��
        private static readonly Color color1 = new Color(.14f, .65f, 1.00f);
        private static readonly Color color2 = new Color(.89f, .12f, .12f);
        private static readonly Color color3 = new Color(.09f, .85f, .43f);
        private static readonly Color color4 = new Color(.80f, .24f, 1.00f);
        private static readonly Color color5 = new Color(1.00f, .79f, .0f);
        private static readonly Color color6 = new Color(.09f, .80f, .85f);
        private static readonly Color color7 = new Color(.66f, .85f, .09f);
        private static readonly Color color8 = new Color(1.0f, 0.63f, 0.66f);
        private static readonly Color color9 = new Color(1.0f, 0.62f, 0.35f);
        private static readonly Color color10 = new Color(0.86f, 0.55f, 0.92f);
        private static readonly Color color11 = new Color(1.0f, 0.90f, 0.40f);
        private static readonly Color color12 = new Color(1.0f, 0.9215f, 0.0156f);
        private static readonly Color color13 = new Color(0.0f, 1.0f, 1.0f);
        private static readonly Color color14 = new Color(0.0f, 1.0f, 0.0f);

        public static void EnabledLog()
        {
            UnityEngine.Debug.unityLogger.logEnabled = true;
            Debug.unityLogger.filterLogType = LogType.Error | LogType.Assert | LogType.Warning | LogType.Log | LogType.Exception;
        }

        public static void DisableLog()
        {
            UnityEngine.Debug.unityLogger.logEnabled = false;
            Debug.unityLogger.filterLogType = LogType.Error;
        }

        public static void Log(string s, Object context)
        {
            sb.Clear();
            sb.Append(s);

            Debug.Log(sb.ToString(), context);
        }

        public static void Log(string s, params object[] p)
        {
            sb.Clear();
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);

            Debug.Log(sb.ToString());
        }

        public static void Log(object o)
        {
            Debug.Log(o);
        }

        public static void LogUtil(string s, Object context)
        {
            LogColor("[������־]", color10, s, context);
        }

        public static void LogUtil(string s, params object[] p)
        {
            LogColor("[������־]", color10, s, p);
        }

        public static void LogUtil(object o)
        {
            LogColor("[������־]", color10, o);
        }

        public static void LogVersion(string s, Object context)
        {
            LogColor("[�汾��־]", color9, s, context);
        }

        public static void LogVersion(string s, params object[] p)
        {
            LogColor("[�汾��־]", color9, s, p);
        }

        public static void LogVersion(object o)
        {
            LogColor("[�汾��־]", color9, o);
        }

        public static void LogSDK(string s, Object context)
        {
            LogColor("[SDK��־]", color8, s, context);
        }

        public static void LogSDK(string s, params object[] p)
        {
            LogColor("[SDK��־]", color8, s, p);
        }

        public static void LogSDK(object o)
        {
            LogColor("[SDK��־]", color8, o);
        }

        public static void LogModule(string s, Object context)
        {
            LogColor("[ģ����־]", color1, s, context);
        }

        public static void LogModule(string s, params object[] p)
        {
            LogColor("[ģ����־]", color1, s, p);
        }

        public static void LogModule(object o)
        {
            LogColor("[ģ����־]", color1, o);
        }

        public static void LogNet(string s, Object context)
        {
            LogColor("[������־]", color5, s, context);
        }

        public static void LogNet(string s, params object[] p)
        {
            LogColor("[������־]", color5, s, p);
        }

        public static void LogNet(object o)
        {
            LogColor("[������־]", color5, o);
        }

        public static void LogConfig(string s, Object context)
        {
            LogColor("[������־]", color7, s, context);
        }

        public static void LogConfig(string s, params object[] p)
        {
            LogColor("[������־]", color7, s, p);
        }

        public static void LogConfig(object o)
        {
            LogColor("[������־]", color7, o);
        }

        public static void LogView(string s, Object context)
        {
            LogColor("[��ͼ��־]", color12, s, context);
        }

        public static void LogView(string s, params object[] p)
        {
            LogColor("[��ͼ��־]", color12, s, p);
        }

        public static void LogView(object o)
        {
            LogColor("[��ͼ��־]", color12, o);
        }

        public static void LogEvent(string s, Object context)
        {
            LogColor("[�¼���־]", color13, s, context);
        }

        public static void LogEvent(string s, params object[] p)
        {
            LogColor("[�¼���־]", color13, s, p);
        }

        public static void LogEvent(object o)
        {
            LogColor("[�¼���־]", color13, o);
        }

        public static void LogEntity(string s, Object context)
        {
            LogColor("[ʵ����־]", color3, s, context);
        }

        public static void LogEntity(string s, params object[] p)
        {
            LogColor("[ʵ����־]", color3, s, p);
        }

        public static void LogEntity(object o)
        {
            LogColor("[ʵ����־]", color3, o);
        }

        public static void LogAsset(string s, Object context)
        {
            LogColor("[�ʲ���־]", color14, s, context);
        }

        public static void LogAsset(string s, params object[] p)
        {
            LogColor("[�ʲ���־]", color14, s, p);
        }

        public static void LogAsset(object o)
        {
            LogColor("[�ʲ���־]", color14, o);
        }

        private static void LogColor(string logName, Color color, string s, Object context)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append(logName);
            sb.Append("</color>");
            sb.Append(s);
            Debug.Log(sb.ToString(), context);
        }

        private static void LogColor(string logName, Color color, string s, params object[] p)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append(logName);
            sb.Append("</color>");
            if (p != null && p.Length > 0)
                sb.AppendFormat(s, p);
            else
                sb.Append(s);
            Debug.Log(sb.ToString());
        }

        private static void LogColor(string logName, Color color, object o)
        {
            sb.Clear();
            sb = sb.AppendFormat(@"<color=#{0}>", ColorUtility.ToHtmlStringRGB(color));
            sb.Append(DateTime.Now);
            sb.Append(logName);
            sb.Append("</color>");
            sb.Append(o);
            Debug.Log(sb.ToString());
        }

        public static void LogException(Exception exception, Object context)
        {
            Debug.LogException(exception, context);
        }

        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        public static void LogAssertion(string s, Object context)
        {
            Debug.LogAssertion(s, context);
        }

        public static void LogAssertion(string s, params object[] p)
        {
            Debug.LogAssertion((p != null && p.Length > 0 ? string.Format(s, p) : s));
        }

        public static void LogAssertion(object o)
        {
            Debug.LogAssertion(o);
        }

        public static void LogWarning(string s, Object context)
        {
            Debug.LogWarning(s, context);
        }

        public static void LogWarning(string s, params object[] p)
        {
            Debug.LogWarning((p != null && p.Length > 0 ? string.Format(s, p) : s));
        }

        public static void LogWarning(object o)
        {
            Debug.LogWarning(o);
        }

        public static void LogError(string s, Object context)
        {
            Debug.LogError(s, context);
        }

        public static void LogError(string s, params object[] p)
        {
            Debug.LogError((p != null && p.Length > 0 ? string.Format(s, p) : s));
        }

        public static void LogError(object o)
        {
            Debug.LogError(o);
        }
    }

}
