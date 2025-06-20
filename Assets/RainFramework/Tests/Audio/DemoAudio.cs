using Rain.Core;
using Rain.Launcher;
using UnityEngine;
using UnityEngine.Audio;

namespace Rain.Tests
{
    public class DemoAudio : MonoBehaviour
    {
        void Start()
        {
            /*----------背景音乐----------*/
            //assetName资产名
            //callback播放完成回调
            //loop是否循环
            //priority优先级，高的覆盖低的
            //fadeDuration淡入持续时间
            RA.Audio.PlayMusic("assetName", CallBack, loop: true, priority: 1, fadeDuration: 3f); // 背景音乐
            float progress = RA.Audio.ProgressMusic; // 获取进度
            RA.Audio.SetProgressMusic = 0.5f; // 设置进度
            RA.Audio.VolumeMusic = 0.5f; // 设置音量，自动保存至PlayerPrefs
            RA.Audio.SwitchMusic = false; // 设置开关，自动保存至PlayerPrefs
            RA.Audio.SetMusicComplete(CallBack); // 设置完成回调
            
            /*----------人声----------*/
            RA.Audio.PlayVoice("assetName", CallBack, true, 1, 3f); // 角色语音
            float progressVoice = RA.Audio.ProgressVoice; // 获取进度
            RA.Audio.SetProgressVoice = 0.5f; // 设置进度
            RA.Audio.VolumeVoice = 0.5f; // 设置音量，自动保存至PlayerPrefs
            RA.Audio.SwitchVoice = false; // 设置开关，自动保存至PlayerPrefs
            RA.Audio.SetVoiceComplete(CallBack); // 设置完成回调
            
            /*----------特效声----------*/
            RA.Audio.PlayUISound("assetName", CallBack, true, 1, 3f); // ui音效
            RA.Audio.PlayBtnClick("assetName", CallBack, false, 2, 3f); // 按钮音效
            RA.Audio.PlayAudioEffect("assetName", CallBack, false, 2, 3f); // 音效特效
            RA.Audio.VolumeAudioEffect = 0.5f; // 设置音量，自动保存至PlayerPrefs
            RA.Audio.SwitchAudioEffect = false; // 设置开关，自动保存至PlayerPrefs
            RA.Audio.SetUISoundComplete(CallBack); // 设置完成回调
            RA.Audio.SetBtnClickComplete(CallBack); // 设置完成回调
            RA.Audio.SetAudioEffectComplete(CallBack); // 设置完成回调
            
            /*----------一次性3D音效----------*/
            //assetName资产名
            //isRandom是否随机音量音高
            //audioPosition音频播放位置
            //volume音量
            //spatialBlend2d到3d的比例
            //maxNum最大同时播放个数
            //callback播放完成回调
            RA.Audio.PlayAudioEffect3D("assetName", isRandom: true, transform.position, volume: 1f, spatialBlend: 1f, maxNum: 5, CallBack);
            RA.Audio.VolumeAudioEffect = 0.5f; // 设置音量，自动保存至PlayerPrefs
            RA.Audio.SwitchAudioEffect = false; // 设置开关，自动保存至PlayerPrefs
            
            /*----------其他功能----------*/
            
            //可选，设置混音组F8AudioMixer，需手动放到可加载目录
            RA.Audio.SetAudioMixer(RA.Asset.Load<AudioMixer>("F8AudioMixer"));

            /*----------全局控制----------*/
            RA.Audio.PauseAll(); // 暂停所有，不包括AudioEffect
            RA.Audio.ResumeAll(); // 恢复所有，不包括AudioEffect
            RA.Audio.StopAll(); // 停止所有，不包括AudioEffect

            RA.Audio.UnloadAll(true); // 卸载所有音频和音效。（true:完全卸载，包括正在使用的）

            void CallBack()
            {

            }
        }
    }
}
