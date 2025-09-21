using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;
using TMPro;
using Rain.UI;

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
        [SerializeField] public GameObject techContent;
        [SerializeField] public UIControlDemo_DynamicTab techContent_UIControlDemo_DynamicTab;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}