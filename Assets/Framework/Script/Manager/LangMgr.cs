using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using UnityEditor;

public class LangMgr : MonoSingleton<LangMgr>
{
    public SystemLanguage lastSystemLang;
    public SystemLanguage currLang;
    public Dictionary<string, string> langDict = new Dictionary<string, string>();
    public List<SystemLanguage> supportLanguage = new List<SystemLanguage>() { SystemLanguage.Chinese, SystemLanguage.English };

    public void Init()
    {
        LoadLangConfig();
    }

    public void LoadLangConfig()
    {
        lastSystemLang = currLang;
        currLang = DataMgr.Ins.settingData.language;
        StartCoroutine(LoadConfig());
    }

    IEnumerator LoadConfig()
    {
        string filePath = $"{Application.streamingAssetsPath}/Lang/{currLang}.json";
#if UNITY_IOS || UNITY_EDITOR_OSX
    filePath = "file://"+filePath;
#endif
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(filePath);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            langDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(unityWebRequest.downloadHandler.text);
            RefreshAllTextLang();
        }
        else
        {
            Debug.LogError("澶氳瑷€閰嶇疆璇诲彇澶辫触" + unityWebRequest.error);
        }
    }

    public string Get(string _tid)
    {
        if (!langDict.ContainsKey(_tid))
        {
            Debug.LogError($"鎵句笉鍒拌瑷€{currLang}鏂囨湰{_tid}");
            return _tid;
        }
        return langDict[_tid];
    }

    public void RefreshAllTextLang()
    {
        //鍚庡彴鍒囧洖
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

    /// <summary>
    /// 鑾峰彇鍚嶇О
    /// </summary>
    /// <returns></returns>
    public string GetLanguageName(SystemLanguage _lang)
    {
        if (_lang == SystemLanguage.Chinese)
            return "绠€浣撲腑鏂?;
        else if (_lang == SystemLanguage.English)
            return "English";
        return "unkown";
    }
}
