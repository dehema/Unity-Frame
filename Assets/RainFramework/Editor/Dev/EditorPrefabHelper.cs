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


    [MenuItem(RainUIMenuPrefix + "按钮")]
    public static void CreateButton()
    {
        CreatePrefab(folderPath + "CommonButton");
    }

    [MenuItem(RainUIMenuPrefix + "文本按钮")]
    public static void CreateTextButton()
    {
        CreatePrefab(folderPath + "TextButton");
    }

    [MenuItem(RainUIMenuPrefix + "图标+副文本按钮")]
    public static void IconSubTextButtonButton()
    {
        CreatePrefab(folderPath + "IconSubTextButton");
    }

    [MenuItem(RainUIMenuPrefix + "close按钮")]
    public static void CreateCloseButton()
    {
        CreatePrefab(folderPath + "$btClose#Button");
    }

    [MenuItem(RainUIMenuPrefix + "ToggleGroup")]
    public static void CreateToggleGroup()
    {
        CreatePrefab(folderPath + "ToggleGroup");
    }

    [MenuItem(RainUIMenuPrefix + "Slider")]
    public static void CreateSlider()
    {
        CreatePrefab(folderPath + "Slider");
    }

    [MenuItem(RainUIMenuPrefix + "IconButton左右")]
    public static void CreateIconButtonLeftRight()
    {
        CreatePrefab(folderPath + "IconButton左右");
    }

    [MenuItem(RainUIMenuPrefix + "IconButton上下")]
    public static void CreateIconButtonUpDown()
    {
        CreatePrefab(folderPath + "IconButton上下");
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
