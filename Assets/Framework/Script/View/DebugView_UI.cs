using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class DebugView : BaseView
{
    [HideInInspector]
    public GameObject bg;
    [HideInInspector]
    public Image bg_Image;
    [HideInInspector]
    public Button bg_Button;
    [HideInInspector]
    public GameObject content;
    [HideInInspector]
    public RectTransform content_Rect;
    [HideInInspector]
    public GameObject viewList;
    [HideInInspector]
    public RectTransform viewList_Rect;
    [HideInInspector]
    public GameObject btUIItem;
    [HideInInspector]
    public GameObject closeAfterOpenView;
    [HideInInspector]
    public Toggle closeAfterOpenView_Toggle;
    [HideInInspector]
    public GameObject btStartGame;
    [HideInInspector]
    public Button btStartGame_Button;
    [HideInInspector]
    public GameObject btTips;
    [HideInInspector]
    public Button btTips_Button;
    [HideInInspector]
    public GameObject btEnterBattleField;
    [HideInInspector]
    public Button btEnterBattleField_Button;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
        viewList = transform.Find("$content#Rect/$viewList#Rect").gameObject;
        viewList_Rect = viewList.GetComponent<RectTransform>();
        btUIItem = transform.Find("$content#Rect/$viewList#Rect/uiGrid/$btUIItem").gameObject;
        closeAfterOpenView = transform.Find("$content#Rect/$viewList#Rect/$closeAfterOpenView#Toggle").gameObject;
        closeAfterOpenView_Toggle = closeAfterOpenView.GetComponent<Toggle>();
        btStartGame = transform.Find("$content#Rect/page/$btStartGame#Button").gameObject;
        btStartGame_Button = btStartGame.GetComponent<Button>();
        btTips = transform.Find("$content#Rect/page/$btTips#Button").gameObject;
        btTips_Button = btTips.GetComponent<Button>();
        btEnterBattleField = transform.Find("$content#Rect/page/$btEnterBattleField#Button").gameObject;
        btEnterBattleField_Button = btEnterBattleField.GetComponent<Button>();
    }
}