using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
public class PostEventScript : MonoSingleton<PostEventScript>
{
    public string version = "1.2";
    public string GameCode = NetInfoMgr.Ins.GameCode;
    //channel
#if UNITY_IOS
    private string Channel = "AppStore";
#elif UNITY_ANDROID
    private string Channel = "GooglePlay";
#else
    private string Channel = "GooglePlay";
#endif


    private void OnApplicationPause(bool pause)
    {
        PostEventScript.GetInstance().sendGameProgress();
    }

    public Text text;

    protected void Awake()
    {
        version = Application.version;
        StartCoroutine("autoMessage");
    }
    IEnumerator autoMessage()
    {
        while (true)
        {
            yield return new WaitForSeconds(300f);
            PostEventScript.GetInstance().sendGameProgress();
        }
    }
    private void Start()
    {
        if (SaveDataManager.GetInt("event_day") != DateTime.Now.Day && SaveDataManager.GetString("user_servers_id").Length != 0)
        {
            SaveDataManager.SetInt("event_day", DateTime.Now.Day);
        }
    }
    public void SendNoParaEvent(string event_id)
    {
        SendEvent(event_id);
    }
    public void sendGameProgress(List<string> valueList = null)
    {
        if (valueList == null)
        {
            valueList = new List<string>();
        }
        valueList.Clear();
        valueList.Add(DataMgr.Ins.playerData.gold.Value.ToString());
        valueList.Add(DataMgr.Ins.playerData.cash.Value.ToString());
        valueList.Add(((int)Time.time).ToString());

        if (SaveDataManager.GetString(CConfig.sv_LocalServerId) == null)
        {
            return;
        }
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("gameCode", GameCode);
        wwwForm.AddField("userId", SaveDataManager.GetString(CConfig.sv_LocalServerId));

        wwwForm.AddField("gameVersion", version);

        wwwForm.AddField("channel", Channel);

        for (int i = 0; i < valueList.Count; i++)
        {
            wwwForm.AddField("resource" + (i + 1), valueList[i]);
        }



        StartCoroutine(SendPost(NetInfoMgr.Ins.BaseUrl + "/api/client/game_progress", wwwForm,
        (error) =>
        {
            Debug.Log(error);
        },
        (message) =>
        {
            Debug.Log(message);
        }));
    }

    public void SendEvent(PostEventType event_id, object p1 = null, object p2 = null, object p3 = null)
    {
        SendEvent(((int)event_id).ToString(), p1?.ToString(), p2?.ToString(), p3?.ToString());
    }

    public void SendEvent(string event_id, string p1 = null, string p2 = null, string p3 = null)
    {
        if (text != null)
        {
            if (int.Parse(event_id) < 9100 && int.Parse(event_id) >= 9000)
            {
                if (p1 == null)
                {
                    p1 = "";
                }
                text.text += "\n" + DateTime.Now.ToString() + "id:" + event_id + "  p1:" + p1;
            }
        }
        if (SaveDataManager.GetString(CConfig.sv_LocalServerId) == null)
        {
            NetInfoMgr.Ins.Login();
            return;
        }
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("gameCode", GameCode);
        wwwForm.AddField("userId", SaveDataManager.GetString(CConfig.sv_LocalServerId));
        //Debug.Log("userId:" + SaveDataManager.GetString(CConfig.sv_LocalServerId));
        wwwForm.AddField("version", version);
        //Debug.Log("version:" + version);
        wwwForm.AddField("channel", Channel);
        //Debug.Log("channel:" + channal);
        wwwForm.AddField("operateId", event_id);
        Debug.Log("operateId:" + event_id);


        if (p1 != null)
        {
            wwwForm.AddField("params1", p1);
        }
        if (p2 != null)
        {
            wwwForm.AddField("params2", p2);
        }
        if (p3 != null)
        {
            wwwForm.AddField("params3", p3);
        }
        StartCoroutine(SendPost(NetInfoMgr.Ins.BaseUrl + "/api/client/log", wwwForm,
        (error) =>
        {
            Debug.Log(error);
        },
        (message) =>
        {
            Debug.Log(message);
        }));
    }
    IEnumerator SendPost(string _url, WWWForm wwwForm, Action<string> fail, Action<string> success)
    {
        //Debug.Log(SerializeDictionaryToJsonString(dic));
        UnityWebRequest request = UnityWebRequest.Post(_url, wwwForm);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isNetworkError)
        {
            fail(request.error);
            endRequest();
        }
        else
        {
            success(request.downloadHandler.text);
            endRequest();
        }
    }
    private void endRequest()
    {
        StopCoroutine("SendGet");
    }


}