using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class DebugView : BaseView
{
    ObjPool btUIPool;

    public override void Init(params object[] _params)
    {
        base.Init(_params);
        InitUIPool();
        viewList_Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.pixelRect.height / 2 - 140);
        btStartGame_Button.SetButton(OnClickStartGame);
        btTips_Button.SetButton(() => { Utility.PopTips(DateTime.Now.ToString()); });
    }

    public void OnClickStartGame()
    {
        btStartGame_Button.interactable = false;
        GameMgr.Ins.StartGame();
        Close();
    }

    private void InitUIPool()
    {
        btUIPool = PoolMgr.Ins.CreatePool(btUIItem);
        foreach (var uiConfig in UIMgr.Ins.allViewConfig.view)
        {
            string uiName = uiConfig.Key;
            GameObject item = btUIPool.Get();
            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (uiName == string.Empty || uiName == string.Empty)
                {
                    UIMgr.Ins.OpenView(uiName, string.Empty);
                }
                else
                {
                    UIMgr.Ins.OpenView(uiName);
                }
                if (closeAfterOpenView_Toggle.isOn)
                {
                    Close();
                }
            });
            GetComponentInChildren<Text>("Text", item.transform).text = GetUINameInChinese(uiConfig);
        }
    }

    private string GetUINameInChinese(KeyValuePair<string, ViewConfigModel> _config)
    {
        string name = _config.Value.comment;
        if (string.IsNullOrEmpty(name))
        {
            name = _config.Key;
        }
        return name;
    }
}
