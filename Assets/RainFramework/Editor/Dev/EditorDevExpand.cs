using Rain.UI;
using UnityEditor;
using UnityEngine;

public class EditorDevExpand
{
    [MenuItem("GameObject/A工具/只显示&&点击这个物体", priority = 0)]
    public static void PickingAndIsolateObj()
    {
        Object obj = Selection.activeObject;
        if (obj == null)
        {
            return;
        }
        //显示
        GameObject go = obj as GameObject;
        //点击
        HideAllGameObject();
        SceneVisibilityManager.instance.EnablePicking(go, true);
        SceneVisibilityManager.instance.Show(go, true);
    }

    [MenuItem("GameObject/A工具/只显示最后一个NormalLayer普通页面", priority = 1)]
    public static void OnlyShowLastNormalUI()
    {
        OnlyShowLastUI(ViewLayer.NormalLayer.ToString());
    }

    [MenuItem("GameObject/A工具/只显示最后一个DialogLayer弹窗", priority = 2)]
    public static void OnlyShowLastPopUI()
    {
        OnlyShowLastUI(ViewLayer.DialogLayer.ToString());
    }

    [MenuItem("GameObject/A工具/只显示最后一个TipsLayer消息", priority = 3)]
    public static void OnlyShowLastTipsUI()
    {
        OnlyShowLastUI(ViewLayer.TipsLayer.ToString());
    }

    public static void OnlyShowLastUI(string _rootName)
    {
        GameObject uiRoot = UIMgr.Ins.gameObject;
        if (uiRoot == null)
        {
            return;
        }
        Transform tf = uiRoot.transform.Find(_rootName);
        if (tf == null)
        {
            return;
        }
        if (tf.childCount <= 0)
        {
            return;
        }

        GameObject uigo = null;
        for (int i = tf.childCount - 1; i >= 0; i--)
        {
            var tmp = tf.GetChild(i).gameObject;
            if (!tmp.activeSelf)
            {
                continue;
            }
            uigo = tf.GetChild(i).gameObject;
            break;
        }
        if (uigo == null)
        {
            return;
        }
        HideAllGameObject();
        SceneVisibilityManager.instance.EnablePicking(uigo, true);
        SceneVisibilityManager.instance.Show(uigo, true);
        //Hierarchy面板选择该物体
        EditorGUIUtility.PingObject(uigo);
        Selection.activeGameObject = uigo;
    }

    [MenuItem("GameObject/A工具/重置 显示&&点击", priority = 999)]
    public static void PickingAndIsolateReset()
    {
        ShowAllGameObject();
    }

    /// <summary>
    /// 显示和可点击所有的物体
    /// </summary>
    private static void ShowAllGameObject()
    {
        foreach (GameObject item in GameObject.FindObjectsOfType<GameObject>())
        {
            SceneVisibilityManager.instance.Show(item, true);
            SceneVisibilityManager.instance.EnablePicking(item, true);
        }
    }

    /// <summary>
    /// 隐藏和不可点击所有的物体
    /// </summary>
    private static void HideAllGameObject()
    {
        foreach (GameObject item in GameObject.FindObjectsOfType<GameObject>())
        {
            SceneVisibilityManager.instance.Hide(item, true);
            SceneVisibilityManager.instance.DisablePicking(item, true);
        }
    }
}
