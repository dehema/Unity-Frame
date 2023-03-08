using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ExampleWidget : BaseUI
{
    [HideInInspector]
    public GameObject txt;

    internal void _LoadUI()    
    {
        txt = transform.Find("$txt").gameObject;
    }
}