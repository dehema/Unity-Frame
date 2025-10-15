using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 打包工具窗口
/// </summary>
public class BuildToolWindow : EditorWindow
{
    private string exportPath = "";
    private BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
    private bool developmentBuild = false;
    private bool copyToStreamingAssets = false;
    private bool buildAssetBundles = false;
    private bool clearBuildCache = false;

    private Vector2 scrollPosition;
    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;

    [MenuItem("开发工具/打包工具 _F5")] // F5快捷键
    public static void ShowWindow()
    {
        BuildToolWindow window = GetWindow<BuildToolWindow>("打包工具");
        window.minSize = new Vector2(400, 600);
        window.Show();
    }

    private void OnEnable()
    {
        // 初始化导出路径 - 默认桌面
        if (string.IsNullOrEmpty(exportPath))
        {
            exportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "RainBuilds");
        }

        // 初始化目标平台 - 默认当前平台
        buildTarget = EditorUserBuildSettings.activeBuildTarget;

        // 初始化样式
        InitializeStyles();
    }

    private void InitializeStyles()
    {
        titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 20,
            alignment = TextAnchor.MiddleCenter,
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
        };
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 标题
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("RainFramework 打包工具", titleStyle);
        EditorGUILayout.Space(20);

        // 导出目录设置
        DrawExportPathSection();

        EditorGUILayout.Space(20);

        // 资源准备
        DrawResourcePreparationSection();

        EditorGUILayout.Space(20);

        // 打包选项
        DrawBuildOptionsSection();

        EditorGUILayout.Space(20);

        // 一键打包按钮
        DrawBuildButton();

        EditorGUILayout.Space(20);

        // 其他工具按钮
        DrawUtilityButtons();

        EditorGUILayout.EndScrollView();
    }

    private void DrawExportPathSection()
    {
        EditorGUILayout.LabelField("【导出设置】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("安装包导出目录:", GUILayout.Width(120));
        exportPath = EditorGUILayout.TextField(exportPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("选择导出目录", exportPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                exportPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawResourcePreparationSection()
    {
        EditorGUILayout.LabelField("【资源准备】", EditorStyles.boldLabel);
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

    private void DrawAtlasList()
    {
        string atlasPath = "Assets/AssetBundles/Art/Atlas";
        
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
                    EditorGUILayout.LabelField($"• {fileName}", GUILayout.Width(200));
                    
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

    private string FormatFileSize(long bytes)
    {
        if (bytes < 1024)
            return $"{bytes} B";
        else if (bytes < 1024 * 1024)
            return $"{bytes / 1024.0:F1} KB";
        else
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }

    private void DrawBuildOptionsSection()
    {
        EditorGUILayout.LabelField("【打包选项】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        // 目标平台
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("目标平台:", buildTarget);

        // 开发版本
        developmentBuild = EditorGUILayout.Toggle("开发版本:", developmentBuild);

        EditorGUILayout.EndVertical();
    }

    private void DrawBuildButton()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("打包", buttonStyle, GUILayout.Height(50), GUILayout.Width(200)))
        {
            StartBuild();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawUtilityButtons()
    {
        EditorGUILayout.LabelField("【工具】", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("打开导出目录"))
        {
            OpenExportDirectory();
        }

        if (GUILayout.Button("清理导出目录"))
        {
            ClearExportDirectory();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("构建AssetBundle"))
        {
            BuildAssetBundles();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void StartBuild()
    {
        try
        {
            EditorUtility.DisplayProgressBar("打包中", "正在准备打包...", 0f);

            // 创建导出目录
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            // 构建AssetBundle
            if (buildAssetBundles)
            {
                EditorUtility.DisplayProgressBar("打包中", "构建AssetBundle...", 0.2f);
                BuildAssetBundles();
            }

            // 复制到StreamingAssets
            if (copyToStreamingAssets)
            {
                EditorUtility.DisplayProgressBar("打包中", "复制到StreamingAssets...", 0.3f);
                CopyToStreamingAssets();
            }

            // 开始打包
            EditorUtility.DisplayProgressBar("打包中", "正在打包...", 0.4f);
            BuildPlayer();

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包完成", $"打包完成！\n导出目录: {exportPath}", "确定");
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包失败", $"打包失败: {e.Message}", "确定");
        }
    }

    private void BuildPlayer()
    {
        string[] scenes = GetBuildScenes();
        string buildPath = GetBuildPath();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = buildTarget,
            options = developmentBuild ? BuildOptions.Development : BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    private string[] GetBuildScenes()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

    private string GetBuildPath()
    {
        string fileName = GetBuildFileName();
        return Path.Combine(exportPath, fileName);
    }

    private string GetBuildFileName()
    {
        string platform = buildTarget.ToString();
        string extension = GetBuildExtension();
        return $"{Application.productName}_{platform}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
    }

    private string GetBuildExtension()
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            case BuildTarget.StandaloneOSX:
                return ".app";
            case BuildTarget.StandaloneLinux64:
                return "";
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.iOS:
                return "";
            default:
                return "";
        }
    }

    private void BuildAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, buildTarget);
    }

    private void CopyToStreamingAssets()
    {
        // 这里可以添加复制逻辑
        Debug.Log("复制到StreamingAssets功能待实现");
    }

    private void OpenExportDirectory()
    {
        if (Directory.Exists(exportPath))
        {
            EditorUtility.RevealInFinder(exportPath);
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "导出目录不存在", "确定");
        }
    }

    private void ClearExportDirectory()
    {
        if (Directory.Exists(exportPath))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要清理导出目录吗？", "确定", "取消"))
            {
                Directory.Delete(exportPath, true);
                Directory.CreateDirectory(exportPath);
                Debug.Log("导出目录已清理");
            }
        }
    }

    private void BuildAtlas()
    {
        try
        {
            EditorUtility.DisplayProgressBar("打包图集", "正在扫描UI文件夹...", 0f);

            string sourcePath = "Assets/AssetBundles/Art/Resources/UI";
            string atlasPath = "Assets/AssetBundles/Art/Atlas";

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
        }
        catch (Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("打包图集失败", $"错误: {e.Message}", "确定");
        }
    }

    private void CreateAtlasForFolder(string folderPath, string folderName, string atlasPath)
    {
        Debug.Log($"开始处理文件夹: {folderName}, 路径: {folderPath}");
        
        // 获取文件夹中的所有图片文件
        string[] imageFiles = Directory.GetFiles(folderPath, "*.png", SearchOption.AllDirectories);
        //Debug.Log($"找到 {imageFiles.Length} 个PNG文件");

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
                //Debug.Log($"添加纹理: {relativePath}");
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

        //Debug.Log($"成功收集到 {textures.Count} 个有效纹理");

        try
        {
            // 使用Unity的SpriteAtlas创建图集
            SpriteAtlas spriteAtlas = new SpriteAtlas();
            Debug.Log($"创建SpriteAtlas对象成功");

            // 设置图集参数
            SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            spriteAtlas.SetTextureSettings(textureSettings);
            Debug.Log($"设置纹理参数成功");

            SpriteAtlasPackingSettings packingSettings = new SpriteAtlasPackingSettings
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2
            };
            spriteAtlas.SetPackingSettings(packingSettings);
            Debug.Log($"设置打包参数成功");

            // 添加所有纹理到图集
            var objects = new UnityEngine.Object[textures.Count];
            for (int i = 0; i < textures.Count; i++)
            {
                objects[i] = textures[i];
            }
            spriteAtlas.Add(objects);
            Debug.Log($"添加 {objects.Length} 个对象到图集");

            // 创建图集资源文件
            string atlasAssetPath = Path.Combine(atlasPath, $"{folderName}.spriteatlas");
            Debug.Log($"创建图集文件: {atlasAssetPath}");
            
            AssetDatabase.CreateAsset(spriteAtlas, atlasAssetPath);
            Debug.Log($"图集资源创建成功");
            
            // 创建SpriteData
            CreateSpriteData(atlasAssetPath, textures, folderName);
            
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

    private void CreateSpriteData(string atlasPath, List<Texture2D> textures, string folderName)
    {
        // 创建SpriteData文件
        string spriteDataPath = atlasPath.Replace(".spriteatlas", "_SpriteData.asset");

        // 这里可以创建自定义的SpriteData ScriptableObject
        // 包含图集中每个sprite的位置和大小信息
        Debug.Log($"为图集 {folderName} 创建了 {textures.Count} 个sprite，图集文件: {atlasPath}");
    }

    private void ClearAtlas()
    {
        string atlasPath = "Assets/AssetBundles/Art/Atlas";

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
                EditorUtility.DisplayDialog("清理完成", "图集文件已清理", "确定");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("提示", "Atlas目录不存在", "确定");
        }
    }
}
