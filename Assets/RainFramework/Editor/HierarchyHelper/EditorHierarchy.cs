using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorHierarchy
{
    const string folderPath = "Prefab/EditorHierarchy/";
    [MenuItem("GameObject/A组件/文本", priority = 0)]
    public static void CreateText()
    {
        CreatePrefab(folderPath + "Text");
    }

    [MenuItem("GameObject/A组件/按钮", priority = 1)]
    public static void CreateButton()
    {
        CreatePrefab(folderPath + "CommonButton");
    }

    [MenuItem("GameObject/A组件/文本按钮", priority = 2)]
    public static void CreateTextButton()
    {
        CreatePrefab(folderPath + "TextButton");
    }

    [MenuItem("GameObject/A组件/close按钮", priority = 3)]
    public static void CreateCloseButton()
    {
        CreatePrefab(folderPath + "$btClose#Button");
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
