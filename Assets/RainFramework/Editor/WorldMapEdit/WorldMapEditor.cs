using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rain.Core;

/// <summary>
/// 世界地图编辑器
/// </summary>
public class WorldMapEditor : EditorWindow
{
    // 世界地图预制体路径
    const string worldMapPrefabPath = "Assets/AssetBundles/Art/Resources/Prefab/WorldMapTile";
    
    // 预制体相关
    private GameObject[] worldMapPrefabs;
    private string[] prefabNames;
    private int selectedPrefabIndex = 0;
    private GameObject currentPrefab;
    
    // TileMap相关
    private Tilemap currentTilemap;
    private TileBase selectedTile;
    private TileBase defaultTile;
    
    // 地图尺寸相关
    private int mapWidth = 200;
    private int mapHeight = 200;
    private int mapCount = 1;
    
    // 窗口状态
    private Vector2 scrollPosition;
    private bool showTilePalette = true;
    
    [MenuItem("开发工具/世界地图编辑器")]
    public static void ShowWindow()
    {
        WorldMapEditor window = GetWindow<WorldMapEditor>("世界地图编辑器");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }
    
    private void OnEnable()
    {
        LoadWorldMapPrefabs();
    }
    
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        
        // 标题
        EditorGUILayout.LabelField("世界地图编辑器", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);
        
        // 预制体选择区域
        DrawPrefabSelector();
        
        EditorGUILayout.Space(10);
        
