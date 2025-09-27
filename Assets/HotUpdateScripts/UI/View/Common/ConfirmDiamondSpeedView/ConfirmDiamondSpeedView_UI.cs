using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class ConfirmDiamondSpeedView : Rain.UI.BaseView
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
        [SerializeField] public GameObject tgNoHintToday;
        [SerializeField] public Toggle tgNoHintToday_Toggle;
        [SerializeField] public GameObject txtRemainTime;
        [SerializeField] public TextMeshProUGUI txtRemainTime_Text;
        [SerializeField] public GameObject btFinish;
        [SerializeField] public Button btFinish_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}