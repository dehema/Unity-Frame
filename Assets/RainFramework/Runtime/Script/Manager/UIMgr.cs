using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

namespace Rain.UI
{
    public class UIMgr : MonoBehaviour
    {
        public UIViewConfig allViewConfig;
        Dictionary<string, BaseView> allView = new Dictionary<string, BaseView>();
        Dictionary<string, BaseView> allShowView = new Dictionary<string, BaseView>();
        Dictionary<string, Transform> layerRoots = new Dictionary<string, Transform>();
        static UIMgr _ins;
        public const int _viewOrderInLayerInterval = 5; //每个视图的间隔
        public const int _layerInterval = 400; //每个层级之间的间隔
        const int _layerMax = 32767;
        public const string uiPrefabPath = "Prefab/View/";
        public static UIMgr Ins
        {
            get
            {
                if (_ins == null)
                {
                    GameObject obj = new GameObject(typeof(UIMgr).Name);
                    _ins = obj.AddComponent<UIMgr>();
                    DontDestroyOnLoad(obj);
                }
                return _ins;
            }
        }

        private void Awake()
        {
            allViewConfig = ConfigMgr.Ins.LoadUIConfig();
            InitLayerRoot();
            Utility.Log("读取到allViewConfig\n" + Newtonsoft.Json.JsonConvert.SerializeObject(allViewConfig, Newtonsoft.Json.Formatting.Indented));
        }

        /// <summary>
        /// 生成层级节点 所有视图的父物体
        /// </summary>
        public void InitLayerRoot()
        {
            foreach (var item in allViewConfig.layer)
            {
                GameObject go = Tools.Ins.Create2DGo(item.Key, transform);
                layerRoots[item.Key] = go.transform;
            }
        }

        public T OpenView<T>(params object[] _params) where T : BaseView
        {
            string viewName = typeof(T).ToString();
            return OpenView(viewName, _params) as T;
        }

        /// <summary>
        /// 打开指定名称的UI视图
        /// </summary>
        /// <param name="_viewName">视图名称</param>
        /// <param name="_params">传递给视图的参数</param>
        /// <returns>打开的UI视图</returns>
        public BaseUI OpenView(string _viewName, params object[] _params)
        {
            // 尝试获取已存在的视图
            BaseView view = null;
            allView.TryGetValue(_viewName, out view);

            // 如果视图已存在且正在显示，则直接返回
            if (view != null && view.IsShow)
            {
                return view;
            }

            // 如果视图不存在，则创建新视图
            if (view == null)
            {
                try
                {
                    // 构建预制体路径并加载
                    string prefabPath = $"{uiPrefabPath}{_viewName}/{_viewName}";
                    GameObject prefab = Resources.Load<GameObject>(prefabPath);
                    
                    if (prefab == null)
                    {
                        Debug.LogError($"无法加载UI预制体: {prefabPath}");
                        return null;
                    }
                    
                    // 实例化并设置视图
                    GameObject viewGo = Instantiate(prefab);
                    viewGo.name = _viewName;
                    
                    view = viewGo.GetComponent<BaseView>();
                    if (view == null)
                    {
                        Debug.LogError($"UI预制体 {_viewName} 没有挂载BaseView组件");
                        Destroy(viewGo);
                        return null;
                    }
                    
                    // 设置视图配置和父节点
                    if (allViewConfig.view.TryGetValue(_viewName, out var config))
                    {
                        view.viewConfig = config;
                        view.transform.SetParent(layerRoots[view.viewConfig.layer]);
                    }
                    else
                    {
                        Debug.LogWarning($"未找到视图 {_viewName} 的配置");
                    }
                    
                    // 添加到视图字典
                    allView[_viewName] = view;
                    view.Init(_params);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"创建UI视图 {_viewName} 时发生错误: {e.Message}");
                    return null;
                }
            }
            else
            {
                // 如果视图已存在但未显示，则激活它
                view.gameObject.SetActive(true);
            }

