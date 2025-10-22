using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DotLiquid;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Rain.Core
{
    //异步加载完成的回调
    public delegate void OnAssetObject<T>(T obj) where T : Object;
    public delegate void OnAllAssetObject<TObject>(Dictionary<string, TObject> objs) where TObject : Object;

    public class AssetMgr : ModuleSingleton<AssetMgr>, IModule
    {
        private AssetBundleMgr _assetBundleManager;

        private ResMgr _resourcesManager;

        //强制更改资产加载模式为远程（微信小游戏使用）
        public static bool ForceRemoteAssetBundle = false;

        public const string DirSuffix = "_Directory";

        //资产信息
        public struct AssetInfo
        {
            //目标资产类型
            public readonly AssetTypeEnum AssetType;

            //AssetName
            public readonly string AssetName;

            //直接资产请求路径相对路径，Assets开头的
            public readonly string AssetPath;

            //直接资产捆绑请求路径（仅适用于资产捆绑类型），完全路径
            public readonly string AssetBundlePath;

            //AB名
            public readonly string AbName;
            public AssetInfo(
                AssetTypeEnum assetType = default,
                string assetName = default,
                string assetPath = default,
                string assetBundlePathWithoutAb = default,
                string abName = default)
            {
                AssetType = assetType;
                AssetName = assetName;
                AssetPath = assetPath;
                AssetBundlePath = assetBundlePathWithoutAb + abName;
                AbName = abName;
            }

        }
        //资产访问标志
        [System.Flags]
        public enum AssetAccessMode : byte
        {
            None = 0,
            Unknown = 0b00000001,
            Resource = 0b00000010,
            AssetBundle = 0b00000100,
            RemoteAssetBundle = 0b00001000
        }

        //资产类型
        public enum AssetTypeEnum : byte
        {
            None,
            Resource,
            AssetBundle
        }

        // 是否采用编辑器模式
        private bool _isEditorMode = false;
        public bool IsEditorMode
        {
            get
            {
#if UNITY_EDITOR
                return _isEditorMode || UnityEditor.EditorPrefs.GetBool(Application.dataPath.GetHashCode() + "IsEditorMode", false);
#else
                    return false;
#endif
            }
            set
            {
                _isEditorMode = value;
            }
        }


        //如果信息合法，则该值为真
        public bool IsLegal(ref AssetInfo assetInfo)
        {
#if UNITY_EDITOR
            if (IsEditorMode)
            {
                if (assetInfo.AssetType == AssetTypeEnum.Resource)
                    if (assetInfo.AssetPath != default || SearchAsset(assetInfo.AssetName, AssetAccessMode.Resource) != null)
                    {
                        return true;
                    }

                if (assetInfo.AssetType == AssetTypeEnum.AssetBundle)
                    if ((assetInfo.AssetPath != default && assetInfo.AssetBundlePath != default) || SearchAsset(assetInfo.AssetName, AssetAccessMode.AssetBundle) != null)
                    {
                        return true;
                    }

                return false;
            }
#endif
            if (assetInfo.AssetType == AssetTypeEnum.None)
                return false;

            if (assetInfo.AssetType == AssetTypeEnum.Resource &&
                assetInfo.AssetPath == default)
                return false;

            if (assetInfo.AssetType == AssetTypeEnum.AssetBundle &&
                (assetInfo.AssetPath == default || assetInfo.AssetBundlePath == default))
                return false;

            return true;
        }

        /// <summary>
        /// 根据提供的资产路径和访问选项推断资产类型。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="accessMode">访问模式。</param>
        /// <returns>资产信息。</returns>
        public AssetInfo GetAssetInfo(string assetName,
            AssetAccessMode accessMode = AssetAccessMode.Unknown)
        {
            if (ForceRemoteAssetBundle)
            {
                accessMode = AssetAccessMode.RemoteAssetBundle;
            }

            if (accessMode.HasFlag(AssetAccessMode.Resource))
            {
                bool showTip = !IsEditorMode;

                return GetAssetInfoFromResource(assetName, showTip);
            }
            else if (accessMode.HasFlag(AssetAccessMode.AssetBundle))
            {
                bool showTip = !IsEditorMode;

                return GetAssetInfoFromAssetBundle(assetName, false, showTip);
            }
            else if (accessMode.HasFlag(AssetAccessMode.Unknown))
            {
                AssetInfo r = GetAssetInfoFromAssetBundle(assetName);
                if (!IsLegal(ref r))
                {
                    r = GetAssetInfoFromResource(assetName);
                }

                if (IsLegal(ref r))
                {
                    return r;
                }
                else
                {
                    Debug.LogError("AssetBundle和Resource都找不到指定资源可用的索引：" + assetName);
                    return new AssetInfo();
                }
            }
            else if (accessMode.HasFlag(AssetAccessMode.RemoteAssetBundle))
            {
                AssetInfo r = GetAssetInfoFromAssetBundle(assetName, true);
                if (!IsLegal(ref r))
                {
                    r = GetAssetInfoFromResource(assetName);
                }

                if (IsLegal(ref r))
                {
                    return r;
                }
                else
                {
                    Debug.LogError("AssetBundle找不到指定远程资源可用的索引：" + assetName);
                    return new AssetInfo();
                }
            }
            return new AssetInfo();
        }

        /// <summary>
        /// 通过资产捆绑加载程序和对象名称获取资产对象。
        /// </summary>
        /// <typeparam name="T">资产对象的目标对象类型。</typeparam>
        /// <param name="assetName">资产对象的路径。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        /// <returns>找到的资产对象。</returns>
        public T GetAssetObject<T>(
            string assetName,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
            where T : Object
        {
            AssetInfo info = GetAssetInfo(assetName, mode);

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                T o = ResMgr.Ins.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                return o;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        subAssetName, out EditorLoader editorLoader);
                }
#endif
                T o = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader);
                if (o != null)
                {
                    return o;
                }
                AssetBundleLoader ab = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);
                ab.Expand(info.AssetPath, typeof(T), subAssetName);
                o = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader2);
                if (o != null)
                {
                    return o;
                }
                Debug.LogError("获取不到资产或者类型错误！");
            }

            return null;
        }

        /// <summary>
        /// 同步加载资源对象。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="assetType">目标资产类型。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        /// <returns>加载的资产对象。</returns>
        public Object GetAssetObject(
            string assetName,
            System.Type assetType,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                Object o = ResMgr.Ins.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);
                return o;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        out EditorLoader editorLoader, assetType, subAssetName);
                }
