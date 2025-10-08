using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class MainView : Rain.UI.BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject playerIcon;
        [SerializeField] public Image playerIcon_Image;
        [SerializeField] public GameObject btExplore;
        [SerializeField] public Button btExplore_Button;
        [SerializeField] public GameObject btWorldMap;
        [SerializeField] public Button btWorldMap_Button;
        [SerializeField] public GameObject btChatPanel;
        [SerializeField] public Button btChatPanel_Button;
        [SerializeField] public GameObject iconChatMenu;
        [SerializeField] public Image iconChatMenu_Image;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}