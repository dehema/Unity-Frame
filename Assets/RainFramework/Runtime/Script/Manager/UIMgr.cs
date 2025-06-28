using System;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

namespace Rain.UI
{
    public class UIMgr : ModuleSingletonMono<UIMgr>, IModule
    {
        private UIViewConfig _uiViewConfig;
        private Dictionary<string, BaseView> _allView = new Dictionary<string, BaseView>();
        private Dictionary<string, Transform> layerRoots = new Dictionary<string, Transform>();
        public const int _viewOrderInLayerInterval = 5; //每个视图的间隔
        public const int _layerInterval = 400; //每个层级之间的间隔
        const int _layerMax = 32767;
        public const string uiPrefabPath = "Prefab/View/";

        public void Init(UIViewConfig uiViewConfig)
        {
            _uiViewConfig = uiViewConfig;
            InitLayerRoot();
        }

        /// <summary>
        /// 生成层级节点 所有视图的父物体
        /// </summary>
        public void InitLayerRoot()
        {
            foreach (var item in _uiViewConfig.layer)
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
            _allView.TryGetValue(_viewName, out view);

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

                    ViewConfig viewConfig = GetViewConfig(_viewName);

                    AddView(viewGo, viewConfig, _params);
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
                view.OnOpen(_params);
            }

            // 设置排序顺序和层级
            view.canvas.sortingOrder = _layerMax;
            view.transform.SetAsLastSibling();

            // 刷新所有视图层级
            RefreshAllViewLayer();

            // 调用视图的打开方法
            view.OnOpen(_params);

            return view;
        }

        /// <summary>
        /// 获取UI配置
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public ViewConfig GetViewConfig(string viewName)
        {
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

        private BaseView AddView(GameObject viewGo, ViewConfig viewConfig, params object[] _params)
        {
            // 实例化并设置视图
            BaseView view = viewGo.GetComponent<BaseView>();
            if (view == null)
            {
                Debug.LogError($"UI预制体 {viewConfig.viewName} 没有挂载BaseView组件");
                Destroy(viewGo);
                return null;
            }

            view.viewConfig = viewConfig;
            if (!layerRoots.ContainsKey(viewConfig.layer))
            {
                Debug.LogError($"UI预制体 {viewConfig.viewName} 找不到对应的layer {viewConfig.layer}");
            }
            view.transform.SetParent(layerRoots[viewConfig.layer]);

            // 添加到视图字典
            _allView[viewConfig.viewName] = view;
            view.Init(_params);
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
            if (!_allView.ContainsKey(_viewName))
            {
                //Utility.Log(_viewName + "不存在");
                return;
            }
            GameObject view = _allView[_viewName].gameObject;
            view.name = _viewName;
            BaseView baseView = view.GetComponent<BaseView>();
            if (!view.activeSelf)
            {

                //Utility.Log("重复关闭UI:" + _viewName);
                return;
            }
            baseView.OnClose(() =>
            {
                baseView.gameObject.SetActive(false);
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
            List<List<BaseView>> views = new List<List<BaseView>>();
            Dictionary<string, int> layerIndexDict = new Dictionary<string, int>();
            //从配置中遍历所有的UI
            foreach (var layer in _uiViewConfig.layer)
            {
                views.Add(new List<BaseView>());
                layerIndexDict[layer.Key] = layerIndexDict.Count;
            }
            //遍历所有打开的UI
            foreach (var view in _allView)
            {
                if (view.Value.IsShow)
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
                    _layer = _uiViewConfig.layer[item[0].viewConfig.layer].order * _layerInterval;
                }
                for (int i = 0; i < item.Count; i++)
                {
                    BaseView baseView = item[i];
                    baseView.canvas.sortingOrder = _layer + i * _viewOrderInLayerInterval;
                }
            }
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
        public UILoader OpenViewAsync<T>(object[] _params = null, Action onComplete = null) where T : BaseView
        {
            string viewName = typeof(T).ToString();
            return OpenViewAsync(viewName, _params, onComplete);
        }

        // 异步加载，使用id作为参数
        public UILoader OpenViewAsync(string viewName, object[] _params = null, Action onComplete = null)
        {
            ViewConfig viewConfig = GetViewConfig(viewName);
            return LoadAsync(viewConfig, _params);
        }


        private UILoader LoadAsync(ViewConfig viewConfig, object[] _params, Action onComplete = null)
        {
            viewConfig.uiLoader = new UILoader();
            viewConfig.uiLoader.SetOnCompleted(onComplete);
            AssetManager.Ins.LoadAsync<GameObject>(viewConfig.viewName, (res) =>
            {
                GameObject viewGo = Instantiate(res);
                AddView(viewGo, viewConfig, _params);
                viewConfig.uiLoader.UILoadSuccess();
            });
            return viewConfig.uiLoader;
        }
    }
}