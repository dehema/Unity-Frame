using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using Path = System.IO.Path;

/// <summary>
/// AB包设置工具
/// </summary>
public class BuildToolAB : BuildToolBase
{
    const string atlasPath = "Assets/AssetBundles/Art/Atlas";

    public BuildToolAB(BuildToolConfig _config) : base(_config)
    {
    }

    /// <summary>
    /// 绘制AB包设置界面
    /// </summary>
    public void DrawABSettingsSection()
    {
        EditorGUILayout.BeginHorizontal();
        isWindowExpanded = EditorGUILayout.Foldout(isWindowExpanded, "【AB包设置】", true, EditorStyles.foldoutHeader);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        if (isWindowExpanded)
        {
            EditorGUILayout.BeginVertical("box");

            // AB包设置按钮
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("一键设置AB", GUILayout.Height(30)))
            {
                SetAllAssetBundles();
            }

            if (GUILayout.Button("清除AB设置", GUILayout.Height(30)))
            {
                ClearAllAssetBundles();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // AB包输出路径设置
            EditorGUILayout.LabelField("AB包输出路径:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            config.abOutputPath = EditorGUILayout.TextField(config.abOutputPath);
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("选择AB包输出路径", config.abOutputPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    config.abOutputPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // 线上AB包地址设置
            EditorGUILayout.LabelField("线上AB包地址:", EditorStyles.boldLabel);
            config.onlineABUrl = EditorGUILayout.TextField(config.onlineABUrl);

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
            config.autoBuildAB = EditorGUILayout.Toggle("自动打包AB包", config.autoBuildAB);

            if (GUI.changed)
            {
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
    /// 设置所有资源的ab
    /// </summary>
    public void SetAllAssetBundles()
    {
        SetAllAtlasAssetBundles();
    }

    /// <summary>
    /// 一键设置所有图集的AB包标签
    /// </summary>
    public void SetAllAtlasAssetBundles()
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
    /// 清除所有资源的AB包设置
    /// </summary>
    public void ClearAllAssetBundles()
    {
        try
        {
            // 获取所有资源
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

            EditorUtility.DisplayProgressBar("清除AB包设置", "正在扫描资源...", 0f);

            int clearedCount = 0;
            int totalAssets = allAssetPaths.Length;

            for (int i = 0; i < allAssetPaths.Length; i++)
            {
                string assetPath = allAssetPaths[i];

                // 跳过非资源文件
                if (assetPath.StartsWith("Assets/") == false)
                    continue;

                // 跳过文件夹
                if (Directory.Exists(assetPath))
                    continue;

                try
                {
                    EditorUtility.DisplayProgressBar("清除AB包设置",
                        $"正在处理: {Path.GetFileName(assetPath)}",
                        (float)i / totalAssets);

                    AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                    if (importer != null)
                    {
                        // 检查是否有AB包设置
                        if (!string.IsNullOrEmpty(importer.assetBundleName) ||
                            !string.IsNullOrEmpty(importer.assetBundleVariant))
                        {
                            // 清除AB包设置
                            importer.assetBundleName = "";
                            importer.assetBundleVariant = "";
                            clearedCount++;

                            Debug.Log($"已清除AB包设置: {assetPath}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"清除资源 {assetPath} 的AB包设置失败: {e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Debug.Log($"AB包设置清除完成 - 共清除 {clearedCount} 个资源的AB包设置");

            EditorUtility.DisplayDialog("清除完成",
                $"已清除 {clearedCount} 个资源的AB包设置", "确定");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("清除失败", $"错误: {e.Message}", "确定");
        }
    }

    /// <summary>
    /// 生成AssetBundle名称
    /// </summary>
    private string GenerateAssetBundleName(string atlasPath)
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
    private void DrawABStatusInfo()
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
    /// 一键打包所有AssetBundle到AB包输出目录
    /// </summary>
    public void BuildAllAssetBundles()
    {
        try
        {
            if (string.IsNullOrEmpty(config.abOutputPath))
            {
                EditorUtility.DisplayDialog("错误", "请先设置AB包输出路径", "确定");
                return;
            }

            // 确保输出目录存在
            if (!Directory.Exists(config.abOutputPath))
            {
                Directory.CreateDirectory(config.abOutputPath);
            }

            //删除目录下的所有文件和子文件夹
            CleanDirectory(config.abOutputPath);


            EditorUtility.DisplayProgressBar("打包AB包", "正在构建AssetBundle...", 0f);

            // 构建AssetBundle
            BuildPipeline.BuildAssetBundles(config.abOutputPath,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Debug.Log($"AB包打包完成，输出目录: {config.abOutputPath}");
            System.Diagnostics.Process.Start("Explorer.exe", config.abOutputPath);
            EditorUtility.DisplayDialog("打包完成", $"AB包已打包到:\n{config.abOutputPath}", "确定");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包失败", $"错误: {e.Message}", "确定");
        }
    }

    /// <summary>
    /// 清除文件夹下的所有文件和子文件夹 
    /// </summary>
    /// <param name="_direPath"></param>
    private void CleanDirectory(string _direPath)
    {
        try
        {
            if (Directory.Exists(_direPath))
            {
                string[] files = Directory.GetFiles(_direPath, "*", SearchOption.AllDirectories);
                string[] directories = Directory.GetDirectories(_direPath, "*", SearchOption.AllDirectories);

                // 删除所有文件
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        //Debug.Log($"已删除文件: {file}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"删除文件失败: {file}, 错误: {e.Message}");
                    }
                }

                // 删除所有子目录（从最深层的目录开始删除）
                Array.Sort(directories, (x, y) => y.Length.CompareTo(x.Length));
                foreach (string directory in directories)
                {
                    try
                    {
                        Directory.Delete(directory);
                        Debug.Log($"已删除目录: {directory}");
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"删除目录失败: {directory}, 错误: {e.Message}");
                    }
                }

                Debug.Log($"已清理AB包输出目录: {_direPath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"清理AB包输出目录失败: {e.Message}");
        }
    }

    /// <summary>
    /// 上传AssetBundle到服务器
    /// </summary>
    public void UploadAssetBundles()
    {
        if (string.IsNullOrEmpty(config.onlineABUrl))
        {
            EditorUtility.DisplayDialog("提示", "请先设置线上AB包地址", "确定");
            return;
        }

        if (string.IsNullOrEmpty(config.abOutputPath) || !Directory.Exists(config.abOutputPath))
        {
            EditorUtility.DisplayDialog("错误", "AB包输出目录不存在，请先打包AB包", "确定");
            return;
        }

        // TODO: 实现上传功能
        EditorUtility.DisplayDialog("上传功能", "上传功能待实现\n" +
            $"本地路径: {config.abOutputPath}\n" +
            $"服务器地址: {config.onlineABUrl}", "确定");
    }

    /// <summary>
    /// 打开AB包目录
    /// </summary>
    public void OpenAssetBundleDirectory()
    {
        if (string.IsNullOrEmpty(config.abOutputPath))
        {
            EditorUtility.DisplayDialog("错误", "请先设置AB包输出路径", "确定");
            return;
        }

        if (!Directory.Exists(config.abOutputPath))
        {
            EditorUtility.DisplayDialog("提示", "AB包输出目录不存在", "确定");
            return;
        }

        EditorUtility.RevealInFinder(config.abOutputPath);
    }

    /// <summary>
    /// 检查是否启用自动打包AB包
    /// </summary>
    public bool IsAutoBuildABEnabled()
    {
        return config.autoBuildAB;
    }

    /// <summary>
    /// 获取所有图集的AB包信息
    /// </summary>
    public void GetAtlasABInfo()
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
