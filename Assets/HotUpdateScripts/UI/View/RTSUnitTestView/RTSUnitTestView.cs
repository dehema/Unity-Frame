using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class RTSUnitTestView : BaseView
{
    ObjPool unitPool;
    Action<UnitConfig> actionCreateUnit;

    RTSUnitTestViewParams viewParams;
    bool isShowUnitList = true;
    public override void Init(IViewParams _viewParams = null)
    {
        base.Init(viewParams);
        viewParams = _viewParams as RTSUnitTestViewParams;
        actionCreateUnit = viewParams.actionCreateUnit;
        ui.btShowUnitList_Button.SetButton(OnClickShowUnitList);
        InitUnitList();
    }

    void InitUnitList()
    {
        unitPool = PoolMgr.Ins.CreatePool(ui.unitItem);

        foreach (UnitConfig unitConfig in ConfigMgr.Ins.allUnitConfig.unit)
        {
            GameObject item = unitPool.Get();
            Text textName = item.transform.Find("text").GetComponent<Text>();
            textName.text = LangMgr.Ins.Get(unitConfig.name);
            Button bt = item.GetComponent<Button>();
            bt.SetButton(() =>
            {
                actionCreateUnit?.Invoke(unitConfig);
            });
        }
    }

    public void OnClickShowUnitList()
    {
        isShowUnitList = !isShowUnitList;
        Transform transArrow = ui.btShowUnitList.transform.Find("Image");
        transArrow.localEulerAngles = isShowUnitList ? new Vector3(0, 0, 180) : Vector3.zero;
        ui.left_float_Rect.DOLocalMoveX(isShowUnitList ? 0 : -ui.left_float_Rect.rect.width, 0.5f);
    }
}

public class RTSUnitTestViewParams : IViewParams
{
    public Action<UnitConfig> actionCreateUnit;
}
