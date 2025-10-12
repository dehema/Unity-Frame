using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class CountDownView : BaseView
{
    private float currentTime;
    [SerializeField] private Sprite[] numberSprites;  // 0-9的Sprite数组
    CountDownViewParam viewParam;
    bool startCountDown = false;
    DBInt db;

    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        db = new DBInt();
    }

    private float lastClickTime = -1f;
    private float doubleClickTimeThreshold = 0.3f; // 双击时间阈值，300毫秒

    public override void OnOpen(IViewParam _viewParam)
    {
        base.OnOpen(_viewParam);
        if (_viewParam == null)
        {
            _viewParam = new CountDownViewParam();
        }
        viewParam = _viewParam as CountDownViewParam;
        currentTime = viewParam.countDown;
        db.Value = Mathf.CeilToInt(viewParam.countDown);
        db.Bind(UpdateNumberDisplay);
        TimerMgr.Ins.AddTimer(this, delay: viewParam.delay, onComplete: () =>
        {
            startCountDown = true;
        });
        float width = Screen.width / 2;
        ui.bar1_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        ui.bar1_Rect.DoWidth(width, 0.5f);
        ui.bar2_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        ui.bar2_Rect.DoWidth(width, 0.5f);
    }

    public void Update()
    {
        if (Input.anyKeyDown)
        {
            OnDoubleClick();
            return;
        }
        if (!startCountDown)
            return;
        currentTime -= Time.deltaTime;
        int newNumber = Mathf.CeilToInt(currentTime);
        if (newNumber != db.Value && newNumber >= 0)
        {
            db.Value = newNumber;
        }
        if (currentTime <= 0)
        {
            Close();
        }
    }
    void UpdateNumberDisplay(DBModify dm)
    {
        if (db.Value >= 0 && db.Value < numberSprites.Length)
        {
            ui.number_Image.sprite = numberSprites[db.Value];
        }
    }

    // 双击close
    private void OnDoubleClick()
    {
        float currentClickTime = Time.time;
        if (currentClickTime - lastClickTime < doubleClickTimeThreshold)
        {
            // 双击检测成功，触发关闭方法
            Close();
        }
        lastClickTime = currentClickTime;
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        viewParam?.cb();
        db.Clear();
    }
}

public class CountDownViewParam : IViewParam
{
    public float countDown = 3;
    public float delay = 0.5f;
    public Action cb;
}
