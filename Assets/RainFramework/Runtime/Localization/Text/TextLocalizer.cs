#if LOCALIZER_TMP
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Rain.Core
{
	public class TextLocalizer : LocalizerBase
	{
		public string textId;

		protected override void Prepare()
		{
#if LOCALIZER_TMP
			var component = ComponentFinder.Find<TextMesh, Text, TMP_Text>(this);
#else
			var component = ComponentFinder.Find<TextMesh, Text>(this);
#endif
			if (component == null) return;

			if (component is TextMesh textMesh)
			{
				injector = new TextMeshInjector(textMesh);
			}
			else if (component is Text text)
			{
				injector = new UITextInjector(text);
			}
#if LOCALIZER_TMP
			else if (component is TMP_Text tmp)
			{
				injector = new TMPInjector(tmp);
			}
#endif
		}

		internal override void Localize()
		{
			if (injector == null)
			{
				return;
			}
			ChangeID(textId);
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

			this.textId = textId;
			var text = Localization.Ins.GetTextFromId(textId);
			injector.Inject(text, this);
			return true;
		}

		public void Clear()
		{
			textId = null;
			injector?.Inject("", this);
		}
	}
}
