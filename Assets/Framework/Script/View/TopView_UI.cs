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
    public GameObject btHome;
    [HideInInspector]
    public Button btHome_Button;
    [HideInInspector]
    public GameObject btGold;
    [HideInInspector]
    public Button btGold_Button;
    [HideInInspector]
    public GameObject iconGold;
    [HideInInspector]
    public Image iconGold_Image;
    [HideInInspector]
    public GameObject goldNum;
    [HideInInspector]
    public Text goldNum_Text;
    [HideInInspector]
    public GameObject iconCash;
    [HideInInspector]
    public Image iconCash_Image;
    [HideInInspector]
    public GameObject cashNum;
    [HideInInspector]
    public Text cashNum_Text;
    [HideInInspector]
    public GameObject btAmazon;
    [HideInInspector]
    public Button btAmazon_Button;
    [HideInInspector]
    public GameObject iconAmazon;
    [HideInInspector]
    public Image iconAmazon_Image;
    [HideInInspector]
    public GameObject amazonNum;
    [HideInInspector]
    public Text amazonNum_Text;
    [HideInInspector]
    public GameObject btSetting;
    [HideInInspector]
    public Button btSetting_Button;
    [HideInInspector]
    public GameObject btCash;
    [HideInInspector]
    public Button btCash_Button;
    [HideInInspector]
    public GameObject btTips;
    [HideInInspector]
    public Button btTips_Button;

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
        btHome = transform.Find("$content#Rect/$top#Rect/$btHome#Button").gameObject;
        btHome_Button = btHome.GetComponent<Button>();
        btGold = transform.Find("$content#Rect/$top#Rect/attribute/$btGold#Button").gameObject;
        btGold_Button = btGold.GetComponent<Button>();
        iconGold = transform.Find("$content#Rect/$top#Rect/attribute/$btGold#Button/$iconGold#Image").gameObject;
        iconGold_Image = iconGold.GetComponent<Image>();
        goldNum = transform.Find("$content#Rect/$top#Rect/attribute/$btGold#Button/$goldNum#Text").gameObject;
        goldNum_Text = goldNum.GetComponent<Text>();
        iconCash = transform.Find("$content#Rect/$top#Rect/attribute/cash/$iconCash#Image").gameObject;
        iconCash_Image = iconCash.GetComponent<Image>();
        cashNum = transform.Find("$content#Rect/$top#Rect/attribute/cash/$cashNum#Text").gameObject;
        cashNum_Text = cashNum.GetComponent<Text>();
        btAmazon = transform.Find("$content#Rect/$top#Rect/attribute/$btAmazon#Button").gameObject;
        btAmazon_Button = btAmazon.GetComponent<Button>();
        iconAmazon = transform.Find("$content#Rect/$top#Rect/attribute/$btAmazon#Button/$iconAmazon#Image").gameObject;
        iconAmazon_Image = iconAmazon.GetComponent<Image>();
        amazonNum = transform.Find("$content#Rect/$top#Rect/attribute/$btAmazon#Button/$amazonNum#Text").gameObject;
        amazonNum_Text = amazonNum.GetComponent<Text>();
        btSetting = transform.Find("$content#Rect/$top#Rect/$btSetting#Button").gameObject;
        btSetting_Button = btSetting.GetComponent<Button>();
        btCash = transform.Find("$content#Rect/bottom/$btCash#Button").gameObject;
        btCash_Button = btCash.GetComponent<Button>();
        btTips = transform.Find("$content#Rect/bottom/$btTips#Button").gameObject;
        btTips_Button = btTips.GetComponent<Button>();
    }
}