using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorHierarchy
{
    const string folderPath = "Prefab/EditorHierarchy/";
    const string RainUIMenuPrefix = "GameObject/Rain组件/";

    [MenuItem(RainUIMenuPrefix + "文本", priority = 0)]
    public static void CreateText()
    {
        CreatePrefab(folderPath + "Text");
    }


    [MenuItem(RainUIMenuPrefix + "按钮", priority = 1)]
    public static void CreateButton()
    {
        CreatePrefab(folderPath + "CommonButton");
    }

    [MenuItem(RainUIMenuPrefix + "文本按钮", priority = 2)]
    public static void CreateTextButton()
    {
        CreatePrefab(folderPath + "TextButton");
    }

    [MenuItem(RainUIMenuPrefix + "close按钮", priority = 3)]
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
