using System;
using System.Collections;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LoadSceneView : BaseView
{
    LoadSceneViewParams _viewParams;
    float progressMax = 0;
    float currProgress = 0;
    public override void OnOpen(IViewParams viewParams = null)
    {
        base.OnOpen(viewParams);
        _viewParams = viewParams as LoadSceneViewParams;
        currProgress = 0;
        Utility.DONumVal(0, 1, num => { progressMax = num; }, 0.5f);
        StartCoroutine(LoadLevel());
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        _viewParams.CloseCB?.Invoke();
    }

    IEnumerator LoadLevel()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_viewParams.targetSceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            currProgress = Mathf.Min(progressMax, operation.progress);
            ui.progress_Slider.value = currProgress;
            ui.txtProgress_Text.text = string.Format(LangMgr.Ins.Get("1672306530"), (int)(currProgress * 100));
            if (currProgress >= 0.9F)
            {
                ui.progress_Slider.value = 1.0f;
                ui.txtProgress_Text.text = string.Format(LangMgr.Ins.Get("1672306530"), 100);
                operation.allowSceneActivation = true;
                Close();
            }
            yield return null;
        }
    }
}
