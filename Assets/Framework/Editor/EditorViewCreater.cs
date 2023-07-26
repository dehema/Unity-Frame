using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;
using UnityEditor.SceneManagement;
using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine.UI;

public class EditorViewCreater
{
    static Scene currScene;
    const string tempViewCreater = "/._tempViewCreater.txt";
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

    /// <summary>
    /// 此标记可以让脚本在编译后在调用一次
    /// </summary>
    [DidReloadScripts]
    public static void OnCompileScripts()
    {
        if (!Application.isEditor || EditorApplication.isPlaying)
            return;
        string tempFilePath = Directory.GetParent(Application.dataPath).FullName + tempViewCreater;
        if (!File.Exists(tempFilePath))
            return;
        string viewName = File.ReadAllText(tempFilePath);
        File.Delete(tempFilePath);
        GameObject prefab = GameObject.Find(viewName);
        AddScriptAndSavePrefab(viewName, prefab);
        EditorExportUI.ExportViewUI(prefab);
        EditorSceneManager.SaveOpenScenes();
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

        string tempFile = Directory.GetParent(Application.dataPath).FullName + tempViewCreater;
        File.WriteAllText(tempFile, _viewName);
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
        string relativePath = $"/Resources/View/";
        string prefabPath = Application.dataPath + relativePath + sceneName + ".prefab";
        //if (File.Exists(prefabPath))
        //{
        //    EditorUtility.DisplayDialog("错误", $"已存在同名预制体--->{prefabPath}", "ok");
        //    return null;
        //}
        Debug.Log("生成View预制体" + prefabPath, viewGo);
        GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(viewGo, prefabPath, InteractionMode.AutomatedAction);
        return prefab;
    }



    public static bool CreateViewScript(string _viewName)
    {
        string relativePath = $"/Script/View/{_viewName}.cs";
        string viewPath = Application.dataPath + relativePath;
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
