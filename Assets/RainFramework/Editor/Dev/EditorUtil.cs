using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rain.Core
{
    public static class EditorUtil
    {
        /// <summary>
        /// 获取资源的AssetBundle包名
        /// </summary>
        /// <returns></returns>
        public static string GetResAssetBundleName(string assetPath)
        {
            //如果是图集
            if (assetPath.StartsWith(BuildToolAtlas.sourcePath))
            {
                // 获取路径中 UI 之后的第一个文件夹名字
                string pathAfterUI = assetPath.Substring(BuildToolAtlas.sourcePath.Length);

                // 去掉开头的斜杠
                if (pathAfterUI.StartsWith("/"))
                {
                    pathAfterUI = pathAfterUI.Substring(1);
                }

                // 获取第一个文件夹名字
                int firstSlashIndex = pathAfterUI.IndexOf("/");
                if (firstSlashIndex > 0)
                {
                    return pathAfterUI.Substring(0, firstSlashIndex);
                }
                else if (firstSlashIndex == -1 && !string.IsNullOrEmpty(pathAfterUI))
                {
                    // 如果路径中没有斜杠，说明文件直接在UI文件夹下
                    return Path.GetFileNameWithoutExtension(pathAfterUI);
                }
            }

            // 如果不是目标路径，返回文件本身的名字
            return Path.GetFileNameWithoutExtension(assetPath);

        }
    }
}
