using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIControlDemoView : BaseView
{
    public override void Init(params object[] _params)
    {
        base.Init(_params);
        ui.btClose_Button.SetButton(Close);
        InitMenu();
    }

    readonly SortedDictionary<string, string> prefabDict = new SortedDictionary<string, string>()
    {
        { "动态网格","UIControlDemo_DynamicContainer"},
        { "动态选项卡_横向","UIControlDemo_DynamicTab_Hor"},
        { "动态选项卡_纵向","UIControlDemo_DynamicTab_Ver"},
    };
    GameObject currControl;
    void InitMenu()
    {
        ui.deMenu_Dropdown.AddOptions(prefabDict.Keys.ToList());
        ui.deMenu_Dropdown.onValueChanged.AddListener((option) =>
        {
            if (currControl != null)
            {
                Destroy(currControl);
            }
            GameObject prefab = Resources.Load<GameObject>("Prefab/View/UIControlDemoView/" + prefabDict.Values.ElementAt(option));
            currControl = Instantiate(prefab, ui.controlParent.transform);
            RectTransform rect = currControl.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
        });
        ui.deMenu_Dropdown.value = -1;
    }
}
