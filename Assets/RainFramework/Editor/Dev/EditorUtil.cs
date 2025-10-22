using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rain.Core
{
    public static class EditorUtil
    {
        /// <summary>
        /// ��ȡ��Դ��AssetBundle����
        /// </summary>
        /// <returns></returns>
        public static string GetResAssetBundleName(string assetPath)
        {
            //�����ͼ��
            if (assetPath.StartsWith(BuildToolAtlas.sourcePath))
            {
                // ��ȡ·���� UI ֮��ĵ�һ���ļ�������
                string pathAfterUI = assetPath.Substring(BuildToolAtlas.sourcePath.Length);

                // ȥ����ͷ��б��
                if (pathAfterUI.StartsWith("/"))
                {
                    pathAfterUI = pathAfterUI.Substring(1);
                }

                // ��ȡ��һ���ļ�������
                int firstSlashIndex = pathAfterUI.IndexOf("/");
                if (firstSlashIndex > 0)
                {
                    return pathAfterUI.Substring(0, firstSlashIndex);
                }
                else if (firstSlashIndex == -1 && !string.IsNullOrEmpty(pathAfterUI))
                {
                    // ���·����û��б�ܣ�˵���ļ�ֱ����UI�ļ�����
                    return Path.GetFileNameWithoutExtension(pathAfterUI);
                }
            }

            // �������Ŀ��·���������ļ����������
            return Path.GetFileNameWithoutExtension(assetPath);

        }
    }
}
