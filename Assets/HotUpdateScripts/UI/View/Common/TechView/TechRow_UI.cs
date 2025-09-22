using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rain.UI;
using Rain.Core;

public partial class TechRow : Rain.Core.InfiniteScrollItem
{
    [System.Serializable]
    struct SerializableUIComponents
    {
        [SerializeField] public GameObject topLine;
        [SerializeField] public GameObject roots;
        [SerializeField] public HorizontalLayoutGroup roots_Hor;
    }

    [Header("自动序列化组件")]
    [SerializeField] private SerializableUIComponents ui;
}