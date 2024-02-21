using Codice.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class EditorTranslate : MonoBehaviour
{
    /// <summary>
    /// 点击翻译
    /// </summary>
    public static void OnClickTranslateLanguage()
    {
        Dictionary<string, string> chineseDict = ReadChineseJson(SystemLanguage.Chinese);
        Dictionary<string, string> englishDict = ReadChineseJson(SystemLanguage.English);
        List<string> transLangKey = new List<string>();
        StringBuilder transStr = new StringBuilder();
        foreach (var item in chineseDict)
        {
            if (!englishDict.ContainsKey(item.Key))
            {
                transLangKey.Add(item.Key);
            }
        }
        foreach (var item in transLangKey)
        {
            transStr.Append(chineseDict[item]);
            if (transLangKey.Last() != item)
            {
                transStr.Append("\n");
            }
        }
        string transTxt = transStr.ToString();
        //执行翻译
        List<string> transVal = TransStr(transTxt);
        if (transLangKey.Count != transVal.Count)
        {
            Debug.LogError("翻译出错，key和val的数量对不上");
            return;
        }
        //填充翻译后的数据
        for (int i = 0; i < transLangKey.Count; i++)
        {
            string key = transLangKey[i];
            string val = transVal[i];
            englishDict[key] = val;
        }
        //排序 跟中文文本一样
        Dictionary<string, string> newEnglishDict = new Dictionary<string, string>();
        foreach (var item in chineseDict)
        {
            newEnglishDict.Add(item.Key, englishDict[item.Key]);
        }
        string transFileTxt = JsonConvert.SerializeObject(newEnglishDict);
        File.WriteAllText(EditorDevTools.GetLangPath(SystemLanguage.English), transFileTxt);
        Debug.Log("翻译完成,共翻译" + transLangKey.Count + "个多语言");
    }

    static Dictionary<string, string> ReadChineseJson(SystemLanguage language)
    {
        string path = EditorDevTools.GetLangPath(language);
        string content = File.ReadAllText(path);
        Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        return dict;
    }

    /// <summary>
    /// 翻译字符串
    /// </summary>
    static List<string> TransStr(string q)
    {
        // 源语言
        string from = "zh";
        // 目标语言
        string to = "en";
        //appId
        string appId = "20230925001829523";
        //随机数
        string salt = Random.Range(0, 100000).ToString();
        // 密钥
        string secretKey = "CLJf59XylJlwYqwE35TW";
        string sign = EncryptString(appId + q + salt + secretKey);
        string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
        url += "q=" + HttpUtility.UrlEncode(q);
        url += "&from=" + from;
        url += "&to=" + to;
        url += "&appid=" + appId;
        url += "&salt=" + salt;
        url += "&sign=" + sign;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";
        request.ContentType = "text/html;charset=UTF-8";
        request.UserAgent = null;
        request.Timeout = 6000;
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();
        Console.ReadLine();
        BaiduTranslate baiduTranslate = JsonConvert.DeserializeObject<BaiduTranslate>(retString);
        List<string> strList = new List<string>();
        foreach (var item in baiduTranslate.trans_result)
        {
            strList.Add(item.dst);
        }
        return strList;
    }

    // 计算MD5值
    public static string EncryptString(string str)
    {
        MD5 md5 = MD5.Create();
        // 将字符串转换成字节数组
        byte[] byteOld = Encoding.UTF8.GetBytes(str);
        // 调用加密方法
        byte[] byteNew = md5.ComputeHash(byteOld);
        // 将加密结果转换为字符串
        StringBuilder sb = new StringBuilder();
        foreach (byte b in byteNew)
        {
            // 将字节转换成16进制表示的字符串，
            sb.Append(b.ToString("x2"));
        }
        // 返回加密的字符串
        return sb.ToString();
    }
}

public class BaiduTranslate
{
    public string from;
    public string to;
    public BaiduTransResult[] trans_result;
}

public class BaiduTransResult
{
    public string src;
    public string dst;

}