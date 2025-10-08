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
            chatData.msg = "这次我们得了第几名？";
            dataList.Add(chatData);
        }
    }


    /// <summary>
    /// 聊天菜单名称
    /// </summary>
    /// <param name="chatMenuType"></param>
    /// <returns></returns>
    public string GetChatMenuName(ChatMenuType chatMenuType)
    {
        switch (chatMenuType)
        {
            case ChatMenuType.World:
            default:
                return "世界";
            case ChatMenuType.Alliance:
                return "联盟";
            case ChatMenuType.Person:
                return "个人";
        }
    }
}
