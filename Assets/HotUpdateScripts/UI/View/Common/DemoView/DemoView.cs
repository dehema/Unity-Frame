using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class DemoView : BaseView
{
    DemoViewParam _viewParam;

    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);

        //��ͼ����
        _viewParam = viewParam as DemoViewParam;
        Debug.Log(_viewParam.number);
        Debug.Log(_viewParam.tips);
        _viewParam?.action();
        _viewParam?.actionParams("this is a param!");

        //ע�ᰴť�¼�
        //ui.btClose_Button.
        ui.btClose_Button.SetButton(Close);
    }

    public override void OnOpen(IViewParam viewParam = null)
    {
        base.OnOpen(viewParam);
        Debug.Log("UI��");

        //ͬ������
        Sprite sprite = AssetMgr.Ins.Load<Sprite>("Btn_Rectangle00_n_Green");
        ui.imgBuild1_Image.sprite = sprite;

        //�첽����
        BaseLoader baseLoader = AssetMgr.Ins.LoadAsync<Sprite>("Btn_Rectangle00_n_Blue", (OnAssetObject) =>
        {
            ui.imgBuild2_Image.sprite = OnAssetObject;
        });

        TimerMgr.Ins.AddTimer(this, step: 1, field: 0, onSecond: () =>
        {
            Debug.Log("onSecond");
        }, onComplete: () =>
        {
            Debug.Log("onComplete");
        });

        //AssetMgr.Ins.Load<>
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

    public class DemoViewParam : IViewParam
    {
        public int number;
        public string tips;
        public Action action;
        public Action<string> actionParams;
    }
}