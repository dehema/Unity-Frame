using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class DemoView : Rain.UI.BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject imgBuild1;
        [SerializeField] public Image imgBuild1_Image;
        [SerializeField] public GameObject imgBuild2;
        [SerializeField] public Image imgBuild2_Image;
        [SerializeField] public GameObject imgBuild3;
        [SerializeField] public Image imgBuild3_Image;
        [SerializeField] public GameObject btClose;
        [SerializeField] public Button btClose_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}