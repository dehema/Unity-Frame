using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D;

/// <summary>
/// 图集工具类
/// </summary>
public class BuildToolAtlas : BuildToolBase
{
    private const string sourcePath = "Assets/AssetBundles/Art/Resources/UI";
    private const string atlasPath = "Assets/AssetBundles/Art/Atlas";

    public BuildToolAtlas(BuildToolConfig _config) : base(_config)
    {
    }

    /// <summary>
    /// 绘制图集列表
    /// </summary>
    public static void DrawAtlasList()
    {
        if (Directory.Exists(atlasPath))
        {
            // 获取所有.spriteatlas文件
            string[] atlasFiles = Directory.GetFiles(atlasPath, "*.spriteatlas", SearchOption.AllDirectories);

            if (atlasFiles.Length > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"图集列表 ({atlasFiles.Length} 个):", EditorStyles.boldLabel);

                // 创建滚动区域
                EditorGUILayout.BeginVertical("box");

                foreach (string atlasFile in atlasFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(atlasFile);
                    string relativePath = atlasFile.Replace(Application.dataPath, "Assets");

                    EditorGUILayout.BeginHorizontal();

                    // 显示图集名称
                    EditorGUILayout.LabelField($"{fileName}", GUILayout.Width(200));

                    // 显示文件大小
                    FileInfo fileInfo = new FileInfo(atlasFile);
                    string fileSize = FormatFileSize(fileInfo.Length);
                    EditorGUILayout.LabelField(fileSize, GUILayout.Width(80));

                    // 选择按钮
                    if (GUILayout.Button("选择", GUILayout.Width(50)))
                    {
                        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(relativePath);
                        if (atlas != null)
                        {
                            Selection.activeObject = atlas;
                            EditorGUIUtility.PingObject(atlas);
                        }
                    }

                    // 删除按钮
                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        if (EditorUtility.DisplayDialog("确认删除", $"确定要删除图集 {fileName} 吗？", "确定", "取消"))
                        {
                            AssetDatabase.DeleteAsset(relativePath);
                            AssetDatabase.Refresh();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("暂无图集文件", EditorStyles.centeredGreyMiniLabel);
            }
        }
        else
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Atlas目录不存在", EditorStyles.centeredGreyMiniLabel);
        }
    }

    /// <summary>
    /// 绘制资源准备部分
    /// </summary>
    public void DrawResourcePreparationSection()
    {
        EditorGUILayout.BeginHorizontal();
        isWindowExpanded = EditorGUILayout.Foldout(isWindowExpanded, "【资源准备】", true, EditorStyles.foldoutHeader);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        if (isWindowExpanded)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("打包图集", GUILayout.Height(30)))
            {
                BuildAtlas();
            }

