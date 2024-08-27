using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoSingleton<SceneMgr>
{
    //Ҫ�л��ĳ�������
    string targetSceneName;
    SceneID currSceneID;
    /// <summary>
    /// �л�����
    /// </summary>
    /// <param name="_sceneID"></param>
    public void ChangeScene(SceneID _sceneID, Action _changeSuccess = null)
    {
        LoadSceneViewParams viewParams = new LoadSceneViewParams();
        viewParams.targetSceneName = _sceneID.ToString();
        targetSceneName = viewParams.targetSceneName;
        viewParams.CloseCB = () =>
        {
            currSceneID = _sceneID;
            _changeSuccess?.Invoke();
            Debug.Log("�л�������" + viewParams.targetSceneName);
            OnSceneChangeComplete();
        };
        OnSceneStartChange();
        UIMgr.Ins.OpenView<LoadSceneView>(viewParams);
    }

    /// <summary>
    /// ������ʼ��ת
    /// </summary>
    public void OnSceneStartChange()
    {
        if (currSceneID == SceneID.MainScene)
        {
        }
    }

    /// <summary>
    /// ������ת���
    /// </summary>
    void OnSceneChangeComplete()
    {
        Debug.Log("��ת������:" + targetSceneName);
    }

    public bool IsWorld
    {
        get
        {
            return currSceneID == SceneID.MainScene;
        }
    }

    public bool IsBattleField
    {
        get
        {
            return currSceneID == SceneID.BattleField;
        }
    }
}