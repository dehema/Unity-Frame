using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;
using TMPro;

public partial class ExampleView : ExampleViewParent
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject btButton;
        [SerializeField] public Button btButton_Button;
        [SerializeField] public Image btButton_Image;
        [SerializeField] public GameObject txtBt;
        [SerializeField] public TextMeshProUGUI txtBt_Text;
        [SerializeField] public GameObject btClose;
        [SerializeField] public Button btClose_Button;
        [SerializeField] public GameObject goldNum;
        [SerializeField] public TextMeshProUGUI goldNum_Text;
        [SerializeField] public GameObject btAddGold;
        [SerializeField] public Button btAddGold_Button;
        [SerializeField] public GameObject btUnBindAllDataBind;
        [SerializeField] public Button btUnBindAllDataBind_Button;
        [SerializeField] public GameObject exampleWidgetWithSuper;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}