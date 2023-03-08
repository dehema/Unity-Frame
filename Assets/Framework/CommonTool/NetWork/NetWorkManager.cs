/**
 * 
 * 网络请求管理器
 * 
 * ***/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetWorkManager : MonoSingleton<NetWorkManager>
{
    //get请求列表
    private List<NetWorkGetObject> NetWorkGetList;
    //post请求列表
    private List<NetWorkPostObject> NetWorkPostList;
    public NetWorkManager()
    {
        //初始化
        NetWorkGetList = new List<NetWorkGetObject>();
        NetWorkPostList = new List<NetWorkPostObject>();
    }

    /// <summary>
    /// 获取当前Get请求列表中请求的个数
    /// </summary>
    public int GetNetWorkGetListCount
    {
        get { return NetWorkGetList.Count; }
    }

    /// <summary>
    /// 获取当前Post请求列表中请求的个数
    /// </summary>
    public int GetNetWorkPostListCount
    {
        get { return NetWorkPostList.Count; }
    }

    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="url">Get请求的URL</param>
    /// <param name="success">Get请求成功的回调</param>
    /// <param name="fail">Get请求失败的回调</param>
    public void HttpGet(string url, Action<UnityWebRequest> success, Action fail)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.Log("HttpGet请求URL地址不能为空");
            return;
        }
        NetWorkGetObject o = new NetWorkGetObject(url, success, fail);
        //添加到管理列表中
        NetWorkGetList.Add(o);
        //开始请求
        StartCoroutine("Get", o);
    }

    /// <summary>
    /// Post请求
    /// </summary>
    /// <param name="url">Post请求的URL</param>
    /// <param name="form">Post请求的表单数据</param>
    /// <param name="success">Post请求成功的回调</param>
    /// <param name="fail">Post请求失败的回调</param>
    public void HttpPost(string url, WWWForm form, Action<UnityWebRequest> success, Action fail)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.Log("HttpPost请求URL地址不能为空");
            return;
        }
        NetWorkPostObject o = new NetWorkPostObject(url, form, success, fail);
        //添加到post请求管理列表中
        NetWorkPostList.Add(o);
        //开始请求
        StartCoroutine("Post", o);
    }

    /// <summary>
    /// Get请求的协程
    /// </summary>
    /// <param name="obj">Get请求对象</param>
    /// <returns></returns>
    IEnumerator Get(NetWorkGetObject obj)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(obj.Url);
        webRequest.SendWebRequest();
        while (!webRequest.isDone)
        {
            yield return 1;
        }
        if (webRequest.isDone)
        {
            //从管理列表中移除
            if (NetWorkGetList.Contains(obj))
            {
                NetWorkGetList.Remove(obj);
            }
        }
        //yield return webRequest.SendWebRequest();
        //异常处理,请求失败
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log("请求" + obj.Url + "失败，错误：" + webRequest.error);
            obj.GetFail();
        }
        else
        {
            //Debug.Log(webRequest.downloadHandler.text);
            obj.GetSuccess(webRequest);
        }
        //终止本次协程
        yield break;
    }



    /// <summary>
    /// Post协程
    /// </summary>
    /// <returns></returns>
    IEnumerator Post(NetWorkPostObject obj)
    {
        //WWWForm form = new WWWForm();
        ////键值对
        //form.AddField("key", "value");
        //form.AddField("name", "mafanwei");
        //form.AddField("blog", "qwe25878");

        UnityWebRequest webRequest = UnityWebRequest.Post(obj.URL, obj.Form);

        yield return webRequest.SendWebRequest();
        //异常处理
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.Log("Post请求" + obj.URL + "失败，错误：" + webRequest.error);
            obj.PostFail();
        }
        else
        {
            obj.PostSuccess(webRequest);
            //Debug.Log(webRequest.downloadHandler.text);
        }

        //从管理列表中移除
        if (NetWorkPostList.Contains(obj))
        {
            NetWorkPostList.Remove(obj);
        }
        //终止本次协程
        yield break;
    }
}
