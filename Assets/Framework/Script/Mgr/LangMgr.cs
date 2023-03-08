using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class LangMgr : MonoSingleton<LangMgr>
{
    public SystemLanguage lastSystemLang;
    public SystemLanguage currLang;
    public Dictionary<string, string> langDict = new Dictionary<string, string>();

    public void Init()
    {
        var sl = Application.systemLanguage;
        lastSystemLang = sl;
        if (sl == SystemLanguage.Chinese || sl == SystemLanguage.ChineseSimplified || sl == SystemLanguage.ChineseTraditional)
        {
            currLang = SystemLanguage.Chinese;
        }
        else if (sl == SystemLanguage.Portuguese)
        {
            currLang = SystemLanguage.Portuguese;
        }
        else
        {
            currLang = SystemLanguage.English;
        }
        StartCoroutine(LoadConfig());
    }

    IEnumerator LoadConfig()
    {
        string filePath = $"{Application.streamingAssetsPath}/Lang/{currLang}.json";
#if UNITY_IOS || UNITY_EDITOR_OSX
        filePath = "file://" + filePath;
#endif
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            langDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(unityWebRequest.downloadHandler.text);
            Utility.Dump(langDict, "���������ö�ȡ�ɹ�");
            RefreshAllTextLang();
        }
        else
        {
            Debug.LogError("���������ö�ȡʧ��" + unityWebRequest.error);
        }
    }

    public string Get(string _tid)
    {
        if (!langDict.ContainsKey(_tid))
        {
            Debug.LogError($"�Ҳ�������{currLang}�ı�{_tid}");
            return _tid;
        }
        return langDict[_tid];
    }

    public void RefreshAllTextLang()
    {
        //��̨�л�
        if (Application.systemLanguage != lastSystemLang)
        {
            Lang[] langs = UIMgr.Ins.gameObject.GetComponentsInChildren<Lang>(true);
            foreach (var item in langs)
            {
                item.Refresh();
            }
        }
    }

    public void OnChangeLang()
    {
        LoadConfig();
    }
}