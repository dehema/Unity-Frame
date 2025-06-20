using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoMessageManager : MonoBehaviour
    {
        private object[] data = new object[] { 123123, "asdasd" };

        private void Awake()
        {
            RA.Message.AddEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            RA.Message.AddEventListener(10001, OnPlayerSpawned2, this);
        }

        private void Start()
        {
            RA.Message.DispatchEvent(MessageEvent.ApplicationFocus);
            RA.Message.DispatchEvent(10001, data);
            //全局时需要执行RemoveEventListener
            RA.Message.RemoveEventListener(MessageEvent.ApplicationFocus, OnPlayerSpawned, this);
            RA.Message.RemoveEventListener(10001, OnPlayerSpawned2, this);
        }

        private void OnPlayerSpawned()
        {
            RLog.Log("OnPlayerSpawned");
        }

        private void OnPlayerSpawned2(params object[] obj)
        {
            RLog.Log("OnPlayerSpawned2");
            if (obj is { Length: > 0 })
            {
                RLog.Log(obj[0]);
                RLog.Log(obj[1]);
            }
        }
    }
}
