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
            Debug.LogError("多语言配置读取失败" + unityWebRequest.error);
        }
    }

    public string Get(string _tid)
    {
        if (!langDict.ContainsKey(_tid))
        {
            Debug.LogError($"找不到语言{currLang}文本{_tid}");
            return _tid;
        }
        return langDict[_tid];
    }

    public void RefreshAllTextLang()
    {
        //后台切回
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
    /// 获取名称
    /// </summary>
    /// <returns></returns>
    public string GetLanguageName(SystemLanguage _lang)
    {
        if (_lang == SystemLanguage.Chinese)
            return "简体中文";
        else if (_lang == SystemLanguage.English)
            return "English";
        return "unkown";
    }
}