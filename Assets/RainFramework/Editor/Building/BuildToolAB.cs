using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// AB包设置工具
/// </summary>
public static class BuildToolAB
{
    const string atlasPath = "Assets/AssetBundles/Art/Atlas";

    private static bool isABSectionExpanded = true;

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
        
        EditorGUILayout.HelpBox("一键设置AB：自动为所有图集设置AssetBundle标签，标签名从图集路径自动生成", MessageType.Info);
        
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
