using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoF8Log : MonoBehaviour
    {
        void Start()
        {
            /*----------Log基础功能----------*/
            RLog.Log(this);
            RLog.Log("测试{0}", 1);
            RLog.Log("3123测试", this);
            RLog.LogNet("1123{0}", "测试");
            RLog.LogEvent(this);
            RLog.LogConfig(this);
            RLog.LogView(this);
            RLog.LogEntity(this);


            /*----------Log其他功能----------*/
            // 开启写入log文件
            RA.Log.OnEnterGame();
            // 开启捕获错误日志
            //RLog.GetCrashErrorMessage();
            //// 开始监视代码使用情况
            //RLog.Watch();
            //RLog.Log(RLog.UseMemory);
            //RLog.Log(RLog.UseTime);
        }
    }
}
