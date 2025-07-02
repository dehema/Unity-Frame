using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rain.Core;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace Rain.UI.Editor
{
    enum BaseUIType
    {
        BaseUI,
        BaseView,
        BasePoolItem,
    }

    public class EditorExportUI
    {
        static GameObject uiPrefab;
        /// <summary>
        /// 导出视图的UI
        /// </summary>
        public static void ExportViewUI(GameObject _uiPrefab = null)
        {
            try
            {
                // 1. 检查运行状态
                if (EditorApplication.isPlaying)
                {
                    EditorUtility.DisplayDialog("错误", "正在运行时不能导出", "确定");
                    return;
                }

                // 2. 获取预制体
                uiPrefab = _uiPrefab;
                if (uiPrefab == null)
                {
                    PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                    if (stage == null)
                    {
                        EditorUtility.DisplayDialog("错误", "请先打开一个View的预制体，进入预制体模式", "确定");
                        return;
                    }
                    uiPrefab = currActiveBaseUIPrefab;
                    if (uiPrefab == null)
                    {
                        EditorUtility.DisplayDialog("错误", "无法获取当前预制体", "确定");
                        return;
                    }
                }

                // 3. 获取组件和基本信息
                BaseUI baseUI = uiPrefab.GetComponent<BaseUI>();
                if (baseUI == null)
                {
                    EditorUtility.DisplayDialog("错误", "预制体必须包含继承自BaseUI的组件", "确定");
                    return;
                }
                BaseUIType baseUIType = BaseUIType.BaseUI;
                if (baseUI is BaseView)
                {
                    baseUIType = BaseUIType.BaseView;
                }
                else if (baseUI is BasePoolItem)
                {
                    baseUIType = BaseUIType.BasePoolItem;
                }

                // 设置是否为视图导出
                string viewName = uiPrefab.name;

                // 4. 获取UI结构内容
                List<Transform> allRoot = ForeachRoot(uiPrefab.transform);
                List<Transform> tfList = GetRegularRoot(allRoot);
                string uiModelContent = GetUIModelContent(tfList);

                // 5. 确定父类名称
                string superClassName = baseUIType.ToString();

                // 6. 查找脚本路径
                string viewScriptFolderPath = FindViewScriptPath(viewName);
                if (string.IsNullOrEmpty(viewScriptFolderPath))
                {
                    EditorUtility.DisplayDialog("错误", "找不到与预制体同名脚本", "确定");
                    return;
                }

                // 确保目录存在
                if (!Directory.Exists(viewScriptFolderPath))
                {
                    Directory.CreateDirectory(viewScriptFolderPath);
                }

                // 7. 生成脚本内容
                string templatePath = GetTemplatePath(baseUIType, superClassName);
                string content = GenerateScriptContent(templatePath, viewName, uiModelContent, superClassName);

                // 8. 写入文件并处理
                string viewUIPath = Path.Combine(viewScriptFolderPath, viewName + "_UI.cs");
                string oldMd5 = File.Exists(viewUIPath) ? GetFileMD5(viewUIPath) : string.Empty;

                File.WriteAllText(viewUIPath, content);
                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(viewUIPath));

                string newMd5 = GetFileMD5(viewUIPath);
                if (oldMd5 == newMd5)
                {
                    // 文件没变动不用重构工程，直接序列化
                    SetSerializedObject(uiPrefab);
                }
                else
                {
                    EditorViewCreater.AddTempFileLabels(fieldNeedSerializedObject, viewName);
                    AssetDatabase.Refresh();
                }

                Debug.Log($"成功导出UI: {viewUIPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"导出UI时发生错误: {ex.Message}\n{ex.StackTrace}");
                EditorUtility.DisplayDialog("错误", $"导出UI时发生错误: {ex.Message}", "确定");
            }
        }

        /// <summary>
        /// 查找视图脚本路径
        /// </summary>
        private static string FindViewScriptPath(string viewName)
        {
            foreach (var item in AssetDatabase.FindAssets(viewName + " t:Script"))
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(item);
                if (scriptPath.Contains("/" + viewName + ".cs"))
                {
                    return scriptPath.Replace(viewName + ".cs", string.Empty);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取模板路径
        /// </summary>
        private static string GetTemplatePath(BaseUIType baseUIType, string superClassName)
        {
            switch (baseUIType)
            {
                case BaseUIType.BaseUI:
                    return PathExportBaseUITemplate;
                case BaseUIType.BaseView:
                    return PathExportBaseViewTemplate;
                case BaseUIType.BasePoolItem:
                default:
                    return PathExportBasePoolItemTemplate;
            }
        }

        /// <summary>
        /// 生成脚本内容
        /// </summary>
        private static string GenerateScriptContent(string templatePath, string viewName, string uiModelContent, string superClassName)
        {
            string content = File.ReadAllText(templatePath);
            content = content.Replace("#ScriptName#", viewName);
            content = content.Replace("#Content#", "");
            content = content.Replace("#UIModelContent#", uiModelContent);
            content = content.Replace("#Superclass#", superClassName);
            return content;
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
            get
            {
                PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null)
                {
                    return prefabStage.prefabContentsRoot;
                }
                Scene scene = EditorSceneManager.GetActiveScene();
                GameObject prefab = GameObject.Find(scene.name);
                foreach (var rootObj in scene.GetRootGameObjects())
                {
                    if (rootObj.name == scene.name && rootObj.GetComponent<BaseUI>() != null)
                    {
                        return rootObj;
                    }
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
                EditorViewCreater.RemoveTempFileLabels(fieldNeedSerializedObject);
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
            Debug.Log("开始序列化");
            List<Transform> allRoot = ForeachRoot(_uiPrefab.transform);
            List<Transform> tfList = GetRegularRoot(allRoot);
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
            SerializedProperty uiProperty = serializedObj.FindProperty("ui");
            if (uiProperty == null)
            {
                Debug.Log("找不到ui结构体");
                return;
            }
            foreach (var item in dic)
            {
                string rootName = item.Key;
                List<string> componentNames = item.Value;
                SerializedProperty rootField = uiProperty.FindPropertyRelative(rootName);
                if (rootField == null)
                {
                    Debug.Log("找不到序列化对象，跳出");
                    continue;
                }
                string rootFullName = GetRootFullName(rootName, componentNames);
                GameObject goRoot = _uiPrefab.transform.FindRecursive(rootFullName).gameObject;
                rootField.objectReferenceValue = goRoot;
                foreach (var componentName in componentNames)
                {
                    string componentRootName = rootName + "_" + componentName;
                    SerializedProperty componentField = uiProperty.FindPropertyRelative(componentRootName);
                    Component component = goRoot.GetComponent(GetComponentFullName(componentName));
                    componentField.objectReferenceValue = component;
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
        /// BaseUI模板脚本
        /// </summary>
        /// <returns></returns>
        public static string PathExportBaseUITemplate { get { return Application.dataPath + "/RainFramework/Editor/UI/AutoCreator/ExportBaseUITemplate.txt"; } }

        /// <summary>
        /// BaseView上所有UI模板脚本
        /// </summary>
        /// <returns></returns>
        public static string PathExportBaseViewTemplate { get { return Application.dataPath + "/RainFramework/Editor/UI/AutoCreator/ExportBaseViewScriptTemplate.txt"; } }

        /// <summary>
        /// BasePoolItem上所有UI模板脚本
        /// </summary>
        /// <returns></returns>
        public static string PathExportBasePoolItemTemplate { get { return Application.dataPath + "/RainFramework/Editor/UI/AutoCreator/ExportBasePoolItemTemplate.txt"; } }

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
                scriptStr.Append($"        [SerializeField] public GameObject {goName};\n");
                foreach (string _componentName in item.Value)
                {
                    string componentFullName = GetComponentFullName(_componentName);
                    scriptStr.Append($"        [SerializeField] public {componentFullName} {goName + "_" + _componentName};\n");
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
                        else
                        {
                            bool res = EditorUtility.DisplayDialog("错误", $"在名为{componentFullName}的对象上找不到{componentName}组件", "确定", "自动添加");
                            if (res == false)
                            {
                                try
                                {
                                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                                    foreach (Assembly assembly in assemblies)
                                    {
                                        Type[] types = assembly.GetTypes();
                                        foreach (Type type in types)
                                        {
                                            if (type.Name == componentName && typeof(Component).IsAssignableFrom(type))
                                            {
                                                item.gameObject.AddComponent(type);
                                                components.Add(componentName);
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                    EditorUtility.DisplayDialog("错误", $"在名为{componentFullName}的对象上尝试添加{componentName}组件失败，请手动添加", "确定");
                                }
                            }
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

        static bool IsBaseView(GameObject item)
        {
            return item.GetComponent<BaseView>() != null;
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
            Transform rootNode = rootParent;
            while (rootParent != null && rootParent.parent != null)
            {
                rootPath = rootParent.gameObject.name + "/" + rootPath;
                rootParent = rootParent.parent;
                rootNode = rootParent;
            }
            if (!IsBaseView(rootNode.gameObject))
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
}