using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class TechView : Rain.UI.BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject tgTeckCategory;
        [SerializeField] public Toggle tgTeckCategory_Toggle;
        [SerializeField] public GameObject btClose;
        [SerializeField] public Button btClose_Button;
        [SerializeField] public GameObject techContent;
        [SerializeField] public TabController techContent_TabController;
        [SerializeField] public GameObject tabPage;
        [SerializeField] public TabPage tabPage_TabPage;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}