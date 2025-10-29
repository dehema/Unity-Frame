using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class BagView : Rain.UI.BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject btClose;
        [SerializeField] public Button btClose_Button;
        [SerializeField] public GameObject tgItemTypeRes;
        [SerializeField] public Toggle tgItemTypeRes_Toggle;
        [SerializeField] public GameObject tgItemTypeSpeed;
        [SerializeField] public Toggle tgItemTypeSpeed_Toggle;
        [SerializeField] public GameObject tgItemTypeBuff;
        [SerializeField] public Toggle tgItemTypeBuff_Toggle;
        [SerializeField] public GameObject tgItemTypeEquip;
        [SerializeField] public Toggle tgItemTypeEquip_Toggle;
        [SerializeField] public GameObject tgItemTypeOther;
        [SerializeField] public Toggle tgItemTypeOther_Toggle;
        [SerializeField] public GameObject InfiniteScroll;
        [SerializeField] public InfiniteScroll InfiniteScroll_InfiniteScroll;
        [SerializeField] public RectTransform InfiniteScroll_Rect;
        [SerializeField] public GameObject selItemDialog;
        [SerializeField] public BagViewSelItemDialog selItemDialog_BagViewSelItemDialog;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}