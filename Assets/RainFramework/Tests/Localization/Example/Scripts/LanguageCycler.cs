using System.Collections;
using Rain.Core;
using Rain.Launcher;
using Rain.UI;
using UnityEngine;

namespace Rain.Tests
{
	public class LanguageCycler : MonoBehaviour
	{
		public GameObject StoryCanvas;
		public GameObject UICanvas;
		IEnumerator Start()
		{
			// 初始化模块中心
			ModuleCenter.Initialize(this);
			
			// 初始化版本
			RA.HotUpdate = ModuleCenter.CreateModule<HotUpdateManager>();
        
			// 按顺序创建模块，可按需添加
			RA.Msg = ModuleCenter.CreateModule<MsgMgr>();
			RA.Input = ModuleCenter.CreateModule<InputManager>(new DefaultInputHelper());
			RA.Storage = ModuleCenter.CreateModule<StorageManager>();
			RA.Timer = ModuleCenter.CreateModule<TimerMgr>();
			RA.Procedure = ModuleCenter.CreateModule<ProcedureManager>();
			RA.Network = ModuleCenter.CreateModule<NetworkManager>();
			RA.FSM = ModuleCenter.CreateModule<FSMManager>();
			RA.GameObjectPool = ModuleCenter.CreateModule<GameObjectPool>();
			RA.Asset = ModuleCenter.CreateModule<AssetMgr>();
#if UNITY_WEBGL
            yield return AssetBundleManager.Instance.LoadAssetBundleManifest(); // WebGL专用，如果游戏中没有使用任何AB包加载资源，可以删除此方法的调用！
#endif
			RA.Audio = ModuleCenter.CreateModule<AudioMgr>();
			RA.Tween = ModuleCenter.CreateModule<F8Tween>();
			RA.UI = ModuleCenter.CreateModule<UIManager>();
#if UNITY_WEBGL
            yield return F8DataManager.Instance.LoadLocalizedStringsIEnumerator(); // WebGL专用
#endif
			RA.Local = ModuleCenter.CreateModule<Localization>();
			RA.SDK = ModuleCenter.CreateModule<SDKManager>();
			RA.Download = ModuleCenter.CreateModule<DownloadManager>();
			RA.Log = ModuleCenter.CreateModule<LogMgr>();
			
			StoryCanvas.SetActive(true);
			UICanvas.SetActive(true);
			yield break;
		}

		void Update()
		{
			// 更新框架
			ModuleCenter.Update();
			if (Input.GetKeyDown(KeyCode.Return))
			{
				Cycle();
			}
		}

		void LateUpdate()
		{
			// 更新模块
			ModuleCenter.LateUpdate();
		}

		private void FixedUpdate()
		{
			ModuleCenter.FixedUpdate();
		}

		public void Cycle()
		{
			Localization.Ins.ActivateNextLanguage();
		}
	}
}

