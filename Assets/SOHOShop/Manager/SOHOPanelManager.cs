using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHOPanelManager : MonoBehaviour
{
    public static SOHOPanelManager instance;

    public enum SourcePanel { RedeemPanel, GoldAndAmazonPanel}

    public enum SourceButton { Cashout, Revise }
    [HideInInspector]
    public SourcePanel sourcePanelName;
    [HideInInspector]
    public SourceButton sourcePanelButton;
    public CashRedeem cashoutCashRedeem;
    public GoldRedeem cashoutGoldRedeem;

    private void Awake()
    {
        instance = this;
    }

    public void InitSourcePanel(SourcePanel _name, SourceButton _button, CashRedeem cashRedeem, GoldRedeem goldRedeem)
    {
        sourcePanelName = _name;
        sourcePanelButton = _button;
        cashoutCashRedeem = cashRedeem;
        cashoutGoldRedeem = goldRedeem;
    } 
}
