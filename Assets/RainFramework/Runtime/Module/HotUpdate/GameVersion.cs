using System;
using System.Collections.Generic;

namespace Rain.Core
{
    public class GameVersion
    {
        public string Version;                  //版本号
        public string AssetRemoteAddress;       //资源远程地址
        public bool EnableHotUpdate = true;     //是否开启热更
        public List<string> HotUpdateVersion;   //热更版本列表
        public bool EnablePackage;              //是否开启分包
        public List<string> SubPackage;         //分包列表

        public GameVersion(string version, string assetRemoteAddress = null, bool enableHotUpdate = false,
            List<string> hotUpdateVersion = null, bool enablePackage = false, List<string> subPackage = null)
        {
            Version = version;
            AssetRemoteAddress = assetRemoteAddress;
            EnableHotUpdate = enableHotUpdate;
            HotUpdateVersion = hotUpdateVersion;
            EnablePackage = enablePackage;
            SubPackage = subPackage;
        }

        public GameVersion()
        {

        }
    }

    public class GameConfig
    {
        public static GameVersion LocalGameVersion = new GameVersion();

        public static GameVersion RemoteGameVersion = new GameVersion();

        public static Dictionary<string, AssetBundleMap.AssetMapping> RemoteAssetBundleMap =
            new Dictionary<string, AssetBundleMap.AssetMapping>();
        /// <summary>
        /// 判断版本号大小，1为version1大，-1为version2大，0为一样大
        /// </summary>
        /// <param name="version1">版本1</param>
        /// <param name="version2">版本2</param>
        public static int CompareVersions(string version1, string version2)
        {
            // Split the versions into individual components
            string[] version1Components = version1.Split('.');
            string[] version2Components = version2.Split('.');

            // Determine the maximum length of version components
            int maxLength = Math.Max(version1Components.Length, version2Components.Length);

            // Compare each component one by one
            for (int i = 0; i < maxLength; i++)
            {
                int v1 = i < version1Components.Length ? Convert.ToInt32(version1Components[i]) : 0;
                int v2 = i < version2Components.Length ? Convert.ToInt32(version2Components[i]) : 0;

                if (v1 < v2)
                {
                    return -1; // version1 is less than version2
                }
                else if (v1 > v2)
                {
                    return 1; // version1 is greater than version2
                }
            }

            return 0; // versions are equal
        }
    }
}
