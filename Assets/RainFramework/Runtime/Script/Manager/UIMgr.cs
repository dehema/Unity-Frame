using System;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

namespace Rain.UI
{
    /// <summary>
    /// UI管理器，负责UI视图的创建、显示、隐藏和层级管理
    /// </summary>
    public class UIMgr : ModuleSingletonMono<UIMgr>, IModule
    {
        /// <summary>UI视图配置</summary>
        private UIViewConfig _uiViewConfig;

        /// <summary>所有已创建的视图字典，键为视图名称</summary>
        private Dictionary<string, BaseView> _allView = new Dictionary<string, BaseView>();

        /// <summary>各层级的根节点字典，键为层级名称</summary>
        private Dictionary<string, Transform> _layerRoots = new Dictionary<string, Transform>();

        /// <summary>每个视图在同一层级内的排序间隔</summary>
        public const int VIEW_ORDER_IN_LAYER_INTERVAL = 5;

        /// <summary>不同层级之间的排序间隔</summary>
        public const int LAYER_INTERVAL = 400;

        /// <summary>最大排序值</summary>
        private const int LAYER_MAX = 32767;

        /// <summary>
        /// 初始化UI管理器
        /// </summary>
        /// <param name="uiViewConfig">UI视图配置数据</param>
        public void Init(UIViewConfig uiViewConfig)
        {
            _uiViewConfig = uiViewConfig;
            InitLayerRoot();
        }

        /// <summary>
        /// 初始化所有UI层级的根节点
        /// </summary>
        public void InitLayerRoot()
        {
            if (_uiViewConfig == null || _uiViewConfig.layer == null)
            {
                Debug.LogError("UI层级配置为空，无法初始化层级根节点");
                return;
            }

            foreach (var item in _uiViewConfig.layer)
            {
                GameObject go = Tools.Ins.Create2DGo(item.Key, transform);
                _layerRoots[item.Key] = go.transform;
            }
        }

        /// <summary>
        /// 通过泛型打开UI视图
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <param name="_params">传递给视图的参数</param>
        /// <returns>打开的UI视图实例</returns>
        public T OpenView<T>(IViewParams viewParams = null) where T : BaseView
        {
            string viewName = typeof(T).ToString();
            return OpenView(viewName, viewParams) as T;
        }

        /// <summary>
        /// 打开指定名称的UI视图
        /// </summary>
        /// <param name="_viewName">视图名称</param>
        /// <param name="viewParams">传递给视图的参数</param>
        /// <returns>打开的UI视图</returns>
        public BaseView OpenView(string _viewName, IViewParams viewParams = null)
        {
            if (string.IsNullOrEmpty(_viewName))
            {
                Debug.LogError("视图名称不能为空");
                return null;
            }

            // 尝试获取已存在的视图
            BaseView view = null;
            _allView.TryGetValue(_viewName, out view);

            // 如果视图已存在且正在显示，则直接返回
            if (view != null && view.IsShow)
            {
                // 如果视图已经打开，可以考虑刷新参数
                view.OnOpen(viewParams);
                return view;
            }

            // 获取视图配置
            ViewConfig viewConfig = GetViewConfig(_viewName);
            if (viewConfig == null)
            {
                Debug.LogError($"无法获取视图配置: {_viewName}");
                return null;
            }

            // 如果视图不存在，则创建新视图
            if (view == null)
            {
                // 构建预制体路径并加载
                string prefabPath = $"View/{_viewName}/{_viewName}";
                GameObject prefab = Resources.Load<GameObject>(prefabPath);

                if (prefab == null)
                {
                    Debug.LogError($"无法加载UI预制体: {prefabPath}");
                    return null;
                }

                // 实例化并设置视图
                GameObject viewGo = Instantiate(prefab);
                view = AddView(viewGo, viewConfig, viewParams);

                if (view == null)
                {
                    return null;
                }
            }
            else
            {
                // 如果视图已存在但未显示，则激活它
                view.gameObject.SetActive(true);
            }

            // 设置排序顺序和层级
            view.canvas.sortingOrder = LAYER_MAX;
            view.transform.SetAsLastSibling();

            // 刷新所有视图层级
            RefreshViewLayer(viewConfig.layer);

            // 调用视图的打开方法
            view.OnOpen(viewParams);

            return view;
        }

