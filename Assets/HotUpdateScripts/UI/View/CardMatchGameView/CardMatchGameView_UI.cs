using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class CardMatchGameView : BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject cardMatchCardItem;
        [SerializeField] public GameObject slider;
        [SerializeField] public SliderCountDown slider_SliderCountDown;
        [SerializeField] public GameObject btClose;
        [SerializeField] public Button btClose_Button;
        [SerializeField] public GameObject block;
        [SerializeField] public Image block_Image;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}