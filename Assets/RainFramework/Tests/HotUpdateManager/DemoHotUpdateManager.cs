using System;
using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoHotUpdateManager : MonoBehaviour
    {
        IEnumerator Start()
        {
            // 初始化本地版本
            RA.HotUpdate.InitLocalVersion();

            // 初始化远程版本
            yield return RA.HotUpdate.InitRemoteVersion();
            
            // 初始化资源版本
            yield return RA.HotUpdate.InitAssetVersion();
            
            // 检查需要热更的资源，总大小
            Tuple<Dictionary<string, string>, long> result  = RA.HotUpdate.CheckHotUpdate();
            var hotUpdateAssetUrl = result.Item1;
            var allSize = result.Item2;
            
            // 资源热更新
            RA.HotUpdate.StartHotUpdate(hotUpdateAssetUrl, () =>
            {
                RLog.Log("完成");
            }, () =>
            {
                RLog.Log("失败");
            }, progress =>
            {
                RLog.Log("进度：" + progress);
            });

            // 检查未加载的分包
            List<string> subPackage = RA.HotUpdate.CheckPackageUpdate(GameConfig.LocalGameVersion.SubPackage);
            
            // 分包加载
            RA.HotUpdate.StartPackageUpdate(subPackage, () =>
            {
                RLog.Log("完成");
            }, () =>
            {
                RLog.Log("失败");
            }, progress =>
            {
                RLog.Log("进度：" + progress);
            });
        }
    }
}
