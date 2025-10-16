using Rain.Core;
using Rain.UI;

namespace Rain.Launcher
{
    public static class RA
    {
        //相当于重命名
        /* ------------------------核心模块------------------------ */

        // 数据
        private static DataMgr _dataMgr;
        // 全局消息
        private static MsgMgr _message;
        // 输入管理-->使用了消息模块
        private static InputManager _inputManager;
        // 游戏时间管理-->使用了消息模块
        private static TimerMgr _timer;
        // 资产管理
        private static AssetMgr _asset;
        // 音频管理-->使用了资产模块-->使用了游戏对象池模块-->使用了补间动画模块-->使用了时间模块
        private static AudioMgr _audio;
        // UI界面管理-->使用了资产模块
        private static UIMgr _uiMgr;
        // 下载管理器
        private static DownloadManager _downloadManager;
        // 日志助手
        private static LogMgr _logMgr;


        /* ------------------------可选模块------------------------ */
        // 热更新版本管理-->使用了下载模块-->使用了资产模块
        private static HotUpdateManager _hotUpdateManager;

        public static DataMgr Data
        {
            get
            {
                if (_dataMgr == null)
                    _dataMgr = ModuleCenter.CreateModule<DataMgr>();
                return _dataMgr;
            }
            set
            {
                if (_dataMgr == null)
                    _dataMgr = value;
            }
        }
        public static MsgMgr Msg
        {
            get
            {
                if (_message == null)
                    _message = ModuleCenter.CreateModule<MsgMgr>();
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

