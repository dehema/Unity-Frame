using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用户提现账户
/// </summary>
public class UserAccount
{
    // 用户账户提现类型
    public enum WithdrawMethod { PayPal, CashApp, Paytm, Tez, Coinbase, BancoInter, Nubank, LinePay, Alipay, Sberbank }

    public WithdrawMethod method { get; set; }
    public string email { get; set; }
}
