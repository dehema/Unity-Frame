using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

public class RTSUnitTestSceneGUI : MonoBehaviour
{
    RTSUnitTestSceneMgr unitTestMgr;

    const float buttonWidth = 150f;
    const float buttonHeight = 50f;
    const float buttonSpacing = 10f;
    bool isSidebarVisible = true;
    string toggleButtonText = "隐藏按钮";

    private void Start()
    {
        unitTestMgr = GetComponent<RTSUnitTestSceneMgr>();
        GetUnitInfo();
    }

    void OnGUI()
    {
        GUI.skin.button.fontSize = 26;
        GUI.skin.label.fontSize = 26;
        GUI.skin.window.fontSize = 26;
        GUI.skin.button.fontSize = 26;

        // 绘制切换按钮
        Rect toggleRect = new Rect(10, 10, buttonWidth, buttonHeight);
        if (GUI.Button(toggleRect, toggleButtonText))
        {
            // 切换显示状态
            isSidebarVisible = !isSidebarVisible;
            // 更新按钮文本
            toggleButtonText = isSidebarVisible ? "隐藏按钮" : "显示按钮";
        }

        // 如果按钮列可见，则绘制按钮
        if (isSidebarVisible)
        {
            DrawSidebarButtons();
        }
    }

    // 绘制侧边栏按钮
    void DrawSidebarButtons()
    {
        float startY = buttonHeight + 20;

        int index = 0;
        foreach (var item in ConfigMgr.Unit.DataList)
        {
            UnitConfig unitConfig = item;
            // 计算每个按钮的位置
            float yPos = startY + index * (buttonHeight + buttonSpacing);
            Rect buttonRect = new Rect(10, yPos, buttonWidth, buttonHeight);

            // 绘制按钮并检测点击
            if (GUI.Button(buttonRect, LangMgr.Ins.Get(unitConfig.Name)))
            {
                Debug.Log(unitConfig.FullID);
                unitTestMgr.CreateUnit(unitConfig);
            }
            index++;
        }
    }

    /// <summary>
    /// 单位名称 <路径ID, 名字>
    /// </summary>
    Dictionary<string, string> unitNameDict = new Dictionary<string, string>();
    private void GetUnitInfo()
    {
        foreach (var item in ConfigMgr.Unit.DataList)
        {
            unitNameDict[item.Name] = LangMgr.Ins.Get(item.Name);
        }
    }
}
