using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public class AudioMgr : MonoSingleton<AudioMgr>
{
    public readonly string audioVolumeConfigPath = Application.streamingAssetsPath + "/AudioVolumeConfig.json";
    /// <summary>
    ///  音量配置
    /// </summary>
    AudioVolumeData volumeData = new AudioVolumeData();
    AudioSource soundAudio;
    AudioSource musicAudio;
    readonly string soundFilePath = "Audio/sound/";
    readonly string musicFilePath = "Audio/music/";
    public float soundVolume { get { return soundAudio.volume; } set { soundAudio.volume = value; } }
    public float musicVolume { get { return musicAudio.volume; } set { musicAudio.volume = value; } }
    public Dictionary<AudioMusic, OverlayMusicData> newMusicDict = new Dictionary<AudioMusic, OverlayMusicData>();

    private void Awake()
    {
        StartCoroutine(LoadConfig());
        soundAudio = new GameObject("soundAudio").AddComponent<AudioSource>();
        soundAudio.transform.SetParent(transform);
        musicAudio = new GameObject("musicAudio").AddComponent<AudioSource>();
        musicAudio.transform.SetParent(transform);
        musicAudio.loop = true;
    }

    private IEnumerator LoadConfig()
    {
        string path = audioVolumeConfigPath;
#if UNITY_IOS || UNITY_EDITOR_OSX
    path = "file://"+path;
#endif
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(path);
        yield return unityWebRequest.SendWebRequest();
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            string config = unityWebRequest.downloadHandler.text;
            volumeData = JsonConvert.DeserializeObject<AudioVolumeData>(config);
        }
        else
        {
            Debug.LogError("音频配置读取失败" + unityWebRequest.error);
        }
    }

    public float GetVolume(string _audioName, Type _type)
    {
        float val = 1f;
        if (_type == typeof(AudioSound))
        {
            if (volumeData.soundVolume.ContainsKey(_audioName))
            {
                val = volumeData.soundVolume[_audioName];
            }
        }
        else if (_type == typeof(AudioMusic))
        {
            if (volumeData.musicVolume.ContainsKey(_audioName))
            {
                val = volumeData.musicVolume[_audioName];
            }
        }
        return val;
    }

    /// <summary>
    /// 播放音乐
    /// </summary>
    /// <param name="_music"></param>
    public void PlayMusic(AudioMusic _music)
    {
        GetVolume(_music.ToString(), typeof(AudioMusic));
        musicAudio.clip = GetAudioClip(_music);
        musicAudio.Play();
    }
    public void Stop()
    {
        musicAudio.Play();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(AudioSound _sound)
    {
        if (_sound == AudioSound.None)
        {
            return;
        }
        AudioClip audioClip = GetAudioClip(_sound);
        if (audioClip != null)
        {
            soundAudio.PlayOneShot(audioClip, GetVolume(_sound.ToString(), typeof(AudioSound)));
        }
    }

    public AudioClip GetAudioClip(AudioSound _sound)
    {
        return Resources.Load<AudioClip>(soundFilePath + _sound.ToString());
    }

    public AudioClip GetAudioClip(AudioMusic _music)
    {
        return Resources.Load<AudioClip>(musicFilePath + _music.ToString());
    }

    /// <summary>
    /// 创建一个新的music可以叠加能主动停止
    /// </summary>
    public void AddOverlayMusic(AudioMusic _audioMusic, bool _loop = true)
    {
        if (newMusicDict.ContainsKey(_audioMusic))
        {
            return;
        }
        OverlayMusicData overlayData = new OverlayMusicData();
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        overlayData.audioSource = audio;
        audio.clip = GetAudioClip(_audioMusic);
        audio.loop = _loop;
        audio.Play();
        if (!_loop)
        {
            overlayData.timer = Timer.Ins.SetTimeOut(() =>
            {
                RemoveOverlayMusic(_audioMusic);
            }, audio.clip.length);
        }
        newMusicDict[_audioMusic] = overlayData;
    }

    /// <summary>
    /// 停止一个新的music播放
    /// </summary>
    public void RemoveOverlayMusic(AudioMusic _audioMusic)
    {
        if (!newMusicDict.ContainsKey(_audioMusic))
        {
            return;
        }
        OverlayMusicData overlayData = newMusicDict[_audioMusic];
        overlayData.timer?.Remove();
        Destroy(overlayData.audioSource);
        newMusicDict.Remove(_audioMusic);
    }

    /// <summary>
    /// 叠加背景音
    /// </summary>
    public class OverlayMusicData
    {
        public AudioSource audioSource;
        public TimerHandler timer;
    }
}

//音效
public enum AudioSound
{
    None,
    Sound_GoldCoin,
    Sound_OneArmBandit,
    Sound_PopShow,
    Sound_UIButton,
    Sound_Hint,
    Sound_Undo,
    Sound_PassLevel,
    Sound_ChestOpen,
}

//场景音效
public enum AudioMusic
{
    Sound_BGM,
}