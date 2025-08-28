using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Rain.Core;
using Rain.RTS.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class RTSUnitTestView : BaseView
{
    ObjPool unitPool;
    List<Button> btUnits = new List<Button>();
    bool isShowUnitList = true;
    public override void Init(IViewParam _viewParams = null)
    {
        base.Init(_viewParams);

        ui.btShowUnitList_Button.SetButton(OnClickShowUnitList);
        ui.btAddDummy_Button.SetButton(AddDummy);
        InitUnitList();

    }

    private void Start()
    {
        //场景默认创建一个假人
        ui.btAddDummy_Button.onClick.Invoke();
        //一个士兵
        btUnits[3].onClick.Invoke();
    }

    void AddDummy()
    {
        BaseBattleUnit dummy = RTSUnitTestSceneMgr.Ins.CreateDummy();
        dummy.gameObject.name = "Dummy";
        dummy.transform.position = new Vector3(0, 0, -10);
    }

    void InitUnitList()
    {
        unitPool = PoolMgr.Ins.CreatePool(ui.unitItem);
        List<int> idList = new List<int>() { 1101, 1201, 1301, 1401 };
        foreach (int _id in idList)
        {
            UnitConfig _unitConfig = ConfigMgr.Ins.allUnitConfig.GetUnitConfig(_id);
            UnitConfig unitConfig = new UnitConfig(_unitConfig);
            GameObject item = unitPool.Get();
            Text textName = item.transform.Find("text").GetComponent<Text>();
            textName.text = LangMgr.Ins.Get(unitConfig.name);
            Button bt = item.GetComponent<Button>();
            btUnits.Add(bt);
            bt.SetButton(() =>
            {
                BaseBattleUnit unit = RTSUnitTestSceneMgr.Ins.CreatePlayerUnit(unitConfig);
                unit.transform.position = new Vector3(0, 0, 10);
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
