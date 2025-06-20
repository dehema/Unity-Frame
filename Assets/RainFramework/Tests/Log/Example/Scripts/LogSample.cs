using System.Collections;
using Rain.Core;
using UnityEngine;

namespace Rain.Tests
{
    public class LogSample : MonoBehaviour
    {
        private void Start()
        {
            Function.Instance.AddCommand(this, "TestLog", new object[] { 2 });
            Function.Instance.AddCommand(this, "TestCommand", new object[] { "by command text" });
            Function.Instance.AddCommand(this, "TestCategory", new object[] { "category1", "category2" });
            Function.Instance.AddCommand(this, "SendExceptionLog");
            Function.Instance.AddCommand(this, "SendAssertLog");
            Function.Instance.AddCommand(this, "SendErrorLog");
            Function.Instance.AddCommand(this, "SendWarningLog");
            Function.Instance.AddCheatKeyCallback((cheatKey) =>
            {
                RLog.Log("Call cheat key callback with : " + cheatKey);
            });

            StartCoroutine(SendLog());
        }

        private IEnumerator SendLog()
        {
            int count = 0;
            while (true)
            {
                yield return new WaitForSeconds(5.0f);

                RLog.Log("***** Sample Send Log : " + count++.ToString());
            }
        }

        public void SendThreadLog()
        {
            for (int index = 0; index < 10; index++)
            {
                System.Threading.Thread t =
                    new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadProc));
                t.Start(index);
            }
        }

        public static void ThreadProc(object index)
        {
            throw new System.Exception("thread : " + index);
        }

        private void TestLog(int index)
        {
            RLog.Log("*TestLog() : " + index.ToString());
            RLog.Log("category1 log");

            RLog.Log(LogViewer.Instance.MakeLogWithCategory("Test message with category", "TestCategory"));
            RLog.Log("$(category)TempCategory$(category");
            RLog.Log("$(category)TempCategory$(category)");
            RLog.Log("$(category)TempCategory$(");
            RLog.Log("$(category)TempCategory$(category) Test");

            RLog.Log(SystemInformation.Instance.ToString());
        }

        private void TestCommand(string text)
        {
            RLog.Log("TestCommand : " + text);
        }

        private void TestCategory(string category1, string category2)
        {
            RLog.Log(LogViewer.Instance.MakeLogWithCategory("Log with category(" + category1 + ")", category1));
            RLog.Log(LogViewer.Instance.MakeLogWithCategory("Log with category(" + category2 + ")", category2));
        }

        public void SendExceptionLog()
        {
            RLog.LogException(new System.Exception("Exception log"));
        }

        private void SendAssertLog()
        {
            RLog.LogAssertion("Assert log");
        }

        private void SendErrorLog()
        {
            RLog.LogError("Error log");
        }

        private void SendWarningLog()
        {
            RLog.LogWarning("Warning log");
        }
    }
}