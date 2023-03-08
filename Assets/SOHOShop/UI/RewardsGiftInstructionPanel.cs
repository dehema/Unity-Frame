using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardsGiftInstructionPanel : BaseUIForms
{
    public Button CloseBtn;
    // Start is called before the first frame update
    void Start()
    {
        CloseBtn.onClick.AddListener(() => 
        {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
