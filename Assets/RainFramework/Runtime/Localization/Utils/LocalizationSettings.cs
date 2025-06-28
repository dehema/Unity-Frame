using UnityEngine;

namespace Rain.Core
{
	public static class LocalizationSettings
	{
		class Definition
		{
			public string currentLanguageName;
		}

		public static string LoadLanguageSettings()
		{
#if UNITY_EDITOR
			var json = UnityEditor.EditorPrefs.GetString(Application.dataPath.GetHashCode() + LocalizationConst.CurrentLanguageKey, "");
			if (json == "")
			{
				// 根据系统语言设置
				Localization.Ins.CurrentLanguageName = Application.systemLanguage.ToString();
				return Application.systemLanguage.ToString();
			}
			else
			{
				var definition = JsonUtility.FromJson<Definition>(json);
				Localization.Ins.CurrentLanguageName = definition.currentLanguageName;
				return definition.currentLanguageName;
			}
#else
			Definition definition = StorageManager.Ins.GetObject<Definition>(LocalizationConst.CurrentLanguageKey, true);
			if (definition == null)
			{
				Localization.Ins.CurrentLanguageName = Application.systemLanguage.ToString();
			}
			else
			{
				Localization.Ins.CurrentLanguageName = definition.currentLanguageName;
			}
			return Localization.Ins.CurrentLanguageName;
#endif
		}

		public static void SaveLanguageSettings()
		{
#if UNITY_EDITOR
			var definition = new Definition { currentLanguageName = Localization.Ins.CurrentLanguageName };
			var json = JsonUtility.ToJson(definition);
			UnityEditor.EditorPrefs.SetString(Application.dataPath.GetHashCode() + LocalizationConst.CurrentLanguageKey, json);
#else
			var definition = new Definition { currentLanguageName = Localization.Ins.CurrentLanguageName };
			StorageManager.Ins.SetObject<Definition>(LocalizationConst.CurrentLanguageKey, definition, true);
#endif
        }
    }
}
