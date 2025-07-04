using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class CountDownView : BaseView
{
    private float currentTime;
    [SerializeField] private Sprite[] numberSprites;  // 0-9µÄSpriteÊý×é
    CountDownViewParams viewParams;
    bool startCountDown = false;
    DBInt db;

    public override void Init(IViewParams viewParams = null)
    {
        base.Init(viewParams);
        db = new DBInt();
    }

    public override void OnOpen(IViewParams _viewParams)
    {
        base.OnOpen(_viewParams);
        if (_viewParams == null)
        {
            _viewParams = new CountDownViewParams();
        }
        viewParams = _viewParams as CountDownViewParams;
        currentTime = viewParams.countDown;
        db.Value = Mathf.CeilToInt(viewParams.countDown);
        db.Bind(UpdateNumberDisplay);
        AddTimer(delay: viewParams.delay, onComplete: () =>
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

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
        viewParams?.cb();
        db.Clear();
    }
}

public class CountDownViewParams : IViewParams
{
    public float countDown = 3;
    public float delay = 0.5f;
    public Action cb;
}
