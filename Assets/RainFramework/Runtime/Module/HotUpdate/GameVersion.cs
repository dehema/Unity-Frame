using System;
using System.Collections.Generic;

namespace Rain.Core
{
    public class GameVersion
    {
        public string Version = "0.0.0";        //版本号
        public string GameRemoteAddress;        //游戏远程地址
        public bool EnableHotUpdate = true;     //是否开启热更
        public List<string> HotUpdateVersion;   //热更版本列表
        public bool EnablePackage;              //是否开启分包
        public List<string> SubPackage;         //分包列表

        public GameVersion(string version, string gameRemoteAddress = null, bool enableHotUpdate = false,
            List<string> hotUpdateVersion = null, bool enablePackage = false, List<string> subPackage = null)
        {
            Version = version;
            GameRemoteAddress = gameRemoteAddress;
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
        /// <summary>
        /// 本地版本信息
        /// </summary>
        public static GameVersion LocalGameVersion = new GameVersion();

        /// <summary>
        /// 远程版本信息
        /// </summary>
        public static GameVersion RemoteGameVersion = new GameVersion();

        /// <summary>
        /// 本地AB包映射表
        /// </summary>
        public static AssetBundleMap LocalAssetBundleMap = new AssetBundleMap();

        /// <summary>
        /// 远端AB包映射表
        /// </summary>
        public static AssetBundleMap RemoteAssetBundleMap = new AssetBundleMap();

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
