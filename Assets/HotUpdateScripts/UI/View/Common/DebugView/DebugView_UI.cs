using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class DebugView : Rain.UI.BaseView
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
        [SerializeField] public GameObject keepOpenView;
        [SerializeField] public Toggle keepOpenView_Toggle;
        [SerializeField] public GameObject btStartGame;
        [SerializeField] public Button btStartGame_Button;
        [SerializeField] public GameObject tdScene;
        [SerializeField] public Button tdScene_Button;
        [SerializeField] public GameObject rtsUnitTest;
        [SerializeField] public Button rtsUnitTest_Button;
        [SerializeField] public GameObject btLoadPlayerData;
        [SerializeField] public Button btLoadPlayerData_Button;
        [SerializeField] public GameObject btSavePlayerData;
        [SerializeField] public Button btSavePlayerData_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}