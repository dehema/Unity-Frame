using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldAmazonRedeemItem : MonoBehaviour
{
    public Image PayImage;
    public GameObject State_1;
    public GameObject GoldImage;
    public GameObject AmazonImage;
    public Image ProgressImage;
    public Text CashoutText;
    public Button CashOutBtn;
    
    public GameObject State_2;
    public Text RankText;

    private double goldOrAmazon;
    private GoldRedeem goldRedeemItem;

    private void Start()
    {
        CashOutBtn.onClick.AddListener(() =>
        {
            if (goldOrAmazon >= goldRedeemItem.num)
            {
                SOHOPanelManager.instance.InitSourcePanel(SOHOPanelManager.SourcePanel.GoldAndAmazonPanel, SOHOPanelManager.SourceButton.Cashout, null, goldRedeemItem);
                if (SOHOShopDataManager.instance.currentUserAccount == null)
                {
                    UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("WithdrawPanel"));
                }
                else
                {
                    UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("ConfirmEmailPanel"));
                }
            }
            else
            {
                ToastManager.GetInstance().ShowToast(goldRedeemItem.type == "gold" ? "Gold is not enough" : "Amazon is not enough");
            }
            
        });
    }

    public void InitItem(GoldRedeem item, double gold, double amazon, int index)
    {
        goldRedeemItem = item;

        UserAccount currentUserAccount = SOHOShopDataManager.instance.currentUserAccount;
        if (currentUserAccount == null)
        {
            PayImage.sprite = Resources.Load<Sprite>("SOHOShop/UI_Redeem/UI_Pay/" + UserAccount.WithdrawMethod.PayPal.ToString());
        }
        else
        {
            PayImage.sprite = Resources.Load<Sprite>("SOHOShop/UI_Redeem/UI_Pay/" + currentUserAccount.method.ToString());
        }

        
        if (goldRedeemItem.state == Redeem.RedeemState.Init)
        {
            State_1.SetActive(true);
            State_2.SetActive(false);
            GameObject ActiveObj = item.type == "gold" ? GoldImage : AmazonImage;
            goldOrAmazon = item.type == "gold" ? gold : amazon;
            ActiveObj.SetActive(true);
            ActiveObj.transform.Find("Text").GetComponent<Text>().text = NumberUtil.DoubleToStr(goldOrAmazon) + "/" + item.num;
            ProgressImage.fillAmount = (float)(goldOrAmazon / item.num);
            CashoutText.text = "$" + item.cashout;

            CashOutBtn.gameObject.SetActive(item.state == Redeem.RedeemState.Init);

        } else if (goldRedeemItem.state == Redeem.RedeemState.Waiting)
        {
            State_1.SetActive(false);
            State_2.SetActive(true);
            RankText.text = "Your current queue position is: " + goldRedeemItem.rank;
        }
        
    }

}
