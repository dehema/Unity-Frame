using System.Collections;
using Rain.Core;
using UnityEngine;
using Rain.UI;
using System;
using UnityEngine.UIElements;

namespace Rain.Launcher
{
    public class GameLauncher : MonoBehaviour
    {
        IEnumerator Start()
        {
            DontDestroyOnLoad(Camera.main);

            Application.targetFrameRate = 60;
            // 初始化模块中心
            ModuleCenter.Initialize(this);

            //热更模块
            RA.HotUpdate = ModuleCenter.CreateModule<HotUpdateMgr>();

            //按顺序创建模块，可按需添加
            RA.Msg = ModuleCenter.CreateModule<MsgMgr>();
            RA.Data = ModuleCenter.CreateModule<DataMgr>();
            RA.Download = ModuleCenter.CreateModule<DownloadMgr>();
            //RA.Input = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
            //RA.Storage = ModuleCenter.CreateModule<StorageManager>();
            RA.Timer = ModuleCenter.CreateModule<TimerMgr>();
            //RA.Procedure = ModuleCenter.CreateModule<ProcedureManager>();
            //RA.Network = ModuleCenter.CreateModule<NetworkManager>();
            //RA.FSM = ModuleCenter.CreateModule<FSMManager>();
            //RA.GameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
            RA.Asset = ModuleCenter.CreateModule<AssetMgr>();
#if UNITY_WEBGL
            yield return AssetBundleManager.Instance.LoadAssetBundleManifest(); // WebGL专用，如果游戏中没有使用任何AB包加载资源，可以删除此方法的调用！
#endif
            //RA.Config = ModuleCenter.CreateModule<F8DataManager>();
            RA.Audio = ModuleCenter.CreateModule<AudioMgr>();
            //RA.Tween = ModuleCenter.CreateModule<F8Tween>();
            RA.UIMgr = ModuleCenter.CreateModule<UIMgr>();
#if UNITY_WEBGL
            yield return F8DataManager.Instance.LoadLocalizedStringsIEnumerator(); // WebGL专用
#endif
            //RA.Local = ModuleCenter.CreateModule<Localization>();
            //RA.SDK = ModuleCenter.CreateModule<SDKManager>();
            //RA.Download = ModuleCenter.CreateModule<DownloadManager>();
            RA.Log = ModuleCenter.CreateModule<LogMgr>();
            RA.Scene = ModuleCenter.CreateModule<SceneMgr>();
            LogMgr.Ins.OnEnterGame();

            // 游戏初始化
            ConfigMgr.Ins.LoadAllConfig();
            DataMgr.Ins.Load();
            LangMgr.Ins.Init();
            UIMgr.Ins.Init(ConfigMgr.Ins.UIViewConfig);
            CityMgr.Ins.Init();

            //开启游戏
            StartGame();
            yield break;
        }

        // 开始游戏
        public void StartGame()
        {
#if UNITY_EDITOR
            Application.runInBackground = true;
            DebugView.DebugViewParam viewParam = new DebugView.DebugViewParam();
            viewParam.actionEnterGame = EnterGame;
            UIMgr.Ins.OpenView(ViewName.DebugView, viewParam);

            //UIMgr.Ins.OpenViewAsync(ViewName.DebugView);
            Util.Log("hello world");
#else
            //LoginViewParam param = new LoginViewParam();
            //param.action = () =>
            //{
            //    EnterGame();
            //};
            //UIMgr.Ins.OpenView<LoginView>(param);
            UIMgr.Ins.OpenView<LoginView>();
#endif
        }

        private void EnterGame()
        {
            SceneMgr.Ins.ChangeScene(SceneID.MainCity, () => { });

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

        private void OnApplicationPause(bool pause)
        {
            if (!pause && !Application.isEditor)
            {
                if (ModuleCenter.Contains<DataMgr>())
                {
                    DataMgr.Ins?.SaveGameData();
                }
            }
        }
    }
}