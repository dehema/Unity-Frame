using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorHierarchy
{
    [MenuItem("GameObject/自定义组件/文本", priority = 0)]
    public static void CreateText()
    {
        CreatePrefab("Prefab/EditorHierarchy/Text");
    }

    [MenuItem("GameObject/自定义组件/按钮", priority = 1)]
    public static void CreateButton()
    {
        CreatePrefab("Prefab/EditorHierarchy/Button");
    }

    [MenuItem("GameObject/自定义组件/文本按钮", priority = 2)]
    public static void CreateTextButton()
    {
        CreatePrefab("Prefab/EditorHierarchy/TextButton");
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