#endif
                Object o = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader);
                if (o != null)
                {
                    return o;
                }
                AssetBundleLoader ab = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);
                ab.Expand(info.AssetPath, assetType, subAssetName);
                o = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader2);
                if (o != null)
                {
                    return o;
                }
                Debug.LogError("获取不到资产或者类型错误！");
            }

            return null;
        }

        /// <summary>
        /// 同步加载资源对象。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="mode">访问模式。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <returns>加载的资产对象。</returns>
        public Object GetAssetObject(
            string assetName,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                Object o = ResMgr.Ins.GetAssetObject(assetPath, null, subAssetName, out ResourcesLoader loader);
                return o;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAsset<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        subAssetName, out EditorLoader editorLoader);
                }
#endif
                Object o = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out AssetBundleLoader loader);
                if (o != null)
                {
                    return o;
                }
                AssetBundleLoader ab = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);
                ab.Expand(info.AssetPath, default, subAssetName);
                o = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out AssetBundleLoader loader2);
                if (o != null)
                {
                    return o;
                }
                Debug.LogError("获取不到资产！");
            }

            return null;
        }

        /// <summary>
        /// 获取所有资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Dictionary<string, TObject> GetAllAssetObject<TObject>(string assetName, AssetAccessMode mode = AssetAccessMode.Unknown) where TObject : Object
        {
            Dictionary<string, TObject> allAsset = new Dictionary<string, TObject>();
            foreach (var item in GetAllAssetObject(assetName, mode))
            {
                if (item.Value is TObject value)
                {
                    allAsset[item.Key] = value;
                }
            }
            return allAsset;
        }

        /// <summary>
        /// 获取所有资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Dictionary<string, Object> GetAllAssetObject(string assetName, AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                return ResMgr.Ins.GetAllAssetObject(assetPath);
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAllAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        out EditorLoader editorLoader);
                }
#endif
                return AssetBundleMgr.Ins.GetAllAssetObject(info.AssetBundlePath);
            }

            return null;
        }

        /// <summary>
        /// 加载所有资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Dictionary<string, TObject> LoadAll<TObject>(string assetName, AssetAccessMode mode = AssetAccessMode.Unknown) where TObject : Object
        {
            Dictionary<string, TObject> allAsset = new Dictionary<string, TObject>();
            foreach (var item in LoadAll(assetName, mode))
            {
                if (item.Value is TObject value)
                {
                    allAsset[item.Key] = value;
                }
            }
            return allAsset;
        }

        /// <summary>
        /// 加载所有资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Dictionary<string, Object> LoadAll(string assetName, AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                ResMgr.Ins.LoadAll(assetPath, null, null, out ResourcesLoader loader);

                return ResMgr.Ins.GetAllAssetObject(assetPath);
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAllAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        out EditorLoader editorLoader);
                }
#endif
                AssetBundleMgr.Ins.Load(assetName, null, ref info, null, true);

                return AssetBundleMgr.Ins.GetAllAssetObject(info.AssetBundlePath);
            }

            return null;
        }

        /// <summary>
        /// 异步加载所有资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public BaseLoader LoadAllAsync<TObject>(string assetName, OnAllAssetObject<TObject> callback = null, AssetAccessMode mode = AssetAccessMode.Unknown) where TObject : Object
        {
            void NonGenericCallback(Dictionary<string, Object> allAssets)
            {
                if (allAssets != null)
                {
                    Dictionary<string, TObject> filteredAssets = new Dictionary<string, TObject>();
                    foreach (var kvp in allAssets)
                    {
                        if (kvp.Value is TObject typedObject)
                        {
                            filteredAssets[kvp.Key] = typedObject;
                        }
                    }

                    callback?.Invoke(filteredAssets);
                }
                else
                {
                    callback?.Invoke(null);
                }
            }

            return LoadAllAsync(assetName, NonGenericCallback, mode);
        }

        /// <summary>
        /// 异步加载所有资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public BaseLoader LoadAllAsync(string assetName, OnAllAssetObject<Object> callback = null, AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                End();
                return new BaseLoader();
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                ResMgr.Ins.LoadAll(assetPath, null, null, out ResourcesLoader loader, true);
                End(ResMgr.Ins.GetAllAssetObject(assetPath));
                return loader;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    var allAsset = AssetDatabaseMgr.Ins.EditorLoadAllAsset(
                        info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath, out EditorLoader editorLoader);
                    End(allAsset);
                    return editorLoader;
                }