        /// <summary>
        /// 获取指定视图的配置信息
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns>视图配置，如果不存在则返回null</returns>
        public ViewConfig GetViewConfig(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                Debug.LogError("视图名称不能为空");
                return null;
            }
            if (_uiViewConfig.view.ContainsKey(viewName))
            {
                return _uiViewConfig.view[viewName];
            }
            else
            {
                Debug.LogWarning($"未找到视图 {viewName} 的配置");
                return null;
            }
        }

        /// <summary>
        /// 添加视图到管理器中
        /// </summary>
        /// <param name="viewGo">视图游戏对象</param>
        /// <param name="viewConfig">视图配置</param>
        /// <param name="viewParams">初始化参数</param>
        /// <returns>添加的视图实例</returns>
        private BaseView AddView(GameObject viewGo, ViewConfig viewConfig, IViewParams viewParams = null)
        {
            if (viewGo == null || viewConfig == null)
            {
                Debug.LogError("视图游戏对象或配置为空");
                return null;
            }

            // 获取并验证视图组件
            BaseView view = viewGo.GetComponent<BaseView>();
            if (view == null)
            {
                Debug.LogError($"UI预制体 {viewConfig.viewName} 没有挂载BaseView组件");
                return null;
            }

            // 设置视图配置
            view.viewConfig = viewConfig;

            // 设置父节点
            if (!_layerRoots.ContainsKey(viewConfig.layer))
            {
                Debug.LogError($"UI预制体 {viewConfig.viewName} 找不到对应的layer {viewConfig.layer}");
                return null;
            }

            // 设置父节点并保持缩放
            view.transform.SetParent(_layerRoots[viewConfig.layer], false);

            // 添加到视图字典
            _allView[viewConfig.viewName] = view;

            // 初始化视图
            view.Init(viewParams);
            return view;
        }

        /// <summary>
        /// 通过泛型关闭UI视图
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        public void CloseView<T>() where T : BaseView
        {
            string viewName = typeof(T).ToString();
            CloseView(viewName);
        }

