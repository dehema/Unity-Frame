using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;
using TMPro;

public partial class TechRoot : Rain.UI.BaseUI
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bgLock;
        [SerializeField] public GameObject bgStudied;
        [SerializeField] public GameObject bgCanStudy;
        [SerializeField] public GameObject icon;
        [SerializeField] public Image icon_Image;
        [SerializeField] public GameObject txName;
        [SerializeField] public TextMeshProUGUI txName_Text;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}