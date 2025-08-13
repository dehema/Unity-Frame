using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;
using static SettingField;

public partial class RTSUnitTestView : BaseView
{
    ObjPool unitPool;
    List<Button> btUnits = new List<Button>();
    bool isShowUnitList = true;
    public override void Init(IViewParams _viewParams = null)
    {
        base.Init(_viewParams);
        ui.btShowUnitList_Button.SetButton(OnClickShowUnitList);
        ui.btAddDummy_Button.SetButton(() => { RTSUnitTestSceneMgr.Ins.CreateDummy(); });
        InitUnitList();

        //场景默认创建一个假人一个士兵
        ui.btAddDummy_Button.onClick.Invoke();
        btUnits.First().onClick.Invoke();
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
            btUnits.Add(bt);
            bt.SetButton(() =>
            {
                RTSUnitTestSceneMgr.Ins.CreatePlayerUnit(unitConfig);
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