        // TileMap编辑区域
        if (currentPrefab != null)
        {
            DrawTileMapEditor();
        }
        else
        {
            EditorGUILayout.HelpBox("请选择一个世界地图预制体", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// 绘制预制体选择器
    /// </summary>
    private void DrawPrefabSelector()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("预制体选择", EditorStyles.boldLabel);
        
        if (worldMapPrefabs != null && worldMapPrefabs.Length > 0)
        {
            // 预制体下拉列表
            int newIndex = EditorGUILayout.Popup("世界地图预制体", selectedPrefabIndex, prefabNames);
            if (newIndex != selectedPrefabIndex)
            {
                selectedPrefabIndex = newIndex;
                LoadSelectedPrefab();
            }
            
            // 按钮区域
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新预制体列表", GUILayout.Height(25)))
            {
                LoadWorldMapPrefabs();
            }
            
            if (GUILayout.Button("显示预制体文件夹", GUILayout.Height(25)))
            {
                ShowPrefabFolder();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("未找到世界地图预制体", MessageType.Warning);
            
            // 按钮区域
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新预制体列表", GUILayout.Height(25)))
            {
                LoadWorldMapPrefabs();
            }
            
            if (GUILayout.Button("显示预制体文件夹", GUILayout.Height(25)))
            {
                ShowPrefabFolder();
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// 绘制TileMap编辑器
    /// </summary>
    private void DrawTileMapEditor()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("TileMap编辑器", EditorStyles.boldLabel);
        
        if (currentTilemap != null)
        {
            // TileMap信息
            EditorGUILayout.LabelField($"当前TileMap: {currentTilemap.name}");
            EditorGUILayout.LabelField($"网格大小: {currentTilemap.cellSize}");
            EditorGUILayout.LabelField($"瓦片数量: {currentTilemap.GetUsedTilesCount()}");
            
            EditorGUILayout.Space(5);
            
            // 地图尺寸设置
            EditorGUILayout.LabelField("地图尺寸", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("宽度:", GUILayout.Width(50));
            mapWidth = EditorGUILayout.IntField(mapWidth, GUILayout.Width(80));
            EditorGUILayout.LabelField("高度:", GUILayout.Width(50));
            mapHeight = EditorGUILayout.IntField(mapHeight, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图数量:", GUILayout.Width(80));
            mapCount = EditorGUILayout.IntField(mapCount, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();
            
            // 限制尺寸范围
            mapWidth = Mathf.Clamp(mapWidth, 1, 1000);
            mapHeight = Mathf.Clamp(mapHeight, 1, 1000);
            mapCount = Mathf.Clamp(mapCount, 1, 100);
            
            EditorGUILayout.Space(5);
            
            // 瓦片选择
            EditorGUILayout.LabelField("瓦片选择", EditorStyles.boldLabel);
            selectedTile = (TileBase)EditorGUILayout.ObjectField("选择瓦片", selectedTile, typeof(TileBase), false);
            defaultTile = (TileBase)EditorGUILayout.ObjectField("默认瓦片", defaultTile, typeof(TileBase), false);
            
            EditorGUILayout.Space(5);
            
            // 工具按钮
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("清除所有瓦片", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("确认清除", "确定要清除所有瓦片吗？", "确定", "取消"))
                {
                    ClearAllTiles();
                }
            }
            
            if (GUILayout.Button("刷新TileMap", GUILayout.Height(25)))
            {
                RefreshTileMap();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // 一键填充功能
            EditorGUILayout.LabelField("一键填充", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("使用默认瓦片填充地图", GUILayout.Height(30)))
            {
                if (defaultTile != null)
                {
                    if (EditorUtility.DisplayDialog("确认填充", $"确定要使用默认瓦片填充 {mapWidth}x{mapHeight} 的地图吗？", "确定", "取消"))
                    {
                        FillMapWithDefaultTile();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请先选择默认瓦片", "确定");
                }
            }
            
            if (GUILayout.Button("创建新地图", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("确认创建", $"确定要创建 {mapWidth}x{mapHeight} 的新地图吗？\n这将清除所有现有瓦片。", "确定", "取消"))
                {
                    CreateNewMap();
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(5);
            
            // 一键赋值地图功能
            EditorGUILayout.LabelField("一键赋值地图", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("赋值到其他地图", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("确认赋值", $"确定要将当前地图设置赋值到其他 {mapCount - 1} 个地图吗？", "确定", "取消"))
                {
                    AssignToOtherMaps();
                }
            }
            
            if (GUILayout.Button("批量创建地图", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("确认批量创建", $"确定要批量创建 {mapCount} 个 {mapWidth}x{mapHeight} 的地图吗？", "确定", "取消"))
                {
                    BatchCreateMaps();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("当前预制体中没有找到TileMap组件", MessageType.Warning);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    /// <summary>
    /// 加载世界地图预制体
    /// </summary>
    private void LoadWorldMapPrefabs()
    {
        // 获取指定路径下的所有预制体
        string[] prefabPaths = FileTools.GetSpecifyFilesInFolder(worldMapPrefabPath, new string[] { ".prefab" });
        
        if (prefabPaths != null && prefabPaths.Length > 0)
        {
            worldMapPrefabs = new GameObject[prefabPaths.Length];
            prefabNames = new string[prefabPaths.Length];
            
            for (int i = 0; i < prefabPaths.Length; i++)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPaths[i]);
                worldMapPrefabs[i] = prefab;
                prefabNames[i] = prefab != null ? prefab.name : "未知预制体";
            }
            
            // 默认选择第一个预制体
            if (selectedPrefabIndex >= worldMapPrefabs.Length)
            {
                selectedPrefabIndex = 0;
            }
            
            LoadSelectedPrefab();
        }
        else
        {
            worldMapPrefabs = null;
            prefabNames = null;
            selectedPrefabIndex = 0;
            currentPrefab = null;
            currentTilemap = null;
        }
    }
    
    /// <summary>
    /// 加载选中的预制体
    /// </summary>
    private void LoadSelectedPrefab()
    {
        if (worldMapPrefabs != null && selectedPrefabIndex < worldMapPrefabs.Length)
        {
            currentPrefab = worldMapPrefabs[selectedPrefabIndex];
            RefreshTileMap();
        }
        else
        {
            currentPrefab = null;
            currentTilemap = null;
        }
    }
    
    /// <summary>
    /// 刷新TileMap组件
    /// </summary>
    private void RefreshTileMap()
    {
        if (currentPrefab != null)
        {
            // 在预制体中查找TileMap组件
            Tilemap[] tilemaps = currentPrefab.GetComponentsInChildren<Tilemap>();
            if (tilemaps.Length > 0)
            {
                currentTilemap = tilemaps[0]; // 使用第一个TileMap
            }
            else
            {
                currentTilemap = null;
            }
        }
        else
        {
            currentTilemap = null;
        }
    }
    
    /// <summary>
    /// 清除所有瓦片并立即刷新显示
    /// </summary>
    private void ClearAllTiles()
    {
        if (currentTilemap != null)
        {
            // 记录撤销操作
            Undo.RegisterCompleteObjectUndo(currentTilemap, "Clear All Tiles");
            
            // 清除所有瓦片
            currentTilemap.ClearAllTiles();
            
            // 立即刷新Scene视图和预制体
            EditorUtility.SetDirty(currentTilemap);
            EditorUtility.SetDirty(currentPrefab);
            
            // 强制刷新所有相关视图
            SceneView.RepaintAll();
            EditorApplication.QueuePlayerLoopUpdate();
            
            // 刷新编辑器窗口
            Repaint();
            
            // 强制刷新预制体资源
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    /// <summary>
    /// 在Project窗口中显示预制体文件夹内容
    /// </summary>
    private void ShowPrefabFolder()
    {
        // 确保文件夹存在
        if (!System.IO.Directory.Exists(worldMapPrefabPath))
        {
            EditorUtility.DisplayDialog("文件夹不存在", $"预制体文件夹不存在：\n{worldMapPrefabPath}", "确定");
            return;
        }
        
        // 获取文件夹中的所有预制体文件
        string[] prefabFiles = System.IO.Directory.GetFiles(worldMapPrefabPath, "*.prefab", System.IO.SearchOption.TopDirectoryOnly);
        
        if (prefabFiles.Length > 0)
        {
            // 选择第一个预制体文件并显示
            string firstPrefabPath = prefabFiles[0].Replace("\\", "/");
            Object prefabAsset = AssetDatabase.LoadAssetAtPath<Object>(firstPrefabPath);
            
            if (prefabAsset != null)
            {
                // 在Project窗口中高亮显示第一个预制体
                EditorGUIUtility.PingObject(prefabAsset);
                Selection.activeObject = prefabAsset;
                
                // 展开文件夹并显示内容
                EditorUtility.FocusProjectWindow();
            }
        }
        else
        {
            // 如果没有预制体文件，则显示文件夹本身
            Object folder = AssetDatabase.LoadAssetAtPath<Object>(worldMapPrefabPath);
            if (folder != null)
            {
                EditorGUIUtility.PingObject(folder);
                Selection.activeObject = folder;
            }
            else
            {
                EditorUtility.DisplayDialog("无法显示文件夹", $"无法加载文件夹：\n{worldMapPrefabPath}", "确定");
            }
        }
    }
    
    /// <summary>
    /// 使用默认瓦片填充整个地图
    /// </summary>
    private void FillMapWithDefaultTile()
    {
        if (currentTilemap == null || defaultTile == null)
        {
            return;
        }
        
        // 记录撤销操作
        Undo.RegisterCompleteObjectUndo(currentTilemap, "Fill Map With Default Tile");
        
        // 从0开始填充整个地图区域
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                currentTilemap.SetTile(position, defaultTile);
            }
        }
        
        // 立即刷新显示
        EditorUtility.SetDirty(currentTilemap);
        EditorUtility.SetDirty(currentPrefab);
        SceneView.RepaintAll();
        EditorApplication.QueuePlayerLoopUpdate();
        Repaint();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"已使用默认瓦片填充 {mapWidth}x{mapHeight} 的地图");
    }
    
    /// <summary>
    /// 创建新地图（清除现有瓦片并填充默认瓦片）
    /// </summary>
    private void CreateNewMap()
    {
        if (currentTilemap == null)
        {
            return;
        }
        
        // 记录撤销操作
        Undo.RegisterCompleteObjectUndo(currentTilemap, "Create New Map");
        
        // 清除所有现有瓦片
        currentTilemap.ClearAllTiles();
        
        // 如果设置了默认瓦片，则填充地图
        if (defaultTile != null)
        {
            FillMapWithDefaultTile();
        }
        else
        {
            // 即使没有默认瓦片也要刷新显示
            EditorUtility.SetDirty(currentTilemap);
            EditorUtility.SetDirty(currentPrefab);
            SceneView.RepaintAll();
            EditorApplication.QueuePlayerLoopUpdate();
            Repaint();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        Debug.Log($"已创建 {mapWidth}x{mapHeight} 的新地图");
    }
    
    /// <summary>
    /// 将当前地图设置赋值到其他地图
    /// </summary>
    private void AssignToOtherMaps()
    {
        if (currentTilemap == null || worldMapPrefabs == null || worldMapPrefabs.Length <= 1)
        {
            EditorUtility.DisplayDialog("提示", "需要至少2个预制体才能进行赋值操作", "确定");
            return;
        }
        
        // 记录撤销操作
        Undo.RegisterCompleteObjectUndo(worldMapPrefabs, "Assign To Other Maps");
        
        int assignedCount = 0;
        
        // 遍历所有预制体，跳过当前选中的预制体
        for (int i = 0; i < worldMapPrefabs.Length && assignedCount < mapCount - 1; i++)
        {
            if (i == selectedPrefabIndex) continue; // 跳过当前选中的预制体
            
            GameObject targetPrefab = worldMapPrefabs[i];
            if (targetPrefab == null) continue;
            
            // 在目标预制体中查找TileMap组件
            Tilemap[] targetTilemaps = targetPrefab.GetComponentsInChildren<Tilemap>();
            if (targetTilemaps.Length == 0) continue;
            
            Tilemap targetTilemap = targetTilemaps[0];
            
            // 清除目标地图的所有瓦片
            targetTilemap.ClearAllTiles();
            
            // 复制当前地图的所有瓦片到目标地图
            if (defaultTile != null)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        Vector3Int position = new Vector3Int(x, y, 0);
                        targetTilemap.SetTile(position, defaultTile);
                    }
                }
            }
            
            // 标记目标预制体为脏状态
            EditorUtility.SetDirty(targetTilemap);
            EditorUtility.SetDirty(targetPrefab);
            
            assignedCount++;
        }
        
        // 刷新显示
        SceneView.RepaintAll();
        EditorApplication.QueuePlayerLoopUpdate();
        Repaint();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"已将当前地图设置赋值到 {assignedCount} 个其他地图");
    }
    
    /// <summary>
    /// 批量创建地图
    /// </summary>
    private void BatchCreateMaps()
    {
        if (worldMapPrefabs == null || worldMapPrefabs.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "没有可用的预制体进行批量创建", "确定");
            return;
        }
        
        // 记录撤销操作
        Undo.RegisterCompleteObjectUndo(worldMapPrefabs, "Batch Create Maps");
        
        int createdCount = 0;
        int maxMaps = Mathf.Min(mapCount, worldMapPrefabs.Length);
        
        // 遍历指定数量的预制体
        for (int i = 0; i < maxMaps; i++)
        {
            GameObject targetPrefab = worldMapPrefabs[i];
            if (targetPrefab == null) continue;
            
            // 在目标预制体中查找TileMap组件
            Tilemap[] targetTilemaps = targetPrefab.GetComponentsInChildren<Tilemap>();
            if (targetTilemaps.Length == 0) continue;
            
            Tilemap targetTilemap = targetTilemaps[0];
            
            // 清除目标地图的所有瓦片
            targetTilemap.ClearAllTiles();
            
            // 使用默认瓦片填充地图
            if (defaultTile != null)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        Vector3Int position = new Vector3Int(x, y, 0);
                        targetTilemap.SetTile(position, defaultTile);
                    }
                }
            }
            
            // 标记目标预制体为脏状态
            EditorUtility.SetDirty(targetTilemap);
            EditorUtility.SetDirty(targetPrefab);
            
            createdCount++;
        }
        
        // 刷新显示
        SceneView.RepaintAll();
        EditorApplication.QueuePlayerLoopUpdate();
        Repaint();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"已批量创建 {createdCount} 个 {mapWidth}x{mapHeight} 的地图");
    }
}
