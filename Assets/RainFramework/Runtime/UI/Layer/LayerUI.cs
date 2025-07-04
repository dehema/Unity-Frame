using System;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.UI
{
    public class LayerUI : MonoBehaviour
    {
        protected Dictionary<string, ViewParams> uiViews = new Dictionary<string, ViewParams>();
        protected Dictionary<string, ViewParams> uiCache = new Dictionary<string, ViewParams>();
        
        private Canvas _canvas;
        private CanvasScaler _canvasScaler;
        public CanvasScaler CanvasScaler => _canvasScaler;
        private GraphicRaycaster _graphicRaycaster;
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster;
        
        private void Awake()
        {
            // 获取组件
            _canvas = gameObject.AddComponent<Canvas>();
            _canvasScaler = gameObject.AddComponent<CanvasScaler>();
            _graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();
        }

        public void Init(int sortOrder = 0, string sortingLayerName = "Default", RenderMode renderMode = RenderMode.ScreenSpaceOverlay, bool pixelPerfect = false, UnityEngine.Camera camera = null)
        {
            _canvas.renderMode = renderMode;
            _canvas.worldCamera = camera;
            _canvas.sortingOrder = sortOrder;
            _canvas.sortingLayerName = sortingLayerName;
            _canvas.pixelPerfect = pixelPerfect;
        }
        
        public void SetCanvasScaler(CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPixelSize,
            float scaleFactor = 1f,
            float referencePixelsPerUnit = 100f)
        {
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.scaleFactor = scaleFactor;
            _canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        }
        
        public void SetCanvasScaler(CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize,
            Vector2? referenceResolution = null,
            CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight,
            float matchWidthOrHeight = 0f,
            float referencePixelsPerUnit = 100f)
        {
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.referenceResolution = referenceResolution ?? new Vector2(800, 600);;
            _canvasScaler.screenMatchMode = screenMatchMode;
            _canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
            _canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        }
        
        public void SetCanvasScaler(CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize,
            CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points,
            float fallbackScreenDPI = 96f,
            float defaultSpriteDPI = 96f,
            float referencePixelsPerUnit = 100f)
        {
            _canvasScaler.uiScaleMode = scaleMode;
            _canvasScaler.physicalUnit = physicalUnit;
            _canvasScaler.fallbackScreenDPI = fallbackScreenDPI;
            _canvasScaler.defaultSpriteDPI = defaultSpriteDPI;
            _canvasScaler.referencePixelsPerUnit = referencePixelsPerUnit;
        }

        public string Add(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;
            var guid = Guid.NewGuid().ToString();

            if (IsDuplicateLoad(prefabPath, out var viewParams))
            {
                return string.Empty;
            }

            viewParams = GetOrCreateViewParams(prefabPath, guid);

            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = callbacks;
            viewParams.Valid = true;

            Load(viewParams);

            return guid;
        }

        public UILoader AddAsync(int uiId, UIConfig config, object[] parameters = null, UICallbacks callbacks = null)
        {
            var prefabPath = config.AssetName;
            var guid = Guid.NewGuid().ToString();

            if (IsDuplicateLoad(prefabPath, out var viewParams))
            {
                return viewParams.UILoader;
            }

            viewParams = GetOrCreateViewParams(prefabPath, guid);

            viewParams.UIid = uiId;
            viewParams.Params = parameters;
            viewParams.Callbacks = callbacks;
            viewParams.Valid = true;

            return LoadAsync(viewParams);
        }

        protected bool IsDuplicateLoad(string prefabPath, out ViewParams viewParams)
        {
            if (uiViews.TryGetValue(prefabPath, out viewParams) && viewParams.Valid)
            {
                RLog.LogView($"UI重复加载：{prefabPath}");
                return true;
            }
            return false;
        }

        protected ViewParams GetOrCreateViewParams(string prefabPath, string guid)
        {
            if (!uiViews.TryGetValue(prefabPath, out var viewParams))
            {
                if (!uiCache.TryGetValue(prefabPath, out viewParams))
                {
                    viewParams = new ViewParams();
                }
                viewParams.Guid = guid;
                viewParams.PrefabPath = prefabPath;
                uiViews.Add(viewParams.PrefabPath, viewParams);
            }
            return viewParams;
        }

        protected void Load(ViewParams viewParams)
        {
            var vp = uiCache.GetValueOrDefault(viewParams.PrefabPath);
            if (vp != null && vp.Go != null)
            {
                CreateNode(vp);
            }
            else
            {
                GameObject res = AssetMgr.Ins.Load<GameObject>(viewParams.PrefabPath);
                GameObject childNode = Instantiate(res, gameObject.transform, false);
                childNode.name = viewParams.PrefabPath;
                viewParams.Go = childNode;
                
                DelegateComponent comp = childNode.AddComponent<DelegateComponent>();
                viewParams.DelegateComponent = comp;
                viewParams.BaseView = childNode.GetComponent<F8BaseView>();
                comp.ViewParams = viewParams;
                
                viewParams.UILoader = new UILoader();
                viewParams.UILoader.Guid = viewParams.Guid;
                CreateNode(viewParams);
                viewParams.UILoader.UILoadSuccess();
            }
        }

        protected UILoader LoadAsync(ViewParams viewParams)
        {
            var vp = uiCache.GetValueOrDefault(viewParams.PrefabPath);
            if (vp != null && vp.Go != null)
            {
                return CreateNode(vp);
            }
            else
            {
                viewParams.UILoader = new UILoader();
                viewParams.UILoader.Guid = viewParams.Guid;
                AssetMgr.Ins.LoadAsync<GameObject>(viewParams.PrefabPath, (res) =>
                {
                    GameObject childNode = Instantiate(res, gameObject.transform, false);
                    childNode.name = viewParams.PrefabPath;
                    viewParams.Go = childNode;
                
                    DelegateComponent comp = childNode.AddComponent<DelegateComponent>();
                    viewParams.DelegateComponent = comp;
                    viewParams.BaseView = childNode.GetComponent<F8BaseView>();
                    comp.ViewParams = viewParams;
                    CreateNode(viewParams);
                    viewParams.UILoader.UILoadSuccess();
                });
                
                return viewParams.UILoader;
            }
        }
        
        public UILoader CreateNode(ViewParams viewParams)
        {
            UIManager.Ins.GetCurrentUIids().Add(viewParams.UIid);
            
            viewParams.Valid = true;

            var comp = viewParams.DelegateComponent;
            viewParams.Go.transform.SetParent(gameObject.transform, false);
            if (viewParams.Go.activeSelf == false)
            {
                viewParams.Go.SetActive(true);
            }
            
            comp.Add();
            
            return viewParams.UILoader;
        }

        public void Close(string prefabPath, bool isDestroy)
        {
            if (isDestroy)
            {
                RemoveCache(prefabPath);
            }
            
            var children = GetChildrens();
            foreach (var comp in children)
            {
                var viewParams = comp.ViewParams;
                if (viewParams.PrefabPath == prefabPath)
                {
                    uiViews.Remove(viewParams.PrefabPath);
                    if (!isDestroy)
                    {
                        uiCache[viewParams.PrefabPath] = viewParams;
                    }
                    comp.Remove(isDestroy);
                    viewParams.Valid = false;
                }
            }
        }

        protected void RemoveCache(string prefabPath)
        {
            if (uiCache.TryGetValue(prefabPath, out var viewParams))
            {
                uiViews.Remove(viewParams.PrefabPath);
                uiCache.Remove(prefabPath);
                var childNode = viewParams.Go;
                Destroy(childNode);
            }
        }

        public GameObject GetByGuid(string guid)
        {
            var children = GetChildrens();
            foreach (var comp in children)
            {
                if (comp.ViewParams != null && comp.ViewParams.Guid == guid)
                {
                    return comp.gameObject;
                }
            }

            return null;
        }

        public List<GameObject> GetByUIid(int uiid)
        {
            List<GameObject> nodeList = null;
            var children = GetChildrens();
    
            foreach (var comp in children)
            {
                if (comp.ViewParams != null && comp.ViewParams.UIid == uiid)
                {
                    if (nodeList == null)
                    {
                        nodeList = new List<GameObject>();
                    }
                    nodeList.Add(comp.gameObject);
                }
            }

            return nodeList;
        }

        public bool Has(string prefabPathOrGuid)
        {
            var children = GetChildrens();
            foreach (var comp in children)
            {
                if (comp.ViewParams.Guid == prefabPathOrGuid || comp.ViewParams.PrefabPath == prefabPathOrGuid)
                {
                    return true;
                }
            }

            return false;
        }
        
        protected List<DelegateComponent> GetChildrens()
        {
            var result = new List<DelegateComponent>();
            var children = gameObject.transform.childCount;
            for (var i = 0; i < children; i++)
            {
                var comp = gameObject.transform.GetChild(i).GetComponent<DelegateComponent>();
                if (comp != null && comp.ViewParams != null && comp.ViewParams.Valid && comp.isActiveAndEnabled)
                {
                    result.Add(comp);
                }
            }

            return result;
        }

        public void Clear(bool isDestroy)
        {
            if (isDestroy)
            {
                foreach (var value in uiViews.Values)
                {
                    var comp = value.DelegateComponent;
                    comp.Remove(true);
                    value.Valid = false;
                }
                
                foreach (var value in uiCache.Values)
                {
                    var childNode = value.Go;
                    Destroy(childNode);
                }
                uiCache.Clear();
            }
            else
            {
                foreach (var value in uiViews.Values)
                {
                    uiCache[value.PrefabPath] = value;
                    var comp = value.DelegateComponent;
                    comp.Remove(false);
                    value.Valid = false;
                }
            }
            
            uiViews.Clear();
        }
    }
}
