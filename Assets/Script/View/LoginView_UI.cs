using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class LoginView : BaseView
{
    [SerializeField]
    public GameObject bg;
    public Image bg_Image;
    public Button bg_Button;
    [SerializeField]
    public GameObject content;
    public RectTransform content_Rect;
    [SerializeField]
    public GameObject ver;
    public RectTransform ver_Rect;
    public VerticalLayoutGroup ver_Ver;
    [SerializeField]
    public GameObject userTg;
    public Toggle userTg_Toggle;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
        ver = transform.Find("$content#Rect/$ver#Rect,Ver").gameObject;
        ver_Rect = ver.GetComponent<RectTransform>();
        ver_Ver = ver.GetComponent<VerticalLayoutGroup>();
        userTg = transform.Find("$content#Rect/$ver#Rect,Ver/$userTg#Toggle").gameObject;
        userTg_Toggle = userTg.GetComponent<Toggle>();
    }
}