/**
 * 
 * 网络请求的post对象
 * 
 * ***/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetWorkPostObject 
{
    //post请求地址
    public string URL;
    //post的数据表单
    public WWWForm Form;
    //post成功回调
    public Action<UnityWebRequest> PostSuccess;
    //post失败回调
    public Action PostFail;
    public NetWorkPostObject(string url,WWWForm  form,Action<UnityWebRequest> success,Action fail)
    {
        URL = url;
        Form = form;
        PostSuccess = success;
        PostFail = fail;
    }
}
