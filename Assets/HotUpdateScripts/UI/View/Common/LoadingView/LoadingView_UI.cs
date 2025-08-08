using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class LoadingView : BaseView
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
    public GameObject imgSlider;
    [HideInInspector]
    public Image imgSlider_Image;
    [HideInInspector]
    public GameObject txtProgress;
    [HideInInspector]
    public Text txtProgress_Text;
    [HideInInspector]
    public GameObject txtLoading;
    [HideInInspector]
    public Text txtLoading_Text;
}