#endif
                AssetBundleLoader loader = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);

                if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                {
                    loader = AssetBundleMgr.Ins.LoadAsync(assetName, null, info, null, (b) =>
                    {
                        End(AssetBundleMgr.Ins.GetAllAssetObject(info.AssetBundlePath));
                    }, true);
                    return loader;
                }
                else
                {
                    loader.Expand(info.AssetPath, null, null, true);
                    End(AssetBundleMgr.Ins.GetAllAssetObject(info.AssetBundlePath));
                    return loader;
                }
            }
            void End(Dictionary<string, Object> allAsset = null)
            {
                callback?.Invoke(allAsset);
            }

            return null;
        }

        /// <summary>
        /// 加载子资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Dictionary<string, TObject> LoadSub<TObject>(string assetName, AssetAccessMode mode = AssetAccessMode.Unknown) where TObject : Object
        {
            Dictionary<string, TObject> allAsset = new Dictionary<string, TObject>();
            foreach (var item in LoadSub(assetName, mode))
            {
                if (item.Value is TObject value)
                {
                    allAsset[item.Key] = value;
                }
            }

            return allAsset;
        }

        /// <summary>
        /// 加载子资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Dictionary<string, Object> LoadSub(string assetName, AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                ResMgr.Ins.LoadAll(assetPath, null, Guid.NewGuid().ToString(), out ResourcesLoader loader);

                return ResMgr.Ins.GetAllAssetObject(assetPath);
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAllAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        out EditorLoader editorLoader);
                }
#endif
                AssetBundleMgr.Ins.Load(assetName, null, ref info, Guid.NewGuid().ToString());

                return AssetBundleMgr.Ins.GetAllAssetObject(info.AssetBundlePath);
            }

            return null;
        }

        /// <summary>
        /// 异步加载子资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public BaseLoader LoadSubAsync<TObject>(string assetName, OnAllAssetObject<TObject> callback = null, AssetAccessMode mode = AssetAccessMode.Unknown) where TObject : Object
        {
            void NonGenericCallback(Dictionary<string, Object> allAssets)
            {
                if (allAssets != null)
                {
                    Dictionary<string, TObject> filteredAssets = new Dictionary<string, TObject>();
                    foreach (var kvp in allAssets)
                    {
                        if (kvp.Value is TObject typedObject)
                        {
                            filteredAssets[kvp.Key] = typedObject;
                        }
                    }

                    callback?.Invoke(filteredAssets);
                }
                else
                {
                    callback?.Invoke(null);
                }
            }

            return LoadSubAsync(assetName, NonGenericCallback, mode);
        }

        /// <summary>
        /// 异步加载子资产
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="callback"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public BaseLoader LoadSubAsync(string assetName, OnAllAssetObject<Object> callback = null, AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                End();
                return new BaseLoader();
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                ResMgr.Ins.LoadAll(assetPath, null, Guid.NewGuid().ToString(), out ResourcesLoader loader);
                End(ResMgr.Ins.GetAllAssetObject(assetPath));
                return loader;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    var allAsset = AssetDatabaseMgr.Ins.EditorLoadAllAsset(
                        info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath, out EditorLoader editorLoader);
                    End(allAsset);
                    return editorLoader;
                }
#endif
                AssetBundleLoader loader = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);

                if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                {
                    loader = AssetBundleMgr.Ins.LoadAsync(assetName, null, info, Guid.NewGuid().ToString(), (b) =>
                    {
                        End(AssetBundleMgr.Ins.GetAllAssetObject(info.AssetBundlePath));
                    });
                    return loader;
                }
                else
                {
                    loader.Expand(info.AssetPath, null, Guid.NewGuid().ToString());
                    End(AssetBundleMgr.Ins.GetAllAssetObject(info.AssetBundlePath));
                    return loader;
                }
            }
            void End(Dictionary<string, Object> allAsset = null)
            {
                callback?.Invoke(allAsset);
            }

            return null;
        }

        /// <summary>
        /// 同步加载资源对象。
        /// </summary>
        /// <typeparam name="T">目标资产类型。</typeparam>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        /// <returns>加载的资产对象。</returns>
        public T Load<T>(
            string assetName,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
            where T : Object
        {
            //获取资源信息
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
                return null;

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                //如果加载过资源对象 直接返回资源池引用
                T o = ResMgr.Ins.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                if (o != null)
                {
                    return o;
                }

                if (subAssetName.IsNullOrEmpty())
                {
                    //没有加载过资源对象
                    //直接根据路径加载资源对象
                    return ResMgr.Ins.Load<T>(assetPath);
                }
                return ResMgr.Ins.LoadAll<T>(assetPath, subAssetName, out ResourcesLoader loader2);
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        subAssetName, out EditorLoader editorLoader);
                }
#endif
                //检查是否已经加载过该ab
                T o2 = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    return o2;
                }

                if (loader == null || loader.AssetBundleContent == null)
                {
                    //重新加载ab
                    AssetBundleMgr.Ins.Load(assetName, typeof(T), ref info, subAssetName);
                    loader = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);
                }

                T o = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader2);
                if (o != null)
                {
                    return o;
                }

                loader.Expand(info.AssetPath, typeof(T), subAssetName);
                return AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader3);
            }

            return null;
        }

        /// <summary>
        /// 同步加载资源文件夹。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="mode">访问模式。</param>
        public void LoadDir(
            string assetName,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            assetName += DirSuffix;
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
                return;

            if (info.AssetType == AssetTypeEnum.Resource)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    RLog.LogAsset("编辑器模式下无需加载文件夹");
                    return;
                }
#endif
                string assetPath = info.AssetPath;
                if (string.IsNullOrEmpty(assetPath))
                {
                    return;
                }

                // For single asset path, we don't need to iterate
                string subAssetName = assetPath;
                AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                string subAssetPath = subInfo.AssetPath;
                Object o = ResMgr.Ins.GetAssetObject(subAssetPath, null, null, out ResourcesLoader loader);

                if (o == null)
                {
                    ResMgr.Ins.Load(subAssetPath);
                }
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return;
                }
