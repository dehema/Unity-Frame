#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// CameraController_Base的编辑器脚本
/// 支持所有继承自CameraController_Base的子类
/// </summary>
[CustomEditor(typeof(CameraController_Base), true)]
public class CameraController_Base_Editor : Editor
{
    // 目标坐标输入字段
    private float targetX = 0f;
    private float targetZ = 0f;
    public override void OnInspectorGUI()
    {
        // 绘制默认的Inspector界面
        DrawDefaultInspector();

        // 添加分隔线
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("编辑器工具", EditorStyles.boldLabel);

        // 获取目标组件
        CameraController_Base cameraController = (CameraController_Base)target;

        // 添加重置相机位置按钮
        if (GUILayout.Button("调整相机视野中心到世界原点 (0,0,0)"))
        {
            cameraController.ResetCameraToWorldCenter();

            // 标记场景为已修改（如果在编辑模式下）
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(cameraController);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(cameraController.gameObject.scene);
            }
        }

        // 添加目标坐标设置UI
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("相机目标设置", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标X坐标:", GUILayout.Width(80));
        targetX = EditorGUILayout.FloatField(targetX);
        EditorGUILayout.LabelField("目标Z坐标:", GUILayout.Width(80));
        targetZ = EditorGUILayout.FloatField(targetZ);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("移动相机看向目标坐标"))
        {
            cameraController.SetCameraPosLookAtPos(new Vector3(targetX, 0, targetZ));

            // 标记场景为已修改（如果在编辑模式下）
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(cameraController);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(cameraController.gameObject.scene);
            }
        }

        // 显示目标坐标信息
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndHorizontal();
        // 显示当前相机位置信息
        if (cameraController.mainCamera != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("当前相机信息", EditorStyles.boldLabel);

            Vector3 targetPos = cameraController.GetCameraLookPos();
            EditorGUILayout.LabelField($"注视坐标: ({targetPos.x:F1}, 0, {targetPos.z:F1})", EditorStyles.miniLabel);


            // 显示当前帧状态
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("当前帧状态", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"是否移动: {cameraController.IsCurrentFrameMoving}");
            EditorGUILayout.LabelField($"是否缩放: {cameraController.IsCurrentFrameZooming}");
        }

        // 显示子类特定信息
        DrawSubclassSpecificGUI(cameraController);
    }

    /// <summary>
    /// 绘制子类特定的GUI内容
    /// </summary>
    protected virtual void DrawSubclassSpecificGUI(CameraController_Base cameraController)
    {
        // 根据子类类型显示不同的内容
        if (cameraController is CameraController_WorldMap)
        {
            DrawWorldCameraGUI((CameraController_WorldMap)cameraController);
        }
        else if (cameraController is CameraController_City)
        {
            DrawCityCameraGUI((CameraController_City)cameraController);
        }
    }

    /// <summary>
    /// 绘制世界相机特定的GUI
    /// </summary>
    private void DrawWorldCameraGUI(CameraController_WorldMap worldCamera)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("世界地图相机", EditorStyles.boldLabel);

        // 显示世界地图相关信息
        if (WorldMapMgr.Ins != null)
        {
            EditorGUILayout.LabelField($"已加载地图数量: {WorldMapMgr.Ins.GetLoadedAreaCount()}");

            Vector3 targetPos = worldCamera.GetCameraLookPos();
            Vector2Int targetCube = WorldMapMgr.Ins.WorldPosToLocal(targetPos);
            EditorGUILayout.LabelField($"注视地块: ({targetCube.x}, {targetCube.y})", EditorStyles.miniLabel);

            Vector2Int mapIndex = WorldMapMgr.Ins.GetAreaIndexFromPosition(targetPos);
            EditorGUILayout.LabelField($"注视地图索引: ({mapIndex.x}, {mapIndex.y})", EditorStyles.miniLabel);

            Vector2Int visibleRange = WorldMapMgr.Ins.GetCurrentVisibleAreaRange();
            EditorGUILayout.LabelField($"可见地图范围: {visibleRange.x}x{visibleRange.y}", EditorStyles.miniLabel);

            // 显示相机视野信息
            string viewInfo = WorldMapMgr.Ins.GetCameraViewInfo();
            EditorGUILayout.HelpBox(viewInfo, MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("WorldMapMgr未找到", MessageType.Warning);
        }
    }

    /// <summary>
    /// 绘制城市相机特定的GUI
    /// </summary>
    private void DrawCityCameraGUI(CameraController_City cityCamera)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("城市相机", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("城市相机模式", MessageType.Info);
    }
}
#endif
