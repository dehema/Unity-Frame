using Rain.F8ExcelDataClass;
using Rain.Core;
using Rain.UI;

namespace Rain.Launcher
{
    public static class RA
    {
        //相当于重命名
        /* ------------------------核心模块------------------------ */

        // 全局消息
        private static MessageManager _message;
        // 输入管理-->使用了消息模块
        private static InputManager _inputManager;
        // 本地存储
        private static StorageManager _storage;
        // 游戏时间管理-->使用了消息模块
        private static TimerMgr _timer;
        // 流程管理
        private static ProcedureManager _procedure;
        // 网络管理
        private static NetworkManager _networkManager;
        // 有限状态机
        private static FSMManager _fsm;
        // 游戏对象池
        private static GameObjectPool _gameObjectPool;
        // 资产管理
        private static AssetMgr _asset;
        // 读取配置表-->使用了资产模块
        private static F8DataManager _config;
        // 音频管理-->使用了资产模块-->使用了游戏对象池模块-->使用了补间动画模块-->使用了时间模块
        private static AudioMgr _audio;
        // 补间动画
        private static F8Tween _tween;
        // UI界面管理-->使用了资产模块
        private static UIManager _ui;
        private static UIMgr _uiMgr;
        // 本地化-->使用了配置模块-->使用了资产模块
        private static Localization _localization;
        // SDK管理-->使用了消息模块
        private static SDKManager _sdkManager;
        // 下载管理器
        private static DownloadManager _downloadManager;
        // 日志助手
        private static LogMgr _logMgr;


        /* ------------------------可选模块------------------------ */
        // 热更新版本管理-->使用了下载模块-->使用了资产模块
        private static HotUpdateManager _hotUpdateManager;

        public static MessageManager Message
        {
            get
            {
                if (_message == null)
                    _message = ModuleCenter.CreateModule<MessageManager>();
                return _message;
            }
            set
            {
                if (_message == null)
                    _message = value;
            }
        }

        public static InputManager Input
        {
            get
            {
                if (_inputManager == null)
                    _inputManager = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
                return _inputManager;
            }
            set
            {
                if (_inputManager == null)
                    _inputManager = value;
            }
        }

        public static StorageManager Storage
        {
            get
            {
                if (_storage == null)
                    _storage = ModuleCenter.CreateModule<StorageManager>();
                return _storage;
            }
            set
            {
                if (_storage == null)
                    _storage = value;
            }
        }

        public static TimerMgr Timer
        {
            get
            {
                if (_timer == null)
                    _timer = ModuleCenter.CreateModule<TimerMgr>();
                return _timer;
            }
            set
            {
                if (_timer == null)
                    _timer = value;
            }
        }

        public static ProcedureManager Procedure
        {
            get
            {
                if (_procedure == null)
                    _procedure = ModuleCenter.CreateModule<ProcedureManager>();
                return _procedure;
            }
            set
            {
                if (_procedure == null)
                    _procedure = value;
            }
        }

        public static NetworkManager Network
        {
            get
            {
                if (_networkManager == null)
                    _networkManager = ModuleCenter.CreateModule<NetworkManager>();
                return _networkManager;
            }
            set
            {
                if (_networkManager == null)
                    _networkManager = value;
            }
        }

        public static FSMManager FSM
        {
            get
            {
                if (_fsm == null)
                    _fsm = ModuleCenter.CreateModule<FSMManager>();
                return _fsm;
            }
            set
            {
                if (_fsm == null)
                    _fsm = value;
            }
        }

        public static GameObjectPool GameObjectPool
        {
            get
            {
                if (_gameObjectPool == null)
                {
                    _gameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
                    ModuleCenter.CreateModule<F8PoolGlobal>();
                }

                return _gameObjectPool;
            }
            set
            {
                if (_gameObjectPool == null)
                {
                    _gameObjectPool = value;
                    ModuleCenter.CreateModule<F8PoolGlobal>();
                }
            }
        }

        public static AssetMgr Asset
        {
            get
            {
                if (_asset == null)
                    _asset = ModuleCenter.CreateModule<AssetMgr>();
                return _asset;
            }
            set
            {
                if (_asset == null)
                    _asset = value;
            }
        }

        public static F8DataManager Config
        {
            get
            {
                if (_config == null)
                    _config = ModuleCenter.CreateModule<F8DataManager>();
                return _config;
            }
            set
            {
                if (_config == null)
                    _config = value;
            }
        }

        public static AudioMgr Audio
        {
            get
            {
                if (_audio == null)
                    _audio = ModuleCenter.CreateModule<AudioMgr>();
                return _audio;
            }
            set
            {
                if (_audio == null)
                    _audio = value;
            }
        }

        public static F8Tween Tween
        {
            get
            {
                if (_tween == null)
                    _tween = ModuleCenter.CreateModule<F8Tween>();
                return _tween;
            }
            set
            {
                if (_tween == null)
                    _tween = value;
            }
        }

        public static UIManager UI
        {
            get
            {
                if (_ui == null)
                    _ui = ModuleCenter.CreateModule<UIManager>();
                return _ui;
            }
            set
            {
                if (_ui == null)
                    _ui = value;
            }
        }

        public static UIMgr UIMgr
        {
            get
            {
                if (_uiMgr == null)
                    _uiMgr = ModuleCenter.CreateModule<UIMgr>();
                return _uiMgr;
            }
            set
            {
                if (_uiMgr == null)
                    _uiMgr = value;
            }
        }

        public static Localization Local
        {
            get
            {
                if (_localization == null)
                    _localization = ModuleCenter.CreateModule<Localization>();
                return _localization;
            }
            set
            {
                if (_localization == null)
                    _localization = value;
            }
        }

        public static SDKManager SDK
        {
            get
            {
                if (_sdkManager == null)
                    _sdkManager = ModuleCenter.CreateModule<SDKManager>();
                return _sdkManager;
            }
            set
            {
                if (_sdkManager == null)
                    _sdkManager = value;
            }
        }

        public static DownloadManager Download
        {
            get
            {
                if (_downloadManager == null)
                    _downloadManager = ModuleCenter.CreateModule<DownloadManager>();
                return _downloadManager;
            }
            set
            {
                if (_downloadManager == null)
                    _downloadManager = value;
            }
        }

        public static LogMgr Log
        {
            get
            {
                if (_logMgr == null)
                    _logMgr = ModuleCenter.CreateModule<LogMgr>();
                return _logMgr;
            }
            set
            {
                if (_logMgr == null)
                    _logMgr = value;
            }
        }

        public static HotUpdateManager HotUpdate
        {
            get
            {
                if (_hotUpdateManager == null)
                    _hotUpdateManager = ModuleCenter.CreateModule<HotUpdateManager>();
                return _hotUpdateManager;
            }
            set
            {
                if (_hotUpdateManager == null)
                    _hotUpdateManager = value;
            }
        }
    }
}

