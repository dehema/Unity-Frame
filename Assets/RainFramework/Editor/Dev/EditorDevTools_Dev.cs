using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotLiquid;
using Newtonsoft.Json;
using Rain.Core;
using Rain.UI;
using Rain.UI.Editor;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Debug = UnityEngine.Debug;
using Path = System.IO.Path;

public class EditorDevTools_Dev : EditorDevTools_Base
{
    public EditorDevTools_Dev(EditorWindow mainWindow, List<EditorDevTools_Base> subModules = null)
        : base(mainWindow, subModules)
    {
        this.pageName = "开发";
    }

    float gameTimeSpeed = 1;

    public override void DrawContent()
    {
        OnGUIUIPlaying();
        OnGUIEditor();
        OnGUIUI();
        OnGUIUIConfig();
        OnGUIDebug();
    }

    private void OnGUIEditor()
    {
        EditorGUILayout.LabelField("------------------- 编辑 -------------------", style.lbTitle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开编辑器脚本", style.bt))
        {
            OpenEditorScript();
        }
        if (GUILayout.Button("清除PlayerPrefs", style.bt))
        {
            PlayerPrefs.DeleteAll(); ;
        }
        GUILayout.EndHorizontal();
    }

    string createViewName = "ViewNameYouWantCreate";
    private void OnGUIUI()
    {
        //UI
        EditorGUILayout.LabelField("------------------- UI -------------------", style.lbTitle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        createViewName = GUILayout.TextField(createViewName, 30, GUILayout.Width(170));
        if (GUILayout.Button("创建View", style.bt))
        {
            ViewConfig viewConfig = GetViewConfig(createViewName);
            EditorViewCreater.CreateView(viewConfig);
        }
        if (GUILayout.Button("导出UI", style.bt))
        {
            EditorExportUI.ExportViewUI();
        }
        GUILayout.EndHorizontal();
    }
    private void OnGUIUIConfig()
    {
        //UI配置
        EditorGUILayout.LabelField("------------------- 配置 -------------------", style.lbTitle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开View配置", style.bt))
        {
            OpenUIViewConfig();
        }
        if (GUILayout.Button("导出View配置", style.bt))
        {
            ExportViewConfig();
        }
        if (GUILayout.Button("打开Scene配置", style.bt))
        {
            OpenSceneConfig();
        }
        if (GUILayout.Button("导出Scene配置", style.bt))
        {
            ExportSceneConfig();
        }
        //if (GUILayout.Button("打开Excel配置目录", style.bt))
        //{
        //    string path = Directory.GetParent(Application.dataPath).FullName + @"\Product\StaticData";
        //    System.Diagnostics.Process.Start("Explorer.exe", path);
        //}
        //if (GUILayout.Button("导出Excel", style.bt))
        //{
        //    ExportExcel2Json();
        //}
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开Excel配置目录", style.bt))
        {
            string path = Directory.GetParent(Application.dataPath).FullName + @"\Config\Excel";
            EditorUtility.RevealInFinder(path + "/");
        }
        if (GUILayout.Button("导出Excel", style.bt))
        {
            ExportExcel2Json_Luban();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("中文表", style.bt))
        {
            string langName = Application.systemLanguage.ToString();
            if (Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
            {
                langName = SystemLanguage.Chinese.ToString();
            }
            string path = Application.streamingAssetsPath + $"/Lang/{langName}.json";
            Debug.Log(path);
            EditorUtility.OpenWithDefaultApp(path);
        }
        if (GUILayout.Button("多语言汉译英", style.bt))
        {
            EditorTranslate.OnClickTranslateLanguage();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("套用本地配置", style.bt))
            {
                ConfigMgr.Ins.LoadAllConfig();
            }
            if (GUILayout.Button("套用远端配置", style.bt))
            {
                ConfigMgr.Ins.LoadAllConfig(false);
            }
        }
        GUILayout.EndHorizontal();
    }

    private void OnGUIUIPlaying()
    {
        //运行
        EditorGUILayout.LabelField("------------------- 运行 -------------------", style.lbTitle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        if (EditorApplication.isPlaying)
        {
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("停止游戏 Ctrl+P", style.btLarge, GUILayout.Height(40)))
            {
                EditorCoroutineUtility.StartCoroutine(StopGame(), this);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("重启游戏 Ctrl+R", style.btLarge, GUILayout.Height(40)))
            {
                ResetGame();
            }
            GUILayout.FlexibleSpace();
        }
        else
        {
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("启动游戏 Ctrl+P | Ctrl+R", style.btLarge, GUILayout.Height(40)))
            {
                StartGame();
            }
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.white;
            if (GUILayout.Button("生成资源表", style.btLarge, GUILayout.Height(40)))
            {
                GenerateResMap();
            }
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
        if (EditorApplication.isPlaying)
        {
            GUILayout.BeginHorizontal();
            //Time.timeScale
            GUILayout.Label("游戏全局速度 x1 - x10", style.lb, GUILayout.Width(130));
            gameTimeSpeed = GUILayout.HorizontalSlider(gameTimeSpeed, 1, 10, GUILayout.Width(mainWindow.position.width - 420));
            GUILayout.Label("x0.2 - x1", style.lb, GUILayout.Width(50));
            gameTimeSpeed = GUILayout.HorizontalSlider(gameTimeSpeed, 0.2f, 1f, GUILayout.Width(100));
            if (gameTimeSpeed >= 1)
            {
                if ((gameTimeSpeed * 10) % 10 > 5)
                {
                    gameTimeSpeed = Mathf.CeilToInt(gameTimeSpeed);
                }
                else
                {
                    gameTimeSpeed = Mathf.FloorToInt(gameTimeSpeed);
                }
            }
            Time.timeScale = gameTimeSpeed;
            EditorGUILayout.LabelField(gameTimeSpeed.ToString("f2"), style.lb, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GameSettingStatic.ResLog = GUILayout.Toggle(GameSettingStatic.ResLog, "资源log");
            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 生成资源表
    /// </summary>
    private void GenerateResMap()
    {
        string resMapPath = Application.dataPath + "/RainFramework/Resources/";

        // 确保目录存在
        FileTools.CheckDirAndCreateWhenNeeded(resMapPath);

        // 扫描所有能被 Resources.Load 读取的文件
        Dictionary<string, ResMapping> resourceMappings = new Dictionary<string, ResMapping>();

        // 获取所有 Assets 路径
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

        foreach (string assetPath in allAssetPaths)
        {
            // 跳过文件夹和 .meta 文件
            if (!assetPath.StartsWith("Assets") || Directory.Exists(assetPath) || assetPath.EndsWith(".meta"))
                continue;

            // 检查文件是否在 Resources 文件夹下
            if (IsInResourcesFolder(assetPath))
            {
                // 获取相对于 Resources 的路径（用于 Resources.Load）
                string relativePath = GetRelativePathToResources(assetPath);

                // 获取文件名（不含扩展名）作为资源名称
                string fileName = Path.GetFileNameWithoutExtension(assetPath);

                // 获取AB名称
                string abName = EditorUtil.GetResAssetBundleName(assetPath);

                // 创建资源映射
                ResMapping mapping = new ResMapping
                {
                    assetName = fileName,
                    logicPath = relativePath,
                    AbName = abName
                };

                // 如果资源名称已存在，记录警告
                if (resourceMappings.ContainsKey(fileName))
                {
                    RLog.LogAsset($"警告: 资源名称 '{fileName}' 重复，路径: {assetPath}，已存在路径: {resourceMappings[fileName].logicPath}");
                }
                else
                {
                    resourceMappings[fileName] = mapping;
                    //RLog.LogAsset($"添加资源: {fileName} -> {relativePath}");
                }
            }
        }

        // 生成 JSON 文件
        string jsonContent = JsonConvert.SerializeObject(resourceMappings);
        string jsonFilePath = resMapPath + $"{nameof(ResMap)}.json";

        // 删除旧文件
        FileTools.SafeDeleteFile(jsonFilePath);
        FileTools.SafeDeleteFile(jsonFilePath + ".meta");

        // 写入新文件
        if (FileTools.SafeWriteAllText(jsonFilePath, jsonContent))
        {
            RLog.LogAsset($"成功生成资源映射文件: {jsonFilePath}");
            RLog.LogAsset($"共扫描到 {resourceMappings.Count} 个资源文件");
        }
        else
        {
            RLog.LogAsset($"生成资源映射文件失败: {jsonFilePath}");
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 检查文件是否在 Resources 文件夹下
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <returns>是否在 Resources 文件夹下</returns>
    private bool IsInResourcesFolder(string assetPath)
    {
        // 检查路径中是否包含 /Resources/ 或路径以 /Resources 结尾
        return assetPath.Contains("/Resources/") || assetPath.EndsWith("/Resources");
    }
    /// <summary>
    /// 获取相对于Resources文件夹的路径
    /// </summary>
    /// <param name="assetPath">资源路径</param>
    /// <returns>相对于Resources的路径</returns>
    private string GetRelativePathToResources(string assetPath)
    {
        // 找到 Resources 在路径中的位置
        int resourcesIndex = assetPath.IndexOf("/Resources/");
        if (resourcesIndex == -1)
        {
            // 如果路径以 Resources 结尾，去掉最后的 Resources
            if (assetPath.EndsWith("/Resources"))
            {
                return "";
            }
            return assetPath;
        }

        // 获取 Resources 之后的路径部分
        string relativePath = assetPath.Substring(resourcesIndex + "/Resources/".Length);

        // 去掉文件扩展名
        return Path.ChangeExtension(relativePath, null);
    }

    /// <summary>
    /// 调试
    /// </summary>
    private void OnGUIDebug()
    {
        GUILayout.BeginHorizontal();
        GUILayout.EndHorizontal();
    }

    [MenuItem("开发工具/重启 %R")]
    static void ResetGame()
    {
        if (Application.isPlaying)
        {
            _ResetGame();
        }
        else
        {
            StartGame();
        }
    }
    /// <summary>
    /// 当有此文件时说明代码编译后需要重启
    /// </summary>
    readonly static string ReStartGameFile = "temp_ReStartGameFile";
    static void _ResetGame()
    {
        string tempFilePath = Directory.GetParent(Application.dataPath).FullName + ReStartGameFile;
        File.WriteAllText(tempFilePath, string.Empty);
        EditorCoroutineUtility.StartCoroutineOwnerless(ReStart());
    }

    /// <summary>
    /// 此标记可以让脚本在编译后在调用一次
    /// </summary>
    [DidReloadScripts]
    public static void OnCompileScripts()
    {
        if (!Application.isEditor || EditorApplication.isPlaying)
            return;
        string tempFilePath = Directory.GetParent(Application.dataPath).FullName + ReStartGameFile;
        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
            StartGame();
        }
    }

    public IEnumerator StopGame()
    {
        EditorApplication.isPlaying = false;
        EditorUtility.DisplayProgressBar("进度", "等待运行停止", 0.1f);

        yield return new EditorWaitForSeconds(0.1f);

        EditorUtility.ClearProgressBar();
        yield return null;
    }

    static IEnumerator PlayGame()
    {
        if (Application.isPlaying)
            yield break;
        //var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity", OpenSceneMode.Single);
        List<SceneID> sceneList = Enum.GetValues(typeof(SceneID)).Cast<SceneID>().ToList();
        string firstSceneName = sceneList.First().ToString();
        var scene = EditorSceneManager.OpenScene($"Assets/AssetBundles/Scenes/{firstSceneName}.unity", OpenSceneMode.Single);
        EditorSceneManager.SetActiveScene(scene);
        EditorUtility.DisplayProgressBar("打开", $"{firstSceneName}场景", 0.1f);
        yield return new EditorWaitForSeconds(0.1f);
        EditorUtility.ClearProgressBar();
        EditorApplication.isPlaying = true;
    }

    static IEnumerator ReStart()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(PlayGame());
            yield break;
        }
        while (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            yield return new EditorWaitForSeconds(0.1f);
            EditorCoroutineUtility.StartCoroutineOwnerless(PlayGame());
        }
    }

    public static void StartGame()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(PlayGame());
        }
    }

    /// <summary>
    /// 打开此脚本
    /// </summary>
    private static void OpenEditorScript()
    {
        EditorUtility.OpenWithDefaultApp(EditorScriptPath);
    }

    /// <summary>
    /// Excel导成Json
    /// </summary>
    public static void ExportExcel2Json()
    {
        try
        {
            string batPath = Directory.GetParent(Application.dataPath).FullName;
            batPath += "/Product/excel2json/excel2json.bat";
            if (!File.Exists(batPath))
            {
                EditorUtility.DisplayDialog("错误", "没有找到文件" + batPath, "确定");
                return;
            }
            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            FileInfo file = new FileInfo(batPath);
            pro.StartInfo.WorkingDirectory = file.Directory.FullName;
            pro.StartInfo.FileName = batPath;
            pro.StartInfo.CreateNoWindow = false;
            pro.Start();
            pro.WaitForExit();
            Debug.Log("导出完成->Resources/Json/");
        }
        catch (Exception ex)
        {
            Debug.LogError("执行失败 错误原因:" + ex.Message);
        }
    }

    /// <summary>
    /// 导出json_luban
    /// </summary>
    public static void ExportExcel2Json_Luban()
    {
        try
        {
            bool hasError = false;
            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>开始导出配置表");
            string batPath = Directory.GetParent(Application.dataPath).FullName;
            batPath += "/Config/luban/MiniTemplate/gen.bat";
            if (!File.Exists(batPath))
            {
                EditorUtility.DisplayDialog("错误", "没有找到文件" + batPath, "确定");
                return;
            }
            FileInfo file = new FileInfo(batPath);
            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            pro.StartInfo.WorkingDirectory = file.Directory.FullName;
            pro.StartInfo.FileName = batPath;
            pro.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            pro.StartInfo.RedirectStandardError = true;  // 重定向错误输出
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.UseShellExecute = false;
            // 注册输出事件（实时获取输出）
            pro.OutputDataReceived += (sender, e) =>
            {
                string log = e.Data;
                if (!string.IsNullOrEmpty(log))
                {
                    if (log.Contains("|ERROR|===>"))
                    {
                        hasError = true;
                        Debug.LogError(log);
                    }
                    else
                        Debug.Log(log);
                }
            };

            pro.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    hasError = true;
                    Debug.LogError(e.Data);
                }
            };
            pro.Start();
            // 开始异步读取输出流
            pro.BeginOutputReadLine();
            pro.BeginErrorReadLine();
            pro.WaitForExit();
            if (hasError)
            {
                Debug.LogError("导出配置表错误<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
            else
            {
                Debug.Log("导出配置表完成<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("导出配置表错误<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<     " + ex.Message);
        }
    }

    /// <summary>
    /// 编辑器脚本路径
    /// </summary>
    public static string EditorScriptPath { get { return Application.dataPath + "/RainFramework/Editor/Dev/EditorDevTools_Dev.cs"; } }

    /// <summary>
    /// 场景配置文件   必须放在 Resources 下，运行时也会读取
    /// </summary>
    public string SceneConfigPath { get { return Application.dataPath + "/AssetBundles/Art/Resources/Config/SceneConfig.yaml"; } }

    public string SceneTemplatePath { get { return Application.dataPath + "/RainFramework/Runtime/Script/Config/SceneGenTemplate.txt"; } }

    public string SceneGenPath { get { return Application.dataPath + "/RainFramework/Runtime/Script/Config/SceneGen.cs"; } }

    /// <summary>
    /// UI配置文件   必须放在 Resources 下，运行时也会读取
    /// </summary>
    public string UIViewConfigPath { get { return Application.dataPath + "/AssetBundles/Art/Resources/Config/UIViewConfig.yaml"; } }

    public string UIViewTemplatePath { get { return Application.dataPath + "/RainFramework/Runtime/Script/Config/UIViewGenTemplate.txt"; } }

    public string UIViewGenPath { get { return Application.dataPath + "/RainFramework/Runtime/Script/Config/UIViewGen.cs"; } }

    /// <summary>
    /// 打开View的yaml配置文件
    /// </summary>
    public void OpenUIViewConfig()
    {
        EditorUtility.OpenWithDefaultApp(UIViewConfigPath);
    }

    /// <summary>
    /// 导出View配置
    /// </summary>
    public void ExportViewConfig()
    {
        if (!File.Exists(UIViewConfigPath))
        {
            Debug.LogError("找不到 UIViewConfig.yaml 文件");
            return;
        }
        UIViewConfig UIViewConfig = GetUIViewConfig();
        //创建模板
        string template = File.ReadAllText(UIViewTemplatePath);
        Template temp = Template.Parse(template);
        List<object> viewListList = new List<object>();
        foreach (var item in UIViewConfig.layer)
        {
            viewListList.Add(new { comment = item.Value.comment, layerVal = item.Value.order, layerName = item.Key });
        }
        List<object> viewGroupList = new List<object>();
        List<string> _tempGroup = new List<string>();
        foreach (var group in UIViewConfig.view)
        {
            foreach (var item in group.Value)
            {
                if (!_tempGroup.Contains(group.Key))
                {
                    _tempGroup.Add(group.Key);
                    viewGroupList.Add(new { group = group.Key });
                }
            }
        }
        List<object> viewNameList = new List<object>();
        foreach (var group in UIViewConfig.view)
        {
            foreach (var item in group.Value)
            {
                viewNameList.Add(new { viewName = item.Key, comment = item.Value.comment });
            }
        }
        Hash hash = Hash.FromAnonymousObject(new { ViewLayer = viewListList, ViewGroup = viewGroupList, ViewName = viewNameList });
        string result = temp.Render(hash);
        File.WriteAllText(UIViewGenPath, result);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 读取UI配置
    /// </summary>
    /// <returns></returns>
    public UIViewConfig GetUIViewConfig()
    {
        string config = File.ReadAllText(UIViewConfigPath);
        var deserializer = new DeserializerBuilder()
              .WithNamingConvention(CamelCaseNamingConvention.Instance)
              .Build();
        UIViewConfig UIViewConfig = deserializer.Deserialize<UIViewConfig>(config);
        return UIViewConfig;
    }

    /// <summary>
    /// 获取某个View的配置
    /// </summary>
    /// <param name="_viewName"></param>
    /// <returns></returns>
    public ViewConfig GetViewConfig(string _viewName)
    {
        UIViewConfig uiViewConfig = GetUIViewConfig();
        foreach (var group in uiViewConfig.view)
        {
            foreach (var item in group.Value)
            {
                if (item.Key == _viewName)
                {
                    item.Value.viewName = item.Key;
                    item.Value.group = group.Key;
                    return item.Value;
                }
            }
        }
        ViewConfig viewConfig = new ViewConfig();
        viewConfig.viewName = _viewName;
        viewConfig.group = "Common";
        //ViewName
        return viewConfig;
    }

    /// <summary>
    /// 打开View的yaml配置文件
    /// </summary>
    public void OpenSceneConfig()
    {
        EditorUtility.OpenWithDefaultApp(SceneConfigPath);
    }

    /// <summary>
    /// 导出Scene配置
    /// </summary>
    public void ExportSceneConfig()
    {
        if (!File.Exists(SceneConfigPath))
        {
            Debug.LogError("找不到 SceneConfig.yaml 文件");
            return;
        }
        string config = File.ReadAllText(SceneConfigPath);
        var deserializer = new DeserializerBuilder()
              .WithNamingConvention(CamelCaseNamingConvention.Instance)
              .Build();
        AllSceneConfig allSceneConfig = deserializer.Deserialize<AllSceneConfig>(config);

        //创建模板
        string template = File.ReadAllText(SceneTemplatePath);
        Template temp = Template.Parse(template);
        List<object> allConfigList = new List<object>();
        foreach (var item in allSceneConfig.scenes)
        {
            allConfigList.Add(new { comment = item.Value.comment, sceneName = item.Key });
        }
        Hash hash = Hash.FromAnonymousObject(new { scenes = allConfigList });
        string result = temp.Render(hash);
        File.WriteAllText(SceneGenPath, result);
        AssetDatabase.Refresh();

        //向Build Setting中 添加场景
        EditorBuildSettingsScene[] currentScenes = EditorBuildSettings.scenes;
        if (currentScenes.Length != allSceneConfig.scenes.Count)
        {
            List<EditorBuildSettingsScene> sceneList = currentScenes.ToList();
            foreach (var item in allSceneConfig.scenes)
            {
                string scenePath = FileTools.GetFirstFileByName("Assets/AssetBundles/Scenes/", item.Key + ".unity");
                if (string.IsNullOrEmpty(scenePath))
                {
                    Debug.LogError($"场景不存在: {item.Key}.unity (在 Assets/AssetBundles/Scenes/ 及其子目录中未找到)");
                    continue;
                }
                if (sceneList.Any(s => s.path == scenePath))
                {
                    Debug.LogWarning($"场景已存在于BuildSettings中: {scenePath}");
                    continue;
                }
                sceneList.Add(new EditorBuildSettingsScene(scenePath, true));
            }
            currentScenes = sceneList.ToArray();
        }
        EditorBuildSettings.scenes = currentScenes;
    }
}
