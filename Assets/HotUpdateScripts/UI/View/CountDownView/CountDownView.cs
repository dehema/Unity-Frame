using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DG.Tweening;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public partial class CountDownView : BaseView
{
    private float currentTime;
    [SerializeField] private Sprite[] numberSprites;  // 0-9µÄSpriteÊý×é
    CountDownViewParams viewParams;
    bool startCountDown = false;
    DBInt db;
    public override void OnOpen(IViewParams _viewParams = null)
    {
        base.OnOpen(_viewParams);
        viewParams = _viewParams as CountDownViewParams;
        currentTime = viewParams.countDown;
        db = new DBInt(Mathf.CeilToInt(viewParams.countDown));
        db.Bind(UpdateNumberDisplay);
        AddTimer(delay: viewParams.delay, onComplete: () =>
        {
            startCountDown = true;
        });
        ui.bar_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        ui.bar_Rect.DoWidth(Screen.width, 1);
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
    public float countDown = 1;
    public float delay = 3;
    public Action cb;
}
