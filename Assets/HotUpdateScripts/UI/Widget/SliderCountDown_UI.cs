using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class SliderCountDown : BaseUI
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject imgFill;
        [SerializeField] public Image imgFill_Image;
        [SerializeField] public GameObject countDown;
        [SerializeField] public Text countDown_Text;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}