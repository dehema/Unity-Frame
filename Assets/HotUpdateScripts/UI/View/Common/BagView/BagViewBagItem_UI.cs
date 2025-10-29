using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class BagViewBagItem : Rain.Core.InfiniteScrollItem
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject board;
        [SerializeField] public Image board_Image;
        [SerializeField] public Button board_Button;
        [SerializeField] public GameObject icon;
        [SerializeField] public Image icon_Image;
        [SerializeField] public GameObject num;
        [SerializeField] public TextMeshProUGUI num_Text;
        [SerializeField] public GameObject param;
        [SerializeField] public TextMeshProUGUI param_Text;
        [SerializeField] public GameObject onSel;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}