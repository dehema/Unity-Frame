using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class ChatView : Rain.UI.BaseView
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
        [SerializeField] public GameObject tgChatMenu;
        [SerializeField] public Toggle tgChatMenu_Toggle;
        [SerializeField] public GameObject chatContent;
        [SerializeField] public TabController chatContent_TabController;
        [SerializeField] public GameObject tabPage;
        [SerializeField] public TabPage tabPage_TabPage;
        [SerializeField] public GameObject inputMsg;
        [SerializeField] public TMP_InputField inputMsg_Input;
        [SerializeField] public GameObject btSendMsg;
        [SerializeField] public Button btSendMsg_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}