using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;

/// <summary>
/// AB包配置数据
/// </summary>
[System.Serializable]
public class ABConfigData
{
    public string abOutputPath = "";
    public string onlineABUrl = "";
    public bool autoBuildAB = true;
}

/// <summary>
/// AB包设置工具
/// </summary>
public static class BuildToolAB
{
    const string atlasPath = "Assets/AssetBundles/Art/Atlas";
    const string configPath = "Assets/Editor/BuildToolABConfig.json";

    private static bool isABSectionExpanded = true;
    private static ABConfigData configData = new ABConfigData();

    /// <summary>
    /// 初始化配置
    /// </summary>
    static BuildToolAB()
    {
        LoadConfig();
    }

    /// <summary>
    /// 加载配置
    /// </summary>
    private static void LoadConfig()
    {
        try
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                configData = JsonUtility.FromJson<ABConfigData>(json);
            }
            else
            {
                // 设置默认值
                configData.abOutputPath = Path.Combine(Application.persistentDataPath, "AssetBundles");
                configData.onlineABUrl = "";
                configData.autoBuildAB = true;
                SaveConfig();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载AB配置失败: {e.Message}");
            configData = new ABConfigData();
        }
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    private static void SaveConfig()
    {
        try
        {
            string json = JsonUtility.ToJson(configData, true);
            File.WriteAllText(configPath, json);
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"保存AB配置失败: {e.Message}");
        }
    }

    /// <summary>
    /// 绘制AB包设置界面
    /// </summary>
    public static void DrawABSettingsSection()
    {
        EditorGUILayout.BeginHorizontal();
        isABSectionExpanded = EditorGUILayout.Foldout(isABSectionExpanded, "【AB包设置】", true, EditorStyles.foldoutHeader);
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        if (isABSectionExpanded)
        {
            EditorGUILayout.BeginVertical("box");

            // AB包设置按钮
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("一键设置AB", GUILayout.Height(30)))
            {
                SetAllAtlasAssetBundles();
            }

            if (GUILayout.Button("清除AB设置", GUILayout.Height(30)))
            {
                ClearAllAtlasAssetBundles();
            }

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            
            // AB包输出路径设置
            EditorGUILayout.LabelField("AB包输出路径:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            configData.abOutputPath = EditorGUILayout.TextField(configData.abOutputPath);
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("选择AB包输出路径", configData.abOutputPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    configData.abOutputPath = path;
                    SaveConfig();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            
            // 线上AB包地址设置
            EditorGUILayout.LabelField("线上AB包地址:", EditorStyles.boldLabel);
            configData.onlineABUrl = EditorGUILayout.TextField(configData.onlineABUrl);
            
            EditorGUILayout.Space(10);
            
            // AB包操作按钮
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("一键打包AB", GUILayout.Height(30)))
            {
                BuildAllAssetBundles();
            }

            if (GUILayout.Button("上传AB包", GUILayout.Height(30)))
            {
                UploadAssetBundles();
            }

            if (GUILayout.Button("打开AB目录", GUILayout.Height(30)))
            {
                OpenAssetBundleDirectory();
            }

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(10);
            
            // 自动打包选项
            configData.autoBuildAB = EditorGUILayout.Toggle("自动打包AB包", configData.autoBuildAB);
            
            if (GUI.changed)
            {
                SaveConfig();
            }
            
            EditorGUILayout.HelpBox("一键设置AB：自动为所有图集设置AssetBundle标签\n" +
                                   "一键打包AB：构建所有AssetBundle到指定目录\n" +
                                   "上传AB包：将AB包上传到线上服务器", MessageType.Info);
            
            // 显示当前AB包状态
            DrawABStatusInfo();
            
            EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// 一键设置所有图集的AB包标签
    /// </summary>
    public static void SetAllAtlasAssetBundles()
    {
        try
        {
            if (!Directory.Exists(atlasPath))
            {
                EditorUtility.DisplayDialog("错误", "Atlas目录不存在，请先创建图集", "确定");
                return;
            }

            string[] atlasFiles = Directory.GetFiles(atlasPath, "*.spriteatlas", SearchOption.AllDirectories);
            
            if (atlasFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有找到图集文件", "确定");
                return;
            }

            EditorUtility.DisplayProgressBar("设置AB包", "正在处理图集...", 0f);

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < atlasFiles.Length; i++)
            {
                string atlasFile = atlasFiles[i];
                string fileName = Path.GetFileNameWithoutExtension(atlasFile);
                
                try
                {
                    EditorUtility.DisplayProgressBar("设置AB包", $"正在处理: {fileName}", (float)i / atlasFiles.Length);
                    
                    string relativePath = atlasFile.Replace(Application.dataPath, "Assets");
                    SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(relativePath);
                    
                    if (atlas != null)
                    {
                        // 生成AB包名称（基于图集路径）
                        string abName = GenerateAssetBundleName(relativePath);
                        
                        // 设置AssetBundle标签
                        AssetImporter importer = AssetImporter.GetAtPath(relativePath);
                        if (importer != null)
                        {
                            importer.assetBundleName = abName;
                            importer.assetBundleVariant = "";
                            successCount++;
                            Debug.Log($"图集 {fileName} 设置AB包标签: {abName}");
                        }
                        else
                        {
                            failCount++;
                            Debug.LogError($"无法获取图集 {fileName} 的AssetImporter");
                        }
                    }
                    else
                    {
                        failCount++;
                        Debug.LogError($"无法加载图集: {relativePath}");
                    }
                }
                catch (Exception e)
                {
                    failCount++;
                    Debug.LogError($"设置图集 {fileName} AB包标签失败: {e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Debug.Log($"AB包设置完成 - 成功: {successCount}, 失败: {failCount}");
            
            EditorUtility.DisplayDialog("AB包设置完成", 
                $"成功设置 {successCount} 个图集\n" +
                $"失败 {failCount} 个图集", "确定");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("设置AB包失败", $"错误: {e.Message}", "确定");
        }
    }

    /// <summary>
    /// 清除所有图集的AB包设置
    /// </summary>
    public static void ClearAllAtlasAssetBundles()
    {
        try
        {
            if (!Directory.Exists(atlasPath))
            {
                EditorUtility.DisplayDialog("提示", "Atlas目录不存在", "确定");
                return;
            }

            string[] atlasFiles = Directory.GetFiles(atlasPath, "*.spriteatlas", SearchOption.AllDirectories);
            
            if (atlasFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有找到图集文件", "确定");
                return;
            }

            EditorUtility.DisplayProgressBar("清除AB包", "正在处理图集...", 0f);

            int successCount = 0;

            for (int i = 0; i < atlasFiles.Length; i++)
            {
                string atlasFile = atlasFiles[i];
                string fileName = Path.GetFileNameWithoutExtension(atlasFile);
                
                try
                {
                    EditorUtility.DisplayProgressBar("清除AB包", $"正在处理: {fileName}", (float)i / atlasFiles.Length);
                    
                    string relativePath = atlasFile.Replace(Application.dataPath, "Assets");
                    AssetImporter importer = AssetImporter.GetAtPath(relativePath);
                    
                    if (importer != null)
                    {
                        importer.assetBundleName = "";
                        importer.assetBundleVariant = "";
                        successCount++;
                        Debug.Log($"图集 {fileName} 清除AB包标签");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"清除图集 {fileName} AB包标签失败: {e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Debug.Log($"清除AB包设置完成 - 处理了 {successCount} 个图集");
            
            EditorUtility.DisplayDialog("清除AB包设置完成", $"已清除 {successCount} 个图集的AB包设置", "确定");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("清除AB包设置失败", $"错误: {e.Message}", "确定");
        }
    }

    /// <summary>
    /// 生成AssetBundle名称
    /// </summary>
    private static string GenerateAssetBundleName(string atlasPath)
    {
        // 从路径中提取图集名称
        // 例如: Assets/AssetBundles/Art/Atlas/UI.spriteatlas -> UI
        string fileName = Path.GetFileNameWithoutExtension(atlasPath);
        
        // 生成AB包名称，使用小写并添加前缀
        string abName = $"atlas_{fileName.ToLower()}";
        
        return abName;
    }

    /// <summary>
    /// 绘制AB包状态信息
    /// </summary>
    private static void DrawABStatusInfo()
    {
        if (!Directory.Exists(atlasPath))
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Atlas目录不存在", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        string[] atlasFiles = Directory.GetFiles(atlasPath, "*.spriteatlas", SearchOption.AllDirectories);
        
        if (atlasFiles.Length > 0)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField($"图集AB包状态 ({atlasFiles.Length} 个):", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            
            int setCount = 0;
            int unsetCount = 0;
            
            foreach (string atlasFile in atlasFiles)
            {
                string relativePath = atlasFile.Replace(Application.dataPath, "Assets");
                AssetImporter importer = AssetImporter.GetAtPath(relativePath);
                
                if (importer != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(atlasFile);
                    string abName = importer.assetBundleName;
                    bool isSet = !string.IsNullOrEmpty(abName);
                    
                    if (isSet) setCount++;
                    else unsetCount++;
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{fileName}", GUILayout.Width(150));
                    
                    if (isSet)
                    {
                        EditorGUILayout.LabelField($"✓ {abName}", GUILayout.Width(120));
                        GUI.color = Color.green;
                        EditorGUILayout.LabelField("已设置", GUILayout.Width(60));
                        GUI.color = Color.white;
                    }
                    else
                    {
                        EditorGUILayout.LabelField("未设置", GUILayout.Width(120));
                        GUI.color = Color.red;
                        EditorGUILayout.LabelField("未设置", GUILayout.Width(60));
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"统计: 已设置 {setCount} 个, 未设置 {unsetCount} 个", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();
        }
        else
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("暂无图集文件", EditorStyles.centeredGreyMiniLabel);
        }
    }

    /// <summary>
    /// 一键打包所有AssetBundle
    /// </summary>
    public static void BuildAllAssetBundles()
    {
        try
        {
            if (string.IsNullOrEmpty(configData.abOutputPath))
            {
                EditorUtility.DisplayDialog("错误", "请先设置AB包输出路径", "确定");
                return;
            }

            // 确保输出目录存在
            if (!Directory.Exists(configData.abOutputPath))
            {
                Directory.CreateDirectory(configData.abOutputPath);
            }

            EditorUtility.DisplayProgressBar("打包AB包", "正在构建AssetBundle...", 0f);

            // 构建AssetBundle
            BuildPipeline.BuildAssetBundles(configData.abOutputPath, 
                BuildAssetBundleOptions.None, 
                EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Debug.Log($"AB包打包完成，输出目录: {configData.abOutputPath}");
            EditorUtility.DisplayDialog("打包完成", $"AB包已打包到:\n{configData.abOutputPath}", "确定");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包失败", $"错误: {e.Message}", "确定");
        }
    }

    /// <summary>
    /// 上传AssetBundle到服务器
    /// </summary>
    public static void UploadAssetBundles()
    {
        if (string.IsNullOrEmpty(configData.onlineABUrl))
        {
            EditorUtility.DisplayDialog("提示", "请先设置线上AB包地址", "确定");
            return;
        }

        if (string.IsNullOrEmpty(configData.abOutputPath) || !Directory.Exists(configData.abOutputPath))
        {
            EditorUtility.DisplayDialog("错误", "AB包输出目录不存在，请先打包AB包", "确定");
            return;
        }

        // TODO: 实现上传功能
        EditorUtility.DisplayDialog("上传功能", "上传功能待实现\n" +
            $"本地路径: {configData.abOutputPath}\n" +
            $"服务器地址: {configData.onlineABUrl}", "确定");
    }

    /// <summary>
    /// 打开AB包目录
    /// </summary>
    public static void OpenAssetBundleDirectory()
    {
        if (string.IsNullOrEmpty(configData.abOutputPath))
        {
            EditorUtility.DisplayDialog("错误", "请先设置AB包输出路径", "确定");
            return;
        }

        if (!Directory.Exists(configData.abOutputPath))
        {
            EditorUtility.DisplayDialog("提示", "AB包输出目录不存在", "确定");
            return;
        }

        EditorUtility.RevealInFinder(configData.abOutputPath);
    }

    /// <summary>
    /// 检查是否启用自动打包AB包
    /// </summary>
    public static bool IsAutoBuildABEnabled()
    {
        return configData.autoBuildAB;
    }

    /// <summary>
    /// 获取所有图集的AB包信息
    /// </summary>
    public static void GetAtlasABInfo()
    {
        if (!Directory.Exists(atlasPath))
        {
            Debug.LogWarning("Atlas目录不存在");
            return;
        }

        string[] atlasFiles = Directory.GetFiles(atlasPath, "*.spriteatlas", SearchOption.AllDirectories);
        
        Debug.Log($"=== 图集AB包信息 ===");
        Debug.Log($"找到 {atlasFiles.Length} 个图集文件");
        
        foreach (string atlasFile in atlasFiles)
        {
            string relativePath = atlasFile.Replace(Application.dataPath, "Assets");
            AssetImporter importer = AssetImporter.GetAtPath(relativePath);
            
            if (importer != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(atlasFile);
                string abName = string.IsNullOrEmpty(importer.assetBundleName) ? "未设置" : importer.assetBundleName;
                string abVariant = string.IsNullOrEmpty(importer.assetBundleVariant) ? "无" : importer.assetBundleVariant;
                
                Debug.Log($"图集: {fileName} | AB包: {abName} | 变体: {abVariant}");
            }
        }
    }
}
