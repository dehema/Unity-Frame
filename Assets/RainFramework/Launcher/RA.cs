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
        // 输入管理
        private static InputMgr _inputManager;
        // 游戏时间管理
        private static TimerMgr _timer;
        // 资产管理
        private static AssetMgr _asset;
        // 音频管理
        private static AudioMgr _audio;
        // UI界面管理
        private static UIMgr _uiMgr;
        // 下载管理器
        private static DownloadMgr _downloadMgr;
        // 日志助手
        private static LogMgr _logMgr;
        // 热更新版本管理
        private static HotUpdateMgr _hotUpdateMgr;

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

        public static InputMgr Input
        {
            get
            {
                if (_inputManager == null)
                    _inputManager = ModuleCenter.CreateModule<InputMgr>(new DefaultInputHelper());
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

        public static DownloadMgr Download
        {
            get
            {
                if (_downloadMgr == null)
                    _downloadMgr = ModuleCenter.CreateModule<DownloadMgr>();
                return _downloadMgr;
            }
            set
            {
                if (_downloadMgr == null)
                    _downloadMgr = value;
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

        public static HotUpdateMgr HotUpdate
        {
            get
            {
                if (_hotUpdateMgr == null)
                    _hotUpdateMgr = ModuleCenter.CreateModule<HotUpdateMgr>();
                return _hotUpdateMgr;
            }
            set
            {
                if (_hotUpdateMgr == null)
                    _hotUpdateMgr = value;
            }
        }
    }
}