            if (GUILayout.Button("清理图集", GUILayout.Height(30)))
            {
                ClearAtlas();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox("打包图集：根据AssetBundles/Art/Resources/UI下的文件夹结构，在AssetBundles/Art/Atlas中创建对应的图集文件", MessageType.Info);

            // 显示Atlas文件夹内的图集列表
            DrawAtlasList();

            EditorGUILayout.EndVertical();
        }
    }

    /// <summary>
    /// 打包图集
    /// </summary>
    public static void BuildAtlas()
    {
        try
        {
            EditorUtility.DisplayProgressBar("打包图集", "正在扫描UI文件夹...", 0f);

            // 确保Atlas目录存在
            if (!Directory.Exists(atlasPath))
            {
                Directory.CreateDirectory(atlasPath);
                AssetDatabase.Refresh();
            }

            // 获取UI目录下的所有文件夹
            string[] uiFolders = Directory.GetDirectories(sourcePath);
            int totalFolders = uiFolders.Length;
            int processedFolders = 0;

            foreach (string folderPath in uiFolders)
            {
                string folderName = Path.GetFileName(folderPath);
                EditorUtility.DisplayProgressBar("打包图集", $"正在处理文件夹: {folderName}", (float)processedFolders / totalFolders);

                // 创建对应的图集文件
                CreateAtlasForFolder(folderPath, folderName, atlasPath);

                processedFolders++;
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            Debug.Log($"打包图集完成，成功处理 {processedFolders} 个文件夹的图集");

            // 自动进行Pack Preview
            AutoPackPreview();
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包图集失败", $"错误: {e.Message}", "确定");
        }
    }

    /// <summary>
    /// 为文件夹创建图集
    /// </summary>
    private static void CreateAtlasForFolder(string folderPath, string folderName, string atlasPath)
    {
        Debug.Log($"开始处理文件夹: {folderName}, 路径: {folderPath}");

        // 获取文件夹中的所有图片文件
        string[] imageFiles = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);

        if (imageFiles.Length == 0)
        {
            Debug.LogWarning($"文件夹 {folderName} 中没有找到PNG图片文件");
            return;
        }

        // 收集所有图片
        List<Texture2D> textures = new List<Texture2D>();
        foreach (string imageFile in imageFiles)
        {
            string relativePath = imageFile.Replace(Application.dataPath, "Assets");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
            if (texture != null)
            {
                textures.Add(texture);
            }
            else
            {
                Debug.LogWarning($"无法加载纹理: {relativePath}");
            }
        }

        if (textures.Count == 0)
        {
            Debug.LogWarning($"文件夹 {folderName} 中没有有效的纹理文件");
            return;
        }

        try
        {
            // 使用Unity的SpriteAtlas创建图集
            SpriteAtlas spriteAtlas = new SpriteAtlas();

            // 设置图集参数
            SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            spriteAtlas.SetTextureSettings(textureSettings);

            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2
            };
            spriteAtlas.SetPackingSettings(packingSettings);

            // 添加所有纹理到图集
            var objects = new UnityEngine.Object[textures.Count];
            for (int i = 0; i < textures.Count; i++)
            {
                objects[i] = textures[i];
            }
            spriteAtlas.Add(objects);

            // 创建图集资源文件
            string atlasAssetPath = Path.Combine(atlasPath, $"{folderName}.spriteatlas");

            AssetDatabase.CreateAsset(spriteAtlas, atlasAssetPath);

            Debug.Log($"为图集 {folderName} 创建了 {textures.Count} 个sprite，图集文件: {atlasPath}");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"图集 {folderName} 创建完成");
        }
        catch (Exception e)
        {
            Debug.LogError($"创建图集 {folderName} 时发生错误: {e.Message}");
            Debug.LogError($"错误堆栈: {e.StackTrace}");
        }
    }

    /// <summary>
    /// 清理图集
    /// </summary>
    public static void ClearAtlas()
    {
        if (Directory.Exists(atlasPath))
        {
            if (EditorUtility.DisplayDialog("确认清理", "确定要清理所有图集文件吗？", "确定", "取消"))
            {
                // 删除所有图集文件
                string[] atlasFiles = Directory.GetFiles(atlasPath, "*.*", SearchOption.AllDirectories);
                foreach (string file in atlasFiles)
                {
                    File.Delete(file);
                }

                // 删除空文件夹
                string[] emptyDirs = Directory.GetDirectories(atlasPath, "*", SearchOption.AllDirectories);
                foreach (string dir in emptyDirs)
                {
                    if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                    {
                        Directory.Delete(dir);
                    }
                }

                AssetDatabase.Refresh();
            }
        }
        else
        {
            EditorUtility.DisplayDialog("提示", "Atlas目录不存在", "确定");
        }
    }

    /// <summary>
    /// 自动进行Pack Preview
    /// </summary>
    private static void AutoPackPreview()
    {
        try
        {
            if (!Directory.Exists(atlasPath))
            {
                Debug.LogWarning("Atlas目录不存在，无法进行Pack Preview");
                return;
            }

            string[] atlasFiles = Directory.GetFiles(atlasPath, "*.spriteatlas", SearchOption.AllDirectories);

            if (atlasFiles.Length == 0)
            {
                Debug.LogWarning("没有找到图集文件，无法进行Pack Preview");
                return;
            }

            Debug.Log($"开始自动Pack Preview，共 {atlasFiles.Length} 个图集");

            int successCount = 0;
            int failCount = 0;

            for (int i = 0; i < atlasFiles.Length; i++)
            {
                string atlasFile = atlasFiles[i];
                string fileName = Path.GetFileNameWithoutExtension(atlasFile);

                try
                {
                    // 显示进度条
                    EditorUtility.DisplayProgressBar("Pack Preview", $"正在处理图集: {fileName}", (float)i / atlasFiles.Length);

                    string relativePath = atlasFile.Replace(Application.dataPath, "Assets");
                    SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(relativePath);

                    if (atlas != null)
                    {
                        // 执行Pack Preview
                        SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
                        successCount++;
                        Debug.Log($"图集 {fileName} Pack Preview 完成");
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
                    Debug.LogError($"图集 {fileName} Pack Preview 失败: {e.Message}");
                }
            }

            // 清除进度条
            EditorUtility.ClearProgressBar();

            Debug.Log($"Pack Preview 完成 - 成功: {successCount}, 失败: {failCount}");

            // 显示完成提示
            if (successCount > 0)
            {
                EditorUtility.DisplayDialog("Pack Preview 完成", $"成功处理 {successCount} 个图集\n" + $"失败 {failCount} 个图集", "确定");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"自动Pack Preview 过程中发生错误: {e.Message}");
        }
    }

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        else if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";
        else
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }
}