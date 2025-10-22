// code generation.

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace Rain.Core
{
    public class AssetBundleMap
    {
        /// <summary>
        /// 版本
        /// </summary>
        public string Version;
        /// <summary>
        /// ab映射<ab包包名, 映射类>
        /// </summary>
        public Dictionary<string, AssetMapping> ABMap = new Dictionary<string, AssetMapping>();
        /// <summary>
        /// 生成日期
        /// </summary>
        public string CreateTime = DateTime.Now.ToString("G");
    }
    [Preserve]
    public class AssetMapping
    {
        public string AbName;
        public string AssetPath;
        public string Version;
        public string Size;
        public string MD5;
        public string Package;      //分包
        public string Updated;

        /// <summary>
        /// AB资产信息
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="assetPath"></param>
        /// <param name="version"></param>
        /// <param name="size"></param>
        /// <param name="md5"></param>
        /// <param name="package">使用文件夹区分包，例如Package_0目录下的就是包编号：0。</param>
        /// <param name="updated"></param>
        public AssetMapping(string abName, string assetPath, string version, string size, string md5, string package, string updated)
        {
            AbName = abName;
            AssetPath = assetPath;
            Version = version;
            Size = size;
            MD5 = md5;
            Package = package;
            Updated = updated;
        }

        public AssetMapping()
        {

        }
    }
}