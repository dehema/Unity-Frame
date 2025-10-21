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
            if (!Application.isEditor)
            {
                Application.targetFrameRate = 60;
            }
            // 初始化模块中心
            ModuleCenter.Initialize(this);

            //日志
            RA.Log = ModuleCenter.CreateModule<LogMgr>();
            //消息
            RA.Msg = ModuleCenter.CreateModule<MsgMgr>();
            //数据
            RA.Data = ModuleCenter.CreateModule<DataMgr>();
            //下载
            RA.Download = ModuleCenter.CreateModule<DownloadMgr>();
            //计时
            RA.Timer = ModuleCenter.CreateModule<TimerMgr>();
            //资源
            RA.Asset = ModuleCenter.CreateModule<AssetMgr>();
            //音频
            RA.Audio = ModuleCenter.CreateModule<AudioMgr>();
            //UI
            RA.UIMgr = ModuleCenter.CreateModule<UIMgr>();
            //场景
            RA.Scene = ModuleCenter.CreateModule<SceneMgr>();
            //热更
            RA.HotUpdate = ModuleCenter.CreateModule<HotUpdateMgr>();
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