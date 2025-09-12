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
    string toggleButtonText = "���ذ�ť";

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

        // �����л���ť
        Rect toggleRect = new Rect(10, 10, buttonWidth, buttonHeight);
        if (GUI.Button(toggleRect, toggleButtonText))
        {
            // �л���ʾ״̬
            isSidebarVisible = !isSidebarVisible;
            // ���°�ť�ı�
            toggleButtonText = isSidebarVisible ? "���ذ�ť" : "��ʾ��ť";
        }

        // �����ť�пɼ�������ư�ť
        if (isSidebarVisible)
        {
            DrawSidebarButtons();
        }
    }

    // ���Ʋ������ť
    void DrawSidebarButtons()
    {
        float startY = buttonHeight + 20;

        int index = 0;
        foreach (var item in ConfigMgr.Unit.DataList)
        {
            UnitConfig unitConfig = item;
            // ����ÿ����ť��λ��
            float yPos = startY + index * (buttonHeight + buttonSpacing);
            Rect buttonRect = new Rect(10, yPos, buttonWidth, buttonHeight);

            // ���ư�ť�������
            if (GUI.Button(buttonRect, LangMgr.Ins.Get(unitConfig.Name)))
            {
                Debug.Log(unitConfig.FullID);
                unitTestMgr.CreateUnit(unitConfig);
            }
            index++;
        }
    }

    /// <summary>
    /// ��λ���� <·��ID, ����>
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
