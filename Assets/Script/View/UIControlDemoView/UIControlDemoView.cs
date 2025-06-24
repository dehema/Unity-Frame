using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.UI;

public partial class UIControlDemoView : BaseView
{
    [Tooltip("�ؼ�")]
    public List<GameObject> uiControlList = new List<GameObject>();
    [Tooltip("������")]
    public InfiniteScroll scroll;

    public override void Init(params object[] _params)
    {
        base.Init(_params);
        ui.btClose_Button.SetButton(Close);
        InitMenu();
    }

    void InitMenu()
    {
        List<string> optionList = new List<string>();
        foreach (var item in uiControlList)
        {
            optionList.Add(item.name);
        }
        ui.deMenu_Dropdown.AddOptions(optionList);
        ui.deMenu_Dropdown.onValueChanged.AddListener((option) =>
        {
            scroll.MoveTo(option, InfiniteScroll.MoveToType.MOVE_TO_TOP, time: 0.6f);
        });
    }
}
