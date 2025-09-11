using Rain.Core;
using Rain.UI;

namespace Rain.Launcher
{
    public static class RA
    {
        //�൱��������
        /* ------------------------����ģ��------------------------ */

        // ȫ����Ϣ
        private static MsgMgr _message;
        // �������-->ʹ������Ϣģ��
        private static InputManager _inputManager;
        // ���ش洢
        private static StorageManager _storage;
        // ��Ϸʱ�����-->ʹ������Ϣģ��
        private static TimerMgr _timer;
        // ���̹���
        private static ProcedureManager _procedure;
        // �������
        private static NetworkManager _networkManager;
        // ����״̬��
        private static FSMManager _fsm;
        // ��Ϸ�����
        private static GameObjectPool _gameObjectPool;
        // �ʲ�����
        private static AssetMgr _asset;
        // ��Ƶ����-->ʹ�����ʲ�ģ��-->ʹ������Ϸ�����ģ��-->ʹ���˲��䶯��ģ��-->ʹ����ʱ��ģ��
        private static AudioMgr _audio;
        // ���䶯��
        private static F8Tween _tween;
        // UI�������-->ʹ�����ʲ�ģ��
        private static UIManager _ui;
        private static UIMgr _uiMgr;
        // ���ػ�-->ʹ��������ģ��-->ʹ�����ʲ�ģ��
        private static Localization _localization;
        // SDK����-->ʹ������Ϣģ��
        private static SDKManager _sdkManager;
        // ���ع�����
        private static DownloadManager _downloadManager;
        // ��־����
        private static LogMgr _logMgr;


        /* ------------------------��ѡģ��------------------------ */
        // �ȸ��°汾����-->ʹ��������ģ��-->ʹ�����ʲ�ģ��
        private static HotUpdateManager _hotUpdateManager;

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

