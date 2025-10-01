using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ApplyMaterial : MonoBehaviour
{
    [Header("材质设置")]
    [SerializeField] public Material targetMaterial; // 目标材质球
    
    [Header("应用选项")]
    [SerializeField] public bool includeInactive = true; // 是否包含非激活物体
    [SerializeField] public string targetTag = "Untagged"; // 目标标签过滤
    
    // 存储原始材质的字典，用于恢复
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    
    /// <summary>
    /// 修改场景中所有模型的材质并应用到预制体
    /// </summary>
    public void ApplyMaterialToAllModels()
    {
        if (targetMaterial == null)
        {
            Debug.LogWarning("目标材质为空，请先设置材质球！");
            return;
        }
        
        // 清空原始材质记录
        originalMaterials.Clear();
        
        // 查找场景中所有的Renderer组件
        Renderer[] allRenderers = FindObjectsOfType<Renderer>(includeInactive);
        
        int appliedCount = 0;
        int prefabCount = 0;
        int sceneObjectCount = 0;
        
        foreach (Renderer renderer in allRenderers)
        {
            // 跳过UI渲染器
            if (renderer is UnityEngine.UI.Graphic)
                continue;
                
            // 如果设置了标签过滤，检查标签
            if (targetTag != "Untagged" && !renderer.CompareTag(targetTag))
                continue;
                
            // 保存原始材质
            if (!originalMaterials.ContainsKey(renderer))
            {
                originalMaterials[renderer] = new Material[renderer.materials.Length];
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    originalMaterials[renderer][i] = renderer.materials[i];
                }
            }
            
            // 应用新材质
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = targetMaterial;
            }
            renderer.materials = newMaterials;
            appliedCount++;
            
            // 统计预制体和场景对象
#if UNITY_EDITOR
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(renderer.gameObject);
            if (prefabRoot != null)
            {
                prefabCount++;
            }
            else
            {
                sceneObjectCount++;
            }
#endif
        }
        
        Debug.Log($"已成功将材质 '{targetMaterial.name}' 应用到 {appliedCount} 个模型上");
        Debug.Log($"- 预制体实例: {prefabCount} 个");
        Debug.Log($"- 场景对象: {sceneObjectCount} 个");
        
        // 应用所有预制体修改
        ApplyAllPrefabChanges();
    }
    
    /// <summary>
    /// 应用所有预制体的修改
    /// </summary>
    private void ApplyAllPrefabChanges()
    {
#if UNITY_EDITOR
        // 获取场景中所有的预制体实例
        GameObject[] allObjects = FindObjectsOfType<GameObject>(includeInactive);
        HashSet<GameObject> processedPrefabs = new HashSet<GameObject>();
        int appliedPrefabCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            // 检查是否为预制体实例
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(obj);
            if (prefabRoot != null && !processedPrefabs.Contains(prefabRoot))
            {
                processedPrefabs.Add(prefabRoot);
                
                // 获取预制体路径
                string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot);
                if (!string.IsNullOrEmpty(prefabPath))
                {
                    // 检查预制体状态
                    PrefabInstanceStatus prefabStatus = PrefabUtility.GetPrefabInstanceStatus(prefabRoot);
                    if (prefabStatus == PrefabInstanceStatus.Connected)
                    {
                        // 应用预制体修改
                        PrefabUtility.SaveAsPrefabAssetAndConnect(prefabRoot, prefabPath, InteractionMode.AutomatedAction);
                        appliedPrefabCount++;
                        Debug.Log($"已应用预制体修改: {prefabPath}");
                    }
                }
            }
        }
        
        Debug.Log($"总共应用了 {appliedPrefabCount} 个预制体的修改");
#endif
    }
    
    /// <summary>
    /// 恢复所有模型到原始材质
    /// </summary>
    public void RestoreOriginalMaterials()
    {
        int restoredCount = 0;
        foreach (var kvp in originalMaterials)
        {
            if (kvp.Key != null) // 检查Renderer是否还存在
            {
                kvp.Key.materials = kvp.Value;
                restoredCount++;
            }
        }
        
        Debug.Log($"已恢复 {restoredCount} 个模型的原始材质");
        originalMaterials.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ApplyMaterial))]
public class NewBehaviourScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector界面
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("编辑器工具", EditorStyles.boldLabel);
        
        // 获取目标组件
        ApplyMaterial script = (ApplyMaterial)target;
        
        EditorGUILayout.BeginHorizontal();
        
        // 应用材质按钮
        GUI.enabled = script.targetMaterial != null;
        if (GUILayout.Button("应用材质到所有模型", GUILayout.Height(35)))
        {
            if (EditorUtility.DisplayDialog("确认操作", 
                $"确定要将材质 '{script.targetMaterial.name}' 应用到场景中所有模型吗？\n\n" +
                $"包含非激活物体: {script.includeInactive}\n" +
                $"目标标签: {script.targetTag}\n\n" +
                "此操作会修改场景中所有模型的材质并应用到预制体文件！", 
                "确定", "取消"))
            {
                script.ApplyMaterialToAllModels();
                
                // 标记场景为已修改
                EditorUtility.SetDirty(script);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
            }
        }
        
        // 恢复材质按钮
        GUI.enabled = true;
        if (GUILayout.Button("恢复原始材质", GUILayout.Height(35)))
        {
            if (EditorUtility.DisplayDialog("确认操作", 
                "确定要恢复所有模型到原始材质吗？", 
                "确定", "取消"))
            {
                script.RestoreOriginalMaterials();
                
                // 标记场景为已修改
                EditorUtility.SetDirty(script);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        // 显示当前场景信息
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("场景信息", EditorStyles.boldLabel);
        
        Renderer[] allRenderers = FindObjectsOfType<Renderer>(script.includeInactive);
        int taggedCount = 0;
        int prefabRendererCount = 0;
        
        foreach (Renderer renderer in allRenderers)
        {
            if (script.targetTag != "Untagged" && renderer.CompareTag(script.targetTag))
                taggedCount++;
                
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(renderer.gameObject);
            if (prefabRoot != null)
                prefabRendererCount++;
        }
        
        EditorGUILayout.LabelField($"场景中总Renderer数量: {allRenderers.Length}");
        EditorGUILayout.LabelField($"预制体Renderer数量: {prefabRendererCount}");
        if (script.targetTag != "Untagged")
        {
            EditorGUILayout.LabelField($"标签 '{script.targetTag}' 的Renderer数量: {taggedCount}");
        }
        
        // 材质预览
        if (script.targetMaterial != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("材质预览", EditorStyles.boldLabel);
            EditorGUILayout.ObjectField("当前材质", script.targetMaterial, typeof(Material), false);
        }
    }
}
#endif
