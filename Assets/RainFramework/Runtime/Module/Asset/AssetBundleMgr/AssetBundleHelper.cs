using System.IO;
using UnityEngine;

namespace Rain.Core
{
    /// <summary>
    /// 为资产捆绑管理器提供资源类型判断和路由帮助。
    /// </summary>
    public static class AssetBundleHelper
    {
        private static string _streamingAssetsPath;

        private static string GetStreamingAssetsPath()
        {
            if (_streamingAssetsPath == null)
            {
                _streamingAssetsPath = Application.streamingAssetsPath + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _streamingAssetsPath;
        }

        private static string _remoteAddress;

        private static string GetRemoteAddress()
        {
            if (_remoteAddress == null)
            {
                _remoteAddress = GameConfig.LocalGameVersion.GameRemoteAddress + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _remoteAddress;
        }

        private static string _hotUpdatePath;

        private static string GetHotUpdatePath()
        {
            if (_hotUpdatePath == null)
            {
                _hotUpdatePath = Application.persistentDataPath + HotUpdateMgr.HotUpdateDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _hotUpdatePath;
        }

        private static string _packagePath;

        private static string GetPackagePath()
        {
            if (_packagePath == null)
            {
                _packagePath = Application.persistentDataPath + HotUpdateMgr.PackageDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _packagePath;
        }

        private static string _persistentPath;

        private static string GetPersistentPath()
        {
            if (_persistentPath == null)
            {
                _persistentPath = Application.persistentDataPath + HotUpdateMgr.HotUpdateDirName + "/" + URLSetting.AssetBundlesName + "/" + URLSetting.GetPlatformName() + "/";
            }
            return _persistentPath;
        }

        /// <summary>
        /// 源类型的枚举。
        /// </summary>
        public enum SourceType : byte
        {
            None,
            PersistentAssets,
            StreamingAssets,
            HotUpdatePath,
            PackagePath,
            RemoteAddress,
        }

        /// <summary>
        /// 根据环境获取资产捆绑包的路径。
        /// </summary>
        /// <param name="type">源类型。</param>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundlePath(SourceType type = SourceType.StreamingAssets)
        {
            string assetBundlePath;
            switch (type)
            {
                case SourceType.PersistentAssets:
                    assetBundlePath = GetPersistentPath();
                    break;
                case SourceType.StreamingAssets:
                    assetBundlePath = GetStreamingAssetsPath();
                    break;
                case SourceType.HotUpdatePath:
                    assetBundlePath = GetHotUpdatePath();
                    break;
                case SourceType.PackagePath:
                    assetBundlePath = GetPackagePath();
                    break;
                case SourceType.RemoteAddress:
                    if (string.IsNullOrEmpty(GameConfig.LocalGameVersion.GameRemoteAddress))
                    {
                        Debug.LogError("加载远程包需要配置远程地址：AssetRemoteAddress");
                    }
                    assetBundlePath = GetRemoteAddress();
                    break;
                default:
                    Debug.LogError("AssetBundle的源类型不能为空");
                    return null;
            }

            return assetBundlePath;
        }

        /// <summary>
        /// 根据环境获取资产捆绑清单文件的路径。
        /// </summary>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleManifestPath(SourceType type = SourceType.StreamingAssets)
        {
            string platformAssetBundlePath = GetAssetBundlePath(type);
            if (platformAssetBundlePath == null)
                return null;

            string manifestPath = platformAssetBundlePath + URLSetting.GetPlatformName();
            return manifestPath;
        }

        /// <summary>
        /// 根据环境获取带后缀的资产捆绑清单文件的路径。
        /// </summary>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleManifestPathWithSuffix(SourceType type = SourceType.StreamingAssets)
        {
            string manifestPath = GetAssetBundleManifestPath(type);
            if (manifestPath == null)
                return null;

            return manifestPath + ".manifest";
        }

        /// <summary>
        /// 获取资产捆绑包的完整路径。
        /// </summary>
        /// <param name="assetBundleFileName">资产捆绑包的文件名。</param>
        /// <param name="type">源类型。</param>
        /// <returns>正确的路径。</returns>
        public static string GetAssetBundleFullName(string assetBundleFileName = null, SourceType type = SourceType.PersistentAssets)
        {
            string assetBundlePath = GetAssetBundlePath(type);
            if (assetBundlePath == null)
                return null;

            return assetBundlePath + assetBundleFileName;
        }

        /// <summary>
        /// 通过其完整路径确定资产捆绑包的类型。
        /// </summary>
        /// <param name="assetBundleFullName">完整路径。</param>
        /// <returns>源类型。</returns>
        public static SourceType GetAssetBundleSourceType(string assetBundleFullName)
        {
            string streamingAssetsPath = Application.streamingAssetsPath;
            string hotUpdatePath = Application.persistentDataPath + HotUpdateMgr.HotUpdateDirName;
            string packagePath = Application.persistentDataPath + HotUpdateMgr.PackageDirName;

            if (assetBundleFullName.Contains(streamingAssetsPath))
            {
                return SourceType.StreamingAssets;
            }
            else if (assetBundleFullName.Contains(hotUpdatePath))
            {
                return SourceType.HotUpdatePath;
            }
            else if (assetBundleFullName.Contains(packagePath))
            {
                return SourceType.PackagePath;
            }
            else if (FileTools.IsLegalHTTPURI(assetBundleFullName))
            {
                return SourceType.RemoteAddress;
            }
            else
            {
                return SourceType.None;
            }
        }
    }
}