using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class LoadingView : BaseView
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
    public GameObject imgSlider;
    [HideInInspector]
    public Image imgSlider_Image;
    [HideInInspector]
    public GameObject txtProgress;
    [HideInInspector]
    public Text txtProgress_Text;
    [HideInInspector]
    public GameObject txtLoading;
    [HideInInspector]
    public Text txtLoading_Text;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
        imgSlider = transform.Find("$content#Rect/LoadingBG/LoadingProgressBG/$imgSlider#Image").gameObject;
        imgSlider_Image = imgSlider.GetComponent<Image>();
        txtProgress = transform.Find("$content#Rect/LoadingBG/$txtProgress#Text").gameObject;
        txtProgress_Text = txtProgress.GetComponent<Text>();
        txtLoading = transform.Find("$content#Rect/LoadingBG/$txtLoading#Text").gameObject;
        txtLoading_Text = txtLoading.GetComponent<Text>();
    }
}