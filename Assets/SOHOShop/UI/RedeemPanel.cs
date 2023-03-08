using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedeemPanel : BaseUIForms
{
    public Button HomeButton;
    public Button ReviseButton;
    public Button RecordButton;
    public Button InstructionsButton;
    public Transform Container;
    public Image UserAccountMathodImg;
    public Text UserAccountEmailText;
    public HorizontalLayoutGroup layout;

    private void Start()
    {
        HomeButton.onClick.AddListener(() =>
        {
            CloseUIForm(SOHOShopUtil.PanelName(GetType().Name));
        });

        // 修改账户按钮
        ReviseButton.onClick.AddListener(() => {
            SOHOPanelManager.instance.InitSourcePanel(SOHOPanelManager.SourcePanel.RedeemPanel, SOHOPanelManager.SourceButton.Revise, null, null);
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("WithdrawPanel"));
        });

        // 提现历史按钮
        RecordButton.onClick.AddListener(() => {
            StartCoroutine(openRecordPanel());
        });

        InstructionsButton.onClick.AddListener(() => {
            UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("InstructionsPanel"));
        });

        MessageCenterLogic.GetInstance().Register(SOHOShopConst.mg_RefreshCashWithdrawList, (md) => {
            refreshList();
        });

        MessageCenterLogic.GetInstance().Register(SOHOShopConst.mg_RefreshCashWithdrawUserAccount, (md) => {
            refreshUserAccount();
        });

    }

    public override void Display()
    {
        base.Display();
        Debug.Log("public override void Display()");
        CashRedeemManager.instance.InitCashWithdraw();

        refreshList();
        refreshUserAccount();

        // 打点
        PostEventScript.GetInstance().SendEvent("1301", NumberUtil.DoubleToStr(SOHOShopManager.Ins.getCashAction()));

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

    private void refreshList()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        int childCount = Container.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(Container.GetChild(0).gameObject);
            }
        }

        for (int i = CashRedeemManager.instance.CashRedeemList.Count; i > 0; i--)
        {
            CashRedeem item = CashRedeemManager.instance.CashRedeemList[i - 1];
            if (item.state != Redeem.RedeemState.Complete)
            {
                GameObject redeemItem = Instantiate(Resources.Load<GameObject>("SOHOShop/UIPanel/" + (CommonUtil.IsPortrait() ? "Portrait" : "Landscape") + "/RedeemItem"), Container);
                redeemItem.GetComponent<RedeemItem>().InitRedeemItem(item);
            }
        }

        if (!CommonUtil.IsPortrait())
        {
            if (CashRedeemManager.instance.CashRedeemList.Count == 1)
            {
                layout.childAlignment = TextAnchor.MiddleCenter;
                layout.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleCenter);
                layout.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleCenter);
                layout.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
            else
            {
                layout.childAlignment = TextAnchor.MiddleLeft;
                layout.GetComponent<RectTransform>().SetAnchor(AnchorPresets.MiddleLeft);
                layout.GetComponent<RectTransform>().SetPivot(PivotPresets.MiddleLeft);
                layout.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            }
        }

        Container.GetComponent<RectTransform>().sizeDelta = new Vector2(Container.GetComponent<RectTransform>().sizeDelta.x, 430 * CashRedeemManager.instance.CashRedeemList.Count);

    }


    private IEnumerator openRecordPanel()
    {
        UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("RecordPanel"));
        yield return new WaitForSeconds(0.1f);
        MessageCenterLogic.GetInstance().Send(SOHOShopConst.mg_SOHORecordList, new MessageData("cash"));
    }

    public override void Hidding()
    {
        base.Hidding();
    }
}
