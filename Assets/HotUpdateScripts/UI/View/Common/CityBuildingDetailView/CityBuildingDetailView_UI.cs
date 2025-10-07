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
        [SerializeField] public GameObject btBuildingLevelUp;
        [SerializeField] public Button btBuildingLevelUp_Button;
        [SerializeField] public GameObject imgBuildingIcon;
        [SerializeField] public Image imgBuildingIcon_Image;
        [SerializeField] public GameObject lbFurnitureName;
        [SerializeField] public TextMeshProUGUI lbFurnitureName_Text;
        [SerializeField] public GameObject lbFurnitureDesc;
        [SerializeField] public TextMeshProUGUI lbFurnitureDesc_Text;
        [SerializeField] public GameObject btFurnitureLevelUp;
        [SerializeField] public Button btFurnitureLevelUp_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}