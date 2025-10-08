using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

public class ChatMgr : MonoSingleton<ChatMgr>
{
    public List<ChatData> dataList = new List<ChatData>();
    public ChatMgr()
    {
        for (int chatIndex = 0; chatIndex < 100; chatIndex++)
        {
            ChatData chatData = new ChatData();
            chatData.msg = "������ǵ��˵ڼ�����";
            dataList.Add(chatData);
        }
    }


    /// <summary>
    /// ����˵�����
    /// </summary>
    /// <param name="chatMenuType"></param>
    /// <returns></returns>
    public string GetChatMenuName(ChatMenuType chatMenuType)
    {
        switch (chatMenuType)
        {
            case ChatMenuType.World:
            default:
                return "����";
            case ChatMenuType.Alliance:
                return "����";
            case ChatMenuType.Person:
                return "����";
        }
    }
}
