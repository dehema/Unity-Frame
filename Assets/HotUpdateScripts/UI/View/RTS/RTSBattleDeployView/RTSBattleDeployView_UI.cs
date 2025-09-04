using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;
using TMPro;

public partial class RTSBattleDeployView : BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject t;
        [SerializeField] public TextMeshProUGUI t_Text;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}