using System.Collections;
using System.Collections.Generic;
using Rain.Core;
using Rain.F8ExcelDataClass;
using Rain.Launcher;
using Rain.UI;
using UnityEngine;

namespace Rain.Tests
{
    public class TestLauncher : MonoBehaviour
    {
        IEnumerator Start()
        {
            // 初始化模块中心
            ModuleCenter.Initialize(this);

            // 初始化版本
            RA.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();

            // 按顺序创建模块，可按需添加
            RA.Message = ModuleCenter.CreateModule<MessageManager>();
            RA.Input = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
            RA.Storage = ModuleCenter.CreateModule<StorageManager>();
            RA.Timer = ModuleCenter.CreateModule<TimerMgr>();
            RA.Procedure = ModuleCenter.CreateModule<ProcedureManager>();
            RA.Network = ModuleCenter.CreateModule<NetworkManager>();
            RA.FSM = ModuleCenter.CreateModule<FSMManager>();
            RA.GameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
            RA.Asset = ModuleCenter.CreateModule<AssetMgr>();
#if UNITY_WEBGL
            yield return AssetBundleManager.Instance.LoadAssetBundleManifest(); // WebGL专用，如果游戏中没有使用任何AB包加载资源，可以删除此方法的调用！
#endif
            RA.Config = ModuleCenter.CreateModule<F8DataManager>();
            RA.Audio = ModuleCenter.CreateModule<AudioMgr>();
            RA.Tween = ModuleCenter.CreateModule<F8Tween>();
            RA.UI = ModuleCenter.CreateModule<UIManager>();
#if UNITY_WEBGL
            yield return F8DataManager.Instance.LoadLocalizedStringsIEnumerator(); // WebGL专用
#endif
            RA.Local = ModuleCenter.CreateModule<Localization>();
            RA.SDK = ModuleCenter.CreateModule<SDKManager>();
            RA.Download = ModuleCenter.CreateModule<DownloadManager>();
            RA.Log = ModuleCenter.CreateModule<LogMgr>();

            StartGame();
            yield break;
        }

        // 开始游戏
        public void StartGame()
        {

        }

        void Update()
        {
            // 更新模块
            ModuleCenter.Update();
        }

        void LateUpdate()
        {
            // 更新模块
            ModuleCenter.LateUpdate();
        }

        void FixedUpdate()
        {
            // 更新模块
            ModuleCenter.FixedUpdate();
        }
    }
}
