using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMgr : MonoBehaviour
{
    public static MainMgr ins;

    private void Awake()
    {
        ins = this;
        Application.targetFrameRate = 60;
        //屏幕不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (Application.isEditor)
        {
            Application.runInBackground = true;
        }
    }

    void Start()
    {
        PostEventScript.GetInstance().SendEvent("1001");
        StartCoroutine(saveGameTime());
    }

    public void GameInit()
    {
        AudioMgr.Ins.PlayMusic(AudioMusic.Sound_BGM);
    }

    // 每秒保存一次游戏时间
    private IEnumerator saveGameTime()
    {
        float interval = 1f;
        while (true)
        {
            yield return new WaitForSeconds(interval);
            // 保存游戏时间
            float totalGameTime = SaveDataManager.GetFloat(CConfig.sv_TotalGameTime);
            SaveDataManager.SetFloat(CConfig.sv_TotalGameTime, totalGameTime + interval);
        }
    }
}
