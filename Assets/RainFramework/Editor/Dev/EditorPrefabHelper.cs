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
        CreatePrefab(folderPath + "toggleGroup");
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
