using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Rain.Core
{
    public class FileTools
    {
        /// <summary>
        /// 循环递增索引值，当超过最大值时重置为最小值
        /// </summary>
        /// <param name="pIndex">当前索引值（引用传递）</param>
        /// <param name="pMin">最小索引值</param>
        /// <param name="pMax">最大索引值</param>
        public static void IndexNext(ref int pIndex, int pMin, int pMax)
        {
            pIndex += 1;
            if (pIndex > pMax)
                pIndex = pMin;
        }

        /// <summary>
        /// 生成文件的MD5哈希值
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>MD5哈希值字符串，失败返回空字符串</returns>
        public static string CreateMd5ForFile(string filename)
        {
            try
            {
                using (FileStream file = new FileStream(filename, FileMode.Open))
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] retVal = md5.ComputeHash(file);

                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < retVal.Length; i++)
                        {
                            sb.Append(retVal[i].ToString("x2"));
                        }

                        return sb.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("文件不存在或生成MD5失败：" + ex);
                return "";
            }
        }

        /// <summary>
        /// 获取文件夹下所有文件大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static int GetAllFileSize(string filePath)
        {
            int sum = 0;
            if (!Directory.Exists(filePath))
            {
                return 0;
            }

            DirectoryInfo dti = new DirectoryInfo(filePath);

            FileInfo[] fi = dti.GetFiles();

            for (int i = 0; i < fi.Length; ++i)
            {
                sum += Convert.ToInt32(fi[i].Length / 1024);
            }

            DirectoryInfo[] di = dti.GetDirectories();

            if (di.Length > 0)
            {
                for (int i = 0; i < di.Length; i++)
                {
                    sum += GetAllFileSize(di[i].FullName);
                }
            }
            return sum;
        }

        /// <summary>
        /// 获取指定文件大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long GetFileSize(string filePath)
        {
            long sum = 0;
            if (!File.Exists(filePath))
            {
                return 0;
            }
            else
            {
                FileInfo files = new FileInfo(filePath);
                sum += files.Length;
            }
            return sum;
        }

        /// <summary>
        /// 从路径的末尾向前截取指定级别的目录
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="levels"></param>
        /// <returns></returns>
        public static string TruncatePath(string fullPath, int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                fullPath = Path.GetDirectoryName(fullPath);
                if (string.IsNullOrEmpty(fullPath))
                    break;
            }

            return fullPath;
        }

        /// <summary>
        /// 检查URI是否合法（包含协议标识符）
        /// </summary>
        /// <param name="uri">要检查的URI字符串</param>
        /// <returns>如果URI合法返回true，否则返回false</returns>
        public static bool IsLegalURI(string uri)
        {
            return !string.IsNullOrEmpty(uri) && uri.Contains("://");
        }

        /// <summary>
        /// 检查URI是否为合法的HTTP/HTTPS地址
        /// </summary>
        /// <param name="uri">要检查的URI字符串</param>
        /// <returns>如果是HTTP或HTTPS地址返回true，否则返回false</returns>
        public static bool IsLegalHTTPURI(string uri)
        {
            return !string.IsNullOrEmpty(uri) && (uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 将路径格式化为Unity标准路径（使用正斜杠）
        /// </summary>
        /// <param name="path">原始路径</param>
        /// <returns>格式化后的Unity路径</returns>
        public static string FormatToUnityPath(string path)
        {
            return path.Replace("\\", "/");
        }

        /// <summary>
        /// 批量格式化路径数组为Unity标准路径
        /// </summary>
        /// <param name="paths">路径数组</param>
        public static string[] PathsFormatToUnityPath(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = FormatToUnityPath(paths[i]);
            }
            return paths;
        }

        /// <summary>
        /// 批量格式化路径列表为Unity标准路径
        /// </summary>
        /// <param name="paths">路径列表</param>
        public static void PathsFormatToUnityPath(List<string> paths)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                paths[i] = FormatToUnityPath(paths[i]);
            }
        }


        /// <summary>
        /// 将路径格式化为系统文件路径（使用反斜杠）
        /// </summary>
        /// <param name="path">原始路径</param>
        /// <returns>格式化后的系统路径</returns>
        public static string FormatToSysFilePath(string path)
        {
            return path.Replace("/", "\\");
        }

        /// <summary>
        /// 获取文件扩展名（小写）
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件扩展名（小写）</returns>
        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path).ToLower();
        }

        /// <summary>
        /// 获取指定文件夹中符合扩展名条件的文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="extensions">文件扩展名数组，为null时返回所有文件</param>
        /// <param name="exclude">是否排除指定扩展名，true为排除，false为包含</param>
        /// <returns>符合条件的文件路径数组</returns>
        public static string[] GetSpecifyFilesInFolder(string path, string[] extensions = null, bool exclude = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (extensions == null)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            else if (exclude)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
            }
            else
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
            }
        }

        /// <summary>
        /// 根据文件名模式获取指定文件夹中的文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="pattern">文件名模式（支持通配符）</param>
        /// <returns>符合条件的文件路径数组</returns>
        public static string[] GetSpecifyFilesInFolder(string path, string pattern)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return PathsFormatToUnityPath(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
        }

        /// <summary>
        /// 获取指定文件夹中的所有图片文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="includeSubDirs">是否包含子目录，默认为true</param>
        /// <returns>图片文件路径数组</returns>
        public static string[] GetAllImageFilesInFolder(string path, bool includeSubDirs = true)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            // 常见的图片文件扩展名
            string[] imageExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tga", ".tiff", ".psd", ".exr", ".hdr" };
            
            SearchOption searchOption = includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] allFiles = Directory.GetFiles(path, "*.*", searchOption);
            
            return allFiles.Where(file => imageExtensions.Contains(GetFileExtension(file))).ToArray();
        }

        /// <summary>
        /// 根据指定文件名获取文件路径（支持递归搜索）
        /// </summary>
        /// <param name="path">搜索的文件夹路径</param>
        /// <param name="fileName">要查找的文件名（不包含路径）</param>
        /// <param name="includeSubDirs">是否包含子目录，默认为true</param>
        /// <returns>找到的文件路径数组，没有找到则返回空数组</returns>
        public static string[] GetFilesByName(string path, string fileName, bool includeSubDirs = true)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(fileName))
            {
                return new string[0];
            }

            if (!Directory.Exists(path))
            {
                return new string[0];
            }

            SearchOption searchOption = includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] allFiles = Directory.GetFiles(path, "*.*", searchOption);
            
            // 筛选出文件名匹配的文件
            return allFiles.Where(file => Path.GetFileName(file).Equals(fileName, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        /// <summary>
        /// 根据指定文件名获取第一个匹配的文件路径（支持递归搜索）
        /// </summary>
        /// <param name="path">搜索的文件夹路径</param>
        /// <param name="fileName">要查找的文件名（不包含路径）</param>
        /// <param name="includeSubDirs">是否包含子目录，默认为true</param>
        /// <returns>找到的第一个文件路径，没有找到则返回null</returns>
        public static string GetFirstFileByName(string path, string fileName, bool includeSubDirs = true)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            if (!Directory.Exists(path))
            {
                return null;
            }

            SearchOption searchOption = includeSubDirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            string[] allFiles = Directory.GetFiles(path, "*.*", searchOption);
            
            // 返回第一个匹配的文件
            return allFiles.FirstOrDefault(file => Path.GetFileName(file).Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 获取指定文件夹中的所有文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>所有文件路径数组</returns>
        public static string[] GetAllFilesInFolder(string path)
        {
            return GetSpecifyFilesInFolder(path);
        }

        /// <summary>
        /// 获取指定文件夹中的所有子目录
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>所有子目录路径数组</returns>
        public static string[] GetAllDirsInFolder(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// 递归获取指定文件夹中所有文件的文件名（排除.meta文件）
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="FileList">文件列表（用于递归调用）</param>
        /// <returns>文件名列表</returns>
        public static List<string> GetAllFilesName(string path, List<string> FileList)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] fil = dir.GetFiles();
            DirectoryInfo[] dii = dir.GetDirectories();
            foreach (FileInfo f in fil)
            {
                if (!f.FullName.EndsWith(".meta"))
                {
                    Debug.Log(f.Name);
                    FileList.Add(f.Name);
                }
            }

            //获取子文件夹内的文件列表，递归遍历
            foreach (DirectoryInfo d in dii)
            {
                GetAllFilesName(d.FullName, FileList);
            }

            return FileList;
        }

        /// <summary>
        /// 检查文件路径，如果目录不存在则创建目录
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public static void CheckFileAndCreateDirWhenNeeded(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;
            if (!dir_info.Exists)
            {
                Directory.CreateDirectory(dir_info.FullName);
            }
        }

        /// <summary>
        /// 检查目录是否存在，不存在则创建
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        public static void CheckDirAndCreateWhenNeeded(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// 安全写入字节数组到文件
        /// </summary>
        /// <param name="outFile">输出文件路径</param>
        /// <param name="outBytes">要写入的字节数组</param>
        /// <returns>写入成功返回true，失败返回false</returns>
        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllBytes(outFile, outBytes);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    string.Format("SafeWriteAllBytes failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全写入字符串数组到文件（每行一个字符串）
        /// </summary>
        /// <param name="outFile">输出文件路径</param>
        /// <param name="outLines">要写入的字符串数组</param>
        /// <returns>写入成功返回true，失败返回false</returns>
        public static bool SafeWriteAllLines(string outFile, string[] outLines)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllLines(outFile, outLines);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(
                    string.Format("SafeWriteAllLines failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全写入文本到文件
        /// </summary>
        /// <param name="outFile">输出文件路径</param>
        /// <param name="text">要写入的文本内容</param>
        /// <returns>写入成功返回true，失败返回false</returns>
        public static bool SafeWriteAllText(string outFile, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }

                File.WriteAllText(outFile, text);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeWriteAllText failed! path = {0} with err = {1}", outFile,
                    ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全读取文件的所有字节
        /// </summary>
        /// <param name="inFile">输入文件路径</param>
        /// <returns>文件字节数组，失败返回null</returns>
        public static byte[] SafeReadAllBytes(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllBytes failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 安全读取文件的所有行
        /// </summary>
        /// <param name="inFile">输入文件路径</param>
        /// <returns>文件行数组，失败返回null</returns>
        public static string[] SafeReadAllLines(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllLines(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllLines failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 安全读取文件的所有文本内容
        /// </summary>
        /// <param name="inFile">输入文件路径</param>
        /// <returns>文件文本内容，失败返回null</returns>
        public static string SafeReadAllText(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllText(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllText failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        /// <summary>
        /// 递归删除目录及其内容（私有方法）
        /// </summary>
        /// <param name="dirPath">目录路径</param>
        /// <param name="excludeName">要排除的文件名后缀数组</param>
        private static void DeleteDirectory(string dirPath, string[] excludeName = null)
        {
            if (!Directory.Exists(dirPath))
            {
                return;
            }

            string[] files = Directory.GetFiles(dirPath);
            string[] dirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                bool delete = true;
                if (excludeName != null)
                {
                    foreach (string s in excludeName)
                    {
                        if (file.EndsWith(s))
                        {
                            delete = false;
                        }
                    }
                }

                if (delete)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir, excludeName);
            }

            string[] filesAfter = Directory.GetFiles(dirPath);
            string[] dirsAfter = Directory.GetDirectories(dirPath);
            if (filesAfter.Length == 0 && dirsAfter.Length == 0)
            {
                Directory.Delete(dirPath, false);
            }
        }

        /// <summary>
        /// 安全清空目录（删除所有内容后重新创建）
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        /// <param name="excludeName">要排除的文件名后缀数组</param>
        /// <returns>操作成功返回true，失败返回false</returns>
        public static bool SafeClearDir(string folderPath, string[] excludeName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath, excludeName);
                }

                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeClearDir failed! path = {0} with err = {1}", folderPath, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全删除目录及其所有内容
        /// </summary>
        /// <param name="folderPath">目录路径</param>
        /// <param name="excludeName">要排除的文件名后缀数组</param>
        /// <returns>操作成功返回true，失败返回false</returns>
        public static bool SafeDeleteDir(string folderPath, string[] excludeName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath, excludeName);
                }

                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeDeleteDir failed! path = {0} with err: {1}", folderPath, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>操作成功返回true，失败返回false</returns>
        public static bool SafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }

                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeDeleteFile failed! path = {0} with err: {1}", filePath, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全重命名文件
        /// </summary>
        /// <param name="sourceFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>操作成功返回true，失败返回false</returns>
        public static bool SafeRenameFile(string sourceFileName, string destFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    return false;
                }

                if (!File.Exists(sourceFileName))
                {
                    return true;
                }

                SafeDeleteFile(destFileName);
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Move(sourceFileName, destFileName);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeRenameFile failed! path = {0} with err: {1}", sourceFileName,
                    ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全复制文件
        /// </summary>
        /// <param name="fromFile">源文件路径</param>
        /// <param name="toFile">目标文件路径</param>
        /// <returns>操作成功返回true，失败返回false</returns>
        public static bool SafeCopyFile(string fromFile, string toFile)
        {
            try
            {
                if (string.IsNullOrEmpty(fromFile))
                {
                    return false;
                }

                if (!File.Exists(fromFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(toFile);
                SafeDeleteFile(toFile);
                File.Copy(fromFile, toFile, true);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeCopyFile failed! formFile = {0}, toFile = {1}, with err = {2}",
                    fromFile, toFile, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 安全复制目录及其内容
        /// </summary>
        /// <param name="sourceDirName">源目录路径</param>
        /// <param name="destDirName">目标目录路径</param>
        /// <param name="copySubDirs">是否复制子目录</param>
        /// <param name="excludeName">要排除的文件名后缀数组</param>
        /// <returns>操作成功返回true，失败返回false</returns>
        public static bool SafeCopyDirectory(string sourceDirName, string destDirName, bool copySubDirs, string[] excludeName = null)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                if (dir.Exists == false)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();
                if (Directory.Exists(destDirName) == false)
                {
                    Directory.CreateDirectory(destDirName);
                }

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    bool copy = true;
                    if (excludeName != null)
                    {
                        foreach (string s in excludeName)
                        {
                            if (file.Name.EndsWith(s))
                            {
                                copy = false;
                            }
                        }
                    }
                    if (copy)
                    {
                        string temppath = Path.Combine(destDirName, file.Name);
                        file.CopyTo(temppath, true);
                    }
                }

                if (copySubDirs == true)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        SafeCopyDirectory(subdir.FullName, temppath, copySubDirs);
                    }
                }
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeCopyDirectory failed! sourceDirName = {0}, destDirName = {1}, with err = {2}",
                    sourceDirName, destDirName, ex.Message));
                return false;
            }
        }

        /// <summary>
        /// 移动文件到新位置
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="dest">目标文件路径</param>
        /// <param name="overwrite">是否覆盖已存在的文件</param>
        public static void MoveFile(string source, string dest, bool overwrite = true)
        {
            var directoryInfo = new FileInfo(dest).Directory;
            if (directoryInfo != null)
            {
                var targetPath = directoryInfo.FullName;

                if (Directory.Exists(targetPath) == false)
                {
                    Directory.CreateDirectory(targetPath);
                }
            }

            if (File.Exists(source) == true)
            {
                if (overwrite == true)
                {
                    if (File.Exists(dest) == true)
                    {
                        File.SetAttributes(dest, FileAttributes.Normal);
                        File.Delete(dest);
                    }
                }
                File.Move(source, dest);
            }
        }

        /// <summary>
        /// 使用XOR加密数据
        /// </summary>
        /// <param name="targetData">要加密的数据</param>
        /// <param name="key">加密密钥</param>
        /// <returns>加密后的数据</returns>
        public static byte[] Encypt(byte[] targetData, byte key)
        {
            var result = new byte[targetData.Length];
            //key异或
            int dataLength = targetData.Length;
            for (int i = 0; i < dataLength; ++i)
            {
                result[i] = (byte)(targetData[i] ^ key);
            }

            return result;
        }
    }
}