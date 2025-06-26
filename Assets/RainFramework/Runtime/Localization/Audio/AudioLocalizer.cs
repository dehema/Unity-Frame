using UnityEngine;

namespace Rain.Core
{
	public class AudioLocalizer : LocalizerBase
	{
		public string localizedTextID = "";
		public AudioClip[] clips;
		public bool playFromSamePositionWhenInject;

		protected override void Prepare()
		{
			var component = ComponentFinder.Find<AudioSource>(this);
			if (component == null) return;

			if (component is AudioSource audio)
			{
				injector = new AudioSourceInjector(audio);
			}
		}

		internal override void Localize()
		{
			if (injector == null)
			{
				return;
			}
			if (!localizedTextID.IsNullOrEmpty())
			{
				ChangeID(localizedTextID);
				return;
			}
			var index = Localization.Ins.CurrentLanguageIndex;
			injector.Inject(clips?[index], this);
		}
		
		public bool ChangeID(string textId)
		{
			if (string.IsNullOrEmpty(textId)) return false;

#if UNITY_EDITOR
			// for Timeline Preview
			if (!Application.isPlaying)
			{
				Localization.Ins.Load();
				Prepare();
			}
#endif

			if (!Localization.Ins.Has(textId))
			{
				if (Application.isPlaying) RLog.LogError($"Text ID: {textId} 不可用。");
				return false;
			}

			this.localizedTextID = textId;
			var text = Localization.Ins.GetTextFromId(textId);
			injector.Inject(text, this);
			return true;
		}

		public void Clear()
		{
			localizedTextID = null;
			injector?.Inject("", this);
		}
	}
}
