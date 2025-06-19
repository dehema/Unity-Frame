using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Scene = UnityEngine.SceneManagement.Scene;

public class EditorViewCreater
{
    static Scene currScene;
    /// <summary>
    /// 创建示例UI
    /// </summary>
    public static void CreateView(string _viewName)
    {
        if (!CheckRegular(_viewName))
            return;
        CreateViewScene(_viewName);
        CreateViewScript(_viewName);
        ///调用CreateViewScript之后，脚本一定创建成功，这时候需要重编译代码，等编译完成后调用OnCompileScripts
        CreateViewPrefab(_viewName);
        //view.AddComponent(Type.GetType("UnityEngine.Rigidbody, UnityEngine.PhysicsModule"));
        //view.AddComponent(Type.GetType("DebugView, Assembly-CSharp"));
        Debug.Log("创建View--->" + _viewName);
    }

    static readonly string fieldNeedExportViewUI = "NeedExportViewUI";
    /// <summary>
    /// 此标记可以让脚本在编译后在调用一次
    /// </summary>
    [DidReloadScripts]
    public static void OnCompileScripts()
    {
        if (!Application.isEditor || EditorApplication.isPlaying)
            return;
        Dictionary<string, string> labels = GetTempFileLabels();
        if (labels.ContainsKey(fieldNeedExportViewUI))
        {
            RemoveTempFileLabels(fieldNeedExportViewUI);
            string viewName = labels[fieldNeedExportViewUI];
            GameObject prefab = GameObject.Find(viewName);
            AddScriptAndSavePrefab(viewName, prefab);
            EditorExportUI.ExportViewUI(prefab);
            EditorSceneManager.SaveOpenScenes();
        }
    }

    /// <summary>
    /// 获取临时标记
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, string> GetTempFileLabels()
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        if (!File.Exists(TempFilePath))
            return dict;
        string json = File.ReadAllText(TempFilePath);
        List<string> labels = new List<string>();
        dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        return dict;
    }

    /// <summary>
    /// 增加临时标记
    /// </summary>
    /// <param name="_dict"></param>
    public static void AddTempFileLabels(string _key, string _val)
    {
        Dictionary<string, string> dict = GetTempFileLabels();
        dict[_key] = _val;
        string content = JsonConvert.SerializeObject(dict);
        File.WriteAllText(TempFilePath, content);
    }

    public static void RemoveTempFileLabels(string _key)
    {
        Dictionary<string, string> dict = GetTempFileLabels();
        dict.Remove(_key);
        string content = JsonConvert.SerializeObject(dict);
        File.WriteAllText(TempFilePath, content);
    }

    private static string TempFilePath
    {
        get
        {
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, "._tempViewCreater.txt");
        }
    }

    /// <summary>
    /// 是否存在临时文件
    /// </summary>
    /// <returns></returns>
    public static bool IsExistsTempFile()
    {
        return File.Exists(TempFilePath);
    }

    public static Scene CreateViewScene(string _viewName)
    {
        string templateScenePath = $"Assets/Scenes/TemplateScene.unity";
        string scenePath = $"Assets/Scenes/{_viewName}.unity";
        AssetDatabase.CopyAsset(templateScenePath, scenePath);
        currScene = EditorSceneManager.OpenScene(scenePath);
        return currScene;
    }

    public static GameObject CreateViewPrefab(string _viewName)
    {
        if (Resources.Load<GameObject>(UIMgr.uiPrefabPath + _viewName))
        {
            EditorUtility.DisplayDialog("错误", $"已存在同名预制体--->{UIMgr.uiPrefabPath + _viewName}", "ok");
            return null;
        }
        GameObject view = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(UIMgr.uiPrefabPath + "TemplateView")) as GameObject;
        PrefabUtility.UnpackPrefabInstance(view, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
        view.name = _viewName;
        view.transform.SetAsLastSibling();
        Canvas canvas = view.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        CanvasScaler canvasScaler = view.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        AddTempFileLabels(fieldNeedExportViewUI, _viewName);
        return view;
    }

    static GameObject AddScriptAndSavePrefab(string _viewName, GameObject _view)
    {
        try
        {
            _view.AddComponent(Type.GetType($"{_viewName}, Assembly-CSharp"));
        }
        catch (Exception)
        {
            Debug.Log($"添加组件{_viewName}失败", _view);
            throw;
        }
        string sceneName = EditorSceneManager.GetActiveScene().name;
        GameObject viewGo = GameObject.Find(sceneName);
        string relativePath = "/Resources/View/";
        string dirPath = Application.dataPath + relativePath;
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        string prefabPath = dirPath + sceneName + ".prefab";
        //if (File.Exists(prefabPath))
        //{
        //    EditorUtility.DisplayDialog("错误", $"已存在同名预制体--->{prefabPath}", "ok");
        //    return null;
        //}
        Debug.Log("生成View预制体" + prefabPath, viewGo);
        GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(viewGo, prefabPath, InteractionMode.AutomatedAction);
        return prefab;
    }

    //创建脚本
    public static bool CreateViewScript(string _viewName)
    {
        string dirPath = Application.dataPath + "/Script/View/";
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        string viewPath = dirPath + $"{_viewName}.cs";
        if (File.Exists(viewPath))
        {
            EditorUtility.DisplayDialog("错误", $"已存在同名脚本--->{viewPath}", "ok");
            return false;
        }
        string viewTemp = File.ReadAllText(GetAutoCreateViewTemplatePath());
        string viewScript = viewTemp.Replace("#ViewName#", _viewName);
        File.WriteAllText(viewPath, viewScript);
        Debug.Log("生成View脚本" + viewPath);
        AssetDatabase.Refresh();
        return true;
    }

    /// <summary>
    /// 检查命名规则
    /// </summary>
    /// <param name="_viewName"></param>
    /// <returns></returns>
    public static bool CheckRegular(string _viewName)
    {
        if (string.IsNullOrEmpty(_viewName))
        {
            EditorUtility.DisplayDialog("错误", "在前面的输入框中输入要创建的View", "ok");
            return false;
        }
        foreach (char item in _viewName)
        {
            if ((!char.IsLetter(item) || char.ToLower(item) < 'a' || char.ToLower(item) > 'z') && item != '_')
            {
                EditorUtility.DisplayDialog("错误", "命名不符合规则", "ok");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 获取自动页面的模板文件路径
    /// </summary>
    /// <returns></returns>
    public static string GetAutoCreateViewTemplatePath()
    {
        return Application.dataPath + "/Framework/Script/View/AutoCreateViewTemplate.txt";
    }
}
