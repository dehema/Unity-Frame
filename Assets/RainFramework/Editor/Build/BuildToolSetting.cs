using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 打包设置
/// </summary>
public class BuildToolSetting : BuildToolBase
{
    public BuildToolSetting(BuildToolConfig _config) : base(_config)
    {
        pageName = "打包设置";
    }

    protected override void DrawContent()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("安装包导出目录:", GUILayout.Width(120));
        config.exportPath = EditorGUILayout.TextField(config.exportPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("选择导出目录", config.exportPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                config.exportPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标平台:", GUILayout.Width(80));
        config.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(config.buildTarget);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("打开导出目录"))
        {
            OpenPackageExportDirectory();
        }

        if (GUILayout.Button("清理导出目录"))
        {
            ClearPackageExportDirectory();
        }

        EditorGUILayout.EndHorizontal();

    }

    private void OpenPackageExportDirectory()
    {
        if (Directory.Exists(config.exportPath))
        {
            EditorUtility.RevealInFinder(config.exportPath + "/");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "导出目录不存在", "确定");
        }
    }

    private void ClearPackageExportDirectory()
    {
        if (Directory.Exists(config.exportPath))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清理导出目录吗？", "确定", "取消"))
            {
                Directory.Delete(config.exportPath, true);
                Directory.CreateDirectory(config.exportPath);
                Debug.Log("导出目录已清理");
            }
        }
    }
}
