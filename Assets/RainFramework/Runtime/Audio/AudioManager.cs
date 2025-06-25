using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Rain.Core
{
    [UpdateRefresh]
    public class AudioManager : ModuleSingletonMono<AudioManager>, IModule
    {
        /*----------��������----------*/
        private AudioMusic _audioMusic;
        private float _volumeMusic = 1f;
        private bool _switchMusic = true;

        /*----------����----------*/
        private AudioMusic _audioMusicVoice;
        private float _volumeVoice = 1f;
        private bool _switchVoice = true;

        /*----------��Ч��----------*/
        private AudioMusic _audioMusicBtnClick;

        private AudioMusic _audioMusicUISound;

        private AudioMusic _audioMusicAudioEffect;

        /*----------һ������Ч��----------*/
        private AudioEffect _audioMusicAudioEffect3D;

        private AudioMixerGroup _audioEffectMixerGroup;

        private float _volumeAudioEffect = 1f;
        private bool _switchAudioEffect = true;

        private Transform _transform;
        private AudioMixer _audioMixer;

        private const string _volumeMusicKey = "VolumeMusicKey";
        private const string _switchMusicKey = "SwitchMusicKey";
        private const string _volumeVoiceKey = "VolumeVoiceKey";
        private const string _switchVoiceKey = "SwitchVoiceKey";
        private const string _volumeAudioEffectKey = "VolumeAudioEffect";
        private const string _switchAudioEffectKey = "SwitchAudioEffect";
        public void OnInit(object createParam)
        {
            _transform = this.transform;
            GameObject gameObjectMusic = new GameObject("Music", typeof(AudioSource));
            gameObjectMusic.transform.SetParent(_transform);
            _audioMusic = new AudioMusic();
            _audioMusic.MusicSource = gameObjectMusic.GetComponent<AudioSource>();
            _audioMusic.MusicSource.playOnAwake = false;
            _audioMusic.MusicSource.loop = false;

            GameObject gameObjectVoice = new GameObject("Voice", typeof(AudioSource));
            gameObjectVoice.transform.SetParent(_transform);
            _audioMusicVoice = new AudioMusic();
            _audioMusicVoice.MusicSource = gameObjectVoice.GetComponent<AudioSource>();
            _audioMusicVoice.MusicSource.playOnAwake = false;
            _audioMusicVoice.MusicSource.loop = false;

            GameObject gameObjectBtnClick = new GameObject("BtnClick", typeof(AudioSource));
            gameObjectBtnClick.transform.SetParent(_transform);
            _audioMusicBtnClick = new AudioMusic();
            _audioMusicBtnClick.MusicSource = gameObjectBtnClick.GetComponent<AudioSource>();
            _audioMusicBtnClick.MusicSource.playOnAwake = false;
            _audioMusicBtnClick.MusicSource.loop = false;

            GameObject gameObjectUISound = new GameObject("UISound", typeof(AudioSource));
            gameObjectUISound.transform.SetParent(_transform);
            _audioMusicUISound = new AudioMusic();
            _audioMusicUISound.MusicSource = gameObjectUISound.GetComponent<AudioSource>();
            _audioMusicUISound.MusicSource.playOnAwake = false;
            _audioMusicUISound.MusicSource.loop = false;

            GameObject gameObjectAudioEffect = new GameObject("AudioEffect", typeof(AudioSource));
            gameObjectAudioEffect.transform.SetParent(_transform);
            _audioMusicAudioEffect = new AudioMusic();
            _audioMusicAudioEffect.MusicSource = gameObjectAudioEffect.GetComponent<AudioSource>();
            _audioMusicAudioEffect.MusicSource.playOnAwake = false;
            _audioMusicAudioEffect.MusicSource.loop = false;

            _audioMusicAudioEffect3D = new AudioEffect();

            _volumeMusic = StorageManager.Instance.GetFloat(_volumeMusicKey, 1f);
            _switchMusic = StorageManager.Instance.GetBool(_switchMusicKey, true);

            _volumeVoice = StorageManager.Instance.GetFloat(_volumeVoiceKey, 1f);
            _switchVoice = StorageManager.Instance.GetBool(_switchVoiceKey, true);

            _volumeAudioEffect = StorageManager.Instance.GetFloat(_volumeAudioEffectKey, 1f);
            _switchAudioEffect = StorageManager.Instance.GetBool(_switchAudioEffectKey, true);
        }

        /// <summary>
        /// ����AudioMixer������
        /// </summary>
        /// <param name="audioMixer"></param>
        public void SetAudioMixer(AudioMixer audioMixer)
        {
            _audioMusic.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
            _audioMusicVoice.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/Voice")[0];
            _audioMusicBtnClick.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioMusicUISound.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioMusicAudioEffect.MusicSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioEffectMixerGroup = audioMixer.FindMatchingGroups("Master/SoundFx")[0];
            _audioMixer = audioMixer;
        }

        public void OnUpdate()
        {
            _audioMusic.Tick();
            _audioMusicVoice.Tick();
            _audioMusicBtnClick.Tick();
            _audioMusicUISound.Tick();
            _audioMusicAudioEffect.Tick();
        }

        public void OnLateUpdate()
        {

        }

        public void OnFixedUpdate()
        {

        }

        public void OnTermination()
        {
            StopAll();
            F8Tween.Instance.CancelTween(_audioMusic.AudioTween);
            F8Tween.Instance.CancelTween(_audioMusicVoice.AudioTween);
            F8Tween.Instance.CancelTween(_audioMusicBtnClick.AudioTween);
            F8Tween.Instance.CancelTween(_audioMusicUISound.AudioTween);
            F8Tween.Instance.CancelTween(_audioMusicAudioEffect.AudioTween);

            Destroy(gameObject);
        }

        /*----------��������----------*/

        // ���ñ������ֲ�����ɻص�
        public void SetMusicComplete(Action callback)
        {
            _audioMusic.OnComplete = callback;
        }

        /// <summary>
        /// ���ű������֡�
        /// </summary>
        /// <param name="assetName">�ʲ�����</param>
        /// <param name="callback">������ɻص���</param>
        /// <param name="loop">�Ƿ�ѭ����</param>
        /// <param name="priority">���ȼ����ߵĸ��ǵ͵ġ�</param>
        /// <param name="fadeDuration">�������ʱ�䡣</param>
        public void PlayMusic(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchMusic)
            {
                return;
            }
            if (priority < _audioMusic.Priority)
            {
                return;
            }
            _audioMusic.Load(assetName, callback, loop, priority, fadeDuration);
        }

        // ���ñ����ֲ��Ž���
        public float SetProgressMusic
        {
            set => _audioMusic.Progress = value;
        }

        // ��ȡ�������ֲ��Ž���
        public float ProgressMusic => _audioMusic.Progress;

        // ��ȡ�����ñ�����������
        public float VolumeMusic
        {
            get => _volumeMusic;
            set
            {
                _volumeMusic = value;
                StorageManager.Instance.SetFloat(_volumeMusicKey, value);
                _audioMusic.MusicSource.volume = value;
            }
        }

        // ���ñ�����������
        public void ResetMusic()
        {
            VolumeMusic = _volumeMusic;
        }

        // ���úͻ�ȡ�������ֿ���ֵ
        public bool SwitchMusic
        {
            get => _switchMusic;
            set
            {
                _switchMusic = value;
                StorageManager.Instance.SetBool(_switchMusicKey, value);
                if (!value)
                {
                    _audioMusic.MusicSource.Stop();
                }
            }
        }

        /*----------����----------*/

        // ��������������ɻص�
        public void SetVoiceComplete(Action callback)
        {
            _audioMusicVoice.OnComplete = callback;
        }

        // ��������
        public void PlayVoice(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchVoice)
            {
                return;
            }
            if (priority < _audioMusicVoice.Priority)
            {
                return;
            }
            _audioMusicVoice.Load(assetName, callback, loop, priority, fadeDuration);
        }

        // �����������Ž���
        public float SetProgressVoice
        {
            set => _audioMusicVoice.Progress = value;
        }

        // ��ȡ�������Ž���
        public float ProgressVoice => _audioMusicVoice.Progress;

        // ��ȡ��������������
        public float VolumeVoice
        {
            get => _volumeVoice;
            set
            {
                _volumeVoice = value;
                StorageManager.Instance.SetFloat(_volumeVoiceKey, value);
                _audioMusicVoice.MusicSource.volume = value;
            }
        }

        // ������������
        public void ResetVoice()
        {
            VolumeVoice = _volumeVoice;
        }

        // ���úͻ�ȡ��������ֵ
        public bool SwitchVoice
        {
            get => _switchVoice;
            set
            {
                _switchVoice = value;
                StorageManager.Instance.SetBool(_switchVoiceKey, value);
                if (!value)
                {
                    _audioMusicVoice.MusicSource.Stop();
                }
            }
        }

        /*----------��Ч��Ч����----------*/

        // ��ȡ��������Ч����
        public float VolumeAudioEffect
        {
            get => _volumeAudioEffect;
            set
            {
                _volumeAudioEffect = value;
                StorageManager.Instance.SetFloat(_volumeAudioEffectKey, value);
                _audioMusicBtnClick.MusicSource.volume = value;
                _audioMusicUISound.MusicSource.volume = value;
                _audioMusicAudioEffect.MusicSource.volume = value;
            }
        }

        // ������Ч����
        public void ResetAudioEffect()
        {
            VolumeAudioEffect = _volumeAudioEffect;
        }

        // ���úͻ�ȡ��Ч��������ֵ
        public bool SwitchAudioEffect
        {
            get => _switchAudioEffect;
            set
            {
                _switchAudioEffect = value;
                StorageManager.Instance.SetBool(_switchAudioEffectKey, value);
                if (!value)
                {
                    _audioMusicBtnClick.MusicSource.Stop();
                    _audioMusicUISound.MusicSource.Stop();
                    _audioMusicAudioEffect.MusicSource.Stop();
                }
            }
        }

        /*----------��ť��Ч��Ч----------*/

        // ���ð�ť��Ч������ɻص�
        public void SetBtnClickComplete(Action callback)
        {
            _audioMusicBtnClick.OnComplete = callback;
        }

        // ���Ű�ť��Ч
        public void PlayBtnClick(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < _audioMusicBtnClick.Priority)
            {
                return;
            }
            _audioMusicBtnClick.Load(assetName, callback, loop, priority, fadeDuration);
        }

        /*----------UI��Ч��Ч----------*/

        // ����UI��Ч������ɻص�
        public void SetUISoundComplete(Action callback)
        {
            _audioMusicUISound.OnComplete = callback;
        }

        // ����UI��Ч
        public void PlayUISound(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < _audioMusicUISound.Priority)
            {
                return;
            }
            _audioMusicUISound.Load(assetName, callback, loop, priority, fadeDuration);
        }

        /*----------��Ч��Ч----------*/

        // ������Ч��Ч������ɻص�
        public void SetAudioEffectComplete(Action callback)
        {
            _audioMusicAudioEffect.OnComplete = callback;
        }

        // ������Ч��Ч
        public void PlayAudioEffect(string assetName, Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            if (priority < _audioMusicAudioEffect.Priority)
            {
                return;
            }
            _audioMusicAudioEffect.Load(assetName, callback, loop, priority, fadeDuration);
        }

        /*----------һ����3D��Ч��Ч----------*/
        /// <summary>
        /// ����һ����3D��Ч��Ч��
        /// </summary>
        /// <param name="assetName">�ʲ�����</param>
        /// <param name="isRandom">�Ƿ�����������ߡ�</param>
        /// <param name="audioPosition">��Ƶ����λ�á�</param>
        /// <param name="volume">������</param>
        /// <param name="spatialBlend">2d��3d�ı�����</param>
        /// <param name="maxNum">���ͬʱ���Ÿ�����</param>
        /// <param name="callback">������ɻص���</param>
        public void PlayAudioEffect3D(string assetName, bool isRandom = false, Vector3? audioPosition = null, float volume = 1f, float spatialBlend = 1f,
            int maxNum = 5, Action callback = null)
        {
            if (!_switchAudioEffect)
            {
                return;
            }
            Vector3 actualPosition = audioPosition.GetValueOrDefault(_transform.position);
            float actualVolume = volume * _volumeAudioEffect;
            _audioMusicAudioEffect3D.Load(assetName, actualPosition, actualVolume, spatialBlend, maxNum, callback, _audioEffectMixerGroup, isRandom);
        }

        /*----------ȫ�ֿ���----------*/
        public void ResumeAll()
        {
            _audioMusic.MusicSource.Play();
            _audioMusicVoice.MusicSource.Play();
            _audioMusicBtnClick.MusicSource.Play();
            _audioMusicUISound.MusicSource.Play();
            _audioMusicAudioEffect.MusicSource.Play();
        }

        public void PauseAll()
        {
            _audioMusic.MusicSource.Pause();
            _audioMusicVoice.MusicSource.Pause();
            _audioMusicBtnClick.MusicSource.Pause();
            _audioMusicUISound.MusicSource.Pause();
            _audioMusicAudioEffect.MusicSource.Pause();
        }

        public void StopAll()
        {
            _audioMusic.MusicSource.Stop();
            _audioMusicVoice.MusicSource.Stop();
            _audioMusicBtnClick.MusicSource.Stop();
            _audioMusicUISound.MusicSource.Stop();
            _audioMusicAudioEffect.MusicSource.Stop();
        }

        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            _audioMusic.UnloadAll(unloadAllLoadedObjects);
            _audioMusicVoice.UnloadAll(unloadAllLoadedObjects);
            _audioMusicBtnClick.UnloadAll(unloadAllLoadedObjects);
            _audioMusicUISound.UnloadAll(unloadAllLoadedObjects);
            _audioMusicAudioEffect.UnloadAll(unloadAllLoadedObjects);
            _audioMusicAudioEffect3D.UnloadAll(unloadAllLoadedObjects);
        }

        private float Remap01ToDB(float linearVolume)
        {
            // �������Ϊ0�������������Ϊһ���ǳ�С���������Ա�������������
            if (linearVolume <= 0.0f)
            {
                linearVolume = 0.0001f;
            }

            // ����������ֵת��Ϊ�ֱ�ֵ
            float dbVolume = Mathf.Log10(linearVolume) * 20.0f;

            return dbVolume;
        }
    }
}