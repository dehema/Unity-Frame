using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Rain.Core;
using Rain.UI;
using UnityEngine;

public partial class TechRoot : BaseUI
{

    TechState techState;        // �Ƽ�״̬
    TechRootData techRootData;     // �Ƽ�����
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
        return PlayerMgr.Ins.GetTechState(TechConfig.TechID);
    }

    /// <summary>
    /// �������
    /// </summary>
    void OnClickDetail()
    {
        UIMgr.Ins.OpenView<TechDetailView>();
    }

    public class TechRootData
    {
        public TechConfig techConfig;
        public TechCategoryConfig techCategoryConfig;
    }
}
