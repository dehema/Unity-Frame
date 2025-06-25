using UnityEngine;

public partial class ExampleWidgetWithSuper : ExampleWidgetSuper
{
    [HideInInspector]
    public GameObject txt;

    internal void _LoadUI()
    {
        txt = transform.Find("$txt").gameObject;
    }
}
