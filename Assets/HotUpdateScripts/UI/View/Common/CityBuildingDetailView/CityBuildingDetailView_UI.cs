using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class CityBuildingDetailView : Rain.UI.BaseView
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject bg;
        [SerializeField] public Image bg_Image;
        [SerializeField] public Button bg_Button;
        [SerializeField] public GameObject content;
        [SerializeField] public RectTransform content_Rect;
        [SerializeField] public GameObject lbBuildingName;
        [SerializeField] public TextMeshProUGUI lbBuildingName_Text;
        [SerializeField] public GameObject lbBuildingName_1;
        [SerializeField] public TextMeshProUGUI lbBuildingName_1_Text;
        [SerializeField] public GameObject btBuildingLevelUp;
        [SerializeField] public Button btBuildingLevelUp_Button;
        [SerializeField] public GameObject btBuildingLevelUp_1;
        [SerializeField] public Button btBuildingLevelUp_1_Button;
        [SerializeField] public GameObject btBuildingLevelUp_2;
        [SerializeField] public Button btBuildingLevelUp_2_Button;
        [SerializeField] public GameObject btBuildingLevelUp_3;
        [SerializeField] public Button btBuildingLevelUp_3_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}