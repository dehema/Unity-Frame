using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class DemoView : BaseView
{
    DemoViewParam _viewParams;

    public override void Init(IViewParam viewParams = null)
    {
        base.Init(viewParams);

        //��ͼ����
        _viewParams = viewParams as DemoViewParam;
        Debug.Log(_viewParams.number);
        Debug.Log(_viewParams.tips);
        _viewParams?.action();
        _viewParams?.actionParams("this is a param!");

        //ע�ᰴť�¼�
        //ui.btClose_Button.
        ui.btClose_Button.SetButton(() => { UIMgr.Ins.CloseView<DemoView>(); });

        //ͬ������
        Sprite sprite = AssetMgr.Ins.Load<Sprite>("beastmen_centigors1");
        ui.imgBuild1_Image.sprite = sprite;

        //�첽����
        BaseLoader baseLoader = AssetMgr.Ins.LoadAsync<Sprite>("beastmen_centigors2", (OnAssetObject) =>
        {
            ui.imgBuild2_Image.sprite = OnAssetObject;
        });

        TimerMgr.Ins.AddTimer(this, step: 2, field: 0, onSecond: () =>
        {
            Debug.Log("onSecond");
        }, onComplete: () =>
        {
            Debug.Log("onComplete");
        });
    }

    public override void OnOpen(IViewParam viewParams = null)
    {
        base.OnOpen(viewParams);
        Debug.Log("UI��");
    }

    /// <summary>
    /// �ر��¼� �ص���UIMgr��ʵ��
    /// </summary>
    /// <param name="_cb"></param>
    public override void OnClose(Action _cb)
    {
        Debug.Log("UI�ر�");
        base.OnClose(_cb);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AudioMgr.Ins.PlayAudioEffect("Electronic high shot");


        }
    }
}


public class DemoViewParam : IViewParam
{
    public int number;
    public string tips;
    public Action action;
    public Action<string> actionParams;
}
