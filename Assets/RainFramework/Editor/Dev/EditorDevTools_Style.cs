using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EditorDevTools_Style
{
    public static GUIStyle titleLabelStyle;
    public EditorDevTools_Style()
    {
        titleLabelStyle = new GUIStyle() { fontSize = 20, alignment = TextAnchor.MiddleCenter };
    }
}
