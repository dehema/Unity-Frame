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
        [SerializeField] public GameObject lbFoodNum;
        [SerializeField] public TextMeshProUGUI lbFoodNum_Text;
        [SerializeField] public GameObject lbWoodNum;
        [SerializeField] public TextMeshProUGUI lbWoodNum_Text;
        [SerializeField] public GameObject lbGoldNum;
        [SerializeField] public TextMeshProUGUI lbGoldNum_Text;
        [SerializeField] public GameObject lbDiamondNum;
        [SerializeField] public TextMeshProUGUI lbDiamondNum_Text;
        [SerializeField] public GameObject lbPower;
        [SerializeField] public TextMeshProUGUI lbPower_Text;
        [SerializeField] public GameObject lbVip;
        [SerializeField] public TextMeshProUGUI lbVip_Text;
        [SerializeField] public GameObject btShop;
        [SerializeField] public Button btShop_Button;
        [SerializeField] public GameObject btBag;
        [SerializeField] public Button btBag_Button;
        [SerializeField] public GameObject btAlliance;
        [SerializeField] public Button btAlliance_Button;
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