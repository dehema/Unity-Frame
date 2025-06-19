using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEditor.Progress;
using Object = UnityEngine.Object;

public class EditorExportUI : EditorWindow
{
    static GameObject uiPrefab;
    /// <summary>
    /// 导出的的是视图脚本
    /// </summary>
    static bool isExportView;
    /// <summary>
    /// 导出视图的UI
    /// </summary>
    public static void ExportViewUI(GameObject _uiPrefab = null)
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("错误", "正在运行时不能导出", "ok");
            return;
        }
        uiPrefab = _uiPrefab;
        PrefabStage stage = null;
        if (uiPrefab == null)
        {
            stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (!stage)
            {
                EditorUtility.DisplayDialog("错误", "先打开一个View的预制体，进入预制体模式", "ok");
                return;
            }
        }
        uiPrefab = uiPrefab ?? currActiveBaseUIPrefab;
        BaseView baseView = uiPrefab.GetComponent<BaseView>();
        BaseUI baseUI = uiPrefab.GetComponent<BaseUI>();
        //导出的是否是视图
        isExportView = baseView != null;
        string viewName = uiPrefab.name;
        //Debug.Log(viewName, root);
        List<Transform> allRoot = ForeachRoot(uiPrefab.transform);
        List<Transform> tfList = GetRegularRoot(allRoot);
        string scriptContent = GetUIScriptContent(tfList);
        string uiModelContent = GetUIModelContent(tfList);
        string folderPath = string.Empty;
        //父类名称
        string superClassName = isExportView ? "BaseView" : "BaseUI";
        //获取View脚本路径
        if (baseView != null)
        {
            superClassName = baseView.GetType().BaseType.ToString();
        }
        else if (baseUI != null)
        {
            superClassName = baseUI.GetType().BaseType.ToString();
        }
        foreach (var item in AssetDatabase.FindAssets(uiPrefab.name + " t:Script"))
        {
            string scriptPath = AssetDatabase.GUIDToAssetPath(item);
            if (scriptPath.Contains("/" + viewName + ".cs"))
            {
                folderPath = scriptPath.Replace(viewName + ".cs", string.Empty);
                break;
            }
        }
        //else
        //{
        //    folderPath = $"{Application.dataPath}/Script/Item/";
        //}
        if (string.IsNullOrEmpty(folderPath))
        {
            EditorUtility.DisplayDialog("错误", "找不到与View预制体同名脚本", "ok");
            return;
        }
        string tempViewUIContent = string.Empty;
        if (isExportView)
        {
            tempViewUIContent = File.ReadAllText(GetExportViewScriptTemplatePath());
        }
        else if (superClassName == "PoolItemBase")
        {
            tempViewUIContent = File.ReadAllText(GetExportPoolItemBaseTemplatePath());
        }
        else
        {
            tempViewUIContent = File.ReadAllText(GetExportUIWidgetTemplatePath());
        }
        tempViewUIContent = tempViewUIContent.Replace("#ScriptName#", viewName);
        scriptContent = tempViewUIContent.Replace("#Content#", scriptContent);
        scriptContent = scriptContent.Replace("#UIModelContent#", uiModelContent);
        scriptContent = scriptContent.Replace("#Superclass#", superClassName);
        string viewUIPath = folderPath + viewName + "_UI.cs";
        string oldMd5 = "";
        if (File.Exists(viewUIPath))
        {
            oldMd5 = GetFileMD5(viewUIPath);
        }
        File.WriteAllText(viewUIPath, scriptContent);
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(viewUIPath));
        string newMd5 = GetFileMD5(viewUIPath);
        if (oldMd5 == newMd5)
        {
            //文件没变动不用重构工程 直接序列化
            SetSerializedObject(uiPrefab);
        }
        else
        {
            EditorViewCreater.AddTempFileLabels(fieldNeedSerializedObject, viewName);
            AssetDatabase.Refresh();
        }
        Debug.Log($"导出{viewUIPath}");
    }

    /// <summary>
    /// 获取文件的md5
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileMD5(string filePath)
    {
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            using (MD5 md5 = MD5.Create())
            {
                // 计算文件的MD5哈希值
                byte[] hashBytes = md5.ComputeHash(fileStream);

                // 将字节数组转换为十六进制字符串
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    /// <summary>
    /// 当前活动的预制体 BaseView或BaseUI
    /// </summary>
    /// <returns></returns>
    static GameObject currActiveBaseUIPrefab
    {
        get {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage)
            {
                return prefabStage.prefabContentsRoot;
            }
            Scene scene = EditorSceneManager.GetActiveScene();
            GameObject prefab = GameObject.Find(scene.name);
            if (prefab != null && prefab.GetComponent<BaseUI>())
            {
                return prefab;
            }
            return null;
        }
    }

    static readonly string fieldNeedSerializedObject = "NeedSerializedObject ";
    /// <summary>
    /// 此标记可以让脚本在编译后在调用一次
    /// </summary>
    [DidReloadScripts]
    public static void OnCompileScripts()
    {
        //如果_UI文件被则需要调用序列化对象
        Dictionary<string, string> dict = EditorViewCreater.GetTempFileLabels();
        if (dict.ContainsKey(fieldNeedSerializedObject))
        {
            GameObject root = currActiveBaseUIPrefab;
            if (root != null && root.name == dict[fieldNeedSerializedObject])
            {
                SetSerializedObject(root);
            }
        }
    }

    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <param name="_uiPrefab"></param>
    static void SetSerializedObject(GameObject _uiPrefab)
    {
        EditorViewCreater.RemoveTempFileLabels(fieldNeedSerializedObject);
        Debug.Log("开始序列化");
        List<Transform> allRoot = ForeachRoot(_uiPrefab.transform);
        List<Transform> tfList = GetRegularRoot(allRoot);
        string scriptContent = GetUIScriptContent(tfList);
        string uiModelContent = GetUIModelContent(tfList);
        Dictionary<string, List<string>> dic = GetRootAndComponentsName(tfList);
        BaseUI baseUI = _uiPrefab.GetComponent<BaseUI>();
        SerializedObject serializedObj = new SerializedObject(baseUI);
        if (serializedObj == null)
        {
            Debug.Log("找不到序列化脚本，返回");
            return;
        }
        serializedObj.Update();
        foreach (var item in dic)
        {
            string rootName = item.Key;
            List<string> componentNames = item.Value;
            SerializedProperty rootField = serializedObj.FindProperty(rootName);
            string rootFullName = GetRootFullName(rootName, componentNames);
            if (rootField == null)
            {
                Debug.Log("找不到序列化对象，跳出");
                continue;
            }
            GameObject goRoot = _uiPrefab.transform.FindRecursive(rootFullName).gameObject;
            rootField.objectReferenceValue = goRoot;
            foreach (var componentName in componentNames)
            {
                string componentRootName = rootName + "_" + componentName;
                SerializedProperty componentField = serializedObj.FindProperty(componentRootName);
                componentField.objectReferenceValue = goRoot.GetComponent(GetComponentFullName(componentName));
            }
        }
        // 应用更改
        serializedObj.ApplyModifiedProperties();
    }


    /// <summary>
    /// 根据节点简称和包含的组件获取节点全程
    /// </summary>
    /// <param name="_abbrName"></param>
    /// <param name="_componentNames"></param>
    /// <returns></returns>
    static string GetRootFullName(string _abbrName, List<string> _componentNames)
    {
        string name = "$" + _abbrName;
        if (_componentNames.Count > 0)
        {
            name += "#";
            name += string.Join(",", _componentNames);
        }
        return name;
    }

    /// <summary>
    /// 获取自动导出View上所有UI模板脚本
    /// </summary>
    /// <returns></returns>
    public static string GetExportViewScriptTemplatePath()
    {
        return Application.dataPath + "/Framework/Script/View/ExportViewScriptTemplate.txt";
    }

    /// <summary>
    /// 获取自动导出Prefab上所有UI模板脚本
    /// </summary>
    /// <returns></returns>
    public static string GetExportUIWidgetTemplatePath()
    {
        return Application.dataPath + "/Framework/Script/View/ExportUIWidgetTemplate.txt";
    }

    /// <summary>
    /// 获取自动导出对象池Item上所有UI模板脚本
    /// </summary>
    /// <returns></returns>
    public static string GetExportPoolItemBaseTemplatePath()
    {
        return Application.dataPath + "/Framework/Script/View/ExportPoolItemBaseTemplate.txt";
    }

    /// <summary>
    /// 获取所有符合规则的UI
    /// </summary>
    /// <returns></returns>
    public static List<Transform> GetRegularRoot(List<Transform> _tfList)
    {
        List<Transform> tfList = new List<Transform>();
        foreach (var item in _tfList)
        {
            string name = item.name;
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }
            if (!name.StartsWith("$"))
            {
                continue;
            }
            tfList.Add(item);
        }
        return tfList;
    }

    /// <summary>
    /// 获取ViewUI脚本的内容
    /// </summary>
    /// <param name="_tfList"></param>
    /// <returns></returns>
    public static string GetUIScriptContent(List<Transform> _tfList)
    {
        StringBuilder scriptStr = new StringBuilder();
        foreach (var item in _tfList)
        {
            string name = item.name;
            string goName = name.Replace("$", string.Empty).Split('#')[0];
            //处理某个节点
            string rootPath = GetRootFullPath(item.transform);
            scriptStr.Append($"        {goName} = transform.Find(\"{rootPath}\").gameObject;\n");
            if (name.Contains("#"))
            {
                string components = name.Replace("$", string.Empty).Split('#')[1];
                foreach (var componentName in components.Split(','))
                {
                    string componentFullName = GetComponentFullName(componentName);
                    if (item.GetComponent(componentFullName) == null)
                    {
                        EditorGUIUtility.PingObject(item);
                        EditorUtility.DisplayDialog("错误", $"物体{goName}找不到组件{componentFullName}", "ok");
                    }
                    else
                    {
                        scriptStr.Append($"        {goName + "_" + componentName} = {goName}.GetComponent<{componentFullName}>();\n");
                        //Debug.Log(component, componentRoot);
                    }
                }
            }
        }
        //Debug.Log(scriptStr.ToString());
        return scriptStr.ToString();
    }

    /// <summary>
    /// 获取视图UI的模型类内容
    /// </summary>
    /// <param name="_tfList"></param>
    /// <returns></returns>
    public static string GetUIModelContent(List<Transform> _tfList)
    {
        StringBuilder scriptStr = new StringBuilder();
        Dictionary<string, List<string>> dic = GetRootAndComponentsName(_tfList);
        foreach (var item in dic)
        {
            string goName = item.Key;
            //scriptStr.Append($"    [HideInInspector]\n");
            scriptStr.Append($"    [SerializeField]\n");
            scriptStr.Append($"    public GameObject {goName};\n");
            foreach (string _componentName in item.Value)
            {
                string componentFullName = GetComponentFullName(_componentName);
                scriptStr.Append($"    public {componentFullName} {goName + "_" + _componentName};\n");
            }
        }
        return scriptStr.ToString();
    }

    /// <summary>
    /// 获取节点名字和对应的组件名字
    /// </summary>
    /// <param name="_tfList"></param>
    /// <returns></returns>
    public static Dictionary<string, List<string>> GetRootAndComponentsName(List<Transform> _tfList)
    {
        Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

        foreach (var item in _tfList)
        {
            string name = item.name;
            List<string> components = new List<string>();
            string goName = name.Replace("$", string.Empty).Split('#')[0];
            if (name.Contains("#"))
            {
                string allComponentName = name.Replace("$", string.Empty).Split('#')[1];
                foreach (var componentName in allComponentName.Split(','))
                {
                    string componentFullName = GetComponentFullName(componentName);
                    if (item.GetComponent(componentFullName) != null)
                    {
                        components.Add(componentName);
                    }
                }
            }
            dic[goName] = components;
        }
        return dic;
    }

    /// <summary>
    /// 获取组件简称
    /// </summary>
    /// <returns></returns>
    public static string GetComponentFullName(string _name)
    {
        string name = _name;
        switch (name)
        {
            case "Rect":
                name = "RectTransform";
                break;
            case "Hor":
                name = "HorizontalLayoutGroup";
                break;
            case "Ver":
                name = "VerticalLayoutGroup";
                break;
        }
        return name;
    }

    /// <summary>
    /// 获取一个节点的全部路径
    /// </summary>
    /// <param name="_trans"></param>
    /// <returns></returns>
    public static string GetRootFullPath(Transform _trans)
    {
        string rootPath = _trans.gameObject.name;
        Transform rootParent = _trans.transform.parent;
        while (rootParent != null && rootParent.parent != null)
        {
            rootPath = rootParent.gameObject.name + "/" + rootPath;
            rootParent = rootParent.parent;
        }
        if (!isExportView)
        {
            int rootLength = uiPrefab.name.Length + 1;
            rootPath = rootPath.Substring(rootLength, rootPath.Length - rootLength);
        }
        return rootPath;
    }

    /// <summary>
    /// 递归这个页面的所有节点
    /// </summary>
    /// <param name="_root"></param>
    /// <param name="_tfList"></param>
    /// <returns></returns>
    public static List<Transform> ForeachRoot(Transform _root, List<Transform> _tfList = null)
    {
        List<Transform> tfList = _tfList ?? new List<Transform>();
        foreach (Transform child in _root)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(child.gameObject))
            {
                tfList.Add(child);
                continue;
            }
            tfList.Add(child);
            tfList = ForeachRoot(child, tfList);
        }
        return tfList;
    }
}
