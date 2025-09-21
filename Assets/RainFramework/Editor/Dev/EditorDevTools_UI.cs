using System;
using System.Collections.Generic;
using System.IO;
using Rain.UI;
using Rain.UI.Editor;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class EditorDevTools_UI : EditorDevTools_Base
{
    public EditorDevTools_UI(EditorWindow mainWindow, List<EditorDevTools_Base> subModules = null)
        : base(mainWindow, subModules)
    {
        this.pageName = "UI";
        UIMgr.ActionExportUI_Editor = () => { EditorExportUI.ExportViewUI(); };
    }

    public override void DrawContent()
    {
        //if (!EditorApplication.isPlaying)
        //{
        //    GUILayout.BeginVertical();
        //    {
        //        GUILayout.Label("\n 请先运行游戏");
        //    }
        //    GUILayout.EndVertical();
        //    return;
        //}
        //else
        //{
        //}
        DrawGUI();
    }

    void DrawGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("TextMeshPro代替Text", style.bt))
        {
            TextMeshProReplaceText();
        }
        if (GUILayout.Button("替换所有丢失字体的文本", style.bt))
        {
            ReplaceAllFont(true);
        }
        if (GUILayout.Button("替换所有文本的字体", style.bt))
        {
            ReplaceAllFont();
        }
        newfont = EditorGUILayout.ObjectField(newfont, typeof(Font), true) as Font;
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("删除所有丢失的脚本组件", style.bt))
        {
            DelAllMissScripts();
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// TextMeshPro代替Text
    /// </summary>
    public void TextMeshProReplaceText()
    {
        Stage stage = StageUtility.GetCurrentStage();
        // 判断是否是预制体编辑阶段
        if (stage is not PrefabStage)
            return;
        PrefabStage prefabStage = stage as PrefabStage;
        GameObject prefabContentsRoot = prefabStage.prefabContentsRoot;
        // 获取场景中所有的Text组件
        Text[] textComponents = prefabContentsRoot.GetComponentsInChildren<Text>();

        if (textComponents.Length == 0)
        {
            Debug.Log("场景中没有找到Text组件");
            return;
        }

        int replaceCount = 0;

        foreach (Text textComponent in textComponents)
        {
            // 记录Undo操作
            Undo.RegisterCompleteObjectUndo(textComponent.gameObject, "Replace Text with TextMeshProUGUI");

            // 保存原Text组件的属性
            GameObject gameObject = textComponent.gameObject;
            Transform transform = textComponent.transform;

            // 保存Text组件的所有属性
            string text = textComponent.text;
            Font font = textComponent.font;
            int fontSize = textComponent.fontSize;
            FontStyle fontStyle = textComponent.fontStyle;
            Color color = textComponent.color;
            TextAnchor alignment = textComponent.alignment;
            bool supportRichText = textComponent.supportRichText;
            HorizontalWrapMode horizontalOverflow = textComponent.horizontalOverflow;
            VerticalWrapMode verticalOverflow = textComponent.verticalOverflow;
            bool bestFit = textComponent.resizeTextForBestFit;
            int minSize = textComponent.resizeTextMinSize;
            int maxSize = textComponent.resizeTextMaxSize;
            float lineSpacing = textComponent.lineSpacing;
            bool raycastTarget = textComponent.raycastTarget;
            Material material = textComponent.material;

            // 保存RectTransform属性
            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
            Vector2 anchoredPosition = rectTransform.anchoredPosition;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            Vector2 anchorMin = rectTransform.anchorMin;
            Vector2 anchorMax = rectTransform.anchorMax;
            Vector2 pivot = rectTransform.pivot;
            Vector3 localScale = rectTransform.localScale;
            Quaternion localRotation = rectTransform.localRotation;

            // 删除原Text组件
            GameObject.DestroyImmediate(textComponent);

            // 添加TextMeshProUGUI组件
            TextMeshProUGUI tmpComponent = gameObject.AddComponent<TextMeshProUGUI>();

            // 还原基本属性
            tmpComponent.text = text;
            tmpComponent.fontSize = fontSize;
            tmpComponent.color = color;
            tmpComponent.richText = supportRichText;
            tmpComponent.raycastTarget = raycastTarget;
            tmpComponent.lineSpacing = lineSpacing;

            // 转换字体样式
            if ((fontStyle & FontStyle.Bold) != 0)
                tmpComponent.fontStyle |= FontStyles.Bold;
            if ((fontStyle & FontStyle.Italic) != 0)
                tmpComponent.fontStyle |= FontStyles.Italic;

            // 转换对齐方式
            tmpComponent.alignment = ConvertTextAnchorToTMPAlignment(alignment);

            // 转换溢出模式
            if (horizontalOverflow == HorizontalWrapMode.Wrap)
            {
                tmpComponent.enableWordWrapping = true;
            }
            else
            {
                tmpComponent.enableWordWrapping = false;
            }

            // 处理垂直溢出
            if (verticalOverflow == VerticalWrapMode.Truncate)
            {
                tmpComponent.overflowMode = TextOverflowModes.Truncate;
            }
            else if (verticalOverflow == VerticalWrapMode.Overflow)
            {
                tmpComponent.overflowMode = TextOverflowModes.Overflow;
            }

            // 处理自适应大小
            if (bestFit)
            {
                tmpComponent.enableAutoSizing = true;
                tmpComponent.fontSizeMin = minSize;
                tmpComponent.fontSizeMax = maxSize;
            }

            // 尝试设置字体（如果有对应的TMP字体资源）
            if (font != null)
            {
                // 尝试查找对应的TMP字体资源
                string fontName = font.name;
                TMP_FontAsset tmpFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/" + fontName + " SDF");
                if (tmpFont == null)
                {
                    tmpFont = Resources.Load<TMP_FontAsset>(fontName + " SDF");
                }
                if (tmpFont != null)
                {
                    tmpComponent.font = tmpFont;
                }
                else
                {
                    Debug.LogWarning($"未找到对应的TMP字体资源: {fontName} SDF，使用默认字体");
                }
            }

            // 还原RectTransform属性
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.localScale = localScale;
            rectTransform.localRotation = localRotation;

            replaceCount++;
        }

        Debug.Log($"成功替换了 {replaceCount} 个Text组件为TextMeshProUGUI");

        // 标记场景为已修改
        EditorUtility.SetDirty(EditorUtility.InstanceIDToObject(0));
        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(stage);
        if (!string.IsNullOrEmpty(path))
            PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, path);
    }

    /// <summary>
    /// 转换Text的TextAnchor到TextMeshPro的TextAlignmentOptions
    /// </summary>
    private TextAlignmentOptions ConvertTextAnchorToTMPAlignment(TextAnchor textAnchor)
    {
        switch (textAnchor)
        {
            case TextAnchor.UpperLeft:
                return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter:
                return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight:
                return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft:
                return TextAlignmentOptions.MidlineLeft;
            case TextAnchor.MiddleCenter:
                return TextAlignmentOptions.Midline;
            case TextAnchor.MiddleRight:
                return TextAlignmentOptions.MidlineRight;
            case TextAnchor.LowerLeft:
                return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter:
                return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight:
                return TextAlignmentOptions.BottomRight;
            default:
                return TextAlignmentOptions.Center;
        }
    }


    /// <summary>
    /// 删除所有丢失的脚本组件
    /// </summary>
    public static void DelAllMissScripts()
    {
        Action<GameObject, string> action = (GameObject _ui, string _uiPath) =>
        {
            int missNum = 0;
            foreach (var trans in _ui.GetComponentsInChildren<Transform>(true))
            {
                missNum += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(trans.gameObject);
            }
            if (missNum > 0)
            {
                PrefabUtility.SaveAsPrefabAsset(_ui, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(_ui));
                Debug.Log(string.Format("{0}删除{1}个丢失脚本", _uiPath, missNum));
            }
        };
        ForeachAllUIPrefab(action);
    }

    /// <summary>
    /// 遍历所有UI预制体
    /// </summary>
    public static void ForeachAllUIPrefab(Action<GameObject, string> _action)
    {
        List<string> PrefabPath = new List<string>();
        Action<string, string> FindPrefabPath = (_resDirPath, prefix) =>
        {
            foreach (var filePath in Directory.GetFiles(_resDirPath + prefix, "*.prefab", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(filePath);
                fileName = fileName.Replace(".prefab", string.Empty);
                PrefabPath.Add(prefix + fileName);
            }
        };
        FindPrefabPath(Application.dataPath + "/Resources/", "View/");
        FindPrefabPath(Application.dataPath + "/Framework/Resources/", "View/");
        foreach (var path in PrefabPath)
        {
            GameObject ui = PrefabUtility.InstantiatePrefab(Resources.Load(path) as GameObject) as GameObject;
            _action(ui, path);
            GameObject.DestroyImmediate(ui);
        }
    }

    [SerializeField]
    static Font newfont;
    /// 替换所有丢失字体的文本组件
    /// </summary>
    /// </summary>
    /// <param name="_onlyMissFont">只有丢失字体的文本</param>
    public static void ReplaceAllFont(bool _onlyMissFont = false)
    {
        if (newfont == null)
        {
            EditorUtility.DisplayDialog("提示", "先设置新字体", "确定");
            return;
        }
        Action<GameObject, string> action = (GameObject _ui, string _uiPath) =>
        {
            int textNum = 0;
            foreach (var text in _ui.GetComponentsInChildren<Text>(true))
            {
                if (_onlyMissFont && text.font != null)
                {
                    continue;
                }
                textNum++;
                text.font = newfont;
            }
            if (textNum > 0)
            {
                PrefabUtility.SaveAsPrefabAsset(_ui, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(_ui));
                Debug.Log($"{_uiPath}替换{textNum}个文本");
            }
        };
        ForeachAllUIPrefab(action);
    }
}
