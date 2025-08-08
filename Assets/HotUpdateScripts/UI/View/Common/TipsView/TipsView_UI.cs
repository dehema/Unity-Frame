using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class TipsView : BaseView
{
    [HideInInspector]
    public GameObject bg;
    [HideInInspector]
    public Image bg_Image;
    [HideInInspector]
    public Button bg_Button;
    [HideInInspector]
    public GameObject content;
    [HideInInspector]
    public RectTransform content_Rect;
    [HideInInspector]
    public GameObject templeteTips;
    [HideInInspector]
    public GameObject commonFloat;
    [HideInInspector]
    public RectTransform commonFloat_Rect;
    [HideInInspector]
    public GameObject txtCommonFloat;
    [HideInInspector]
    public Text txtCommonFloat_Text;
    [HideInInspector]
    public RectTransform txtCommonFloat_Rect;
}
