using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;

namespace Rain.UI.Editor
{
    public class EditorViewCreater
    {
        static Scene currScene;
        /// <summary>
        /// 创建示例UI
        /// </summary>
        public static void CreateView(ViewConfig _viewConfig)
        {
            string viewName = _viewConfig.viewName;
            if (!CheckRegular(_viewConfig))
                return;
            CreateViewScene(viewName);
            CreateViewScript(_viewConfig);
            ///调用CreateViewScript之后，脚本一定创建成功，这时候需要重编译代码，等编译完成后调用OnCompileScripts
            CreateViewPrefab(viewName);
            //view.AddComponent(Type.GetType("UnityEngine.Rigidbody, UnityEngine.PhysicsModule"));
            //view.AddComponent(Type.GetType("DebugView, Assembly-CSharp"));
            Debug.Log("创建View--->" + viewName);
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

        const string templateScenePath = "Assets/AssetBundles/Scenes/TemplateScene.unity";
        public static Scene CreateViewScene(string _viewName)
        {

            // 检查模板场景是否存在，如果不存在直接中断执行
            if (!File.Exists(templateScenePath))
            {
                Debug.LogError($"模板场景不存在: {templateScenePath}");
            }

            string scenePath = $"Assets/AssetBundles/Scenes/{_viewName}.unity";
            AssetDatabase.CopyAsset(templateScenePath, scenePath);
            currScene = EditorSceneManager.OpenScene(scenePath);
            return currScene;
        }

        public static GameObject CreateViewPrefab(string _viewName)
        {
            string prefabPath = $"View/{_viewName}/{_viewName}";
            if (Resources.Load<GameObject>(prefabPath))
            {
                EditorUtility.DisplayDialog("错误", $"已存在同名预制体--->{prefabPath}", "ok");
                return null;
            }
            const string TemplateViewPath = "View/TemplateView/TemplateView";
            GameObject originPrefab = Resources.Load<GameObject>(TemplateViewPath);
            if (originPrefab == null)
            {
                EditorUtility.DisplayDialog("错误", $"该路径下找不到预制体TemplateView.prefab--->{TemplateViewPath}", "ok");
                return null;
            }
            GameObject view = PrefabUtility.InstantiatePrefab(originPrefab) as GameObject;
            PrefabUtility.UnpackPrefabInstance(view, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            view.name = _viewName;
            view.transform.SetAsLastSibling();
            Canvas canvas = view.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            CanvasScaler canvasScaler = view.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            AddTempFileLabels(fieldNeedExportViewUI, _viewName);
            return view;
        }

        static GameObject AddScriptAndSavePrefab(string _viewName, GameObject _view)
        {
            Type type = null;
            //遍历所有程序集
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type _type = assembly.GetType($"{_viewName}");
                if (_type != null)
                {
                    type = _type;
                }
            }
            if (type == null)
            {
                Debug.Log($"添加组件{_viewName}失败", _view);
            }
            _view.AddComponent(type);

            string sceneName = EditorSceneManager.GetActiveScene().name;
            GameObject viewGo = GameObject.Find(sceneName);
            string dirPath = Application.dataPath + $"/AssetBundles/Art/Resources/View/{_viewName}/";
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

        /// <summary>
        /// 创建脚本
        /// </summary>
        /// <param name="_viewName"></param>
        /// <returns></returns>
        public static bool CreateViewScript(ViewConfig _viewConfig)
        {
            string _viewName = _viewConfig.viewName;
            string dirPath = Path.Combine(Application.dataPath, "HotUpdateScripts/UI/View", _viewConfig.group, _viewName);
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            string viewPath = Path.Combine(dirPath, $"{_viewName}.cs");
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
        public static bool CheckRegular(ViewConfig _viewConfig)
        {
            if (_viewConfig == null)
            {
                EditorUtility.DisplayDialog("错误", "找不view到配置", "ok");
                return false;
            }
            foreach (char item in _viewConfig.viewName)
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
            return Application.dataPath + "/RainFramework/Editor/UI/AutoCreator/AutoCreateViewTemplate.txt";
        }
    }
}