using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Rain.Core
{
    public class AudioMusic
    {
        private Dictionary<string, AudioClip> _audios = new Dictionary<string, AudioClip>();
        // �������ֲ�����ɻص�
        public System.Action OnComplete;
        private float _progress = 0;
        private bool _isPlay = false;
        public int Priority;
        public AudioSource MusicSource;
        public int AudioTween;

        // ��ȡ���ֲ��Ž���
        public float Progress
        {
            get
            {
                if (MusicSource.clip && MusicSource.clip.length > 0)
                {
                    _progress = MusicSource.time / MusicSource.clip.length;
                }
                return _progress;
            }
            set
            {
                _progress = value;
                MusicSource.time = value * MusicSource.clip.length;
            }
        }

        // �������ֲ�����
        public void Load(string url, System.Action callback = null, bool loop = false, int priority = 0, float fadeDuration = 0f)
        {
            MusicSource.loop = loop;
            Priority = priority;

            if (_audios != null && _audios.TryGetValue(url, out AudioClip audioClip) && audioClip)
            {
                PlayClip(audioClip, callback, fadeDuration);
            }
            else
            {
                AssetMgr.Ins.LoadAsync<AudioClip>(url, (asset) =>
                {
                    _audios[url] = asset;

                    PlayClip(_audios[url], callback, fadeDuration);
                });
            }
        }

        private void PlayClip(AudioClip audioClip, System.Action callback = null, float fadeDuration = 0f)
        {
            if (MusicSource.isPlaying)
            {
                _isPlay = false;
                MusicSource.Stop();
                OnComplete?.Invoke();
            }

            MusicSource.clip = audioClip;

            OnComplete = callback;

            MusicSource.Play();

            if (fadeDuration > 0f)
            {
                float tempVolume = MusicSource.volume;
                MusicSource.volume = 0f;
                //AudioTween = F8Tween.Ins.ValueTween(0f, tempVolume, fadeDuration)
                //    .SetOnUpdateFloat((float v) => { MusicSource.volume = v; }).ID;
            }
        }

        public void Tick()
        {
            if (MusicSource.clip && MusicSource.time > 0)
            {
                _isPlay = true;
            }
            if (_isPlay && !MusicSource.isPlaying)
            {
                _isPlay = false;
                OnComplete?.Invoke();
            }
        }

        // �ͷ�����������Դ
        public void UnloadAll(bool unloadAllLoadedObjects = true)
        {
            foreach (var item in _audios)
            {
                AssetMgr.Ins.Unload(item.Key, unloadAllLoadedObjects);
            }
            _audios.Clear();
        }
    }
}