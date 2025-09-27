using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class TechDetailView : Rain.UI.BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject imgTechIcon;
        [SerializeField] public Image imgTechIcon_Image;
        [SerializeField] public GameObject txtTechLevel;
        [SerializeField] public TextMeshProUGUI txtTechLevel_Text;
        [SerializeField] public GameObject txtTechState;
        [SerializeField] public TextMeshProUGUI txtTechState_Text;
        [SerializeField] public GameObject txtTechName;
        [SerializeField] public TextMeshProUGUI txtTechName_Text;
        [SerializeField] public GameObject desc;
        [SerializeField] public TextMeshProUGUI desc_Text;
        [SerializeField] public GameObject btFinish;
        [SerializeField] public Button btFinish_Button;
        [SerializeField] public GameObject btStudy;
        [SerializeField] public Button btStudy_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}