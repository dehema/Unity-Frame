using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.Core;

public partial class UIControlDemoView : BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject deMenu;
        [SerializeField] public Dropdown deMenu_Dropdown;
        [SerializeField] public GameObject btClose;
        [SerializeField] public Button btClose_Button;
        [SerializeField] public GameObject scroll;
        [SerializeField] public InfiniteScroll scroll_InfiniteScroll;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;

    internal override void _LoadUI()    
    {
        base._LoadUI();
    }
}