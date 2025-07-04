using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class CountDownView : BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject bar1;
        [SerializeField] public RectTransform bar1_Rect;
        [SerializeField] public GameObject bar2;
        [SerializeField] public RectTransform bar2_Rect;
        [SerializeField] public GameObject number;
        [SerializeField] public Image number_Image;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}