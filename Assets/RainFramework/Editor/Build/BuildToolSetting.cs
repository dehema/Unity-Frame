using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �������
/// </summary>
public class BuildToolSetting : BuildToolBase
{
    public BuildToolSetting(BuildToolConfig _config) : base(_config)
    {
        pageName = "�������";
    }

    protected override void DrawContent()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("��װ������Ŀ¼:", GUILayout.Width(120));
        config.exportPath = EditorGUILayout.TextField(config.exportPath);
        if (GUILayout.Button("���", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("ѡ�񵼳�Ŀ¼", config.exportPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                config.exportPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Ŀ��ƽ̨:", GUILayout.Width(80));
        config.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(config.buildTarget);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("�򿪵���Ŀ¼"))
        {
            OpenPackageExportDirectory();
        }

        if (GUILayout.Button("������Ŀ¼"))
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
            EditorUtility.DisplayDialog("����", "����Ŀ¼������", "ȷ��");
        }
    }

    private void ClearPackageExportDirectory()
    {
        if (Directory.Exists(config.exportPath))
        {
            if (EditorUtility.DisplayDialog("ȷ��", "ȷ��Ҫ������Ŀ¼��", "ȷ��", "ȡ��"))
            {
                Directory.Delete(config.exportPath, true);
                Directory.CreateDirectory(config.exportPath);
                Debug.Log("����Ŀ¼������");
            }
        }
    }
}
