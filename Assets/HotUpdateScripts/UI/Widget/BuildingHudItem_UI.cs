using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class BuildingHudItem : BasePoolItem
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject BuildingProgress;
        [SerializeField] public Slider BuildingProgress_Slider;
        [SerializeField] public GameObject lbBuildingProgress;
        [SerializeField] public TextMeshProUGUI lbBuildingProgress_Text;
        [SerializeField] public GameObject btTips;
        [SerializeField] public Button btTips_Button;
        [SerializeField] public GameObject imgTips;
        [SerializeField] public Image imgTips_Image;
        [SerializeField] public GameObject BuildingName;
        [SerializeField] public TextMeshProUGUI BuildingName_Text;
        [SerializeField] public GameObject BuildingLevel;
        [SerializeField] public TextMeshProUGUI BuildingLevel_Text;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}