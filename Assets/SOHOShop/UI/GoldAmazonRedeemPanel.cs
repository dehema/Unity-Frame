using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldAmazonRedeemPanel : BaseUIForms
{
    public Transform Container;
    public Button BackButton;
    public Button RecordButton;
    public Button InstructionsButton;
    public Image UserAccountMathodImg;
    public Text UserAccountEmailText;
    public Button ReviseButton;

    // Start is called before the first frame update
    void Start()
    {
        InstructionsButton.onClick.AddListener(() => 
        {
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("InstructionsPanel"));
        });
        BackButton.onClick.AddListener(() => {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });

        RecordButton.onClick.AddListener(() => {

            StartCoroutine(openRecordPanel());
        });

        // 修改账户按钮
        ReviseButton.onClick.AddListener(() => {
            SOHOPanelManager.instance.InitSourcePanel(SOHOPanelManager.SourcePanel.GoldAndAmazonPanel, SOHOPanelManager.SourceButton.Revise, null, null);
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("WithdrawPanel"));
        });

        MessageCenterLogic.GetInstance().Register(SOHOShopConst.mg_RefreshGoldAmazonWithdrawList, (md) =>
        {
            refreshList();
        });

        MessageCenterLogic.GetInstance().Register(SOHOShopConst.mg_RefreshCashWithdrawUserAccount, (md) => {
            refreshUserAccount();
        });
    }

    public override void Display()
    {
        base.Display();

        refreshList();
        refreshUserAccount();

        // 打点
        PostEventScript.GetInstance().SendEvent("1302", NumberUtil.DoubleToStr(SOHOShopManager.Ins.getGoldAction()), NumberUtil.DoubleToStr(SOHOShopManager.Ins.getAmazonAction()));

        // just for Tile Fish
        MessageCenterLogic.GetInstance().Send(CConfig.mg_WindowOpen);
    }

    private void refreshList()
    {
        GoldRedeemManager.instance.updateWaitingRank();

        int childCount = Container.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(Container.GetChild(0).gameObject);
            }
        }

        GameObject RedeemItem = Resources.Load<GameObject>("SOHOShop/UIPanel/" + (CommonUtil.IsPortrait() ? "Portrait" : "Landscape") + "/GoldAmazonRedeemItem");
        double gold = SOHOShopManager.Ins.getGoldAction();
        double amazon = SOHOShopManager.Ins.getAmazonAction();
        for (int i = 0; i < GoldRedeemManager.instance.goldRedeemGroup.Length; i++)
        {
            GoldRedeem item = GoldRedeemManager.instance.goldRedeemGroup[i];
            if (item.state != Redeem.RedeemState.Complete)
            {
                GameObject obj = Instantiate(RedeemItem, Container);
                obj.GetComponent<GoldAmazonRedeemItem>().InitItem(item, gold, amazon, i);
            }
        }
    }

    private void refreshUserAccount()
    {
        UserAccount currentUserAccount = SOHOShopDataManager.instance.currentUserAccount;
        if (currentUserAccount == null)
        {
            UserAccountMathodImg.sprite = Resources.Load<Sprite>("SOHOShop/UI_Redeem/UI_Pay/" + UserAccount.WithdrawMethod.PayPal.ToString());
            UserAccountEmailText.text = "Please enter your withdrawal account";
        }
        else
        {
            UserAccountMathodImg.sprite = Resources.Load<Sprite>("SOHOShop/UI_Redeem/UI_Pay/" + currentUserAccount.method.ToString());
            UserAccountEmailText.text = currentUserAccount.email;
        }
    }

    private IEnumerator openRecordPanel()
    {
        UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("RecordPanel"));
        yield return new WaitForSeconds(0.1f);
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_SOHORecordList, new MessageData("gold"));
    }

    public override void Hidding()
    {
        base.Hidding();
        // just for Tile Fish
        MessageCenterLogic.GetInstance().Send(CConfig.mg_WindowClose);
    }
}
