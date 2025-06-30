using UnityEngine;
using UnityEngine.UI;
using Rain.UI;

public partial class LoadSceneView : BaseView
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
    public GameObject progress;
    [HideInInspector]
    public Slider progress_Slider;
    [HideInInspector]
    public GameObject txtProgress;
    [HideInInspector]
    public Text txtProgress_Text;
}
