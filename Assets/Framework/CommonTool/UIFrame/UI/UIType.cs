
/// <summary>
/// 窗体类型 （引用窗体的重要属性[枚举类型]）
/// </summary>
using System;
using UnityEngine;
[Serializable]
public class UIType 
{

    
    /// <summary>
    /// 是否需要清空反向切换
    /// </summary>
    [HideInInspector]
    public bool IsClearReverseChange = false;
    /// <summary>
    /// ui窗体类型   
    /// </summary>
    public UIFormType UIForms_Type = UIFormType.Normal;
    /// <summary>
    /// ui窗体显示类型
    /// </summary>
    public UIFormShowMode UIForm_ShowMode = UIFormShowMode.Normal;
    /// <summary>
    /// ui窗体透明度类型
    /// </summary>
    public UIFormLucenyType UIForm_LucencyType = UIFormLucenyType.Translucence;
    /// <summary>
    /// 
    /// </summary>
    public UIFormShowAnimationType UIForm_animationType = UIFormShowAnimationType.scale;
}
