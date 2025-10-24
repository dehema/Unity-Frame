using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Path = System.IO.Path;
using Newtonsoft.Json;
using Rain.Core;
using System.Collections.Generic;

/// <summary>
/// AB包设置工具
/// </summary>
public class BuildToolAB : BuildToolBase
{
    /// <summary>
    /// ab资源列表 <名字,路径>
    /// 路径是 Assets/AssetBundles/Art/Atlas/atlas_2D.spriteatlas
    /// </summary>
    private Dictionary<string, string> assetsBundlePaths = new Dictionary<string, string>();

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
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("AB包输出路径:", GUILayout.Width(100));
        EditorGUILayout.LabelField(config.ABOutputPath);
        if (GUILayout.Button("打开", BuildToolWindow.btStyle, GUILayout.Width(60)))
            OpenAssetBundleDirectory();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // AB包操作按钮
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("设置AB", BuildToolWindow.btStyle, GUILayout.Height(30)))
        {
            SetAllAssetBundles();
            CreateABFileList();
        }
        if (GUILayout.Button("清除AB", BuildToolWindow.btStyle, GUILayout.Height(30)))
            ClearAllAssetBundles();
        if (GUILayout.Button("打包AB", BuildToolWindow.btStyle, GUILayout.Height(30)))
        {
            BuildAllAssetBundles();
            WriteResourceMapFile();
        }
        if (GUILayout.Button("上传AB", BuildToolWindow.btStyle, GUILayout.Height(30)))
            UploadAssetBundles();
        if (GUILayout.Button("打开上传地址", BuildToolWindow.btStyle, GUILayout.Height(30)))
            OpenOnlineAssetBundleDirectory();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // 自动打包选项
        config.AutoBuildAB = EditorGUILayout.Toggle("自动打包AB包", config.AutoBuildAB);

        // 显示当前AB包状态
        DrawABStatusInfo();
    }

    /// <summary>
    /// 绘制AB包状态信息
    /// </summary>
    private void DrawABStatusInfo()
    {
        if (!Directory.Exists(BuildToolAtlas.atlasPath))
        {
            EditorGUILayout.LabelField("Atlas目录不存在", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        string[] atlasFiles = FileTools.GetSpecifyFilesInFolder(BuildToolAtlas.atlasPath, "*.spriteatlas");
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
                        GUI.color = Color.green;
                        EditorGUILayout.LabelField("✓已设置", GUILayout.Width(60));
                        GUI.color = Color.white;
                    }
                    else
                    {
                        GUI.color = Color.red;
                        EditorGUILayout.LabelField("×未设置", GUILayout.Width(60));
                        GUI.color = Color.white;
                    }

                    // 显示文件路径
                    EditorGUILayout.LabelField(relativePath, GUILayout.ExpandWidth(true));

                    // 选择按钮
                    if (GUILayout.Button("选择", BuildToolWindow.btStyle, GUILayout.Width(50)))
                    {
                        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(relativePath);
                        if (atlas != null)
                        {
                            Selection.activeObject = atlas;
                            EditorGUIUtility.PingObject(atlas);
                        }
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
            EditorGUILayout.LabelField("暂无图集文件", EditorStyles.centeredGreyMiniLabel);
        }
    }

    /// <summary>
    /// 设置所有资源的ab
    /// </summary>
    public void SetAllAssetBundles()
    {
        assetsBundlePaths.Clear();
        SetAllAtlasAssetBundles();
    }

    /// <summary>
    /// 一键设置所有图集的AB包标签
    /// </summary>
    public void SetAllAtlasAssetBundles()
    {
        string[] atlasFiles = FileTools.GetSpecifyFilesInFolder(BuildToolAtlas.atlasPath, "*.spriteatlas");
        if (atlasFiles.Length == 0)
            return;
        FileTools.PathsFormatToUnityPath(atlasFiles);
        EditorUtility.DisplayProgressBar("设置AB包", "正在处理图集...", 0f);
        for (int i = 0; i < atlasFiles.Length; i++)
        {
            string atlasFile = atlasFiles[i];
            string fileName = Path.GetFileNameWithoutExtension(atlasFile);

            EditorUtility.DisplayProgressBar("设置AB包", $"正在处理: {fileName}", (float)i / atlasFiles.Length);

            string relativePath = atlasFile.Replace(Application.dataPath, "Assets");
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(relativePath);

            if (atlas == null || !CheckAssetsBundleRepeat(relativePath))
            {
                Debug.LogError($"无法加载图集:{relativePath}");
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 检查ab资源是否重复
    /// </summary>
    /// <param name="_assetPath"></param>
    private bool CheckAssetsBundleRepeat(string _assetPath)
    {
        // 生成AB包名称（基于图集路径）
        string abName = GenerateAssetBundleName(_assetPath);
        if (assetsBundlePaths.ContainsKey(abName))
        {
            Debug.LogError($"发现重复资源:{_assetPath}");
            return false;
        }
        assetsBundlePaths[abName] = _assetPath;

        // 设置AssetBundle标签
        AssetImporter importer = AssetImporter.GetAtPath(_assetPath);
        if (importer != null)
        {
            importer.assetBundleName = abName;
            importer.assetBundleVariant = "";
            Debug.Log($"设置AB包标签:{abName}");
            return true;
        }
        else
        {
            Debug.LogError($"无法获取AssetImporter:{abName}");
        }
        return false;
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
                            importer.assetBundleVariant = "";
                            importer.assetBundleName = "";
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
    /// 一键打包所有AssetBundle到AB包输出目录
    /// </summary>
    public void BuildAllAssetBundles()
    {
        try
        {
            if (string.IsNullOrEmpty(config.ABOutputPath))
            {
                EditorUtility.DisplayDialog("错误", "请先设置AB包输出路径", "确定");
                return;
            }

            // 确保输出目录存在
            if (!Directory.Exists(config.ABOutputPath))
            {
                Directory.CreateDirectory(config.ABOutputPath);
            }

            //删除目录下的所有文件和子文件夹
            ClearDirectory(config.ABOutputPath);


            EditorUtility.DisplayProgressBar("打包AB包", "正在构建AssetBundle...", 0f);

            // 构建AssetBundle
            BuildPipeline.BuildAssetBundles(config.ABOutputPath,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Debug.Log($"AB包打包完成，输出目录: {config.ABOutputPath}");
            EditorUtility.RevealInFinder(config.ABOutputPath + "/");
            Debug.Log($"打包完成，AB包已打包 到:\n{config.ABOutputPath}");
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
        AssetBundleMap assetBundleMap = new AssetBundleMap();
        assetBundleMap.Version = config.Version;
        // 获取AB包输出目录下的所有文件
        string[] allFiles = FileTools.GetAllFilesInFolder(config.ABOutputPath);
        FileTools.PathsFormatToUnityPath(allFiles);
        foreach (string filePath in allFiles)
        {
            if (filePath.EndsWith(".manifest"))
                continue;
            string relativePath = Path.GetRelativePath(config.ABOutputPath, filePath);
            string md5 = CalculateFileMD5(filePath);

            AssetMapping assetMapping = new AssetMapping();
            assetMapping.AbName = relativePath;
            assetMapping.AssetPath = assetsBundlePaths.ContainsKey(relativePath) ? assetsBundlePaths[relativePath] : relativePath;
            assetMapping.Version = config.Version;
            assetMapping.MD5 = md5;
            assetMapping.Size = new FileInfo(filePath).Length.ToString();
            assetBundleMap.ABMap[relativePath] = assetMapping;
        }
        // 保存文件列表到JSON
        string jsonPath = Path.Combine(Application.persistentDataPath, $"{nameof(AssetBundleMap)}.json");
        string json = JsonConvert.SerializeObject(assetBundleMap, Formatting.Indented);
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
                string[] files = FileTools.GetAllFilesInFolder(config.ABOutputPath);
                string[] directories = FileTools.GetAllDirsInFolder(_direPath);

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
        // 创建目标文件夹
        EditorUtility.DisplayProgressBar("上传AB包", "正在创建目标文件夹...", 0f);
        if (!Directory.Exists(config.ABRemoteAddress))
            Directory.CreateDirectory(config.ABRemoteAddress);

        EditorUtility.DisplayProgressBar("上传AB包", "正在复制文件...", 0.2f);

        // 复制所有文件和文件夹
        int copiedFiles = 0;
        int currentFile = 0;

        // 复制所有文件
        string[] allFiles = FileTools.GetAllFilesInFolder(config.ABOutputPath);
        int totalFileNums = allFiles.Length;
        foreach (string sourceFile in allFiles)
        {
            currentFile++;
            EditorUtility.DisplayProgressBar("上传AB包",
                $"正在复制: {Path.GetFileName(sourceFile)}",
                0.2f + (0.8f * currentFile / totalFileNums));

            // 计算相对路径
            string relativePath = Path.GetRelativePath(config.ABOutputPath, sourceFile);
            string targetFile = Path.Combine(config.ABRemoteAddress, relativePath);

            // 确保目标目录存在
            string targetDir = Path.GetDirectoryName(targetFile);
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // 复制文件
            File.Copy(sourceFile, targetFile, true);
            copiedFiles++;
            //Debug.Log($"已复制文件: {relativePath}");
        }

        EditorUtility.ClearProgressBar();

        Debug.Log($"AB包上传完成 - 共复制 {copiedFiles} 个文件到: {config.ABRemoteAddress}");
        //EditorUtility.DisplayDialog("上传完成", $"AB包已上传到:\n{targetFolderPath}\n\n共复制 {copiedFiles} 个文件", "确定");

        //复制AssetBundleMap.json
        string jsonPath = Path.Combine(Application.persistentDataPath, $"{nameof(AssetBundleMap)}.json");
        if (File.Exists(jsonPath))
        {
            File.Copy(jsonPath, Path.Combine(config.GameRemoteAddress, $"{nameof(AssetBundleMap)}.json"));
        }

        // 打开目标文件夹
        EditorUtility.RevealInFinder(config.ABRemoteAddress + "/");
    }

    /// <summary>
    /// 打开AB包目录
    /// </summary>
    public void OpenAssetBundleDirectory()
    {
        if (string.IsNullOrEmpty(config.ABOutputPath))
        {
            EditorUtility.DisplayDialog("错误", "请先设置AB包输出路径", "确定");
            return;
        }

        if (!Directory.Exists(config.ABOutputPath))
        {
            EditorUtility.DisplayDialog("提示", "AB包输出目录不存在", "确定");
            return;
        }

        EditorUtility.RevealInFinder(config.ABOutputPath + "/");
    }

    /// <summary>
    /// 检查是否启用自动打包AB包
    /// </summary>
    public bool IsAutoBuildABEnabled()
    {
        return config.AutoBuildAB;
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

        string[] atlasFiles = FileTools.GetSpecifyFilesInFolder(BuildToolAtlas.atlasPath, "*.spriteatlas");

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
        EditorUtility.RevealInFinder(config.ABRemoteAddress + "/");
    }

    private static Dictionary<string, string[]> resourceMapping;
    /// <summary>
    /// 写入ResourceMap文件 
    /// RainFramework/AssetMap/Resources下面生成一个资源映射文件
    /// </summary>
    public void WriteResourceMapFile()
    {
        string resourceMapPath = Application.dataPath + "/RainFramework/Resources/" + nameof(ResMap) + ".json";
        FileTools.SafeDeleteFile(resourceMapPath);
        FileTools.SafeDeleteFile(resourceMapPath + ".meta");
        FileTools.CheckFileAndCreateDirWhenNeeded(resourceMapPath);
        AssetDatabase.Refresh();


        FileTools.CheckFileAndCreateDirWhenNeeded(resourceMapPath);
        FileTools.SafeWriteAllText(resourceMapPath, JsonConvert.SerializeObject(resourceMapping));
        AssetDatabase.Refresh();

        RLog.LogAsset($"写入Resources资产数据，生成：{nameof(ResMap)}文件");
    }
}