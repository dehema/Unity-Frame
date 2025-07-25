using System;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rain.UI
{
    public enum LayerType : byte
    {
        Game,
        UI,
        PopUp,
        Dialog,// 模态弹窗（只显示最老的窗口，关闭后自动显示下一个新窗口）
        Notify,// 非模态弹窗（老窗口和新窗口共存，新的显示在前，自己管理窗口关闭）
        Guide
    }

    public struct UIConfig
    {
        public LayerType Layer;
        public string AssetName;
        
        public UIConfig(LayerType layer, string assetName)
        {
            Layer = layer;
            AssetName = assetName;
        }
    }

    public class UIManager : ModuleSingletonMono<UIManager>, IModule
    {
        private LayerGame _layerGame;
        private LayerUI _layerUI;
        private LayerPopUp _layerPopUp;
        private LayerDialog _layerDialog;
        private LayerNotify _layerNotify;
        private LayerGuide _layerGuide;

        private Dictionary<int, UIConfig> _configs = new Dictionary<int, UIConfig>();
        private List<int> _currentUIids = new List<int>();
        
        // 将所有的层放入一个字典中
        private Dictionary<LayerType, LayerUI> _layers;

        // 使用枚举作为参数
        public void Initialize<T>(Dictionary<T, UIConfig> configs) where T : Enum
        {
            Dictionary<int, UIConfig> intConfigs = new Dictionary<int, UIConfig>();
            
            foreach (var kvp in configs)
            {
                intConfigs.Add((int)(object)kvp.Key, kvp.Value);
            }
            
            Initialize(intConfigs);
        }
        
        public void Initialize(Dictionary<int, UIConfig> configs)
        {
            _configs = configs;
            
            GameObject gameGo = new GameObject("LayerGame");
            GameObject uiGo = new GameObject("LayerUI");
            GameObject popupGo = new GameObject("LayerPopUp");
            GameObject dialogGo = new GameObject("LayerDialog");
            GameObject notifyGo = new GameObject("LayerNotify");
            GameObject guideGo = new GameObject("LayerGuide");

            gameGo.SetParent(transform);
            uiGo.SetParent(transform);
            popupGo.SetParent(transform);
            dialogGo.SetParent(transform);
            notifyGo.SetParent(transform);
            guideGo.SetParent(transform);
            
            _layerGame = gameGo.AddComponent<LayerGame>();
            _layerUI = uiGo.AddComponent<LayerUI>();
            _layerPopUp = popupGo.AddComponent<LayerPopUp>();
            _layerDialog = dialogGo.AddComponent<LayerDialog>();
            _layerNotify = notifyGo.AddComponent<LayerNotify>();
            _layerGuide = guideGo.AddComponent<LayerGuide>();
            
            _layerGame.Init(100);
            _layerUI.Init(200);
            _layerPopUp.Init(300);
            _layerDialog.Init(400);
            _layerNotify.Init(500);
            _layerGuide.Init(600);
            
            _layers = new Dictionary<LayerType, LayerUI>
            {
                { LayerType.Game, _layerGame },
                { LayerType.UI, _layerUI },
                { LayerType.PopUp, _layerPopUp },
                { LayerType.Dialog, _layerDialog },
                { LayerType.Notify, _layerNotify },
                { LayerType.Guide, _layerGuide }
            };
        }
        
        public void SetCanvas(LayerType? layer = null, int sortOrder = 0, string sortingLayerName = "Default", RenderMode renderMode = RenderMode.ScreenSpaceOverlay, bool pixelPerfect = false, UnityEngine.Camera camera = null)
        {
            // 如果 layer 为 null，修改所有层
            if (layer == null)
            {
                foreach (var canvasLayer in _layers.Values)
                {
                    canvasLayer.Init(sortOrder, sortingLayerName, renderMode, pixelPerfect, camera);
                }
            }
            else if (_layers.ContainsKey(layer.Value))  // 否则只修改指定层
            {
                _layers[layer.Value].Init(sortOrder, sortingLayerName, renderMode, pixelPerfect, camera);
            }
        }
        
        public void SetCanvasScaler(LayerType? layer = null,
            CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPixelSize,
            float scaleFactor = 1f,
            float referencePixelsPerUnit = 100f)
        {
            // 如果 layer 为 null，修改所有层
            if (layer == null)
            {
                foreach (var canvasLayer in _layers.Values)
                {
                    canvasLayer.SetCanvasScaler(scaleMode, scaleFactor, referencePixelsPerUnit);
                }
            }
            else if (_layers.ContainsKey(layer.Value))  // 否则只修改指定层
            {
                _layers[layer.Value].SetCanvasScaler(scaleMode, scaleFactor, referencePixelsPerUnit);
            }
        }
        
        public void SetCanvasScaler(LayerType? layer = null,
            CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize,
            Vector2? referenceResolution = null,
            CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight,
            float matchWidthOrHeight = 0f,
            float referencePixelsPerUnit = 100f)
        {
            // 如果 layer 为 null，修改所有层
            if (layer == null)
            {
                foreach (var canvasLayer in _layers.Values)
                {
                    canvasLayer.SetCanvasScaler(scaleMode, referenceResolution, screenMatchMode, matchWidthOrHeight, referencePixelsPerUnit);
                }
            }
            else if (_layers.ContainsKey(layer.Value))  // 否则只修改指定层
            {
                _layers[layer.Value].SetCanvasScaler(scaleMode, referenceResolution, screenMatchMode, matchWidthOrHeight, referencePixelsPerUnit);
            }
        }
        
        public void SetCanvasScaler(LayerType? layer = null,
            CanvasScaler.ScaleMode scaleMode = CanvasScaler.ScaleMode.ConstantPhysicalSize,
            CanvasScaler.Unit physicalUnit = CanvasScaler.Unit.Points,
            float fallbackScreenDPI = 96f,
            float defaultSpriteDPI = 96f,
            float referencePixelsPerUnit = 100f)
        {
            // 如果 layer 为 null，修改所有层
            if (layer == null)
            {
                foreach (var canvasLayer in _layers.Values)
                {
                    canvasLayer.SetCanvasScaler(scaleMode, physicalUnit, fallbackScreenDPI, defaultSpriteDPI, referencePixelsPerUnit);
                }
            }
            else if (_layers.ContainsKey(layer.Value))  // 否则只修改指定层
            {
                _layers[layer.Value].SetCanvasScaler(scaleMode, physicalUnit, fallbackScreenDPI, defaultSpriteDPI, referencePixelsPerUnit);
            }
        }
        
        public void OnInit(object createParam)
        {
            if (EventSystem.current == null)
            {
                RLog.LogView("场景中缺少：EventSystem 组件，已自动添加");
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                eventSystem.SetParent(transform);
            }
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
            Destroy(gameObject);
        }
        
        // 同步加载，使用枚举作为参数
        public string ShowNotify<T>(T eventName, string content, UICallbacks callbacks = null) where T : Enum, IConvertible
        {
            int uiId = (int)(object)eventName;
            return ShowNotify(uiId, content, callbacks);
        }
        
        public string ShowNotify(int uiId, string content, UICallbacks callbacks = null)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                RLog.LogView($"打开 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return default;
            }
            return _layerNotify.Show(uiId, config, content, callbacks);
        }

        // 异步加载，使用枚举作为参数
        public UILoader ShowNotifyAsync<T>(T eventName, string content, UICallbacks callbacks = null) where T : Enum, IConvertible
        {
            int uiId = (int)(object)eventName;
            return ShowNotifyAsync(uiId, content, callbacks);
        }
        
        public UILoader ShowNotifyAsync(int uiId, string content, UICallbacks callbacks = null)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                RLog.LogView($"打开 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return default;
            }
            return _layerNotify.ShowAsync(uiId, config, content, callbacks);
        }
        
        public List<int> GetCurrentUIids()
        {
            return _currentUIids;
        }

        // 同步加载，使用枚举作为参数
        public string Open<T>(T eventName, object[] uiArgs = null, UICallbacks callbacks = null) where T : Enum, IConvertible
        {
            int uiId = (int)(object)eventName;
            return Open(uiId, uiArgs, callbacks);
        }
        
        // 同步加载，使用id作为参数
        public string Open(int uiId, object[] uiArgs = null, UICallbacks callbacks = null)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                RLog.LogView($"打开 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return default;
            }
            
            switch (config.Layer)
            {
                case LayerType.Game:
                    return _layerGame.Add(uiId, config, uiArgs, callbacks);
                case LayerType.UI:
                    return _layerUI.Add(uiId, config, uiArgs, callbacks);
                case LayerType.PopUp:
                    return _layerPopUp.Add(uiId, config, uiArgs, callbacks);
                case LayerType.Dialog:
                    return _layerDialog.Add(uiId, config, uiArgs, callbacks);
                case LayerType.Notify:
                    return _layerNotify.Add(uiId, config, uiArgs, callbacks);
                case LayerType.Guide:
                    return _layerGuide.Add(uiId, config, uiArgs, callbacks);
            }

            return default;
        }

        // 异步加载，使用枚举作为参数
        public UILoader OpenAsync<T>(T eventName, object[] uiArgs = null, UICallbacks callbacks = null) where T : Enum, IConvertible
        {
            int uiId = (int)(object)eventName;
            return OpenAsync(uiId, uiArgs, callbacks);
        }
        
        // 异步加载，使用id作为参数
        public UILoader OpenAsync(int uiId, object[] uiArgs = null, UICallbacks callbacks = null)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                RLog.LogView($"打开 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return null;
            }
            
            switch (config.Layer)
            {
                case LayerType.Game:
                    return _layerGame.AddAsync(uiId, config, uiArgs, callbacks);
                case LayerType.UI:
                    return _layerUI.AddAsync(uiId, config, uiArgs, callbacks);
                case LayerType.PopUp:
                    return _layerPopUp.AddAsync(uiId, config, uiArgs, callbacks);
                case LayerType.Dialog:
                    return _layerDialog.AddAsync(uiId, config, uiArgs, callbacks);
                case LayerType.Notify:
                    return _layerNotify.AddAsync(uiId, config, uiArgs, callbacks);
                case LayerType.Guide:
                    return _layerGuide.AddAsync(uiId, config, uiArgs, callbacks);
            }

            return null;
        }
        
        public bool Has<T>(T eventName) where T : Enum, IConvertible
        {
            int uiId = (int)(object)eventName;
            return Has(uiId);
        }

        public bool Has(int uiId)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                RLog.LogView($"检查 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return false;
            }

            bool result = false;

            switch (config.Layer)
            {
                case LayerType.Game:
                    result = _layerGame.Has(config.AssetName);
                    break;
                case LayerType.UI:
                    result = _layerUI.Has(config.AssetName);
                    break;
                case LayerType.PopUp:
                    result = _layerPopUp.Has(config.AssetName);
                    break;
                case LayerType.Dialog:
                    result = _layerDialog.Has(config.AssetName);
                    break;
                case LayerType.Notify:
                    result = _layerNotify.Has(config.AssetName);
                    break;
                case LayerType.Guide:
                    result = _layerGuide.Has(config.AssetName);
                    break;
            }

            return result;
        }

        public GameObject GetByGuid(string guid)
        {
            GameObject result = _layerGame.GetByGuid(guid);
            if (result != null) return result;

            result = _layerUI.GetByGuid(guid);
            if (result != null) return result;

            result = _layerPopUp.GetByGuid(guid);
            if (result != null) return result;

            result = _layerDialog.GetByGuid(guid);
            if (result != null) return result;

            result = _layerNotify.GetByGuid(guid);
            if (result != null) return result;

            result = _layerGuide.GetByGuid(guid);
            if (result != null) return result;

            // 如果所有层都没有找到匹配的 GameObject，返回 null
            return null;
        }

        public List<GameObject> GetByUIid<T>(T eventName) where T : Enum, IConvertible
        {
            int uiId = (int)(object)eventName;
            return GetByUIid(uiId);
        }

        public List<GameObject> GetByUIid(int uiId)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                RLog.LogView($"查找 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return null;
            }
            
            switch (config.Layer)
            {
                case LayerType.Game:
                    return _layerGame.GetByUIid(uiId);
                case LayerType.UI:
                    return _layerUI.GetByUIid(uiId);
                case LayerType.PopUp:
                    return _layerPopUp.GetByUIid(uiId);
                case LayerType.Dialog:
                    return _layerDialog.GetByUIid(uiId);
                case LayerType.Notify:
                    return _layerNotify.GetByUIid(uiId);
                case LayerType.Guide:
                    return _layerGuide.GetByUIid(uiId);
            }

            return null;
        }

        public void Close<T>(T eventName, bool isDestroy = false, string guid = default) where T : Enum, IConvertible
        {
            int uiId = (int)(object)eventName;
            Close(uiId, isDestroy, guid);
        }

        public void Close(int uiId = default, bool isDestroy = false, string guid = default)
        {
            if (!_configs.TryGetValue(uiId, out UIConfig config))
            {
                RLog.LogView($"移除 ID 为 {uiId} 的 UI 失败，未找到配置。");
                return;
            }

            switch (config.Layer)
            {
                case LayerType.Game:
                    _layerGame.Close(config.AssetName, isDestroy);
                    break;
                case LayerType.UI:
                    _layerUI.Close(config.AssetName, isDestroy);
                    break;
                case LayerType.PopUp:
                    _layerPopUp.Close(config.AssetName, isDestroy);
                    break;
                case LayerType.Dialog:
                    _layerDialog.Close(config.AssetName, isDestroy);
                    break;
                case LayerType.Notify:
                    _layerNotify.CloseByGuid(guid, true);
                    break;
                case LayerType.Guide:
                    _layerGuide.Close(config.AssetName, isDestroy);
                    break;
            }

            RemoveAtUIid(uiId);
        }

        private void RemoveAtUIid(int uiId)
        {
            for (int i = _currentUIids.Count - 1; i >= 0; i--)
            {
                if (_currentUIids[i] == uiId)
                {
                    _currentUIids.RemoveAt(i);
                    break; // 退出循环
                }
            }
        }
        
        public void Clear(bool isDestroy = true)
        {
            _layerGame.Clear(isDestroy);
            _layerUI.Clear(isDestroy);
            _layerPopUp.Clear(isDestroy);
            _layerDialog.Clear(isDestroy);
            _layerGuide.Clear(isDestroy);
            _currentUIids.Clear();
        }
    }
}
