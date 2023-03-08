using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum TargetType
{
    Scene,
    UGUI
}
public enum LayoutType
{
    Sprite_First_Weight,
    Sprite_First_Height,
    Screen_First_Weight,
    Screen_First_Height,
    Bottom,
    Top,
    Left,
    Right
}
public enum RunTime
{
    Awake,
    Start,
    None
}
public class AutoLayout : MonoBehaviour
{
    public TargetType Target_Type;
    public LayoutType Layout_Type;
    public RunTime Run_Time;
    public float Layout_Number;
    private void Awake()
    {
        if (Run_Time == RunTime.Awake)
        {
            layoutAction();
        }
    }
    private void Start()
    {
        if (Run_Time == RunTime.Start)
        {
            layoutAction();
        }
    }

    public void layoutAction()
    {
        if (Layout_Type == LayoutType.Sprite_First_Weight)
        {
            if (Target_Type == TargetType.UGUI)
            {

                float scale = Screen.width / Layout_Number;
                //GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.width / w * h);
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        if (Layout_Type == LayoutType.Screen_First_Weight)
        {
            if (Target_Type == TargetType.Scene)
            {
                float scale = GetSystemData.GetInstance().getCameraWidth() / Layout_Number;
                transform.localScale = transform.localScale * scale;
            }
        }
        
        if (Layout_Type == LayoutType.Bottom)
        {
            if (Target_Type == TargetType.Scene)
            {
                float screen_bottom_y = GetSystemData.GetInstance().getCameraHeight() / -2;
                screen_bottom_y += (Layout_Number + (GetSystemData.GetInstance().getSpriteSize(gameObject).y / 2f));
                transform.position = new Vector3(transform.position.x, screen_bottom_y, transform.position.y);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