#endif
                string subAssetName = info.AssetPath;
                if (!string.IsNullOrEmpty(subAssetName))
                {
                    AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                    AssetBundleLoader ab = AssetBundleMgr.Ins.GetAssetBundleLoader(subInfo.AssetBundlePath);
                    if (ab == null || ab.AssetBundleContent == null)
                    {
                        AssetBundleMgr.Ins.Load(subAssetName, null, ref subInfo, null);
                    }
                }
            }
        }

        /// <summary>
        /// 同步加载资源对象。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="assetType">目标资产类型。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        /// <returns>加载的资产对象。</returns>
        public Object Load(
            string assetName,
            System.Type assetType,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
                return null;

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                Object o = ResMgr.Ins.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);
                if (o != null)
                {
                    return o;
                }

                if (subAssetName.IsNullOrEmpty())
                {
                    return ResMgr.Ins.Load(assetPath, assetType);
                }
                return ResMgr.Ins.LoadAll(assetPath, assetType, subAssetName, out ResourcesLoader loader2);
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        out EditorLoader editorLoader, assetType, subAssetName);
                }
#endif
                Object o2 = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    return o2;
                }

                if (loader == null || loader.AssetBundleContent == null)
                {
                    AssetBundleMgr.Ins.Load(assetName, assetType, ref info, subAssetName);
                    loader = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);
                }

                Object o = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader2);
                if (o != null)
                {
                    return o;
                }

                loader.Expand(info.AssetPath, assetType, subAssetName);
                return AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader3);
            }

            return null;
        }

        /// <summary>
        /// 同步加载资源对象。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        /// <returns>加载的资产对象。</returns>
        public Object Load(
            string assetName,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
                return null;

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                Object o = ResMgr.Ins.GetAssetObject(assetPath, null, subAssetName, out ResourcesLoader loader);
                if (o != null)
                {
                    return o;
                }

                if (subAssetName.IsNullOrEmpty())
                {
                    return ResMgr.Ins.Load(assetPath);
                }
                return ResMgr.Ins.LoadAll(assetPath, null, subAssetName, out ResourcesLoader loader2);
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    return AssetDatabaseMgr.Ins.EditorLoadAsset<Object>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        subAssetName, out EditorLoader editorLoader);
                }
#endif
                Object o2 = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    return o2;
                }

                if (loader == null || loader.AssetBundleContent == null)
                {
                    AssetBundleMgr.Ins.Load(assetName, default, ref info, subAssetName);
                    loader = AssetBundleMgr.Ins.GetAssetBundleLoader(info.AssetBundlePath);
                }

                Object o = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out AssetBundleLoader loader2);
                if (o != null)
                {
                    return o;
                }

                loader.Expand(info.AssetPath, null, subAssetName);
                return AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out AssetBundleLoader loader3);
            }

            return null;
        }

        /// <summary>
        /// 异步加载资产对象。
        /// </summary>
        /// <typeparam name="T">目标资产类型。</typeparam>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="callback">异步加载完成时的回调函数。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        public BaseLoader LoadAsync<T>(
            string assetName,
            OnAssetObject<T> callback = null,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
            where T : Object
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                End();
                return new BaseLoader();
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                T o = ResMgr.Ins.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                if (o != null)
                {
                    End(o);
                    return loader;
                }

                if (loader == null || loader.LoaderSuccess == false || o == null)
                {
                    if (subAssetName.IsNullOrEmpty())
                    {
                        return ResMgr.Ins.LoadAsync<T>(assetPath, callback);
                    }
                    else
                    {
                        T subAsset = ResMgr.Ins.LoadAll<T>(assetPath, subAssetName, out ResourcesLoader loader2);
                        End(subAsset);
                        return loader2;
                    }
                }
                return null;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    T o = AssetDatabaseMgr.Ins.EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        subAssetName, out EditorLoader editorLoader);
                    End(o);
                    return editorLoader;
                }
#endif
                T o2 = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    End(o2);
                    return loader;
                }

                if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                {
                    loader = AssetBundleMgr.Ins.LoadAsync(assetName, typeof(T), info, subAssetName, (b) =>
                    {
                        T loadedAsset = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out _);

                        if (loadedAsset == null)
                        {
                            loader?.Expand(info.AssetPath, typeof(T), subAssetName);
                            loadedAsset = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out _);
                        }

                        End(loadedAsset);
                    });
                    return loader;
                }
                // 扩展并获取资源
                loader.Expand(info.AssetPath, typeof(T), subAssetName);
                T expandedAsset = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader finalLoader);
                End(expandedAsset);
                return finalLoader;
            }

            void End(T o = null)
            {
                callback?.Invoke(o);
            }

            return null;
        }

        /// <summary>
        /// 协程加载资产对象。
        /// </summary>
        /// <typeparam name="T">目标资产类型。</typeparam>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        public IEnumerator LoadAsyncCoroutine<T>(string assetName, string subAssetName = null, AssetAccessMode mode = AssetAccessMode.Unknown) where T : Object
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                yield break;
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                T o = ResMgr.Ins.GetAssetObject<T>(assetPath, subAssetName, out ResourcesLoader loader);
                if (o != null)
                {
                    yield return o;
                }
                else
                {
                    if (subAssetName.IsNullOrEmpty())
                    {
                        yield return ResMgr.Ins.LoadAsyncCoroutine<T>(assetPath);
                    }
                    else
                    {
                        yield return ResMgr.Ins.LoadAll<T>(assetPath, subAssetName, out ResourcesLoader loader2);
                    }
                }
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    T o = AssetDatabaseMgr.Ins.EditorLoadAsset<T>(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        subAssetName, out EditorLoader editorLoader);
                    yield return o;
                    yield break;
                }
