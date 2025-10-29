using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class BagViewSelItemDialog : Rain.UI.BaseUI
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject selItemName;
        [SerializeField] public TextMeshProUGUI selItemName_Text;
        [SerializeField] public GameObject selItemDesc;
        [SerializeField] public TextMeshProUGUI selItemDesc_Text;
        [SerializeField] public GameObject btReduceSelNum;
        [SerializeField] public Button btReduceSelNum_Button;
        [SerializeField] public GameObject sliderSelItemNum;
        [SerializeField] public Slider sliderSelItemNum_Slider;
        [SerializeField] public GameObject btAddSelNum;
        [SerializeField] public Button btAddSelNum_Button;
        [SerializeField] public GameObject inputSelItemNum;
        [SerializeField] public TMP_InputField inputSelItemNum_Input;
        [SerializeField] public GameObject btMaxSelNum;
        [SerializeField] public Button btMaxSelNum_Button;
        [SerializeField] public GameObject btSelItemDetail;
        [SerializeField] public Button btSelItemDetail_Button;
        [SerializeField] public GameObject btSelItemUse;
        [SerializeField] public Button btSelItemUse_Button;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}