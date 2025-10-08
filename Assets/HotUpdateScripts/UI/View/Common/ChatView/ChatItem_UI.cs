using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class ChatItem : Rain.Core.InfiniteScrollItem
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject lbName;
        [SerializeField] public TextMeshProUGUI lbName_Text;
        [SerializeField] public GameObject playerIcon;
        [SerializeField] public Image playerIcon_Image;
        [SerializeField] public GameObject lbContent;
        [SerializeField] public TextMeshProUGUI lbContent_Text;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}