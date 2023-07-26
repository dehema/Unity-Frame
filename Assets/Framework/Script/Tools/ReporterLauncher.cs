using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReporterLauncher : MonoBehaviour
{
    void Start()
    {
        //等网络数据返回
        Timer.Ins.SetTimeOut(() =>
        {
            bool enable = Utility.IsDebug && !Application.isEditor;
            if (enable)
            {
                Reporter reporter = gameObject.GetComponent<Reporter>();
                reporter.Initialize();
            }
            Debug.Log("ReporterLauncher启动" + enable);
        }, 5);
    }
}
