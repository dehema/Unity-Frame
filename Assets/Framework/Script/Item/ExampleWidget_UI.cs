using UnityEngine;

public partial class ExampleWidget : BaseUI
{
    [HideInInspector]
    public GameObject txt;

    internal void _LoadUI()
    {
        txt = transform.Find("$txt").gameObject;
    }
}
