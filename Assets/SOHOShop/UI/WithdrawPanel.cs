using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawPanel : BaseUIForms
{
    public InputField EmailText;
    public Button SubmitBtn;
    public Button CancelBtn;
    public GameObject[] ChooseItemList;
    public Text AccountText;

    private int selectedMethodIndex;

    // Start is called before the first frame update
    void Start()
    {
        SubmitBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(EmailText.text))
            {
                // 提示用户填写账户
                ToastManager.GetInstance().ShowToast("Please fill in your account");
            }
            else
            {
                UserAccount userAccount = new UserAccount();
                userAccount.method = (UserAccount.WithdrawMethod)(selectedMethodIndex);
                userAccount.email = EmailText.text;
                SOHOShopDataManager.instance.ReviseUserAccount(userAccount);

                if (SOHOPanelManager.instance.sourcePanelButton == SOHOPanelManager.SourceButton.Cashout 
                && (SOHOPanelManager.instance.sourcePanelName == SOHOPanelManager.SourcePanel.RedeemPanel && SOHOPanelManager.instance.cashoutCashRedeem != null 
                || SOHOPanelManager.instance.sourcePanelName == SOHOPanelManager.SourcePanel.GoldAndAmazonPanel && SOHOPanelManager.instance.cashoutGoldRedeem != null))
                {
                    UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("ConfirmEmailPanel"));
                }

                CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
            }
        });

        CancelBtn.onClick.AddListener(() => {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });

        foreach(GameObject obj in ChooseItemList)
        {
            obj.GetComponent<Button>().onClick.AddListener(() => {
                string indexStr = System.Text.RegularExpressions.Regex.Replace(obj.name, @"[^0-9]+", "");
                int index = indexStr == "" ? 0 : int.Parse(indexStr);
                changeMethod(index);
            });
        }
    }

    public override void Display()
    {
        base.Display();

        if (SOHOShopDataManager.instance.currentUserAccount == null)
        {
            selectedMethodIndex = 0;
            EmailText.text = "";
        }
        else
        {
            EmailText.text = SOHOShopDataManager.instance.currentUserAccount.email;
            selectedMethodIndex = (int)SOHOShopDataManager.instance.currentUserAccount.method;
        }
        AccountText.text = "Please enter your " + ((UserAccount.WithdrawMethod)(selectedMethodIndex)).ToString() + " accout";

        changeMethod(selectedMethodIndex);

    }

    private void changeMethod(int index)
    {
        Sprite selectedBg = Resources.Load<Sprite>("SOHOShop/UI_Redeem/UI_SelectedBG");
        Sprite notSelectedBg = Resources.Load<Sprite>("SOHOShop/UI_Redeem/UI_NotSelectedBG");
        foreach (GameObject item in ChooseItemList)
        {
            item.transform.Find("Image (1)").gameObject.SetActive(false);
            item.GetComponent<Image>().sprite = notSelectedBg;
        }

        selectedMethodIndex = index;
        ChooseItemList[selectedMethodIndex].transform.Find("Image (1)").gameObject.SetActive(true);
        ChooseItemList[selectedMethodIndex].GetComponent<Image>().sprite = selectedBg;

        AccountText.text = "Please enter your " + ((UserAccount.WithdrawMethod)(selectedMethodIndex)).ToString() + " accout";
        if (SOHOShopDataManager.instance.currentUserAccount != null && selectedMethodIndex == (int)SOHOShopDataManager.instance.currentUserAccount.method)
        {
            EmailText.text = SOHOShopDataManager.instance.currentUserAccount.email;
        }
        else
        {
            EmailText.text =  ""; ;
        }
    }
}
