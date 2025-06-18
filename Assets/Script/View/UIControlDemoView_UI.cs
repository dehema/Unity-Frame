using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class UIControlDemoView : BaseView
{
    [SerializeField]
    public GameObject bg;
    public Image bg_Image;
    public Button bg_Button;
    [SerializeField]
    public GameObject content;
    public RectTransform content_Rect;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
    }
}