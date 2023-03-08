/***
 * 
 * 网络请求的get对象
 * 
 * **/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetWorkGetObject 
{
    //get的url
    public string Url;
    //get成功的回调
    public Action<UnityWebRequest> GetSuccess;
    //get失败的回调
    public Action GetFail;
    public NetWorkGetObject(string url,Action<UnityWebRequest> success,Action fail)
    {
        Url = url;
        GetSuccess = success;
        GetFail = fail;
    }
   
}