            // 设置排序顺序和层级
            view.canvas.sortingOrder = _layerMax;
            view.transform.SetAsLastSibling();
            
            // 添加到显示中的视图字典
            allShowView[_viewName] = view;
            
            // 刷新所有视图层级
            RefreshAllViewLayer();
            
            // 调用视图的打开方法
            view.OnOpen(_params);
            
            return view;
        }

        public void CloseView<T>() where T : BaseView
        {
            string viewName = typeof(T).ToString();
            CloseView(viewName);
        }

        public void CloseView(string _viewName)
        {
            //Utility.Log("UIMgr.关闭UI:" + _viewName);
            if (!allShowView.ContainsKey(_viewName))
            {
                //Utility.Log("重复关闭UI:" + _viewName);
                return;
            }
            GameObject view = allView[_viewName].gameObject;
            view.name = _viewName;
            BaseView t = view.GetComponent<BaseView>();
            allShowView.Remove(_viewName);
            t.OnClose(() =>
            {
                t.gameObject.SetActive(false);
                //Timer.Ins.SetTimeOut(RefreshMouseModel, 0.5f);
            });
        }

        public T GetView<T>() where T : BaseView
        {
            string viewName = typeof(T).ToString();
            return GetView(viewName) as T;
        }

        public BaseView GetView(string _viewName)
        {
            if (allView.ContainsKey(_viewName))
            {
                return allView[_viewName];
            }
            return null;
        }

        public List<BaseView> GetAllViewInLayer(string _layer)
        {
            List<BaseView> viewList = new List<BaseView>();
            foreach (var item in allView)
            {
                ViewConfigModel config = GetViewConfig(item.Key);
                if (config.layer == _layer)
                {
                    viewList.Add(item.Value);
                }
            }
            return viewList;
        }

        /// <summary>
        /// 对所有UI的orderInLayer从新排序
        /// </summary>
        public void RefreshAllViewLayer()
        {
            List<List<BaseView>> views = new List<List<BaseView>>();
            Dictionary<string, int> layerIndexDict = new Dictionary<string, int>();
            //从配置中遍历所有的UI
            foreach (var layer in allViewConfig.layer)
            {
                views.Add(new List<BaseView>());
                layerIndexDict[layer.Key] = layerIndexDict.Count;
            }
            //遍历所有打开的UI
            foreach (var view in allShowView)
            {
                views[layerIndexDict[view.Value.viewConfig.layer]].Add(view.Value);
            }
            foreach (List<BaseView> item in views)
            {
                item.Sort((a, b) => { return a.canvas.sortingOrder < b.canvas.sortingOrder ? -1 : 1; });
            }
            foreach (List<BaseView> item in views)
            {
                int _layer = 0;
                if (item.Count > 0)
                {
                    _layer = allViewConfig.layer[item[0].viewConfig.layer].order * _layerInterval;
                }
                for (int i = 0; i < item.Count; i++)
                {
                    BaseView baseView = item[i];
                    baseView.canvas.sortingOrder = _layer + i * _viewOrderInLayerInterval;
                }
            }
        }

        public ViewConfigModel GetViewConfig(string _viewName)
        {
            return allViewConfig.view[_viewName];
        }

        /// <summary>
        /// 设置阻挡UI
        /// </summary>
        /// <param name="show"></param>
        public void SetBlockUI(bool _show)
        {
            if (_show)
            {
                OpenView<BlockView>();
            }
            else
            {
                CloseView<BlockView>();
            }
        }

        /// <summary>
        /// 根据层级获取显示的视图
        /// </summary>
        /// <param name="_layer"></param>
        /// <returns></returns>
        public List<BaseView> GetShowViewsByLayer(string _layer)
        {
            List<BaseView> list = new List<BaseView>();
            foreach (var item in allShowView)
            {
                if (item.Value.viewConfig.layer == _layer)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// 是否显示
        /// </summary>
        /// <returns></returns>
        public bool IsShow<T>() where T : BaseView
        {
            T t = GetView<T>();
            return t != null && t.isActiveAndEnabled;
        }
    }
}