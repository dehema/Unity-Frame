using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotLiquid;
using Rain.Core;
using Rain.UI;
using Rain.UI.Editor;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
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
        if (GUILayout.Button("删除所有丢失的脚本组件", style.bt))
        {
            DelAllMissScripts();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("替换所有丢失字体的文本", style.bt))
        {
            ReplaceAllFont(true);
        }
        if (GUILayout.Button("替换所有文本的字体", style.bt))
        {
            ReplaceAllFont();
        }
        newfont = EditorGUILayout.ObjectField(newfont, typeof(Font), true) as Font;
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
        if (GUILayout.Button("打开Excel配置目录", style.bt))
        {
            string path = Directory.GetParent(Application.dataPath).FullName + @"\Product\StaticData";
            System.Diagnostics.Process.Start("Explorer.exe", path);
        }
        if (GUILayout.Button("导出Excel", style.bt))
        {
            ExportExcel2Json();
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
            if (GUILayout.Button("停止游戏 Ctrl+P", style.btLarge, GUILayout.Height(30)))
            {
                EditorCoroutineUtility.StartCoroutine(StopGame(), this);
            }
        }
        else
        {
            if (GUILayout.Button("启动游戏 Ctrl+P | Ctrl+R", style.btLarge, GUILayout.Height(30)))
            {
                StartGame();
            }
        }
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("重启游戏 Ctrl+R", style.btLarge, GUILayout.Height(30)))
            {
                ResetGame();
            }
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
    /// 删除所有丢失的脚本组件
    /// </summary>
    public static void DelAllMissScripts()
    {
        Action<GameObject, string> action = (GameObject _ui, string _uiPath) =>
        {
            int missNum = 0;
            foreach (var trans in _ui.GetComponentsInChildren<Transform>(true))
            {
                missNum += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(trans.gameObject);
            }
            if (missNum > 0)
            {
                PrefabUtility.SaveAsPrefabAsset(_ui, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(_ui));
                Debug.Log(string.Format("{0}删除{1}个丢失脚本", _uiPath, missNum));
            }
        };
        ForeachAllUIPrefab(action);
    }

    [SerializeField]
    static Font newfont;
    /// 替换所有丢失字体的文本组件
    /// </summary>
    /// </summary>
    /// <param name="_onlyMissFont">只有丢失字体的文本</param>
    public static void ReplaceAllFont(bool _onlyMissFont = false)
    {
        if (newfont == null)
        {
            EditorUtility.DisplayDialog("提示", "先设置新字体", "确定");
            return;
        }
        Action<GameObject, string> action = (GameObject _ui, string _uiPath) =>
        {
            int textNum = 0;
            foreach (var text in _ui.GetComponentsInChildren<Text>(true))
            {
                if (_onlyMissFont && text.font != null)
                {
                    continue;
                }
                textNum++;
                text.font = newfont;
            }
            if (textNum > 0)
            {
                PrefabUtility.SaveAsPrefabAsset(_ui, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(_ui));
                Debug.Log($"{_uiPath}替换{textNum}个文本");
            }
        };
        ForeachAllUIPrefab(action);
    }

    /// <summary>
    /// 遍历所有UI预制体
    /// </summary>
    public static void ForeachAllUIPrefab(Action<GameObject, string> _action)
    {
        List<string> PrefabPath = new List<string>();
        Action<string, string> FindPrefabPath = (_resDirPath, prefix) =>
        {
            foreach (var filePath in Directory.GetFiles(_resDirPath + prefix, "*.prefab", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(filePath);
                fileName = fileName.Replace(".prefab", string.Empty);
                PrefabPath.Add(prefix + fileName);
            }
        };
        FindPrefabPath(Application.dataPath + "/Resources/", "View/");
        FindPrefabPath(Application.dataPath + "/Framework/Resources/", "View/");
        foreach (var path in PrefabPath)
        {
            GameObject ui = PrefabUtility.InstantiatePrefab(Resources.Load(path) as GameObject) as GameObject;
            _action(ui, path);
            GameObject.DestroyImmediate(ui);
        }
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
    /// 编辑器脚本路径
    /// </summary>
    public static string EditorScriptPath { get { return Application.dataPath + "/RainFramework/Editor/Dev/EditorDevTools.cs"; } }

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
        Utility.Log(allSceneConfig);
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
    }
}
