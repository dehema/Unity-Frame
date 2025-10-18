using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using Path = System.IO.Path;
using Newtonsoft.Json;

/// <summary>
/// AB包设置工具
/// </summary>
public class BuildToolAB : BuildToolBase
{
    public BuildToolAB(BuildToolConfig _config) : base(_config)
    {
        pageName = "AB包设置";
    }

    /// <summary>
    /// 绘制AB包设置界面
    /// </summary>
    protected override void DrawContent()
    {
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

        if (GUILayout.Button("设置AB", GUILayout.Height(30)))
            SetAllAssetBundles();
        if (GUILayout.Button("清除AB", GUILayout.Height(30)))
            ClearAllAssetBundles();
        if (GUILayout.Button("打包AB", GUILayout.Height(30)))
        {
            BuildAllAssetBundles();
            CreateABFileList();
        }
        if (GUILayout.Button("上传AB", GUILayout.Height(30)))
            UploadAssetBundles();
        if (GUILayout.Button("打开打包目录", GUILayout.Height(30)))
            OpenAssetBundleDirectory();
        if (GUILayout.Button("打开上传地址", GUILayout.Height(30)))
            OpenOnlineAssetBundleDirectory();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // 自动打包选项
        config.autoBuildAB = EditorGUILayout.Toggle("自动打包AB包", config.autoBuildAB);

        // 显示当前AB包状态
        DrawABStatusInfo();

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
            string[] atlasFiles = Directory.GetFiles(BuildToolAtlas.atlasPath, "*.spriteatlas", SearchOption.AllDirectories);
            if (atlasFiles.Length == 0)
                return;
            EditorUtility.DisplayProgressBar("设置AB包", "正在处理图集...", 0f);
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
                            Debug.Log($"图集 {fileName} 设置AB包标签: {abName}");
                        }
                        else
                        {
                            Debug.LogError($"无法获取图集 {fileName} 的AssetImporter");
                        }
                    }
                    else
                    {
                        Debug.LogError($"无法加载图集: {relativePath}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"设置图集 {fileName} AB包标签失败: {e.Message}");
                }
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
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
        string abName = fileName;

        return abName;
    }

    /// <summary>
    /// 绘制AB包状态信息
    /// </summary>
    private void DrawABStatusInfo()
    {
        if (!Directory.Exists(BuildToolAtlas.atlasPath))
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Atlas目录不存在", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        string[] atlasFiles = Directory.GetFiles(BuildToolAtlas.atlasPath, "*.spriteatlas", SearchOption.AllDirectories);

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
            ClearDirectory(config.abOutputPath);


            EditorUtility.DisplayProgressBar("打包AB包", "正在构建AssetBundle...", 0f);

            // 构建AssetBundle
            BuildPipeline.BuildAssetBundles(config.abOutputPath,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Debug.Log($"AB包打包完成，输出目录: {config.abOutputPath}");
            EditorUtility.RevealInFinder(config.abOutputPath + "/");
            Debug.Log($"打包完成，AB包已打包到:\n{config.abOutputPath}");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包失败", $"错误: {e.Message}", "确定");
        }
    }

    /// <summary>
    /// 创建ab文件列表
    /// </summary>
    void CreateABFileList()
    {
        BuildToolABFileList fileList = new BuildToolABFileList();
        // 获取AB包输出目录下的所有文件
        string[] allFiles = Directory.GetFiles(config.abOutputPath, "*", SearchOption.AllDirectories);
        foreach (string filePath in allFiles)
        {
            try
            {
                string relativePath = Path.GetRelativePath(config.abOutputPath, filePath);
                if (relativePath.EndsWith(".manifest"))
                    continue;
                string md5 = CalculateFileMD5(filePath);
                fileList.fileNames[relativePath] = md5;
                //Debug.Log($"添加文件到列表: {relativePath} | MD5: {md5}");
            }
            catch (Exception e)
            {
                Debug.LogError($"处理文件失败: {filePath}, 错误: {e.Message}");
            }
        }
        // 保存文件列表到JSON
        string jsonPath = Path.Combine(config.abOutputPath, "ABFileList.json");
        string json = JsonConvert.SerializeObject(fileList, Formatting.Indented);
        File.WriteAllText(jsonPath, json);
    }

    /// <summary>
    /// 计算文件MD5
    /// </summary>
    private string CalculateFileMD5(string filePath)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = md5.ComputeHash(stream);
                return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }

    /// <summary>
    /// 清除文件夹下的所有文件和子文件夹 
    /// </summary>
    /// <param name="_direPath"></param>
    private void ClearDirectory(string _direPath)
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

        try
        {
            // 生成当前时间的文件夹名（精确到秒）
            string timeFolderName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string targetFolderPath = Path.Combine(config.onlineABUrl, timeFolderName);

            EditorUtility.DisplayProgressBar("上传AB包", "正在创建目标文件夹...", 0f);

            // 创建目标文件夹
            if (!Directory.Exists(targetFolderPath))
            {
                Directory.CreateDirectory(targetFolderPath);
                Debug.Log($"已创建目标文件夹: {targetFolderPath}");
            }

            EditorUtility.DisplayProgressBar("上传AB包", "正在复制文件...", 0.2f);

            // 复制所有文件和文件夹
            int copiedFiles = 0;
            int totalFiles = Directory.GetFiles(config.abOutputPath, "*", SearchOption.AllDirectories).Length;
            int currentFile = 0;

            // 复制所有文件
            string[] allFiles = Directory.GetFiles(config.abOutputPath, "*", SearchOption.AllDirectories);
            foreach (string sourceFile in allFiles)
            {
                try
                {
                    currentFile++;
                    EditorUtility.DisplayProgressBar("上传AB包",
                        $"正在复制: {Path.GetFileName(sourceFile)}",
                        0.2f + (0.8f * currentFile / totalFiles));

                    // 计算相对路径
                    string relativePath = Path.GetRelativePath(config.abOutputPath, sourceFile);
                    string targetFile = Path.Combine(targetFolderPath, relativePath);

                    // 确保目标目录存在
                    string targetDir = Path.GetDirectoryName(targetFile);
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }

                    // 复制文件
                    File.Copy(sourceFile, targetFile, true);
                    copiedFiles++;

                    Debug.Log($"已复制文件: {relativePath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"复制文件失败: {sourceFile}, 错误: {e.Message}");
                }
            }

            EditorUtility.ClearProgressBar();

            Debug.Log($"AB包上传完成 - 共复制 {copiedFiles} 个文件到: {targetFolderPath}");
            //EditorUtility.DisplayDialog("上传完成", $"AB包已上传到:\n{targetFolderPath}\n\n共复制 {copiedFiles} 个文件", "确定");

            // 打开目标文件夹
            try
            {
                EditorUtility.RevealInFinder(targetFolderPath + "/");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"无法打开目标文件夹: {e.Message}");
            }
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("上传失败", $"错误: {e.Message}", "确定");
        }
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

        EditorUtility.RevealInFinder(config.abOutputPath + "/");
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
        if (!Directory.Exists(BuildToolAtlas.atlasPath))
        {
            Debug.LogWarning("Atlas目录不存在");
            return;
        }

        string[] atlasFiles = Directory.GetFiles(BuildToolAtlas.atlasPath, "*.spriteatlas", SearchOption.AllDirectories);

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

    /// <summary>
    /// 打开线上AB包目录
    /// </summary>
    public void OpenOnlineAssetBundleDirectory()
    {
        if (string.IsNullOrEmpty(config.onlineABUrl))
        {
            EditorUtility.DisplayDialog("错误", "请先设置线上AB包地址", "确定");
            return;
        }

        if (!Directory.Exists(config.onlineABUrl))
        {
            EditorUtility.DisplayDialog("提示", "线上AB包目录不存在", "确定");
            return;
        }

        try
        {
            EditorUtility.RevealInFinder(config.onlineABUrl + "/");
        }
        catch (Exception e)
        {
            Debug.LogError($"打开线上AB包目录失败: {e.Message}");
            EditorUtility.DisplayDialog("错误", $"无法打开线上AB包目录: {e.Message}", "确定");
        }
    }
}

/// <summary>
/// AB包文件列表
/// </summary>
public class BuildToolABFileList
{
    /// <summary>
    /// 版本
    /// </summary>
    public string version = Application.version;
    /// <summary>
    /// 文件名和MD5
    /// </summary>
    public Dictionary<string, string> fileNames = new Dictionary<string, string>();
    /// <summary>
    /// 生成日期
    /// </summary>
    public string createTime = DateTime.Now.ToString("G");
}