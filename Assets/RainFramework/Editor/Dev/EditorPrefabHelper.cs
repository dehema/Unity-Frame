using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorPrefabHelper
{
    const string folderPath = "Prefab/EditorHierarchy/";
    const string RainUIMenuPrefix = "GameObject/ARain组件/";

    [MenuItem(RainUIMenuPrefix + "文本", priority = -1)]
    public static void CreateText()
    {
        CreatePrefab(folderPath + "Text");
    }



    #region Button

    [MenuItem(RainUIMenuPrefix + "按钮")]
    public static void CreateButton()
    {
        CreatePrefab(folderPath + "按钮");
    }

    [MenuItem(RainUIMenuPrefix + "按钮 文本")]
    public static void CreateButtonText()
    {
        CreatePrefab(folderPath + "按钮 文本");
    }

    [MenuItem(RainUIMenuPrefix + "按钮 图标")]
    public static void CreateButtonIcon()
    {
        CreatePrefab(folderPath + "按钮 图标");
    }

    [MenuItem(RainUIMenuPrefix + "按钮 图标 左右")]
    public static void CreateButtonIconLeftRight()
    {
        CreatePrefab(folderPath + "按钮 图标 左右");
    }

    [MenuItem(RainUIMenuPrefix + "按钮 图标 上下")]
    public static void CreateButtonIconUpDown()
    {
        CreatePrefab(folderPath + "按钮 图标 上下");
    }

    [MenuItem(RainUIMenuPrefix + "按钮 图标 副文本")]
    public static void IconSubTextButtonButton()
    {
        CreatePrefab(folderPath + "按钮 图标 副文本");
    }

    [MenuItem(RainUIMenuPrefix + "按钮 关闭")]
    public static void CreateCloseButton()
    {
        CreatePrefab(folderPath + "$btClose#Button");
    }
    #endregion

    #region MyRegion
    [MenuItem(RainUIMenuPrefix + "Toggle")]
    public static void CreateToggle()
    {
        CreatePrefab(folderPath + "Toggle");
    }

    [MenuItem(RainUIMenuPrefix + "ToggleGroupIcon")]
    public static void CreateToggleGroupIcon()
    {
        CreatePrefab(folderPath + "ToggleGroupIcon");
    }

    [MenuItem(RainUIMenuPrefix + "ToggleGroup")]
    public static void CreateToggleGroup()
    {
        CreatePrefab(folderPath + "ToggleGroup");
    }
    #endregion

    [MenuItem(RainUIMenuPrefix + "Slider")]
    public static void CreateSlider()
    {
        CreatePrefab(folderPath + "Slider");
    }

    private static void CreatePrefab(string _path)
    {
        Object obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        GameObject selGo = obj as GameObject;
        GameObject resObj = Resources.Load<GameObject>(_path);
        GameObject go = GameObject.Instantiate(resObj);
        go.name = resObj.name;
        go.transform.parent = selGo.transform;
        EditorGUIUtility.PingObject(go);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition3D = Vector3.zero;
        rect.localScale = Vector3.one;
    }
}
