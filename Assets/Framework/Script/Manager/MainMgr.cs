using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMgr : MonoBehaviour
{
    public static MainMgr Ins;

    private void Awake()
    {
        Ins = this;
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60;
    }

    void Start()
    {
    }

    public void GameInit()
    {
        AudioMgr.Ins.PlayMusic(AudioMusic.Sound_BGM);
    }
}
