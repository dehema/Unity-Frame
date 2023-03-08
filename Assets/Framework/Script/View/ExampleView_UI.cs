using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ExampleView : ExampleViewParent
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
    public GameObject btButton;
    [HideInInspector]
    public Button btButton_Button;
    [HideInInspector]
    public Image btButton_Image;
    [HideInInspector]
    public GameObject txtBt;
    [HideInInspector]
    public Text txtBt_Text;
    [HideInInspector]
    public GameObject btClose;
    [HideInInspector]
    public Button btClose_Button;
    [HideInInspector]
    public GameObject goldNum;
    [HideInInspector]
    public Text goldNum_Text;
    [HideInInspector]
    public GameObject btAddGold;
    [HideInInspector]
    public Button btAddGold_Button;
    [HideInInspector]
    public GameObject btUnBindAllDataBind;
    [HideInInspector]
    public Button btUnBindAllDataBind_Button;
    [HideInInspector]
    public GameObject exampleWidgetWithSuper;
    [HideInInspector]
    public GameObject exampleWidget;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
        btButton = transform.Find("$content#Rect/$btButton#Button,Image").gameObject;
        btButton_Button = btButton.GetComponent<Button>();
        btButton_Image = btButton.GetComponent<Image>();
        txtBt = transform.Find("$content#Rect/$btButton#Button,Image/$txtBt#Text").gameObject;
        txtBt_Text = txtBt.GetComponent<Text>();
        btClose = transform.Find("$content#Rect/$btClose#Button").gameObject;
        btClose_Button = btClose.GetComponent<Button>();
        goldNum = transform.Find("$content#Rect/dataBind/$goldNum#Text").gameObject;
        goldNum_Text = goldNum.GetComponent<Text>();
        btAddGold = transform.Find("$content#Rect/$btAddGold#Button").gameObject;
        btAddGold_Button = btAddGold.GetComponent<Button>();
        btUnBindAllDataBind = transform.Find("$content#Rect/$btUnBindAllDataBind#Button").gameObject;
        btUnBindAllDataBind_Button = btUnBindAllDataBind.GetComponent<Button>();
        exampleWidgetWithSuper = transform.Find("$content#Rect/$exampleWidgetWithSuper").gameObject;
        exampleWidget = transform.Find("$content#Rect/$exampleWidget").gameObject;
    }
}