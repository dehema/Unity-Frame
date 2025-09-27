using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class TechRoot : Rain.UI.BaseUI
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject btDetail;
        [SerializeField] public Button btDetail_Button;
        [SerializeField] public GameObject bgLock;
        [SerializeField] public GameObject bgStudied;
        [SerializeField] public GameObject bgCanStudy;
        [SerializeField] public GameObject icon;
        [SerializeField] public Image icon_Image;
        [SerializeField] public GameObject txName;
        [SerializeField] public TextMeshProUGUI txName_Text;
        [SerializeField] public GameObject upLine;
        [SerializeField] public RectTransform upLine_Rect;
        [SerializeField] public Image upLine_Image;
        [SerializeField] public GameObject downLine;
        [SerializeField] public RectTransform downLine_Rect;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}