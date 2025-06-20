using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoF8Timer : MonoBehaviour
    {
        void Start()
        {
            // 普通Timer,传入自身this，每1秒执行一次，延迟0秒后开始，执行3次(-1表示循环)
            int timeid = RA.Timer.AddTimer(this, 1f, 0f, 3, () => { RLog.Log("tick"); }, () => { RLog.Log("完成"); });

            // FrameTimer,传入自身this，每1帧执行一次，延迟0帧后开始，循环执行(-1表示循环)
            timeid = RA.Timer.AddTimerFrame(this, 1f, 0f, -1, () => { RLog.Log("tick"); },
                () => { RLog.Log("完成"); });

            RA.Timer.RemoveTimer(timeid); // 停止名为timeid的Timer

            // 监听游戏程序获得或失去焦点，重新开始或暂停所有Timer
            RA.Timer.AddListenerApplicationFocus();

            // 手动重新开始或暂停所有Timer，或指定id
            RA.Timer.Pause();
            RA.Timer.Resume();
            // 重新启动所有Timer，或指定id
            RA.Timer.Restart();

            RA.Timer.SetServerTime(1702573904000); // 网络游戏，与服务器对表，单位ms
            RA.Timer.GetServerTime();

            RA.Timer.GetTime(); // 获取游戏中的总时长
        }
    }
}
