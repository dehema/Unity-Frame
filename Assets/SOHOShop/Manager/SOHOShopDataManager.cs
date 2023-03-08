using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOHOShopDataManager : MonoBehaviour
{
    public static SOHOShopDataManager instance;

    public ShopJson shopJson;

    public UserAccount currentUserAccount;     // 当前用户账户
    // 用户地址
    [HideInInspector]
    public string PersonName;
    [HideInInspector]
    public string PersonAddress;
    [HideInInspector]
    public string PersonTel;

    private void Awake()
    {
        instance = this;
    }

    public void InitShopData()
    {
        TextAsset json = Resources.Load<TextAsset>("SOHOShop/LocationJson/ShopJson");
        shopJson = JsonMapper.ToObject<ShopJson>(json.text);

        // 用户账户
        string savedUserAccount = SaveDataManager.GetString(SOHOShopConst.sv_UserAccount);
        if (!string.IsNullOrEmpty(savedUserAccount))
        {
            currentUserAccount = JsonMapper.ToObject<UserAccount>(savedUserAccount);
        }

        // 用户信息
        PersonName = SaveDataManager.GetString(SOHOShopConst.sv_PersonName);
        PersonAddress = SaveDataManager.GetString(SOHOShopConst.sv_PersonAddress);
        PersonTel = SaveDataManager.GetString(SOHOShopConst.sv_PersonTel);

        // 初始化绿币提现数据
        CashRedeemManager.instance.InitCashWithdraw();
        // 初始化浸提提现数据
        GoldRedeemManager.instance.initGoldAmazonRedeemList();
        // 初始化碎片数据
        SOHOPuzzleManager.instance.initPuzzle();
    }


    /// <summary>
    /// 修改当前用户账户
    /// </summary>
    public void ReviseUserAccount(UserAccount newUserAccount)
    {
        currentUserAccount = newUserAccount;
        SaveDataManager.SetString(SOHOShopConst.sv_UserAccount, JsonMapper.ToJson(currentUserAccount));
        // 修改现金提现记录
        CashRedeemManager.instance.ChangeUserAccount();
        // 修改金币提现记录
        GoldRedeemManager.instance.ChangeUserAccount();
    }

    /// <summary>
    /// 保存用户地址
    /// </summary>
    public void SavePersonInformation(string name, string address, string tel)
    {
        PersonName = name;
        PersonAddress = address;
        PersonTel = tel;
        SaveDataManager.SetString(SOHOShopConst.sv_PersonName, PersonName);
        SaveDataManager.SetString(SOHOShopConst.sv_PersonAddress, PersonAddress);
        SaveDataManager.SetString(SOHOShopConst.sv_PersonTel, PersonTel);
    }

}

// 商店数据
public class ShopJson
{
    public string email;
    public int cash_withdraw_time { get; set; }
    public GoldRedeem[] withdraw_group { get; set; }
    public Puzzle[] puzzle_shop_group { get; set; }
    public Dictionary<string, double> puzzle_multi_group { get; set; }
}


