using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Rain.Core;
using Rain.UI;
using UnityEngine;
using static SettingField;
using static TechRoot;

public partial class TechRoot : BaseUI
{

    TechState techState;        // 科技状态
    TechRootData techRootData;     // 科技配置
    TechConfig TechConfig { get { return techRootData.techConfig; } }

    public void Init(TechRootData _techRootData)
    {
        //data
        techRootData = _techRootData;
        techState = RefreshTechState();

        //UI
        ui.txName_Text.text = TechConfig.TechName;
        ui.upLine.SetActive(TechConfig.TechLevel != 1);
        ui.downLine.SetActive(TechConfig.TechLevel != techRootData.techCategoryConfig.MaxLevel);
        ui.btDetail_Button.SetButton(OnClickDetail);
        RefreshUI();
    }

    void RefreshUI()
    {
        ui.bgLock.SetActive(techState == TechState.Lock);
        ui.bgStudied.SetActive(techState == TechState.Studied);
        ui.bgCanStudy.SetActive(techState == TechState.CanStudy);
    }

    TechState RefreshTechState()
    {
        if (DataMgr.Ins.playerData.techs.Contains(TechConfig.TechID))
        {
            return TechState.Studied;
        }
        if (TechConfig.TechLevel == 1)
        {
            techState = TechState.CanStudy; ;
        }
        foreach (string _techID in TechConfig.PreTechs)
        {
            if (!DataMgr.Ins.playerData.techs.Contains(_techID))
            {
                return TechState.Lock;
            }
        }
        return TechState.CanStudy;
    }

    /// <summary>
    /// 点击详情
    /// </summary>
    void OnClickDetail()
    {
        UIMgr.Ins.OpenView<TechDetailView>();
    }

    enum TechState
    {
        Lock,       //未解锁
        Studied,    //已经研究
        CanStudy,   //可研究
    }

    public class TechRootData
    {
        public TechConfig techConfig;
        public TechCategoryConfig techCategoryConfig;
    }
}
