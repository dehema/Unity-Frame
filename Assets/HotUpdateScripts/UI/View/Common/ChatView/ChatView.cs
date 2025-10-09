using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 聊天
/// </summary>
public partial class ChatView : BaseView
{
    ChatViewParam param;
    ObjPool poolChatMenu;
    UIControlDemo_DynamicTableData tableData = new UIControlDemo_DynamicTableData();

    public override void Init(IViewParam _viewParam = null)
    {
        base.Init(_viewParam);
        param = _viewParam as ChatViewParam;
        InitUI();

    }

    public override void OnOpen(IViewParam _viewParam = null)
    {
        base.OnOpen(_viewParam);
    }

    public override void OnClose(Action _cb)
    {
        base.OnClose(_cb);
    }

    void InitUI()
    {
        ui.btClose_Button.SetButton(Close);
        ui.btSendMsg_Button.SetButton(OnClickSendMsg);
        poolChatMenu = PoolMgr.Ins.CreatePool(ui.tgChatMenu);
        InitChatMenu();
    }

    void InitChatMenu()
    {
        poolChatMenu.CollectAll();
        AddChatMenu(ChatMenuType.World);
        AddChatMenu(ChatMenuType.Alliance);
        AddChatMenu(ChatMenuType.Person);
        SetData();
        GameObject firstTag = poolChatMenu.ShowPool.First();
        firstTag.GetComponent<Toggle>().isOn = true;
        firstTag.GetComponent<Tab>().Select();
    }

    private void AddChatMenu(ChatMenuType _chatMenuType)
    {
        //UI
        GameObject item = poolChatMenu.Get();
        Toggle toggle = item.GetComponent<Toggle>();
        Tab tab = item.GetComponent<Tab>();

        item.transform.Find("text").GetComponent<TextMeshProUGUI>().text = ChatMgr.Ins.GetChatMenuName(_chatMenuType);
        item.transform.Find("line").gameObject.SetActive(_chatMenuType != ChatMenuType.Person);

        //data
        ui.chatContent_TabController.AddTab(tab, ui.tabPage_TabPage);
        toggle.SetToggle((_ison) =>
        {
            if (!_ison)
                return;
            ui.chatContent_TabController.GetTab((int)_chatMenuType).Select();
        });
    }

    public void SetData()
    {
        int tabCount = ui.chatContent_TabController.GetTabCount();
        for (int i = 0; i < tabCount; i++)
        {
            UIControlDemo_DynamicTabData tabData = new UIControlDemo_DynamicTabData();
            TechCategoryConfig techCategoryConfig = ConfigMgr.TechCategory.DataList[i];
            TechCategory techCategory = techCategoryConfig.Category;
            for (int chatIndex = 0; chatIndex < ChatMgr.Ins.dataList.Count; chatIndex++)
            {
                ChatData chatData = ChatMgr.Ins.dataList[chatIndex]; ;
                ChatItemData techRowData = new ChatItemData();
                techRowData.index = i;
                techRowData.msg = chatData.msg;
                tabData.AddItem(techRowData);
            }
            tableData.category.Add(tabData);
            Tab tab = ui.chatContent_TabController.GetTab(i);
            tab.SetData(tabData);
        }
    }

    void OnClickSendMsg()
    {
        string msg = ui.inputMsg_Input.text;
        if (string.IsNullOrEmpty(msg))
            return;
        ChatData chatData = new ChatData();
        chatData.msg = msg;
        ChatMgr.Ins.dataList.Insert(0, chatData);

        //UI
        SetData();
    }
}

public class ChatViewParam : IViewParam
{

}
