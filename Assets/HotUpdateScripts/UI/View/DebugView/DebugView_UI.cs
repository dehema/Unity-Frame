using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class DebugView : BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject viewList;
        [SerializeField] public RectTransform viewList_Rect;
        [SerializeField] public GameObject btUIItem;
        [SerializeField] public GameObject closeAfterOpenView;
        [SerializeField] public Toggle closeAfterOpenView_Toggle;
        [SerializeField] public GameObject btStartGame;
        [SerializeField] public Button btStartGame_Button;
        [SerializeField] public GameObject btTips;
        [SerializeField] public Button btTips_Button;
        [SerializeField] public GameObject tdScene;
        [SerializeField] public Button tdScene_Button;
        [SerializeField] public GameObject rtsUnitTest;
        [SerializeField] public Button rtsUnitTest_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}