#endif
                T o2 = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    yield return o2;
                }

                if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                {
                    yield return AssetBundleMgr.Ins.LoadAsyncCoroutine(assetName, typeof(T), info, subAssetName);
                }
                else
                {
                    T o = AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        yield return o;
                    }
                    else
                    {
                        loader.Expand(info.AssetPath, typeof(T), subAssetName);
                        yield return AssetBundleMgr.Ins.GetAssetObject<T>(info.AssetBundlePath, info.AssetPath, subAssetName, out AssetBundleLoader loader3);
                    }
                }
            }
        }

        /// <summary>
        /// 协程加载资产对象。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="assetType">目标资产类型。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        public IEnumerator LoadAsyncCoroutine(string assetName, System.Type assetType = null, string subAssetName = null, AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                yield break;
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                Object o = ResMgr.Ins.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);

                if (o != null)
                {
                    yield return o;
                }
                else
                {
                    if (subAssetName.IsNullOrEmpty())
                    {
                        yield return ResMgr.Ins.LoadAsyncCoroutine(assetPath, assetType);
                    }
                    else
                    {
                        yield return ResMgr.Ins.LoadAll(assetPath, assetType, subAssetName, out ResourcesLoader loader2);
                    }
                }
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    var o = AssetDatabaseMgr.Ins.EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        out EditorLoader editorLoader, assetType, subAssetName);
                    yield return o;
                    yield break;
                }
#endif
                Object o2 = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    yield return o2;
                }

                if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                {
                    yield return AssetBundleMgr.Ins.LoadAsyncCoroutine(assetName, assetType, info, subAssetName);
                }
                else
                {
                    Object o = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader2);
                    if (o != null)
                    {
                        yield return o;
                    }
                    else
                    {
                        loader.Expand(info.AssetPath, assetType, subAssetName);
                        yield return AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader3);
                    }
                }
            }
        }

        /// <summary>
        /// 异步加载资产文件夹。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="callback">异步加载完成时的回调函数。</param>
        /// <param name="mode">访问模式。</param>
        public BaseDirLoader LoadDirAsync(
            string assetName,
            Action callback = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            assetName += DirSuffix;
            AssetInfo info = GetAssetInfo(assetName, mode);
            BaseDirLoader dirLoader = new BaseDirLoader();
            if (!IsLegal(ref info))
            {
                End();
                return dirLoader;
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    RLog.LogAsset("编辑器模式下无需加载文件夹");
                    End();
                    return dirLoader;
                }
#endif
                string assetPath = info.AssetPath;
                if (string.IsNullOrEmpty(assetPath))
                {
                    End();
                    return dirLoader;
                }

                Object o = ResMgr.Ins.GetAssetObject(assetPath, null, null, out ResourcesLoader loader);
                if (o != null)
                {
                    dirLoader.Loaders.Add(loader);
                    End();
                    dirLoader.OnComplete();
                }
                else
                {
                    BaseLoader loader2 = ResMgr.Ins.LoadAsync(assetPath, (b) =>
                    {
                        End();
                        dirLoader.OnComplete();
                    });
                    dirLoader.Loaders.Add(loader2);
                }
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    RLog.LogAsset("编辑器模式下无需加载文件夹");
                    End();
                    return dirLoader;
                }
#endif
                string subAssetName = info.AssetPath;
                if (!string.IsNullOrEmpty(subAssetName))
                {
                    AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                    AssetBundleLoader loader = AssetBundleMgr.Ins.GetAssetBundleLoader(subInfo.AssetBundlePath);
                    if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                    {
                        loader = AssetBundleMgr.Ins.LoadAsync(subAssetName, null,
                            subInfo, "", (b) =>
                            {
                                End();
                                dirLoader.OnComplete();
                            });
                        dirLoader.Loaders.Add(loader);
                    }
                    else
                    {
                        Object o = AssetBundleMgr.Ins.GetAssetObject(subInfo.AssetBundlePath, subInfo.AssetPath, null, null, out AssetBundleLoader loader2);
                        if (o != null)
                        {
                            dirLoader.Loaders.Add(loader2);
                            End();
                            dirLoader.OnComplete();
                        }
                        else
                        {
                            loader.Expand(subInfo.AssetPath, null, "");
                            dirLoader.Loaders.Add(loader);
                            End();
                            dirLoader.OnComplete();
                        }
                    }
                }
            }

            void End()
            {
                callback?.Invoke();
            }

            return dirLoader;
        }

        /// <summary>
        /// 协程加载资产文件夹。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="mode">访问模式。</param>
        public IEnumerable LoadDirAsyncCoroutine(string assetName, AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            assetName += DirSuffix;
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                yield break;
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    RLog.LogAsset("编辑器模式下无需加载文件夹");
                    yield break;
                }
#endif
                string assetPath = info.AssetPath;
                if (string.IsNullOrEmpty(assetPath))
                {
                    yield break;
                }

                // For single asset path, we don't need to iterate
                string subAssetName = assetPath;
                AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                string subAssetPath = subInfo.AssetPath;
                Object o = ResMgr.Ins.GetAssetObject(subAssetPath, null, null, out ResourcesLoader loader);

                if (o != null)
                {
                    yield return o;
                }
                else
                {
                    yield return ResMgr.Ins.LoadAsyncCoroutine(subAssetPath);
                }
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    RLog.LogAsset("编辑器模式下无需加载文件夹");
                    yield break;
                }