        /// <summary>
        /// 关闭指定名称的UI视图
        /// </summary>
        /// <param name="_viewName">视图名称</param>
        public void CloseView(string _viewName)
        {
            // 检查视图是否存在
            if (string.IsNullOrEmpty(_viewName))
            {
                Debug.LogWarning("关闭UI时视图名称为空");
                return;
            }

            if (!_allView.ContainsKey(_viewName))
            {
                Debug.LogWarning($"尝试关闭不存在的UI: {_viewName}");
                return;
            }

            BaseView baseView = _allView[_viewName];
            GameObject viewGo = baseView.gameObject;

            // 检查视图是否已经关闭
            if (!viewGo.activeSelf)
            {
                Debug.LogWarning($"重复关闭UI: {_viewName}");
                return;
            }

            // 调用关闭回调
            baseView.OnClose(() =>
            {
                baseView.gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// 获取指定类型的UI视图
        /// </summary>
        /// <typeparam name="T">视图类型</typeparam>
        /// <returns>UI视图实例</returns>
        public T GetView<T>() where T : BaseView
        {
            string viewName = typeof(T).ToString();
            return GetView(viewName) as T;
        }

        public BaseView GetView(string _viewName)
        {
            if (_allView.ContainsKey(_viewName))
            {
                return _allView[_viewName];
            }
            return null;
        }

        public List<BaseView> GetAllViewInLayer(string _layer)
        {
            List<BaseView> viewList = new List<BaseView>();
            foreach (var item in _allView)
            {
                ViewConfig config = GetViewConfig(item.Key);
                if (config.layer == _layer)
                {
                    viewList.Add(item.Value);
                }
            }
            return viewList;
        }

        /// <summary>
        /// 对所有UI的orderInLayer重新排序
        /// </summary>
        public void RefreshAllViewLayer()
        {
            // 初始化层级数据结构
            var layerGroups = new Dictionary<string, List<BaseView>>(_uiViewConfig.layer.Count);
            
            // 初始化每个层级的列表
            foreach (var layer in _uiViewConfig.layer)
            {
                layerGroups[layer.Key] = new List<BaseView>();
            }
            
            // 收集所有显示中的UI
            foreach (var view in _allView.Values)
            {
                if (view.IsShow && layerGroups.TryGetValue(view.viewConfig.layer, out var layerList))
                {
                    layerList.Add(view);
                }
            }
            
            // 对每个层级的UI进行排序并设置排序顺序
            foreach (var kvp in layerGroups)
            {
                RefreshViewLayerOrder(kvp.Key, kvp.Value);
            }
        }
        
        /// <summary>
        /// 对指定层级的UI进行重新排序
        /// </summary>
        /// <param name="layerKey">层级名称</param>
        public void RefreshViewLayer(string layerKey)
        {
            // 验证层级是否存在
            if (!_uiViewConfig.layer.ContainsKey(layerKey))
            {
                Debug.LogWarning($"尝试刷新不存在的UI层级: {layerKey}");
                return;
            }
            
            // 收集指定层级的所有显示中的UI
            List<BaseView> views = new List<BaseView>();
            foreach (var view in _allView.Values)
            {
                if (view.IsShow && view.viewConfig.layer == layerKey)
                {
                    views.Add(view);
                }
            }
            
            // 对收集到的UI进行排序
            RefreshViewLayerOrder(layerKey, views);
        }
        
        /// <summary>
        /// 对指定层级的UI列表进行排序并设置排序顺序
        /// </summary>
        /// <param name="layerKey">层级名称</param>
        /// <param name="views">该层级的UI列表</param>
        private void RefreshViewLayerOrder(string layerKey, List<BaseView> views)
        {
            // 如果该层没有可见UI，跳过处理
            if (views == null || views.Count == 0) return;
            
            // 按现有排序顺序排序
            views.Sort((a, b) => a.canvas.sortingOrder.CompareTo(b.canvas.sortingOrder));
            
            // 计算基础层级值
            int baseLayerOrder = _uiViewConfig.layer[layerKey].order * LAYER_INTERVAL;
            
            // 设置新的排序顺序
            for (int i = 0; i < views.Count; i++)
            {
                views[i].canvas.sortingOrder = baseLayerOrder + i * VIEW_ORDER_IN_LAYER_INTERVAL;
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
            foreach (var item in _allView)
            {
                if (item.Value.IsShow && item.Value.viewConfig.layer == _layer)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取所有界面
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, BaseView> GetAllView()
        {
            return _allView;
        }

        /// <summary>
        /// 界面是否显示
        /// </summary>
        /// <returns></returns>
        public bool IsShow<T>() where T : BaseView
        {
            T t = GetView<T>();
            return t != null && t.isActiveAndEnabled;
        }

        public void OnInit(object createParam)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnLateUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnTermination()
        {
        }

        // 异步加载，使用枚举作为参数
        public UILoader OpenViewAsync<T>(IViewParams viewParams = null, Action onComplete = null) where T : BaseView
        {
            string viewName = typeof(T).ToString();
            return OpenViewAsync(viewName, viewParams, onComplete);
        }

        // 异步加载，使用id作为参数
        public UILoader OpenViewAsync(string viewName, IViewParams viewParams = null, Action onComplete = null)
        {
            ViewConfig viewConfig = GetViewConfig(viewName);
            return LoadAsync(viewConfig, viewParams);
        }


        private UILoader LoadAsync(ViewConfig viewConfig, IViewParams viewParams = null, Action onComplete = null)
        {
            viewConfig.uiLoader = new UILoader();
            viewConfig.uiLoader.SetOnCompleted(onComplete);
            AssetMgr.Ins.LoadAsync<GameObject>(viewConfig.viewName, (res) =>
            {
                GameObject viewGo = Instantiate(res);
                AddView(viewGo, viewConfig, viewParams);
                viewConfig.uiLoader.UILoadSuccess();
            });
            return viewConfig.uiLoader;
        }
    }
}