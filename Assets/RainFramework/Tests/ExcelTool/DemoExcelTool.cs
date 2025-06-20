using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rain.Core;
using Rain.F8ExcelDataClass;
using Rain.Launcher;

namespace Rain.Tests
{
    public class DemoExcelTool : MonoBehaviour
    {
        // 方式一：读取二进制或者json
        IEnumerator Start()
        {
            // 指定名字加载
            Sheet1 sheet1 = RA.Config.Load<Sheet1>("Sheet1");
            RLog.Log(sheet1.Dict[2].name);
            
            // 同步加载全部配置
            RA.Config.LoadAll();

            // 异步加载全部配置
            // yield return RA.Config.LoadAllAsyncIEnumerator();
            // 也可以这样
            foreach (var item in RA.Config.LoadAllAsync())
            {
                yield return item;
            }
        
            // 单个表单个数据
            RLog.Log(RA.Config.GetSheet1ByID(2).name);
        
            // 单个表全部数据
            foreach (var item in RA.Config.GetSheet1())
            {
                RLog.Log(item.Key);
                RLog.Log(item.Value.name);
            }
        }

        // // 方式二：运行时读取Excel
        // IEnumerator Start()
        // {
        //     // 由于安卓资源都在包内，需要先复制到可读写文件夹1
        //     string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
        //     string[] paths = null;
        //     WWW www = new WWW(assetPath + "/fileindex.txt");
        //     yield return www;
        //     if (www.error != null)
        //     {
        //         RLog.Log(www.error);
        //         yield return null;
        //     }
        //     else
        //     {
        //         string ss = www.text;
        //         // 去除夹杂的空行
        //         string[] lines = ss.Split('\n');
        //         List<string> nonEmptyLines = new List<string>();
        //
        //         foreach (string line in lines)
        //         {
        //             string trimmedLine = line.Trim();
        //
        //             if (!string.IsNullOrEmpty(trimmedLine))
        //             {
        //                 nonEmptyLines.Add(trimmedLine);
        //             }
        //         }
        //
        //         paths = nonEmptyLines.ToArray();
        //     }
        //
        //     for (int i = 0; i < paths.Length; i++)
        //     {
        //         yield return CopyAssets(paths[i].Replace("\r", ""));
        //     }
        //     // 读取Excel文件
        //     ReadExcel.Instance.LoadAllExcelData();
        //     RLog.Log(RA.Config.GetSheet1ByID(1).name);
        //     RLog.Log(RA.Config.GetSheet1());
        // }
        //
        // // 由于安卓资源都在包内，需要先复制到可读写文件夹2
        // IEnumerator CopyAssets(string paths)
        // {
        //     string assetPath = URLSetting.STREAMINGASSETS_URL + "config";
        //     string sdCardPath = Application.persistentDataPath + "/config";
        //     WWW www = new WWW(assetPath + "/" + paths);
        //     yield return www;
        //     if(www.error != null)
        //     {
        //         RLog.Log(www.error);
        //         yield return null;
        //     }
        //     else
        //     {
        //         FileTools.SafeWriteAllBytes(sdCardPath + "/" + paths, www.bytes);
        //     }
        // }
    }
}