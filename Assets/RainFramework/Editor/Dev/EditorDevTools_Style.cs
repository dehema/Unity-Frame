using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EditorDevTools_Style
{
    public GUIStyle lb;
    public GUIStyle lbTitle;
    public GUIStyle bt;
    public GUIStyle btLarge;

    public EditorDevTools_Style(GUISkin _skin)
    {
        lb = new GUIStyle(_skin.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
        lbTitle = new GUIStyle(lb) { fontSize = 16 };

        bt = new GUIStyle(_skin.button) { fontSize = 12, alignment = TextAnchor.MiddleCenter, hover = { textColor = Color.green } };
        btLarge = new GUIStyle(bt) { fontSize = 14, padding = new RectOffset(20, 20, 0, 0) };
    }
}
