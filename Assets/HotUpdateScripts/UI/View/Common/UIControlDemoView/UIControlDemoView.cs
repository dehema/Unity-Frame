using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using UnityEngine.UI;

public partial class UIControlDemoView : BaseView
{
    public override void Init(IViewParam viewParams = null)
    {
        base.Init(viewParams);
        ui.btClose_Button.SetButton(Close);
        InitMenu();
    }

    readonly SortedDictionary<string, string> prefabDict = new SortedDictionary<string, string>()
    {
        { "��̬����","UIControlDemo_DynamicContainer"},
        { "��̬ѡ�_����","UIControlDemo_DynamicTab_Hor"},
        { "��̬ѡ�_����","UIControlDemo_DynamicTab_Ver"},
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
            GameObject prefab = Resources.Load<GameObject>("View/UIControlDemoView/" + prefabDict.Values.ElementAt(option));
            currControl = Instantiate(prefab, ui.controlParent.transform);
            RectTransform rect = currControl.GetComponent<RectTransform>();
            rect.localPosition = Vector3.zero;
        });
        ui.deMenu_Dropdown.value = -1;
    }
}