#endif
                string subAssetName = info.AssetPath;
                if (!string.IsNullOrEmpty(subAssetName))
                {
                    AssetInfo subInfo = GetAssetInfo(subAssetName, mode);
                    AssetBundleLoader loader = AssetBundleMgr.Ins.GetAssetBundleLoader(subInfo.AssetBundlePath);
                    if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                    {
                        yield return AssetBundleMgr.Ins.LoadAsyncCoroutine(subAssetName, null, subInfo, null);
                    }
                    else
                    {
                        Object o = AssetBundleMgr.Ins.GetAssetObject(subInfo.AssetBundlePath, subInfo.AssetPath, null, null, out AssetBundleLoader loader2);
                        if (o != null)
                        {
                            yield return o;
                        }
                        else
                        {
                            loader.Expand(subInfo.AssetPath, null, "");
                            yield return o;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 异步加载资产对象。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="assetType">目标资产类型。</param>
        /// <param name="callback">异步加载完成时的回调函数。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        public BaseLoader LoadAsync(
            string assetName,
            System.Type assetType,
            OnAssetObject<Object> callback = null,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                End();
                return new BaseLoader();
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                Object o = ResMgr.Ins.GetAssetObject(assetPath, assetType, subAssetName, out ResourcesLoader loader);
                if (o != null)
                {
                    End(o);
                    return loader;
                }

                if (loader == null || loader.LoaderSuccess == false || o == null)
                {
                    if (subAssetName.IsNullOrEmpty())
                    {
                        return ResMgr.Ins.LoadAsync(assetPath, assetType, callback);
                    }
                    else
                    {
                        Object subAsset = ResMgr.Ins.LoadAll(assetPath, assetType, subAssetName, out ResourcesLoader loader2);
                        End(subAsset);
                        return loader2;
                    }
                }
                return null;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    Object o = AssetDatabaseMgr.Ins.EditorLoadAsset(info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath,
                        out EditorLoader editorLoader, assetType, subAssetName);
                    End(o);
                    return editorLoader;
                }
#endif
                Object o2 = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    End(o2);
                    return loader;
                }

                if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                {
                    loader = AssetBundleMgr.Ins.LoadAsync(assetName, assetType, info, subAssetName, (b) =>
                    {
                        Object loadedAsset = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out _);

                        if (loadedAsset == null)
                        {
                            loader?.Expand(info.AssetPath, assetType, subAssetName);
                            loadedAsset = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out _);
                        }

                        End(loadedAsset);
                    });
                    return loader;
                }
                // 扩展并获取资源
                loader.Expand(info.AssetPath, assetType, subAssetName);
                Object expandedAsset = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, assetType, subAssetName, out AssetBundleLoader finalLoader);
                End(expandedAsset);
                return finalLoader;
            }

            void End(Object o = null)
            {
                callback?.Invoke(o);
            }

            return null;
        }

        /// <summary>
        /// 异步加载资产对象。
        /// </summary>
        /// <param name="assetName">资产路径字符串。</param>
        /// <param name="callback">异步加载完成时的回调函数。</param>
        /// <param name="subAssetName">子资产名称。</param>
        /// <param name="mode">访问模式。</param>
        public BaseLoader LoadAsync(
            string assetName,
            OnAssetObject<Object> callback = null,
            string subAssetName = null,
            AssetAccessMode mode = AssetAccessMode.Unknown)
        {
            AssetInfo info = GetAssetInfo(assetName, mode);
            if (!IsLegal(ref info))
            {
                End();
                return new BaseLoader();
            }

            if (info.AssetType == AssetTypeEnum.Resource)
            {
                string assetPath = info.AssetPath;
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    assetPath = info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath;
                }
#endif
                Object o = ResMgr.Ins.GetAssetObject(assetPath, null, subAssetName, out ResourcesLoader loader);
                if (o != null)
                {
                    End(o);
                    return loader;
                }

                if (loader == null || loader.LoaderSuccess == false || o == null)
                {
                    if (subAssetName.IsNullOrEmpty())
                    {
                        return ResMgr.Ins.LoadAsync(assetPath, callback);
                    }
                    else
                    {
                        Object subAsset = ResMgr.Ins.LoadAll(assetPath, null, subAssetName, out ResourcesLoader loader2);
                        End(subAsset);
                        return loader2;
                    }
                }
                return null;
            }
            else if (info.AssetType == AssetTypeEnum.AssetBundle)
            {
#if UNITY_EDITOR
                if (IsEditorMode)
                {
                    Object o = AssetDatabaseMgr.Ins.EditorLoadAsset<Object>(
                        info.AssetPath == null ? SearchAsset(assetName) : info.AssetPath, subAssetName, out EditorLoader editorLoader);
                    End(o);
                    return editorLoader;
                }
#endif
                Object o2 = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out AssetBundleLoader loader);
                if (o2 != null)
                {
                    End(o2);
                    return loader;
                }

                if (loader == null || loader.AssetBundleContent == null || loader.GetDependentNamesLoadFinished() < loader.AddDependentNames())
                {
                    loader = AssetBundleMgr.Ins.LoadAsync(assetName, null, info, subAssetName, (b) =>
                    {
                        Object loadedAsset = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out _);

                        if (loadedAsset == null)
                        {
                            loader?.Expand(info.AssetPath, null, subAssetName);
                            loadedAsset = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out _);
                        }

                        End(loadedAsset);
                    });
                    return loader;
                }
                // 扩展并获取资源
                loader.Expand(info.AssetPath, null, subAssetName);
                Object expandedAsset = AssetBundleMgr.Ins.GetAssetObject(info.AssetBundlePath, info.AssetPath, null, subAssetName, out AssetBundleLoader finalLoader);
                End(expandedAsset);
                return finalLoader;
            }

            void End(Object o = null)
            {
                callback?.Invoke(o);
            }

            return null;
        }

        /// <summary>
        /// 从Resources中获取资源信息
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="showTip"></param>
        /// <returns></returns>
        private AssetInfo GetAssetInfoFromResource(string assetName, bool showTip = false)
        {
            ResMapping resMapping = GetResInResMappings(assetName);
            if (resMapping != null)
            {
                return new AssetInfo(AssetTypeEnum.Resource, assetName, resMapping.logicPath, default, default);
            }

            if (showTip)
            {
                Debug.LogError("Resource找不到指定资源可用的索引：" + assetName);
            }
            return new AssetInfo(AssetTypeEnum.Resource, assetName);
        }

        /// <summary>
        /// 从资源映射表中获取资源映射信息
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private ResMapping GetResInResMappings(string assetName)
        {
            ResMap.ResMappings.TryGetValue(assetName, out ResMapping resMapping);
            return resMapping;
        }

        private AssetInfo GetAssetInfoFromAssetBundle(string assetName, bool remote = false, bool showTip = false)
        {
            if (GameConfig.LocalAssetBundleMap.ABMap.TryGetValue(assetName, out AssetMapping assetmpping))
            {
                if (remote || ForceRemoteAssetBundle)
                {
                    return new AssetInfo(AssetTypeEnum.AssetBundle, assetName, assetmpping.AssetPath, AssetBundleMgr.GetRemoteAssetBundleCompletePath(), assetmpping.AbName);
                }
                else
                {
                    if (ResMap.ResMappings.TryGetValue(assetName, out ResMapping resMapping))
                        return new AssetInfo(AssetTypeEnum.AssetBundle, assetName, assetmpping.AssetPath, AssetBundleMgr.GetAssetBundlePathWithoutAb(assetName), resMapping.AbName);
                    else
                        return new AssetInfo(AssetTypeEnum.AssetBundle, assetName, assetmpping.AssetPath, AssetBundleMgr.GetAssetBundlePathWithoutAb(assetName), assetmpping.AbName);
                }
            }

            if (showTip)
            {
                Debug.LogError("AssetBundle找不到指定资源可用的索引：" + assetName);
            }
            return new AssetInfo(AssetTypeEnum.AssetBundle, assetName);
        }

        /// <summary>
        /// 通过资源名称同步卸载。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="unloadAllLoadedObjects">完全卸载。</param>
        public void Unload(string assetName, bool unloadAllLoadedObjects = false)
        {
#if UNITY_EDITOR
            if (IsEditorMode)
            {
                AssetDatabaseMgr.Ins.Unload(SearchAsset(assetName));

                AssetInfo editorRes = GetAssetInfoFromResource(assetName);
                if (IsLegal(ref editorRes))
                {
                    ResMgr.Ins.Unload(editorRes.AssetPath);
                }
                return;
            }
#endif
            AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
            if (IsLegal(ref ab))
            {
                AssetBundleMgr.Ins.Unload(ab.AssetBundlePath, unloadAllLoadedObjects);
            }
            AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
            if (IsLegal(ref abRemote))
            {
                AssetBundleMgr.Ins.Unload(abRemote.AssetBundlePath, unloadAllLoadedObjects);
            }
            AssetInfo res = GetAssetInfoFromResource(assetName);
            if (IsLegal(ref res))
            {
                ResMgr.Ins.Unload(res.AssetPath);
            }
        }

        /// <summary>
        /// 通过资源名称异步卸载。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="unloadAllLoadedObjects">
        /// 完全卸载。
        /// </param>
        /// <param name="callback">异步卸载完成时的回调函数。</param>
        public void UnloadAsync(string assetName, bool unloadAllLoadedObjects = false, AssetBundleLoader.OnUnloadFinished callback = null)
        {
#if UNITY_EDITOR
            if (IsEditorMode)
            {
                AssetDatabaseMgr.Ins.Unload(SearchAsset(assetName));

                AssetInfo editorRes = GetAssetInfoFromResource(assetName);
                if (IsLegal(ref editorRes))
                {
                    ResMgr.Ins.Unload(editorRes.AssetPath);
                }
                callback?.Invoke();
                return;
            }
#endif
            AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
            if (IsLegal(ref ab))
            {
                AssetBundleMgr.Ins.UnloadAsync(ab.AssetBundlePath, unloadAllLoadedObjects, callback);
            }
            AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
            if (IsLegal(ref abRemote))
            {
                AssetBundleMgr.Ins.UnloadAsync(abRemote.AssetBundlePath, unloadAllLoadedObjects, callback);
            }
            AssetInfo res = GetAssetInfoFromResource(assetName);
            if (IsLegal(ref res))
            {
                ResMgr.Ins.Unload(res.AssetPath);
                callback?.Invoke();
            }
        }

        /// <summary>
        /// 通过资源名称获取加载器的加载进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <returns>加载进度。</returns>
        public float GetLoadProgress(string assetName)
        {
#if UNITY_EDITOR
            if (IsEditorMode)
            {
                return 1f;
            }
#endif
            float progress = 2.1f;

            string assetBundlePath = "";
            string assetBundlePathRemote = "";

            AssetInfo ab = GetAssetInfoFromAssetBundle(assetName);
            if (IsLegal(ref ab))
            {
                assetBundlePath = ab.AssetBundlePath;
            }

            AssetInfo abRemote = GetAssetInfoFromAssetBundle(assetName, true);
            if (IsLegal(ref abRemote))
            {
                assetBundlePathRemote = abRemote.AssetBundlePath;
            }

            AssetInfo res = GetAssetInfoFromResource(assetName);
            if (IsLegal(ref res))
            {
                float resProgress = ResMgr.Ins.GetLoadProgress(res.AssetPath);
                if (resProgress > -1f)
                {
                    progress = Mathf.Min(progress, resProgress);
                }
            }

            float bundleProgress = AssetBundleMgr.Ins.GetLoadProgress(assetBundlePath);
            if (bundleProgress > -1f)
            {
                progress = Mathf.Min(progress, bundleProgress);
            }

            float bundleProgressRemote = AssetBundleMgr.Ins.GetLoadProgress(assetBundlePathRemote);
            if (bundleProgressRemote > -1f)
            {
                progress = Mathf.Min(progress, bundleProgressRemote);
            }

            if (progress >= 2f)
            {
                progress = 0f;
            }

            return progress;
        }

        /// <summary>
        /// 获取所有加载器的加载进度。
        /// 正常值范围从 0 到 1。
        /// 但如果没有加载器，则返回 -1。
        /// </summary>
        /// <returns>加载进度。</returns>
        public float GetLoadProgress()
        {
#if UNITY_EDITOR
            if (IsEditorMode)
            {
                return 1f;
            }
#endif
            float progress = 2.1f;
            float abProgress = AssetBundleMgr.Ins.GetLoadProgress();
            if (abProgress > -1f)
            {
                progress = Mathf.Min(progress, abProgress);
            }
            float resProgress = ResMgr.Ins.GetLoadProgress();
            if (resProgress > -1f)
            {
                progress = Mathf.Min(progress, resProgress);
            }
            if (progress >= 2f)
            {
                progress = 0f;
            }
            return progress;
        }

