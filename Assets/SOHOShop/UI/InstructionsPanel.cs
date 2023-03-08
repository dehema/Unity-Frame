using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsPanel : BaseUIForms
{
    public Button CloseBtn;
    public Button LaterBtn;
    // Start is called before the first frame update
    void Start()
    {
        CloseBtn.onClick.AddListener(() => 
        {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });
        LaterBtn.onClick.AddListener(() => {
            openEmail();
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 发邮件
    /// </summary>
    public void openEmail()
    {
        //Debug.Log("发邮件");
        Uri uri = new Uri(string.Format("mailto:{0}?subject={1}", SOHOShopDataManager.instance.shopJson.email, Application.productName));//第二个参数是邮件的标题
        Application.OpenURL(uri.AbsoluteUri);
    }
}
