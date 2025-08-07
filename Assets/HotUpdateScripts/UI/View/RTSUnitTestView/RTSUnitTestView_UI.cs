using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class RTSUnitTestView : BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject left_float;
        [SerializeField] public RectTransform left_float_Rect;
        [SerializeField] public GameObject unitItem;
        [SerializeField] public GameObject btShowUnitList;
        [SerializeField] public Button btShowUnitList_Button;
        [SerializeField] public GameObject right_float;
        [SerializeField] public RectTransform right_float_Rect;
        [SerializeField] public GameObject btAddDummy;
        [SerializeField] public Button btAddDummy_Button;
        [SerializeField] public GameObject btShowOperationList;
        [SerializeField] public Button btShowOperationList_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}