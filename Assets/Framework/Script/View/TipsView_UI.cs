using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class TipsView : BaseView
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
    public GameObject templeteTips;
    [HideInInspector]
    public GameObject commonFloat;
    [HideInInspector]
    public RectTransform commonFloat_Rect;
    [HideInInspector]
    public GameObject txtCommonFloat;
    [HideInInspector]
    public Text txtCommonFloat_Text;
    [HideInInspector]
    public RectTransform txtCommonFloat_Rect;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
        templeteTips = transform.Find("$content#Rect/ver/$templeteTips").gameObject;
        commonFloat = transform.Find("$content#Rect/ver/$commonFloat#Rect").gameObject;
        commonFloat_Rect = commonFloat.GetComponent<RectTransform>();
        txtCommonFloat = transform.Find("$content#Rect/ver/$commonFloat#Rect/$txtCommonFloat#Text,Rect").gameObject;
        txtCommonFloat_Text = txtCommonFloat.GetComponent<Text>();
        txtCommonFloat_Rect = txtCommonFloat.GetComponent<RectTransform>();
    }
}