#if UNITY_EDITOR
        private List<string> searchDirs = new List<string>();
        private List<string> resourcesDirs = new List<string>();
        private List<string> assetBundlesDirs = new List<string>();
        private Dictionary<string, string> findAssetPaths = new Dictionary<string, string>();
        private Dictionary<string, string> resourcesFindAssetPaths = new Dictionary<string, string>();
        private Dictionary<string, string> assetBundlesFindAssetPaths = new Dictionary<string, string>();
        private string SearchAsset(string assetName, AssetAccessMode accessMode = AssetAccessMode.Unknown)
        {
            // 缓存路径
            if (accessMode == AssetAccessMode.Unknown)
            {
                if (findAssetPaths.TryGetValue(assetName, out string value))
                {
                    return value;
                }
            }
            else if (accessMode == AssetAccessMode.Resource)
            {
                if (resourcesFindAssetPaths.TryGetValue(assetName, out string value))
                {
                    return value;
                }
            }
            else if (accessMode == AssetAccessMode.AssetBundle)
            {
                if (assetBundlesFindAssetPaths.TryGetValue(assetName, out string value))
                {
                    return value;
                }
            }

            if (searchDirs.Count <= 0)
            {
                // 获取项目中的所有文件夹路径
                string[] allFolders = UnityEditor.AssetDatabase.GetAllAssetPaths();
                foreach (string folderPath in allFolders)
                {
                    if (System.IO.Directory.Exists(folderPath) && folderPath.Contains("/Resources"))
                    {
                        searchDirs.Add(folderPath);
                        resourcesDirs.Add(folderPath);
                    }
                }
                searchDirs.Add(System.IO.Path.Combine(URLSetting.AssetBundlesPath));
                assetBundlesDirs.Add(System.IO.Path.Combine(URLSetting.AssetBundlesPath));
            }

            // 查找指定资源
            string[] dirs = null;
            if (accessMode == AssetAccessMode.Unknown)
            {
                dirs = searchDirs.ToArray();
            }
            else if (accessMode == AssetAccessMode.Resource)
            {
                dirs = resourcesDirs.ToArray();
            }
            else if (accessMode == AssetAccessMode.AssetBundle)
            {
                dirs = assetBundlesDirs.ToArray();
            }

            string[] guids = UnityEditor.AssetDatabase.FindAssets(assetName, dirs);
            foreach (string guid in guids)
            {
                // 将 GUID 转换为路径
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);

                if (Path.GetFileNameWithoutExtension(assetPath) == assetName)
                {
                    assetPath = assetPath.ToLower();
                    if (accessMode == AssetAccessMode.Unknown)
                    {
                        findAssetPaths[assetName] = assetPath;
                    }
                    else if (accessMode == AssetAccessMode.Resource)
                    {
                        resourcesFindAssetPaths[assetName] = assetPath;
                    }
                    else if (accessMode == AssetAccessMode.AssetBundle)
                    {
                        assetBundlesFindAssetPaths[assetName] = assetPath;
                    }

                    return assetPath;
                }
            }
            return null;
        }
#endif

        public void OnInit(object createParam)
        {
            //读取本地ab配置
            //GameConfig.LocalAssetBundleMap.ABMap = JsonConvert.DeserializeObject<Dictionary<string, AssetMapping>>(Resources.Load<TextAsset>(nameof(AssetBundleMap)).ToString());
            LoadLocalResMap();
            _assetBundleManager = ModuleCenter.CreateModule<AssetBundleMgr>();
            _resourcesManager = ModuleCenter.CreateModule<ResMgr>();
        }

        void LoadLocalResMap()
        {
            if (ResMap.ResMappings.Count == 0)
            {
                //读取本地资源映射表
                TextAsset textAsset = Resources.Load<TextAsset>(nameof(ResMap));
                if (textAsset == null)
                {
                    Debug.LogError($"找不到{nameof(ResMap)}.json文件");
                }
                else
                {
                    string text = textAsset.ToString();
                    ResMap.ResMappings = JsonConvert.DeserializeObject<Dictionary<string, ResMapping>>(text);
                }
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
            base.Destroy();
        }
    }
}