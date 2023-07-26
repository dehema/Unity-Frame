using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class TopView : BaseView
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
    public GameObject top;
    [HideInInspector]
    public RectTransform top_Rect;
    [HideInInspector]
    public GameObject expSlider;
    [HideInInspector]
    public Slider expSlider_Slider;
    [HideInInspector]
    public GameObject txtExp;
    [HideInInspector]
    public Text txtExp_Text;
    [HideInInspector]
    public GameObject txtLevel;
    [HideInInspector]
    public Text txtLevel_Text;
    [HideInInspector]
    public GameObject txtName;
    [HideInInspector]
    public Text txtName_Text;
    [HideInInspector]
    public GameObject goldNum;
    [HideInInspector]
    public Text goldNum_Text;
    [HideInInspector]
    public GameObject debugAddGold1K;
    [HideInInspector]
    public Button debugAddGold1K_Button;
    [HideInInspector]
    public GameObject btSetting;
    [HideInInspector]
    public Button btSetting_Button;
    [HideInInspector]
    public GameObject txtDate;
    [HideInInspector]
    public Text txtDate_Text;
    [HideInInspector]
    public GameObject btTimePause;
    [HideInInspector]
    public Button btTimePause_Button;
    [HideInInspector]
    public GameObject btTimePause_light;
    [HideInInspector]
    public GameObject btTimeNormal;
    [HideInInspector]
    public Button btTimeNormal_Button;
    [HideInInspector]
    public GameObject btTimeNormal_light;
    [HideInInspector]
    public GameObject btTimeQuick;
    [HideInInspector]
    public Button btTimeQuick_Button;
    [HideInInspector]
    public GameObject btTimeQuick_light;
    [HideInInspector]
    public GameObject right;
    [HideInInspector]
    public RectTransform right_Rect;

    internal override void _LoadUI()    
    {
        base._LoadUI();
        bg = transform.Find("$bg#Image,Button").gameObject;
        bg_Image = bg.GetComponent<Image>();
        bg_Button = bg.GetComponent<Button>();
        content = transform.Find("$content#Rect").gameObject;
        content_Rect = content.GetComponent<RectTransform>();
        top = transform.Find("$content#Rect/$top#Rect").gameObject;
        top_Rect = top.GetComponent<RectTransform>();
        expSlider = transform.Find("$content#Rect/$top#Rect/User_Info_1/$expSlider#Slider").gameObject;
        expSlider_Slider = expSlider.GetComponent<Slider>();
        txtExp = transform.Find("$content#Rect/$top#Rect/User_Info_1/$expSlider#Slider/$txtExp#Text").gameObject;
        txtExp_Text = txtExp.GetComponent<Text>();
        txtLevel = transform.Find("$content#Rect/$top#Rect/User_Info_1/Level_Frame/$txtLevel#Text").gameObject;
        txtLevel_Text = txtLevel.GetComponent<Text>();
        txtName = transform.Find("$content#Rect/$top#Rect/User_Info_1/$txtName#Text").gameObject;
        txtName_Text = txtName.GetComponent<Text>();
        goldNum = transform.Find("$content#Rect/$top#Rect/stats/Stats_Gold/$goldNum#Text").gameObject;
        goldNum_Text = goldNum.GetComponent<Text>();
        debugAddGold1K = transform.Find("$content#Rect/$top#Rect/stats/Stats_Gold/$debugAddGold1K#Button").gameObject;
        debugAddGold1K_Button = debugAddGold1K.GetComponent<Button>();
        btSetting = transform.Find("$content#Rect/$top#Rect/$btSetting#Button").gameObject;
        btSetting_Button = btSetting.GetComponent<Button>();
        txtDate = transform.Find("$content#Rect/bottom/$txtDate#Text").gameObject;
        txtDate_Text = txtDate.GetComponent<Text>();
        btTimePause = transform.Find("$content#Rect/bottom/timeSpeed/$btTimePause#Button").gameObject;
        btTimePause_Button = btTimePause.GetComponent<Button>();
        btTimePause_light = transform.Find("$content#Rect/bottom/timeSpeed/$btTimePause#Button/$btTimePause_light").gameObject;
        btTimeNormal = transform.Find("$content#Rect/bottom/timeSpeed/$btTimeNormal#Button").gameObject;
        btTimeNormal_Button = btTimeNormal.GetComponent<Button>();
        btTimeNormal_light = transform.Find("$content#Rect/bottom/timeSpeed/$btTimeNormal#Button/$btTimeNormal_light").gameObject;
        btTimeQuick = transform.Find("$content#Rect/bottom/timeSpeed/$btTimeQuick#Button").gameObject;
        btTimeQuick_Button = btTimeQuick.GetComponent<Button>();
        btTimeQuick_light = transform.Find("$content#Rect/bottom/timeSpeed/$btTimeQuick#Button/$btTimeQuick_light").gameObject;
        right = transform.Find("$content#Rect/$right#Rect").gameObject;
        right_Rect = right.GetComponent<RectTransform>();
    }
}