using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddressPanel : BaseUIForms
{
    public Button CloseBtn;
    public Button ConfirmBtn;
    public Button LaterBtn;
    public InputField NameText;
    public InputField AddressText;
    public InputField TelText;

    // Start is called before the first frame update
    void Start()
    {
        CloseBtn.onClick.AddListener(() => {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });
        LaterBtn.onClick.AddListener(() => {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });

        ConfirmBtn.onClick.AddListener(() =>
        {
            SOHOShopDataManager.instance.SavePersonInformation(NameText.text, AddressText.text, TelText.text);
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });
    }

    public override void Display()
    {
        base.Display();

        NameText.text = SOHOShopDataManager.instance.PersonName;
        AddressText.text = SOHOShopDataManager.instance.PersonAddress;
        TelText.text = SOHOShopDataManager.instance.PersonTel;
    }

}
