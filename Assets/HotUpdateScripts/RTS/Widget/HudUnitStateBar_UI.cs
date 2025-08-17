using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class HudUnitStateBar : BasePoolItem
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject redHp;
        [SerializeField] public Image redHp_Image;
        [SerializeField] public RectTransform redHp_Rect;
        [SerializeField] public GameObject greenHp;
        [SerializeField] public Image greenHp_Image;
        [SerializeField] public RectTransform greenHp_Rect;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}