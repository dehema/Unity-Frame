using System;
using System.Collections;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LoadSceneView : BaseView
{
    LoadSceneViewParam _viewParam;
    float currProgress = 0;
    public override void OnOpen(IViewParam viewParam = null)
    {
        base.OnOpen(viewParam);
        _viewParam = viewParam as LoadSceneViewParam;
        currProgress = 0;
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        _viewParam.CloseCB?.Invoke();
    }

    public void SetViewProgress(float _progress)
    {
        currProgress = _progress;
        ui.progress_Slider.value = currProgress;
        ui.txtProgress_Text.text = string.Format(LangMgr.Ins.Get("1672306530"), (int)(currProgress * 100));
    }
}


public class LoadSceneViewParam : IViewParam
{
    public string targetSceneName;
    public Action CloseCB;
}