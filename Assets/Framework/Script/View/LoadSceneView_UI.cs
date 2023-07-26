using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class LoadSceneView : BaseView
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
    public GameObject progress;
    [HideInInspector]
    public Slider progress_Slider;
    [HideInInspector]
    public GameObject txtProgress;
    [HideInInspector]
    public Text txtProgress_Text;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
        progress = transform.Find("$content#Rect/$progress#Slider").gameObject;
        progress_Slider = progress.GetComponent<Slider>();
        txtProgress = transform.Find("$content#Rect/$progress#Slider/$txtProgress#Text").gameObject;
        txtProgress_Text = txtProgress.GetComponent<Text>();
    }
}