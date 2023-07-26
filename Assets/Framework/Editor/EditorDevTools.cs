using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.EditorCoroutines.Editor;
using System.IO;
using UnityEngine.UI;
using System;
using Object = UnityEngine.Object;
using Path = System.IO.Path;
using DotLiquid;
using System.Collections;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using UnityEditor.Callbacks;

public class EditorDevTools : EditorWindow
{
    static GUILayoutOption[] commonLayout = { GUILayout.Height(30) };
    public static GUIStyle titleLabelStyle;
    float gameTimeSpeed = 1;
    static string editorScriptPath;

    private void OnEnable()
    {
        titleLabelStyle = new GUIStyle() { fontSize = 20, alignment = TextAnchor.MiddleCenter };
    }

    [MenuItem("开发工具/开发工具")]
    static void OpenMainWindow()
    {
        EditorDevTools window = GetWindow<EditorDevTools>();
        window.titleContent = new GUIContent("BM Development Tools");
        window.position = new Rect(400, 100, 640, 480);
        window.Show();
    }

    public static string GetEditorScriptPath()
    {
        return Application.dataPath + "/Script/Editor/EditorDevTools.cs";
    }

    Vector2 scrollPosition;
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        OnGUIUIPlaying();
        OnGUIEditor();
        OnGUIUI();
        OnGUIUIConfig();
        OnGUIUIOther();
        OnGUIDebug();
        EditorGUILayout.EndScrollView();
    }

    private void OnGUIEditor()
    {
        EditorGUILayout.LabelField("<color=white>------------------- 编辑 -------------------</color>", titleLabelStyle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        editorScriptPath = GetEditorScriptPath();
        GUILayout.Label("编辑脚本路径:" + editorScriptPath);
        if (GUILayout.Button("打开编辑器脚本"))
        {
            OpenEditorScript();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("清除PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll(); ;
        }
        if (GUILayout.Button("删除所有丢失的脚本组件"))
        {
            DelAllMissScripts();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("替换所有丢失字体的文本"))
        {
            ReplaceAllFont(true);
        }
        if (GUILayout.Button("替换所有文本的字体"))
        {
            ReplaceAllFont();
        }
        newfont = EditorGUILayout.ObjectField(newfont, typeof(Font)) as Font;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("导出Excel"))
        {
            ExportExcel2Json();
        }
        GUILayout.EndHorizontal();
    }

    string createViewName = "ViewNameYouWantCreate";
    private void OnGUIUI()
    {
        //UI
        EditorGUILayout.LabelField("<color=white>------------------- UI -------------------</color>", titleLabelStyle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        createViewName = GUILayout.TextField(createViewName, 30, GUILayout.Width(170));
        if (GUILayout.Button("创建View"))
        {
            EditorViewCreater.CreateView(createViewName);
        }
        if (GUILayout.Button("导出UI"))
        {
            EditorExportUI.ExportViewUI();
        }
        GUILayout.EndHorizontal();
    }
    private void OnGUIUIConfig()
    {
        //UI配置
        EditorGUILayout.LabelField("<color=white>------------------- 配置 -------------------</color>", titleLabelStyle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("打开View配置文件"))
        {
            OpenUIViewConfig();
        }
        if (GUILayout.Button("导出View配置"))
        {
            ExportViewConfig();
        }
        if (GUILayout.Button("打开Excel配置文件夹"))
        {
            string path = Directory.GetParent(Application.dataPath).FullName + @"\Product\StaticData";
            System.Diagnostics.Process.Start("Explorer.exe", path);
        }
        if (GUILayout.Button("打开多语言配置"))
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
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("套用本地配置"))
            {
                ConfigMgr.Ins.LoadAllConfig();
            }
            if (GUILayout.Button("套用远端配置"))
            {
                ConfigMgr.Ins.LoadAllConfig(false);
            }
        }
        GUILayout.EndHorizontal();
    }

    bool audit = false;
    private void OnGUIUIPlaying()
    {
        //运行
        EditorGUILayout.LabelField("<color=white>------------------- 运行 -------------------</color>", titleLabelStyle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("停止游戏 Ctrl+P", commonLayout))
            {
                EditorCoroutineUtility.StartCoroutine(StopGame(), this);
            }
        }
        else
        {
            if (GUILayout.Button("启动游戏 Ctrl+P | Ctrl+R", commonLayout))
            {
                StartGame();
            }
        }
        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("重启游戏 Ctrl+R", commonLayout))
            {
                ResetGame();
            }
        }
        GUILayout.EndHorizontal();
        if (EditorApplication.isPlaying)
        {
            GUILayout.BeginHorizontal();
            //Time.timeScale
            GUILayout.Label("游戏全局速度 x1 - x10", GUILayout.Width(130));
            gameTimeSpeed = GUILayout.HorizontalSlider(gameTimeSpeed, 1, 10, GUILayout.Width(this.position.width - 420));
            GUILayout.Label("x0.2 - x1", GUILayout.Width(50));
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
            GUIStyle timeScaleStyle = new GUIStyle() { fontSize = 14, alignment = TextAnchor.MiddleLeft };
            EditorGUILayout.LabelField("<color=#02FF19>x" + gameTimeSpeed.ToString("f2") + "</color>", timeScaleStyle, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GameSettingStatic.ResLog = GUILayout.Toggle(GameSettingStatic.ResLog, "资源log");
            audit = GUILayout.Toggle(audit, "审核模式");
            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 其他
    /// </summary>
    private void OnGUIUIOther()
    {
        GUILayout.Label("<color=white>------------------- 其他 -------------------</color>", titleLabelStyle, GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Hierarchy右键->BM工具", GUILayout.Width(150));
        GUILayout.EndHorizontal();
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
            EditorWindow.GetWindow<EditorDevTools>()._ResetGame();
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
    void _ResetGame()
    {
        string tempFilePath = Directory.GetParent(Application.dataPath).FullName + ReStartGameFile;
        File.WriteAllText(tempFilePath, string.Empty);
        EditorCoroutineUtility.StartCoroutine(ReStart(), this);
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
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity", OpenSceneMode.Single);
        EditorSceneManager.SetActiveScene(scene);
        EditorUtility.DisplayProgressBar("打开", "GameScene场景", 0.1f);
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

    [MenuItem("GameObject/BM工具/只 显示&&点击这个物体", priority = 0)]
    public static void PickingAndIsolateObj()
    {
        Object obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        //显示
        GameObject go = obj as GameObject;
        //点击
        HideAllGameObject();
        SceneVisibilityManager.instance.EnablePicking(go, true);
        SceneVisibilityManager.instance.Show(go, true);
    }

    [MenuItem("GameObject/BM工具/只显示最后一个NormalUI", priority = 1)]
    public static void OnlyShowLastNormalUI()
    {
        OnlyShowLastUI(ViewLayer.NormalUI.ToString());
    }

    [MenuItem("GameObject/BM工具/只显示最后一个PopUpUI", priority = 2)]
    public static void OnlyShowLastPopUI()
    {
        OnlyShowLastUI(ViewLayer.PopUI.ToString());
    }

    [MenuItem("GameObject/BM工具/只显示最后一个TipsUI", priority = 3)]
    public static void OnlyShowLastTipsUI()
    {
        OnlyShowLastUI(ViewLayer.TipsUI.ToString());
    }

    public static void OnlyShowLastUI(string _rootName)
    {
        GameObject uiRoot = GameObject.Find(typeof(UIMgr).ToString());
        if (uiRoot == null)
        {
            return;
        }
        Transform tf = uiRoot.transform.Find(_rootName);
        if (tf == null)
        {
            return;
        }
        if (tf.childCount <= 0)
        {
            return;
        }

        GameObject uigo = null;
        for (int i = tf.childCount - 1; i >= 0; i--)
        {
            var tmp = tf.GetChild(i).gameObject;
            if (!tmp.activeSelf)
            {
                continue;
            }
            uigo = tf.GetChild(i).gameObject;
            break;
        }
        if (uigo == null)
        {
            return;
        }
        HideAllGameObject();
        SceneVisibilityManager.instance.EnablePicking(uigo, true);
        SceneVisibilityManager.instance.Show(uigo, true);
        //Hierarchy面板选择该物体
        EditorGUIUtility.PingObject(uigo);
        Selection.activeGameObject = uigo;
    }


    [MenuItem("GameObject/BM工具/重置 显示&&点击", priority = 999)]
    public static void PickingAndIsolateReset()
    {
        ShowAllGameObject();
    }

    /// <summary>
    /// 显示和可点击所有的物体
    /// </summary>
    private static void ShowAllGameObject()
    {
        foreach (GameObject item in FindObjectsOfType<GameObject>())
        {
            SceneVisibilityManager.instance.Show(item, true);
            SceneVisibilityManager.instance.EnablePicking(item, true);
        }
    }

    /// <summary>
    /// 隐藏和不可点击所有的物体
    /// </summary>
    private static void HideAllGameObject()
    {
        foreach (GameObject item in FindObjectsOfType<GameObject>())
        {
            SceneVisibilityManager.instance.Hide(item, true);
            SceneVisibilityManager.instance.DisablePicking(item, true);
        }
    }

    /// <summary>
    /// 打开此脚本
    /// </summary>
    private static void OpenEditorScript()
    {
        editorScriptPath = GetEditorScriptPath();
        Debug.Log("打开脚本" + editorScriptPath);
        EditorUtility.OpenWithDefaultApp(editorScriptPath);
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
            DestroyImmediate(ui);
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
    /// 获取UIView配置Yaml文件路径
    /// </summary>
    /// <returns></returns>
    public string GetUIViewConfigPath()
    {
        return Application.dataPath + "/Framework/Resources/Config/UIView.yaml";
    }

    /// <summary>
    /// 获取UIView配置Yaml文件路径
    /// </summary>
    /// <returns></returns>
    public string GetUIViewTemplatePath()
    {
        return Application.dataPath + "/Framework/Script/Config/UIViewGenTemplate.txt";
    }

    /// <summary>
    /// 获取UIView配置Yaml文件路径
    /// </summary>
    /// <returns></returns>
    public string GetUIViewUIViewGenPath()
    {
        return Application.dataPath + "/Framework/Script/Config/UIViewGen.cs";
    }

    /// <summary>
    /// 打开View的yaml配置文件
    /// </summary>
    public void OpenUIViewConfig()
    {
        EditorUtility.OpenWithDefaultApp(GetUIViewConfigPath());
    }

    /// <summary>
    /// 导出View配置
    /// </summary>
    public void ExportViewConfig()
    {
        string configPath = GetUIViewConfigPath();
        if (!File.Exists(configPath))
        {
            Debug.LogError("找不到UIView.yaml文件");
            return;
        }
        string config = File.ReadAllText(configPath);
        var deserializer = new DeserializerBuilder()
              .WithNamingConvention(CamelCaseNamingConvention.Instance)
              .Build();
        UIViewConfig UIViewConfig = deserializer.Deserialize<UIViewConfig>(config);
        Utility.Dump(UIViewConfig);
        //创建模板
        string template = File.ReadAllText(GetUIViewTemplatePath());
        Template temp = Template.Parse(template);
        List<object> layerConfigList = new List<object>();
        foreach (var item in UIViewConfig.layer)
        {
            layerConfigList.Add(new { layerComment = item.Value.comment, layerVal = item.Value.order, layerName = item.Key });
        }
        List<object> viewList = new List<object>();
        foreach (var item in UIViewConfig.view)
        {
            viewList.Add(new { viewName = item.Key });
        }
        Hash hash = Hash.FromAnonymousObject(new { layer = layerConfigList, view = viewList });
        string result = temp.Render(hash);
        File.WriteAllText(GetUIViewUIViewGenPath(), result);
        AssetDatabase.Refresh();
    }
}