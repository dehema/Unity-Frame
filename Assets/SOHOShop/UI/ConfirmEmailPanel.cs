using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmEmailPanel : BaseUIForms
{
    public Text RewardText;
    public Text AccountText;
    public Text EmailText;
    public Button ConfirmBtn;
    public Button ReviseBtn;

    private void Start()
    {
        ConfirmBtn.onClick.AddListener(() =>
        {
            if (SOHOPanelManager.instance.sourcePanelName == SOHOPanelManager.SourcePanel.RedeemPanel)
            {
                CashRedeemManager.instance.CheckCashWithdraw(SOHOPanelManager.instance.cashoutCashRedeem.id);
            }
            else
            {
                GoldRedeemManager.instance.CashOutGoldRedeem(SOHOPanelManager.instance.cashoutGoldRedeem.id);
            }

            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });

        ReviseBtn.onClick.AddListener(() => {
            SOHOPanelManager.instance.sourcePanelButton = SOHOPanelManager.SourceButton.Revise;
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("WithdrawPanel"));
        });

        // 更新用户账户
        MessageCenterLogic.GetInstance().Register(SOHOShopConst.mg_RefreshCashWithdrawUserAccount, (md) => {
            AccountText.text = "Please confirm " + SOHOShopDataManager.instance.currentUserAccount.method.ToString() + " accout:";
            EmailText.text = SOHOShopDataManager.instance.currentUserAccount.email;
        });
    }

    public override void Display()
    {
        base.Display();

        if (SOHOShopDataManager.instance.currentUserAccount == null)
        {
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("WithdrawPanel"));
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        }
        else
        {
            AccountText.text = "Please confirm " + SOHOShopDataManager.instance.currentUserAccount.method.ToString() +  " accout:";
            EmailText.text = SOHOShopDataManager.instance.currentUserAccount.email;
        }

        if (SOHOPanelManager.instance.sourcePanelName == SOHOPanelManager.SourcePanel.RedeemPanel)
        {
            RewardText.text = "$" + NumberUtil.DoubleToStr(SOHOPanelManager.instance.cashoutCashRedeem.cashout);
        }
        else
        {
            RewardText.text = "$" + NumberUtil.DoubleToStr(SOHOPanelManager.instance.cashoutCashRedeem.cashout);
        }

    }
}
