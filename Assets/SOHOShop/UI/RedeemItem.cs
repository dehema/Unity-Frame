using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedeemItem : MonoBehaviour
{
    public Button CashOutBtn;
    public GameObject State_1;
    public GameObject State_2;
    public GameObject State_3;
    public GameObject State_4;
    public Text State_1_CountDownText;  // 新建状态的提现记录的倒计时
    public Image State_3_FillAmount;
    public Text State_3_ProcessText;
    public Text State_3_DescText;
    public Text State_4_RankText;
    public Button State_4_ItemLong;
    public Text State_4_ItemLongText1;
    public Text State_4_ItemLongText2;
    public Button State_4_DetailBtn;

    private CashRedeem cashRedeem;
    private long init_countdown;         // 距离可提现倒计时
    private Coroutine countdownCoroutinue;

    // Start is called before the first frame update
    void Start()
    {
        // 提现按钮点击
        CashOutBtn.onClick.AddListener(() => {
            SOHOPanelManager.instance.InitSourcePanel(SOHOPanelManager.SourcePanel.RedeemPanel, SOHOPanelManager.SourceButton.Cashout, cashRedeem, null);
            if (SOHOShopDataManager.instance.currentUserAccount == null)
            {
                UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("WithdrawPanel"));
            }
            else
            {
                UIManager.GetInstance().ShowUIForms(SOHOShopUtil.PanelName("ConfirmEmailPanel"));
            }
        });

        State_4_DetailBtn.onClick.AddListener(() =>
        {
            State_4_DetailBtn.gameObject.SetActive(false);
            State_4_ItemLong.gameObject.SetActive(true);
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 590f);
        });
        State_4_ItemLong.onClick.AddListener(() =>
        {
            State_4_DetailBtn.gameObject.SetActive(true);
            State_4_ItemLong.gameObject.SetActive(false);
            GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 430);
        });
    }

    public void InitRedeemItem(CashRedeem cashItem)
    {
        cashRedeem = cashItem;

        State_1.SetActive(false);
        State_2.SetActive(false);
        State_3.SetActive(false);
        State_4.SetActive(false);

        GameObject SelectedState = State_1;
        Redeem.RedeemState state = cashItem.state;
        if (state == Redeem.RedeemState.Init)
        {
            SelectedState = State_1;
            init_countdown = cashItem.endTime - DateUtil.Current();
            if (countdownCoroutinue == null)
            {
                countdownCoroutinue = StartCoroutine(StartCountDown());
            }
        } 
        else if(state == Redeem.RedeemState.Unchecked)
        {
            SelectedState = State_2;
        } 
        else if(state == Redeem.RedeemState.Checked)
        {
            SelectedState = State_3;
            State_3_FillAmount.fillAmount = (float)cashItem.taskValue / SOHOShopConfig.TaskTotalNum;
            string processText = cashItem.taskValue + "/" + SOHOShopConfig.TaskTotalNum;
            State_3_ProcessText.text = processText;
            State_3_DescText.text = "Please Watch " + processText + " AD to approve order";
        }
        else if (state == Redeem.RedeemState.Waiting)
        {
            SelectedState = State_4;
            State_4_RankText.text = "Your current queue position is : " + cashItem.rank;
            State_4_ItemLong.gameObject.SetActive(false);
            State_4_DetailBtn.gameObject.SetActive(true);
            State_4_ItemLongText1.text = "You have collected $" + NumberUtil.DoubleToStr(cashItem.cashout) + " Within 01:00:00";
            State_4_ItemLongText2.text = "There are " + (cashItem.rank - 1) + " players in front. Your position will be refreshed every minute.Thank you for your patience";
        }

        if (SelectedState != null)
        {
            SelectedState.SetActive(true);
            SelectedState.transform.Find("BalanceNumber").GetComponent<Text>().text = "$ " + NumberUtil.DoubleToStr(cashItem.cashout);
            UserAccount userAccount = cashItem.userAccount;

            if (userAccount != null)
            {
                SelectedState.transform.Find("PayPalImage").gameObject.SetActive(true);
                SelectedState.transform.Find("PayPalImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("SOHOShop/UI_Redeem/UI_Pay/" + userAccount.method.ToString());
            }
            else
            {
                SelectedState.transform.Find("PayPalImage").gameObject.SetActive(false);
            }
            
        }

    }

    // 未到可以提现时间，则开启倒计时
    private IEnumerator StartCountDown()
    {
        while(init_countdown > 0)
        {
            init_countdown--;
            State_1_CountDownText.text = DateUtil.SecondsFormat(init_countdown);
            yield return new WaitForSeconds(1f);
            
            if (init_countdown <= 0)
            {
                CashRedeemManager.instance.FinishInitCountDown(cashRedeem);
            }
        }
    }
  

}
