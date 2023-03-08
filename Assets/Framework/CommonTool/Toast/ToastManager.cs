using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastManager : MonoSingleton<ToastManager>
{
    public string Info;

    public void ShowToast(string info)
    {
        Info = info;
        UIManager.GetInstance().ShowUIForms("Toast");
    }
}
