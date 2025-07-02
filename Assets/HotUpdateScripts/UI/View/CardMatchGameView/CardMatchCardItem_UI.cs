using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class CardMatchCardItem : BasePoolItem
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject icon;
        [SerializeField] public Image icon_Image;
        [SerializeField] public GameObject board;
        [SerializeField] public GameObject back;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}