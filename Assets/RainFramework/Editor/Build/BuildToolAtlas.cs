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
    public const string sourcePath = "Assets/AssetBundles/Art/Resources/UI";
    public const string atlasPath = "Assets/AssetBundles/Art/Atlas";

    public BuildToolAtlas(BuildToolConfig _config) : base(_config)
    {
        pageName = "图集工具";
    }

    /// <summary>
    /// 绘制图集列表
    /// </summary>
    public void DrawAtlasList()
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
                    if (GUILayout.Button("选择", BuildToolWindow.btStyle, GUILayout.Width(50)))
                    {
                        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(relativePath);
                        if (atlas != null)
                        {
                            Selection.activeObject = atlas;
                            EditorGUIUtility.PingObject(atlas);
                        }
                    }

                    // 删除按钮
                    if (GUILayout.Button("删除", BuildToolWindow.btStyle, GUILayout.Width(50)))
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
    protected override void DrawContent()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("打包图集", BuildToolWindow.btStyle, GUILayout.Height(30)))
        {
            BuildAtlas();
        }
        if (GUILayout.Button("清理图集", BuildToolWindow.btStyle, GUILayout.Height(30)))
            ClearAtlas();
        if (GUILayout.Button("检测图片尺寸", BuildToolWindow.btStyle, GUILayout.Height(30)))
        {
            CheckImageSizes();
        }
        EditorGUILayout.EndHorizontal();
        
        // 显示超大图片列表
        DrawOversizedImagesList();
        
        DrawAtlasList();
    }

    /// <summary>
    /// 打包图集
    /// </summary>
    public void BuildAtlas()
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
            Debug.LogError($"打包图集失败: {e.Message}");
            EditorUtility.DisplayDialog("打包失败", $"打包图集时发生错误:\n{e.Message}", "确定");
        }
    }

    /// <summary>
    /// 为文件夹创建图集
    /// </summary>
    private void CreateAtlasForFolder(string folderPath, string folderName, string atlasPath)
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
        string atlasAssetPath = Path.Combine(atlasPath, $"atlas_{folderName}.spriteatlas");

        AssetDatabase.CreateAsset(spriteAtlas, atlasAssetPath);

        Debug.Log($"为图集 {folderName} 创建了 {textures.Count} 个sprite，图集文件: {atlasPath}");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"图集 {folderName} 创建完成");
    }

    /// <summary>
    /// 清理图集
    /// </summary>
    public void ClearAtlas()
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
    private void AutoPackPreview()
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

        // 清除进度条
        EditorUtility.ClearProgressBar();

        Debug.Log($"Pack Preview 完成 - 成功: {successCount}, 失败: {failCount}");

        // 显示完成提示
        if (successCount > 0)
        {
            EditorUtility.DisplayDialog("Pack Preview 完成", $"成功处理 {successCount} 个图集\n" + $"失败 {failCount} 个图集", "确定");
        }
    }

    // 存储检测结果
    private List<OversizedImageInfo> oversizedImages = new List<OversizedImageInfo>();
    private bool showOversizedImages = false;
    
    // 忽略列表文件路径
    private const string ignoreListFile = "Assets/Editor/BuildToolAtlas_IgnoreList.json";
    private List<string> ignoredImagePaths = new List<string>();

    /// <summary>
    /// 超大图片信息
    /// </summary>
    [System.Serializable]
    public class OversizedImageInfo
    {
        public string fileName;
        public string relativePath;
        public int width;
        public int height;
        public Texture2D texture;

        public OversizedImageInfo(string fileName, string relativePath, int width, int height, Texture2D texture)
        {
            this.fileName = fileName;
            this.relativePath = relativePath;
            this.width = width;
            this.height = height;
            this.texture = texture;
        }
    }

    /// <summary>
    /// 检测图片尺寸
    /// </summary>
    public void CheckImageSizes()
    {
        if (!Directory.Exists(sourcePath))
        {
            Debug.LogError($"源路径不存在: {sourcePath}");
            return;
        }

        // 加载忽略列表
        LoadIgnoreList();

        EditorUtility.DisplayProgressBar("检测图片尺寸", "正在扫描图片文件...", 0f);

        // 清空之前的结果
        oversizedImages.Clear();

        // 获取所有图片文件
        string[] imageExtensions = { "*.png", "*.jpg", "*.jpeg", "*.tga", "*.psd", "*.tiff" };
        List<string> allImageFiles = new List<string>();

        foreach (string extension in imageExtensions)
        {
            string[] files = Directory.GetFiles(sourcePath, extension, SearchOption.AllDirectories);
            allImageFiles.AddRange(files);
        }

        int totalFiles = allImageFiles.Count;
        int processedFiles = 0;

        foreach (string imagePath in allImageFiles)
        {
            string relativePath = imagePath.Replace(Application.dataPath, "Assets");
            string fileName = Path.GetFileName(imagePath);

            EditorUtility.DisplayProgressBar("检测图片尺寸", $"正在检测: {fileName}", (float)processedFiles / totalFiles);

            // 检查是否在忽略列表中
            if (ignoredImagePaths.Contains(relativePath))
            {
                processedFiles++;
                continue;
            }

            try
            {
                // 加载图片
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);
                if (texture != null)
                {
                    // 检查尺寸
                    if (texture.width > 512 || texture.height > 512)
                    {
                        oversizedImages.Add(new OversizedImageInfo(fileName, relativePath, texture.width, texture.height, texture));
                        Debug.LogWarning($"图片尺寸过大: {relativePath} - {texture.width}x{texture.height}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"检测图片失败: {relativePath} - {e.Message}");
            }

            processedFiles++;
        }

        EditorUtility.ClearProgressBar();

        // 显示检测结果
        showOversizedImages = true;
        Debug.Log($"图片尺寸检测完成 - 总文件数: {totalFiles}, 超大图片: {oversizedImages.Count}");
    }

    /// <summary>
    /// 加载忽略列表
    /// </summary>
    private void LoadIgnoreList()
    {
        ignoredImagePaths.Clear();
        
        if (File.Exists(ignoreListFile))
        {
            try
            {
                string jsonContent = File.ReadAllText(ignoreListFile);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    ignoredImagePaths = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jsonContent) ?? new List<string>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载忽略列表失败: {e.Message}");
                ignoredImagePaths = new List<string>();
            }
        }
    }

    /// <summary>
    /// 保存忽略列表
    /// </summary>
    private void SaveIgnoreList()
    {
        try
        {
            // 确保目录存在
            string directory = Path.GetDirectoryName(ignoreListFile);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(ignoredImagePaths, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(ignoreListFile, jsonContent);
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogError($"保存忽略列表失败: {e.Message}");
        }
    }

    /// <summary>
    /// 忽略图片
    /// </summary>
    private void IgnoreImage(string imagePath)
    {
        if (!ignoredImagePaths.Contains(imagePath))
        {
            ignoredImagePaths.Add(imagePath);
            SaveIgnoreList();
            Debug.Log($"已忽略图片: {imagePath}");
        }
    }

    /// <summary>
    /// 绘制超大图片列表
    /// </summary>
    private void DrawOversizedImagesList()
    {
        if (!showOversizedImages)
            return;

        EditorGUILayout.Space(10);
        
        if (oversizedImages.Count > 0)
        {
            EditorGUILayout.LabelField($"超大图片列表 ({oversizedImages.Count} 个):", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            // 表头
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("图片名称", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("尺寸", EditorStyles.boldLabel, GUILayout.Width(90));
            EditorGUILayout.LabelField("文件大小", EditorStyles.boldLabel, GUILayout.Width(70));
            EditorGUILayout.LabelField("文件路径", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("操作", EditorStyles.boldLabel, GUILayout.Width(120));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // 显示每个超大图片
            foreach (var imageInfo in oversizedImages)
            {
                EditorGUILayout.BeginHorizontal();
                
                // 图片名称
                EditorGUILayout.LabelField(imageInfo.fileName, GUILayout.Width(200));
                
                // 尺寸信息（用绿色显示）
                GUI.color = Color.green;
                EditorGUILayout.LabelField($"{imageInfo.width}x{imageInfo.height}", GUILayout.Width(90));
                GUI.color = Color.white;
                
                // 文件大小
                FileInfo fileInfo = new FileInfo(imageInfo.relativePath);
                string fileSize = FormatFileSize(fileInfo.Length);
                EditorGUILayout.LabelField(fileSize, GUILayout.Width(70));
                
                // 文件路径（填充剩余空间）
                EditorGUILayout.LabelField(imageInfo.relativePath, GUILayout.ExpandWidth(true));
                
                // 选择按钮
                if (GUILayout.Button("选择", BuildToolWindow.btStyle, GUILayout.Width(40)))
                {
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(imageInfo.relativePath);
                    if (texture != null)
                    {
                        Selection.activeObject = texture;
                        EditorGUIUtility.PingObject(texture);
                    }
                }
                
                // 删除按钮
                if (GUILayout.Button("删除", BuildToolWindow.btStyle, GUILayout.Width(40)))
                {
                    // 删除文件
                    AssetDatabase.DeleteAsset(imageInfo.relativePath);
                    AssetDatabase.Refresh();
                    
                    // 从当前列表中移除
                    oversizedImages.Remove(imageInfo);
                    break; // 退出循环，因为列表已修改
                }
                
                // 忽略按钮
                if (GUILayout.Button("忽略", BuildToolWindow.btStyle, GUILayout.Width(40)))
                {
                    //if (EditorUtility.DisplayDialog("确认忽略", $"确定要忽略图片 {imageInfo.fileName} 吗？\n\n忽略后该图片将不再出现在检测列表中。", "确定", "取消"))
                    {
                        IgnoreImage(imageInfo.relativePath);
                        // 从当前列表中移除
                        oversizedImages.Remove(imageInfo);
                        break; // 退出循环，因为列表已修改
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            
            // 操作按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("管理忽略列表", BuildToolWindow.btStyle, GUILayout.Width(120)))
            {
                ShowIgnoreListManager();
            }
            if (GUILayout.Button("关闭列表", BuildToolWindow.btStyle, GUILayout.Width(100)))
            {
                showOversizedImages = false;
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.LabelField("✅ 所有图片尺寸都在 512x512 以内", EditorStyles.centeredGreyMiniLabel);
            
            // 关闭按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("关闭列表", BuildToolWindow.btStyle, GUILayout.Width(100)))
            {
                showOversizedImages = false;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 显示忽略列表管理器
    /// </summary>
    private void ShowIgnoreListManager()
    {
        if (ignoredImagePaths.Count == 0)
        {
            EditorUtility.DisplayDialog("忽略列表", "当前没有忽略的图片。", "确定");
            return;
        }

        string message = $"当前忽略的图片 ({ignoredImagePaths.Count} 个):\n\n";
        foreach (string path in ignoredImagePaths)
        {
            message += $"• {Path.GetFileName(path)}\n";
        }
        message += "\n是否要清空忽略列表？";

        if (EditorUtility.DisplayDialog("忽略列表管理", message, "清空列表", "取消"))
        {
            ignoredImagePaths.Clear();
            SaveIgnoreList();
            EditorUtility.DisplayDialog("操作完成", "忽略列表已清空。", "确定");
        }
    }

    /// <summary>
    /// 格式化文件大小
    /// </summary>
    private string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        else if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";
        else
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }
}