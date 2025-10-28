using System;
using DG.Tweening;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class TopView : BaseView
{
    //data
    bool isShow = true;
    const float showDuration = 0.5f;
    long oldGold = 0;
    int oldExp = 0;

    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        oldGold = DataMgr.Ins.playerData.gold.Value;
        oldExp = DataMgr.Ins.playerData.exp.Value;
        //bind
        InitBinding();
        //event
        //UI
    }

    private void Update()
    {
    }

    void InitBinding()
    {
        DataMgr.Ins.playerData.gold.Bind((dm) =>
        {
            Util.Common.DONumVal(oldGold, DataMgr.Ins.playerData.gold.Value, _num =>
            {
                int num = (int)_num;
                ui.goldNum_Text.text = Util.Text.FormatNum(num);
                oldGold = num;
            });
        });
        DataMgr.Ins.playerData.level.Bind(dm => { ui.txtLevel_Text.text = dm.value.ToString(); });
        DataMgr.Ins.playerData.exp.Bind(dm =>
        {
            int num = (int)dm.value;
            int nextExp = 1000;
            ui.txtExp_Text.text = $"{num}/{nextExp}";
            if (num > oldExp)
            {
                ui.expSlider_Slider.DOValue(num / (float)nextExp, 1);
            }
            else
            {
                ui.expSlider_Slider.value = num / (float)nextExp;
            }
            oldExp = num;
        });
        DataMgr.Ins.playerData.playerName.Bind(dm =>
        {
            ui.txtName_Text.text = dm.value.ToString();
        });
    }

    public void SetShow(bool _isShow, Action _closeCB = null)
    {
        isShow = _isShow;
        if (isShow)
        {
            ui.top_Rect.DOAnchorPos(Vector2.zero, showDuration);
        }
        else
        {
            ui.top_Rect.DOAnchorPos(new Vector2(0, ui.top_Rect.rect.height), showDuration).onComplete = () =>
            {
                _closeCB?.Invoke();
            };
        }
    }
}
