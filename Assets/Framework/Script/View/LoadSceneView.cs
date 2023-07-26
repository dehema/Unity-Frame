using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LoadSceneView : BaseView
{
    LoadSceneViewParams viewParams;
    float progressMax = 0;
    float currProgress = 0;
    public override void OnOpen(params object[] _params)
    {
        base.OnOpen(_params);
        viewParams = _params[0] as LoadSceneViewParams;
        currProgress = 0;
        Utility.DONumVal(0, 1, num => { progressMax = num; }, 0.5f);
        StartCoroutine(LoadLevel());
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        viewParams.CloseCB?.Invoke();
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(viewParams.targetSceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            currProgress = Mathf.Min(progressMax, operation.progress);
            progress_Slider.value = currProgress;
            txtProgress_Text.text = string.Format(LangMgr.Ins.Get("1672306530"), (int)(currProgress * 100));
            if (currProgress >= 0.9F)
            {
                progress_Slider.value = 1.0f;
                txtProgress_Text.text = string.Format(LangMgr.Ins.Get("1672306530"), 100);
                operation.allowSceneActivation = true;
                Close();
            }
            yield return null;
        }
    }
}

public class LoadSceneViewParams
{
    public string targetSceneName;
    public Action CloseCB;
}
