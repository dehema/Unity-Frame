using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Rain.Core
{
    public class HotUpdateMgr : ModuleSingletonMono<HotUpdateMgr>, IModule
    {
        public static string PackageSplit = "Package";
        public const string HotUpdateDirName = "/HotUpdate";
        public static string PackageDirName = "/Package";

        private Downloader hotUpdateDownloader;

        private Downloader packageDownloader;

        public bool UseLocalConfig = false;      //是否使用本地配置
        public string AssetBundlesConfigPath => Path.Combine(GameConfig.LocalGameVersion.GameRemoteAddress, nameof(AssetBundleMap) + ".json");

        public void OnInit(object createParam)
        {
            string text = File.ReadAllText(Path.Combine(Application.persistentDataPath, $"{nameof(GameVersion)}.json"));
            GameConfig.LocalGameVersion = JsonConvert.DeserializeObject<GameVersion>(text);
            InitLocalVersion();
            InitLocalAssetBundleMap();
        }

        // 初始化本地版本
        public void InitLocalVersion()
        {
            if (File.Exists(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json"))
            {
                string json =
                    FileTools.SafeReadAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json");
                GameConfig.LocalGameVersion = JsonConvert.DeserializeObject<GameVersion>(json);
            }
            else
            {
                //没有就写一份
                FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
                    JsonConvert.SerializeObject(GameConfig.LocalGameVersion));
                Debug.Log("创建GameVersion.json");
            }
        }

        public void InitLocalAssetBundleMap()
        {
            if (File.Exists(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json"))
            {
                string json = FileTools.SafeReadAllText(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json");
                GameConfig.LocalAssetBundleMap = JsonConvert.DeserializeObject<AssetBundleMap>(json);
            }
            else
            {
                //没有就写一份
                FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json",
                    JsonConvert.SerializeObject(GameConfig.LocalAssetBundleMap));
                Debug.Log("创建AssetBundleMap.json");
            }
        }

        // 初始化远程版本
        public IEnumerator InitRemoteVersion()
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate && !GameConfig.LocalGameVersion.EnablePackage)
            {
                yield break;
            }
            string path = GameConfig.LocalGameVersion.GameRemoteAddress + "/" + nameof(GameVersion) + ".json";
            Debug.Log($"初始化远程版本：{path}");

            UnityWebRequest webRequest = UnityWebRequest.Get(path);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"获取游戏远程版本失败：{path} ，错误：{webRequest.error}");
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                GameVersion gameVersion = JsonConvert.DeserializeObject<GameVersion>(text);
                GameConfig.RemoteGameVersion = gameVersion;
            }
            webRequest.Dispose();
            webRequest = null;
        }

        // 初始化远端资源版本
        public IEnumerator InitRemoteAssetBundleMap()
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate && !GameConfig.LocalGameVersion.EnablePackage)
                yield break;
            //获取到远端版本信息后再去获取资源信息
            while (GameConfig.RemoteGameVersion == null)
                yield return new WaitForEndOfFrame();

            Debug.Log($"初始化资源版本：{AssetBundlesConfigPath}");

            UnityWebRequest webRequest = UnityWebRequest.Get(AssetBundlesConfigPath);
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"获取游戏资源版本失败：{AssetBundlesConfigPath} ，错误：{webRequest.error}");
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                GameConfig.RemoteAssetBundleMap = JsonConvert.DeserializeObject<AssetBundleMap>(text);
            }
            webRequest.Dispose();
            webRequest = null;
        }

        // 游戏修复，资源清理
        public void GameRepairAssetClean()
        {
            FileTools.SafeClearDir(Application.persistentDataPath + HotUpdateDirName);
            FileTools.SafeClearDir(Application.persistentDataPath + PackageDirName);
            FileTools.SafeDeleteFile(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json");
            FileTools.SafeDeleteFile(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json");
        }

        /// <summary>
        /// 检查需要热更的资源
        /// </summary>
        /// <returns></returns>
        public Tuple<Dictionary<string, string>, long> CheckHotUpdate()
        {
            long allSize = 0;
            Dictionary<string, string> hotUpdateAssetUrl = new Dictionary<string, string>();
            if (!GameConfig.LocalGameVersion.EnableHotUpdate) // 启用热更
            {
                return Tuple.Create(hotUpdateAssetUrl, allSize);
            }

            if (GameConfig.RemoteAssetBundleMap.ABMap.Count <= 0) // 热更资产Map数量
            {
                return Tuple.Create(hotUpdateAssetUrl, allSize);
            }

            int result = GameConfig.CompareVersions(GameConfig.LocalGameVersion.Version,
                GameConfig.RemoteGameVersion.Version);
            if (result >= 0) // 此版本无需热更新
            {
                return Tuple.Create(hotUpdateAssetUrl, allSize);
            }

            var remoteABMap = GameConfig.RemoteAssetBundleMap;

            var localABMap = GameConfig.LocalAssetBundleMap.ABMap;

            foreach (var remoteABMapping in remoteABMap.ABMap)
            {
                localABMap.TryGetValue(remoteABMapping.Key, out AssetMapping assetMapping);

                if ((assetMapping == null || remoteABMapping.Value.MD5 != assetMapping.MD5) // 新增资源，MD5不同则需更新
                    && !remoteABMapping.Value.AbName.IsNullOrEmpty() && !remoteABMapping.Value.MD5.IsNullOrEmpty())
                {
                    string abPath = HotUpdateDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/" + remoteABMapping.Value.AbName;

                    string persistentAbPath = Application.persistentDataPath + abPath;

                    // 校验本地热更资源文件md5
                    if (File.Exists(persistentAbPath) && FileTools.CreateMd5ForFile(persistentAbPath) == remoteABMapping.Value.MD5)
                    {
                        continue;
                    }
                    allSize += string.IsNullOrEmpty(remoteABMapping.Value.Size) ? 0 : long.Parse(remoteABMapping.Value.Size);
                    hotUpdateAssetUrl.TryAdd(remoteABMapping.Key, abPath);
                }
            }
            //返回远端资源信息 大小
            return Tuple.Create(hotUpdateAssetUrl, allSize);
        }

        /// <summary>
        /// 开始热更新包下载
        /// </summary>
        /// <param name="hotUpdateAssetUrl"><远端资产名字, 路径></param>
        /// <param name="completed"></param>
        /// <param name="failure"></param>
        /// <param name="overallProgress"></param>
        public void StartHotUpdate(Dictionary<string, string> hotUpdateAssetUrl, Action completed = null, Action failure = null, Action<float> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnableHotUpdate)
            {
                WriteVersion();
                completed?.Invoke();
                return;
            }

            if (hotUpdateAssetUrl.Count <= 0)
            {
                WriteVersion();
                completed?.Invoke();
                return;
            }

            // 创建热更下载器
            hotUpdateDownloader = DownloadMgr.Ins.CreateDownloader("hotUpdateDownloader", new Downloader());

            // 设置热更下载器回调
            hotUpdateDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                RLog.LogVersion($"获取热更资源完成！：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadFailure += (eventArgs) =>
            {
                Debug.LogError($"获取热更资源失败。：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
                failure?.Invoke();
            };
            hotUpdateDownloader.OnDownloadStart += (eventArgs) =>
            {
                RLog.LogVersion($"开始获取热更资源...：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            hotUpdateDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
                float taskCount = (float)eventArgs.DownloadTaskCount;

                // 计算进度百分比
                float progress = currentTaskIndex / taskCount * 100f;
                overallProgress?.Invoke(progress);
            };
            hotUpdateDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
                RLog.LogVersion($"所有热更资源获取完成！，用时：{eventArgs.TimeSpan}");
                WriteVersion();
                completed?.Invoke();
            };

            List<string> tempDownloadUrl = new List<string>(hotUpdateAssetUrl.Count);
            // 添加下载清单
            foreach (var assetUrl in hotUpdateAssetUrl.Values)
            {
                if (!tempDownloadUrl.Contains(assetUrl))
                {
                    int index = assetUrl.IndexOf('/');
                    string result = assetUrl.Substring(index + 1);
                    hotUpdateDownloader.AddDownload(GameConfig.LocalGameVersion.GameRemoteAddress + assetUrl,
                        Application.persistentDataPath + "/" + result);
                    tempDownloadUrl.Add(assetUrl);
                }
            }
            void WriteVersion()
            {
                if (!GameConfig.RemoteGameVersion.Version.IsNullOrEmpty())
                {
                    GameConfig.LocalGameVersion.Version = GameConfig.RemoteGameVersion.Version;
                    GameConfig.LocalGameVersion.HotUpdateVersion = new List<string>();
                    FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
                        JsonConvert.SerializeObject(GameConfig.LocalGameVersion, Formatting.Indented));
                }

                if (GameConfig.RemoteAssetBundleMap.ABMap.Count > 0)
                {
                    FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(AssetBundleMap) + ".json",
                        JsonConvert.SerializeObject(GameConfig.RemoteGameVersion));
                }

                AssetBundleMgr.Ins.LoadAssetBundleManifestSync();
            }
            // 下载器开始下载
            hotUpdateDownloader.LaunchDownload();
        }

        // 检查需要下载的分包
        public List<string> CheckPackageUpdate(List<string> subPackage)
        {
            List<string> temp = new List<string>(subPackage.Count);
            foreach (var package in subPackage)
            {
                if (GameConfig.LocalGameVersion.SubPackage.Contains(package))
                {
                    temp.Add(package);
                }
            }
            return temp;
        }

        /// <summary>
        /// 开始分包下载
        /// </summary>
        /// <param name="subPackages"></param>
        /// <param name="completed"></param>
        /// <param name="failure"></param>
        /// <param name="overallProgress"></param>
        public void StartPackageUpdate(List<string> subPackages, Action completed = null, Action failure = null, Action<float> overallProgress = null)
        {
            if (!GameConfig.LocalGameVersion.EnablePackage)
            {
                completed?.Invoke();
                return;
            }

            if (subPackages.Count <= 0)
            {
                completed?.Invoke();
                return;
            }

            List<string> downloadPaths = new List<string>(subPackages.Count);

            // 创建分包下载器
            packageDownloader = DownloadMgr.Ins.CreateDownloader("packageDownloader", new Downloader());

            // 设置分包下载器回调
            packageDownloader.OnDownloadSuccess += (eventArgs) =>
            {
                RLog.LogVersion($"获取分包资源完成！：{eventArgs.DownloadInfo.DownloadUrl}");
                downloadPaths.Add(eventArgs.DownloadInfo.DownloadPath);
            };
            packageDownloader.OnDownloadFailure += (eventArgs) =>
            {
                Debug.LogError($"获取分包资源失败。：{eventArgs.DownloadInfo.DownloadUrl}\n{eventArgs.ErrorMessage}");
                failure?.Invoke();
            };
            packageDownloader.OnDownloadStart += (eventArgs) =>
            {
                RLog.LogVersion($"开始获取分包资源...：{eventArgs.DownloadInfo.DownloadUrl}");
            };
            packageDownloader.OnDownloadOverallProgress += (eventArgs) =>
            {
                float currentTaskIndex = (float)eventArgs.CurrentDownloadTaskIndex;
                float taskCount = (float)eventArgs.DownloadTaskCount;

                // 计算进度百分比
                float progress = currentTaskIndex / taskCount * 100f;
                // LogF8.LogVersion(progress);
                overallProgress?.Invoke(progress);
            };
            packageDownloader.OnAllDownloadTaskCompleted += (eventArgs) =>
            {
                RLog.LogVersion($"所有分包资源获取完成！，用时：{eventArgs.TimeSpan}");
#if UNITY_WEBGL
                // 使用协程
				Util.Unity.StartCoroutine(UnZipPackagePathsCo(downloadPaths, completed));
#else
                // 使用多线程
                //_ = UnZipPackagePaths(downloadPaths, completed);
#endif
            };

            // 添加下载清单
            foreach (var package in subPackages)
            {
                string persistentPackagePath = Application.persistentDataPath + "/" + PackageSplit + package + ".zip";
                long fileSizeInBytes = 0;
                if (File.Exists(persistentPackagePath))
                {
                    FileInfo fileInfo = new FileInfo(persistentPackagePath);
                    fileSizeInBytes = fileInfo.Length;
                }
                // 断点续传
                packageDownloader.AddDownload(GameConfig.LocalGameVersion.GameRemoteAddress + "/" + PackageSplit + package + ".zip",
                    persistentPackagePath, fileSizeInBytes, true);
            }

            packageDownloader.LaunchDownload();
        }

        //public IEnumerator UnZipPackagePathsCo(List<string> downloadPaths, Action completed = null)
        //{
        //    foreach (var downloadPath in downloadPaths)
        //    {
        //        // 使用协程
        //        yield return Util.ZipHelper.UnZipFileCoroutine(downloadPath,
        //            Application.persistentDataPath, null, true);
        //        string package = Path.GetFileNameWithoutExtension(downloadPath).Replace(PackageSplit, "");
        //        int subPackageCount = GameConfig.LocalGameVersion.SubPackage.Count;
        //        for (int i = subPackageCount - 1; i >= 0; i--)
        //        {
        //            if (GameConfig.LocalGameVersion.SubPackage[i] == package)
        //            {
        //                GameConfig.LocalGameVersion.SubPackage.RemoveAt(i);
        //            }
        //        }
        //        FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
        //            JsonConvert.SerializeObject(GameConfig.LocalGameVersion));
        //    }

        //    completed?.Invoke();
        //}

        //public async Task UnZipPackagePaths(List<string> downloadPaths, Action completed = null)
        //{
        //    foreach (var downloadPath in downloadPaths)
        //    {
        //        // 使用多线程
        //        await Util.ZipHelper.UnZipFileAsync(downloadPath, Application.persistentDataPath, null, true);
        //        string package = Path.GetFileNameWithoutExtension(downloadPath).Replace(PackageSplit, "");
        //        int subPackageCount = GameConfig.LocalGameVersion.SubPackage.Count;
        //        for (int i = subPackageCount - 1; i >= 0; i--)
        //        {
        //            if (GameConfig.LocalGameVersion.SubPackage[i] == package)
        //            {
        //                GameConfig.LocalGameVersion.SubPackage.RemoveAt(i);
        //            }
        //        }
        //        FileTools.SafeWriteAllText(Application.persistentDataPath + "/" + nameof(GameVersion) + ".json",
        //            JsonConvert.SerializeObject(GameConfig.LocalGameVersion));
        //    }
        //    completed?.Invoke();
        //}

        public static string GetAssetBundlesPath(string fullPath)
        {
            Regex rgx = new Regex(@"AssetBundles[\\/].+$");
            Match matches = rgx.Match(fullPath);

            string assetPath = "";
            if (matches.Success)
                assetPath = matches.Value;

            assetPath = FileTools.FormatToUnityPath(assetPath);
            return assetPath;
        }

        public void OnUpdate()
        {

        }

        public void OnLateUpdate()
        {

        }

        public void OnFixedUpdate()
        {

        }

        public void OnTermination()
        {
            hotUpdateDownloader.CancelDownload();
            hotUpdateDownloader = null;
            packageDownloader.CancelDownload();
            packageDownloader = null;
            Destroy(gameObject);
        }
    }
}
