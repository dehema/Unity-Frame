using Rain.Core;
using Rain.UI;

namespace Rain.Launcher
{
    public static class RA
    {
        //�൱��������
        /* ------------------------����ģ��------------------------ */

        // ����
        private static DataMgr _dataMgr;
        // ȫ����Ϣ
        private static MsgMgr _message;
        // �������-->ʹ������Ϣģ��
        private static InputManager _inputManager;
        // ��Ϸʱ�����-->ʹ������Ϣģ��
        private static TimerMgr _timer;
        // �ʲ�����
        private static AssetMgr _asset;
        // ��Ƶ����-->ʹ�����ʲ�ģ��-->ʹ������Ϸ�����ģ��-->ʹ���˲��䶯��ģ��-->ʹ����ʱ��ģ��
        private static AudioMgr _audio;
        // UI�������-->ʹ�����ʲ�ģ��
        private static UIMgr _uiMgr;
        // ���ع�����
        private static DownloadManager _downloadManager;
        // ��־����
        private static LogMgr _logMgr;


        /* ------------------------��ѡģ��------------------------ */
        // �ȸ��°汾����-->ʹ��������ģ��-->ʹ�����ʲ�ģ��
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

