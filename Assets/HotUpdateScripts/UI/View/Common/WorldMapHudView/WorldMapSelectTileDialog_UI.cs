using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class WorldMapSelectTileDialog : Rain.UI.BaseUI
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject imgTile;
        [SerializeField] public Image imgTile_Image;
        [SerializeField] public GameObject pos;
        [SerializeField] public TextMeshProUGUI pos_Text;
        [SerializeField] public GameObject tileName;
        [SerializeField] public TextMeshProUGUI tileName_Text;
        [SerializeField] public GameObject lbAlly;
        [SerializeField] public TextMeshProUGUI lbAlly_Text;
        [SerializeField] public GameObject btMoveCity;
        [SerializeField] public Button btMoveCity_Button;
        [SerializeField] public GameObject btOccupy;
        [SerializeField] public Button btOccupy_Button;
        [SerializeField] public GameObject btClose;
        [SerializeField] public Button btClose_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}