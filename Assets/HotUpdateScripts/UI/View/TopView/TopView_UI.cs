using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class TopView : Rain.UI.BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject top;
        [SerializeField] public RectTransform top_Rect;
        [SerializeField] public GameObject expSlider;
        [SerializeField] public Slider expSlider_Slider;
        [SerializeField] public GameObject txtExp;
        [SerializeField] public Text txtExp_Text;
        [SerializeField] public GameObject txtLevel;
        [SerializeField] public Text txtLevel_Text;
        [SerializeField] public GameObject txtName;
        [SerializeField] public Text txtName_Text;
        [SerializeField] public GameObject goldNum;
        [SerializeField] public Text goldNum_Text;
        [SerializeField] public GameObject debugAddGold1K;
        [SerializeField] public Button debugAddGold1K_Button;
        [SerializeField] public GameObject btSetting;
        [SerializeField] public Button btSetting_Button;
        [SerializeField] public GameObject right;
        [SerializeField] public RectTransform right_Rect;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}