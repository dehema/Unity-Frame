using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using System.Text;
using System;
using Debug = UnityEngine.Debug;
using UnityEditor;

public class ExcelLoader
{
    string excelDir;
    int headerRow;
    string exportDir;

    public ExcelLoader(string _excelDir, int _headerRow, string _exportDir)
    {
        excelDir = _excelDir;
        headerRow = _headerRow;
        exportDir = _exportDir;
    }

    public void ExportAllJson()
    {
        //生成json
        List<string> excelPaths = GetExcelFiles(excelDir);
        int errorNum = 0;
        foreach (string excelPath in excelPaths)
        {
            bool res = GenerateJson(excelPath, headerRow, exportDir);
            if (!res)
            {
                Debug.LogError($"{excelPath}导出失败");
                errorNum++;
                break;
            }

        }
        Debug.Log($"导出完成\n共导出{excelPaths.Count - errorNum}个文件,导出至\n{exportDir}");
        if (errorNum > 0)
        {
            Debug.LogError($"失败{errorNum}个");
        }
    }

    bool GenerateJson(string excelPath, int headerRow, string exportDir)
    {
        string exePath = getCmdPath();
        // 调用可执行文件
        try
        {
            if (!File.Exists(excelPath))
            {
                Debug.LogError($"找不到excel文件:{excelPath}");
                return false;
            }
            string fileAbbrName = Path.GetFileNameWithoutExtension(excelPath);
            string exportPath = Path.Combine(exportDir, $"{fileAbbrName}.json");
            string commond = $"{exePath} --excel {excelPath} --json {exportPath} --header {headerRow} --s --exclude_prefix #";
            //Debug.Log(commond);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/c {commond}";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            Process process = Process.Start(startInfo);
            //Debug.Log("Excel2JsonEX.CMD.exe 进程已启动");

            // 等待进程结束并打印日志
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            //Debug.Log("Excel2JsonEX.CMD.exe 进程已完成，退出代码: " + process.ExitCode);
            //Debug.Log("命令行输出: " + output);

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("命令行错误: " + error);
                return false;
            }
            if (output.Contains("Error"))
            {
                Debug.LogError("命令行错误: " + output);
                return false;
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("启动 Excel2JsonEX.CMD.exe 失败: " + e.Message);
            return false;
        }
    }

    //获取命令文件位置
    string getCmdPath()
    {
        // 获取脚本当前所在路径
        string exePath = Path.Combine(JsonToolEditor.getRootDirPath(), "Editor/Tool/Excel2JsonEX.CMD.exe");
        //Debug.Log("Excel2Json执行位置: " + exePath);

        // 检查文件是否存在
        if (!File.Exists(exePath))
        {
            Debug.LogError("未找到 Excel2JsonEX.CMD.exe，路径: " + exePath);
            return "";
        }
        return exePath;
    }

    private static readonly string[] excelExtensions = { ".xlsx", ".xls" };
    /// <summary>
    /// 获取指定目录下的所有Excel文件
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    /// <param name="searchSubdirectories">是否搜索子目录</param>
    /// <returns>Excel文件路径列表</returns>
    public List<string> GetExcelFiles(string directoryPath, bool searchSubdirectories = true)
    {
        List<string> excelFiles = new List<string>();
        SearchOption searchOption = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        try
        {
            // 获取所有文件
            string[] allFiles = Directory.GetFiles(directoryPath, "*.*", searchOption);

            // 筛选出Excel文件
            foreach (string file in allFiles)
            {
                string extension = Path.GetExtension(file).ToLowerInvariant();
                if (Array.IndexOf(excelExtensions, extension) >= 0)
                {
                    excelFiles.Add(file);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"警告: 访问被拒绝 - {directoryPath}");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"警告: 目录不存在 - {directoryPath}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"警告: 读取目录时发生I/O错误 - {directoryPath}: {ex.Message}");
        }

        return excelFiles;
    }
}

