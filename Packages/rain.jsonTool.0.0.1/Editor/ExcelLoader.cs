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
        //����json
        List<string> excelPaths = GetExcelFiles(excelDir);
        int errorNum = 0;
        foreach (string excelPath in excelPaths)
        {
            bool res = GenerateJson(excelPath, headerRow, exportDir);
            if (!res)
            {
                Debug.LogError($"{excelPath}����ʧ��");
                errorNum++;
                break;
            }

        }
        Debug.Log($"�������\n������{excelPaths.Count - errorNum}���ļ�,������\n{exportDir}");
        if (errorNum > 0)
        {
            Debug.LogError($"ʧ��{errorNum}��");
        }
    }

    bool GenerateJson(string excelPath, int headerRow, string exportDir)
    {
        string exePath = getCmdPath();
        // ���ÿ�ִ���ļ�
        try
        {
            if (!File.Exists(excelPath))
            {
                Debug.LogError($"�Ҳ���excel�ļ�:{excelPath}");
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
            //Debug.Log("Excel2JsonEX.CMD.exe ����������");

            // �ȴ����̽�������ӡ��־
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            //Debug.Log("Excel2JsonEX.CMD.exe ��������ɣ��˳�����: " + process.ExitCode);
            //Debug.Log("���������: " + output);

            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("�����д���: " + error);
                return false;
            }
            if (output.Contains("Error"))
            {
                Debug.LogError("�����д���: " + output);
                return false;
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("���� Excel2JsonEX.CMD.exe ʧ��: " + e.Message);
            return false;
        }
    }

    //��ȡ�����ļ�λ��
    string getCmdPath()
    {
        // ��ȡ�ű���ǰ����·��
        string exePath = Path.Combine(JsonToolEditor.getRootDirPath(), "Editor/Tool/Excel2JsonEX.CMD.exe");
        //Debug.Log("Excel2Jsonִ��λ��: " + exePath);

        // ����ļ��Ƿ����
        if (!File.Exists(exePath))
        {
            Debug.LogError("δ�ҵ� Excel2JsonEX.CMD.exe��·��: " + exePath);
            return "";
        }
        return exePath;
    }

    private static readonly string[] excelExtensions = { ".xlsx", ".xls" };
    /// <summary>
    /// ��ȡָ��Ŀ¼�µ�����Excel�ļ�
    /// </summary>
    /// <param name="directoryPath">Ŀ¼·��</param>
    /// <param name="searchSubdirectories">�Ƿ�������Ŀ¼</param>
    /// <returns>Excel�ļ�·���б�</returns>
    public List<string> GetExcelFiles(string directoryPath, bool searchSubdirectories = true)
    {
        List<string> excelFiles = new List<string>();
        SearchOption searchOption = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        try
        {
            // ��ȡ�����ļ�
            string[] allFiles = Directory.GetFiles(directoryPath, "*.*", searchOption);

            // ɸѡ��Excel�ļ�
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
            Console.WriteLine($"����: ���ʱ��ܾ� - {directoryPath}");
        }
        catch (DirectoryNotFoundException)
        {
            Console.WriteLine($"����: Ŀ¼������ - {directoryPath}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"����: ��ȡĿ¼ʱ����I/O���� - {directoryPath}: {ex.Message}");
        }

        return excelFiles;
    }
}

