using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class JsonToolEditor : EditorWindow
{
    string excelDirectory;
    string exportDirectory;
    string scriptDirectory;
    static PackageConfigData packageConfigData;
    public string JsonToolEditorConfigPath;

    private const float LABEL_WIDTH = 110f;

    // 用于强制刷新的变量
    private bool needsRepaint = false;

    [MenuItem("开发工具/Json工具")]
    public static void ShowWindow()
    {
        // 创建窗口并获取Game窗口作为参考
        JsonToolEditor window = EditorWindow.GetWindow<JsonToolEditor>("Json工具", typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow"));
        window.minSize = new Vector2(700, 700);
        window.LoadConfig();
    }

    private void OnGUI()
    {
        // 如果需要刷新，则设置GUI为已更改状态
        if (needsRepaint)
        {
            GUI.changed = true;
            needsRepaint = false;
            Repaint();
        }

        // 使用垂直布局
        EditorGUILayout.BeginVertical();

        EditorGUI.BeginChangeCheck();

        GUILayout.Space(10);

        // 第一行 - Excel目录1
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Excel目录", GUILayout.Width(LABEL_WIDTH));
        excelDirectory = EditorGUILayout.TextField(excelDirectory, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择Excel目录", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                excelDirectory = path;
                GUI.changed = true;
                needsRepaint = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        // 第二行 - 导出数据目录
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("导出数据目录", GUILayout.Width(LABEL_WIDTH));
        exportDirectory = EditorGUILayout.TextField(exportDirectory, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择导出数据目录", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                exportDirectory = path;
                GUI.changed = true;
                needsRepaint = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        // 第三行 - 生成映射脚本目录
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("生成映射脚本目录", GUILayout.Width(LABEL_WIDTH));
        scriptDirectory = EditorGUILayout.TextField(scriptDirectory, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择脚本目录", string.IsNullOrEmpty(scriptDirectory) ? scriptDirectory : Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                scriptDirectory = path;
                GUI.changed = true;
                needsRepaint = true;
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // 底部按钮行 - 居中排列
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // 左侧弹性空间使按钮居中

        if (GUILayout.Button("导出数据", GUILayout.Width(80)))
        {
            ExportData();
        }

        GUILayout.Space(20); // 20像素间距

        if (GUILayout.Button("生成脚本", GUILayout.Width(80)))
        {
            GenerateScript();
        }

        GUILayout.FlexibleSpace(); // 右侧弹性空间使按钮居中
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        // 检测是否有变化，如果有则重绘并保存配置
        if (EditorGUI.EndChangeCheck())
        {
            Repaint();
            SaveConfig(); // 自动保存配置
        }
    }

    private void ExportData()
    {
        // 检查必填字段
        if (string.IsNullOrEmpty(excelDirectory) || string.IsNullOrEmpty(exportDirectory))
        {
            EditorUtility.DisplayDialog("错误", "Excel目录和导出数据目录不能为空", "确定");
            return;
        }
        if (!Directory.Exists(excelDirectory))
        {
            Console.WriteLine($"数据源文件夹不存在: {excelDirectory}");
            return;
        }
        if (!Directory.Exists(exportDirectory))
        {
            Console.WriteLine($"导出文件夹不存在: {exportDirectory}");
            return;
        }

        //实现导出功能
        ExcelLoader loader = new ExcelLoader(excelDirectory, 3, exportDirectory);
        loader.ExportAllJson();
    }

    private void GenerateScript()
    {
        Debug.Log("生成脚本功能");
        ScriptGenerator scriptGenerator = new ScriptGenerator(exportDirectory, scriptDirectory);
        scriptGenerator.GenerateScriptsFromJsonFiles();
    }

    public static string getRootDirPath()
    {
        if (packageConfigData == null)
        {
            LoadPackageInfo();
        }
        string path = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Packages", $"{packageConfigData.displayName}.{packageConfigData.version}");
        return path;
    }

    // 保存配置到JSON文件
    private void SaveConfig()
    {
        string directoryPath = Path.GetDirectoryName(JsonToolEditorConfigPath);

        // 确保目录存在
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 创建配置对象

        // 将配置对象序列化为JSON字符串
        JsonToolEditorConfigData configData = new JsonToolEditorConfigData
        {
            excelDirectory = excelDirectory,
            exportDirectory = exportDirectory,
            scriptDirectory = scriptDirectory
        };
        string json = JsonUtility.ToJson(configData, true);

        // 写入文件
        File.WriteAllText(JsonToolEditorConfigPath, json);
        AssetDatabase.Refresh();
    }

    // 从配置文件加载设置
    private void LoadConfig()
    {
        LoadPackageInfo();
        LoadJsonToolEditorConfigData();
        // 同时加载 package.json 信息
    }

    void LoadJsonToolEditorConfigData()
    {
        JsonToolEditorConfigPath = Path.Combine(getRootDirPath(), "Editor/Config/JsonToolEditorConfig.json");
        if (File.Exists(JsonToolEditorConfigPath))
        {
            try
            {
                string json = File.ReadAllText(JsonToolEditorConfigPath);
                var config = JsonUtility.FromJson<JsonToolEditorConfigData>(json);

                excelDirectory = config.excelDirectory;
                exportDirectory = config.exportDirectory;
                scriptDirectory = config.scriptDirectory;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"读取配置文件失败: {e.Message}");
            }
        }
    }

    // 读取 package.json 数据
    private static void LoadPackageInfo()
    {
        try
        {
            // package.json 文件路径
            string packageJsonPath = Path.Combine(Application.dataPath, "../Packages/wm.jsonTool.0.0.1/package.json");

            if (File.Exists(packageJsonPath))
            {
                string json = File.ReadAllText(packageJsonPath);
                PackageConfigData packageData = JsonUtility.FromJson<PackageConfigData>(json);

                // 打印包信息
                //Debug.Log($"加载包信息成功: {packageData.displayName} v{packageData.version}");

                packageConfigData = packageData;
            }
            else
            {
                packageConfigData = new PackageConfigData();
                Debug.LogWarning("未找到 package.json 文件: " + packageJsonPath);
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取 package.json 失败: {e.Message}");
            return;
        }
    }
}
