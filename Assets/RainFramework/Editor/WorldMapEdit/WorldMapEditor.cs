using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rain.Core;
using Newtonsoft.Json;
using System.IO;

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
    private TileBase defaultTile;

    // 随机地图相关
    private List<TileBase> randomTiles = new List<TileBase>();
    private float randomTileRatio = 0.05f; // 随机瓦片比例，默认5%
    private bool showRandomTiles = false;
    private int objectPickerControlID = 0;

    // 地图尺寸相关
    private int mapSize = 1000;         //地图的尺寸
    private int sizePerArea = 100;      //每个区域的尺寸
    private int areaNumPerDire = 10; //单方向上的地图数量
    private int mapCount = 1;

    // 窗口状态
    private Vector2 scrollPosition;

    [MenuItem("开发工具/世界地图编辑器")]
    public static void ShowWindow()
    {
        // 获取Hierarchy窗口类型并停靠到它旁边
        WorldMapEditor window = GetWindow<WorldMapEditor>("世界地图编辑器", typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow"));
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    private void OnEnable()
    {
        LoadWorldMapPrefabs();
        if (defaultTile == null)
        {
            defaultTile = AssetDatabase.LoadAssetAtPath<TileBase>("Assets/AssetBundles/Art/Tile/worldmap ground_4.asset");
        }
        if (randomTiles.Count == 0)
        {
            randomTiles.Add(AssetDatabase.LoadAssetAtPath<TileBase>("Assets/AssetBundles/Art/Tile/worldmap ground_1.asset"));
            randomTiles.Add(AssetDatabase.LoadAssetAtPath<TileBase>("Assets/AssetBundles/Art/Tile/worldmap ground_3.asset"));
        }
    }

    private void OnGUI()
    {
        // 检查对象选择器是否返回了结果
        if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == objectPickerControlID)
        {
            Object selectedObject = EditorGUIUtility.GetObjectPickerObject();
            if (selectedObject is TileBase tileBase)
            {
                // 检查是否已经存在该瓦片
                if (!randomTiles.Contains(tileBase))
                {
                    randomTiles.Add(tileBase);
                }
            }
        }

        // 开始滚动区域
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

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

        // 结束滚动区域
        EditorGUILayout.EndScrollView();
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
            if (GUILayout.Button("打开预制体", GUILayout.Height(25)))
            {
                OpenSelectedPrefab();
            }

            if (GUILayout.Button("刷新预制体列表", GUILayout.Height(25)))
            {
                LoadWorldMapPrefabs();
            }

            if (GUILayout.Button("显示预制体文件夹", GUILayout.Height(25)))
            {
                ShowPrefabFolder();
            }

            if (GUILayout.Button("生成地图数据映射文件", GUILayout.Height(25)))
            {
                CreateWorldMapFile();
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
            EditorGUILayout.LabelField("尺寸:", GUILayout.Width(50));
            sizePerArea = EditorGUILayout.IntField(sizePerArea, GUILayout.Width(80));
            areaNumPerDire = mapSize / sizePerArea;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图数量:", GUILayout.Width(80));
            mapCount = EditorGUILayout.IntField(mapCount, GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            // 限制尺寸范围
            sizePerArea = Mathf.Clamp(sizePerArea, 1, 200);
            mapCount = Mathf.Clamp(mapCount, 1, 100);

            EditorGUILayout.Space(5);

            // 瓦片选择
            EditorGUILayout.LabelField("瓦片选择", EditorStyles.boldLabel);
            defaultTile = (TileBase)EditorGUILayout.ObjectField("默认瓦片", defaultTile, typeof(TileBase), false);

            EditorGUILayout.Space(5);

            // 随机瓦片选择
            EditorGUILayout.LabelField("随机瓦片设置", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            showRandomTiles = EditorGUILayout.Foldout(showRandomTiles, $"随机瓦片列表 ({randomTiles.Count} 个)");
            if (GUILayout.Button("清空", GUILayout.Width(50)))
            {
                randomTiles.Clear();
            }
            EditorGUILayout.EndHorizontal();

            if (showRandomTiles)
            {
                EditorGUILayout.BeginVertical("box");

                // 添加随机瓦片按钮
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("添加随机瓦片", GUILayout.Height(25)))
                {
                    AddRandomTile();
                }
                EditorGUILayout.EndHorizontal();

                // 显示已选择的随机瓦片
                for (int i = randomTiles.Count - 1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"随机瓦片 {i + 1}:", GUILayout.Width(80));
                    randomTiles[i] = (TileBase)EditorGUILayout.ObjectField(randomTiles[i], typeof(TileBase), false);
                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        randomTiles.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            // 随机瓦片比例设置
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("随机瓦片比例:", GUILayout.Width(100));
            randomTileRatio = EditorGUILayout.Slider(randomTileRatio, 0f, 1f);
            EditorGUILayout.LabelField($"{(randomTileRatio * 100):F0}%", GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();

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
                    if (EditorUtility.DisplayDialog("确认填充", $"确定要使用默认瓦片填充 {sizePerArea}x{sizePerArea} 的地图吗？", "确定", "取消"))
                    {
                        FillMapWithDefaultTile();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请先选择默认瓦片", "确定");
                }
            }

            if (GUILayout.Button("生成随机地图", GUILayout.Height(30)))
            {
                if (defaultTile != null)
                {
                    if (EditorUtility.DisplayDialog("确认生成", $"确定要生成 {sizePerArea}x{sizePerArea} 的随机地图吗？\n随机瓦片比例: {(randomTileRatio * 100):F0}%", "确定", "取消"))
                    {
                        GenerateRandomMap();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "请先选择默认瓦片", "确定");
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("创建新地图", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("确认创建", $"确定要创建 {sizePerArea}x{sizePerArea} 的新地图吗？\n这将清除所有现有瓦片。", "确定", "取消"))
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
                if (EditorUtility.DisplayDialog("确认批量创建", $"确定要批量创建 {mapCount} 个 {sizePerArea}x{sizePerArea} 的地图吗？", "确定", "取消"))
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

            RefreshTileDisplay();
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
    /// 生成地图数据映射文件
    /// </summary>
    void CreateWorldMapFile()
    {
        if (currentTilemap == null)
        {
            Debug.LogWarning("未找到当前Tilemap，无法读取瓦片信息。");
            return;
        }

        // 尝试使用tilemap的有效范围进行遍历
        BoundsInt bounds = currentTilemap.cellBounds;

        WorldMapConfig mapConfig = new WorldMapConfig();
        mapConfig.MapSize = mapSize;
        mapConfig.SizePerArea = sizePerArea;
        MapLayer mapLayer = new MapLayer();
        mapConfig.Layers[0] = mapLayer;
        TileArea tileArea = new TileArea();
        mapConfig.Areas[0] = tileArea;

        // 先收集所有tile数据
        List<KeyValuePair<int, TileData>> tileList = new List<KeyValuePair<int, TileData>>();
        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = currentTilemap.GetTile(pos);
            if (tile == null)
                continue;
            if (tile == defaultTile)
                continue;

            int posIndex = pos.y + pos.x * 100;
            TileData tileData = new TileData(posIndex);
            string tileName = tile.name;
            tileData.Type = int.Parse(tileName.Split("_")[1]);
            tileList.Add(new KeyValuePair<int, TileData>(posIndex, tileData));
        }

        // 根据index排序后从小到大进行赋值
        tileList.Sort((a, b) => a.Key.CompareTo(b.Key));
        foreach (var kvp in tileList)
        {
            tileArea.Tiles[kvp.Key] = kvp.Value;
        }
        for (int y = 0; y < areaNumPerDire; y++)
        {
            for (int x = 0; x < areaNumPerDire; x++)
            {
                int index = y * areaNumPerDire + x;
                mapLayer.Areas[index] = 0;
            }
        }
        string json = JsonConvert.SerializeObject(mapConfig);
        File.WriteAllText("Assets/RainFramework/Resources/WorldMapConfig.json", json);
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
        for (int x = 0; x < sizePerArea; x++)
        {
            for (int y = 0; y < sizePerArea; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                currentTilemap.SetTile(position, defaultTile);
            }
        }


        RefreshTileDisplay();

        Debug.Log($"已使用默认瓦片填充 {sizePerArea}x{sizePerArea} 的地图");
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
            RefreshTileDisplay();
        }

        Debug.Log($"已创建 {sizePerArea}x{sizePerArea} 的新地图");
    }

    /// <summary>
    /// 刷新瓦片显示
    /// </summary>
    private void RefreshTileDisplay()
    {
        EditorUtility.SetDirty(currentTilemap);
        EditorUtility.SetDirty(currentPrefab);
        SceneView.RepaintAll();
        EditorApplication.QueuePlayerLoopUpdate();
        Repaint();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 刷新Game视图中的实例化对象
        RefreshGameViewInstances();
    }

    /// <summary>
    /// 刷新Game视图中的实例化对象
    /// </summary>
    private void RefreshGameViewInstances()
    {
        // 查找场景中所有使用当前预制体的实例
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            // 检查是否是当前预制体的实例
            if (PrefabUtility.GetCorrespondingObjectFromSource(obj) == currentPrefab)
            {
                // 强制刷新预制体实例
                PrefabUtility.RevertPrefabInstance(obj, InteractionMode.AutomatedAction);
                EditorUtility.SetDirty(obj);
            }
        }

        // 刷新Game视图
        EditorApplication.QueuePlayerLoopUpdate();
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
                for (int x = 0; x < sizePerArea; x++)
                {
                    for (int y = 0; y < sizePerArea; y++)
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

        RefreshTileDisplay();

        Debug.Log($"已将当前地图设置赋值到 {assignedCount} 个其他地图");
    }

    /// <summary>
    /// 批量创建地图
    /// </summary>
    private void BatchCreateMaps()
    {
        if (currentPrefab == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选择一个预制体", "确定");
            return;
        }

        if (mapCount <= 0)
        {
            EditorUtility.DisplayDialog("提示", "地图数量必须大于0", "确定");
            return;
        }

        string baseName = currentPrefab.name;
        string[] names = baseName.Split("_");
        baseName = names.Length > 0 ? names[0] : baseName;
        string basePath = AssetDatabase.GetAssetPath(currentPrefab);
        string directory = System.IO.Path.GetDirectoryName(basePath);

        int createdCount = 0;
        int skippedCount = 0;

        // 记录撤销操作
        Undo.RegisterCompleteObjectUndo(currentPrefab, "Batch Create Maps");

        for (int i = 1; i <= mapCount; i++)
        {
            string newName = $"{baseName}_{i}";
            string newPath = System.IO.Path.Combine(directory, newName + ".prefab");

            // 检查是否已存在同名预制体
            if (AssetDatabase.LoadAssetAtPath<GameObject>(newPath) != null)
            {
                skippedCount++;
                continue;
            }

            // 复制预制体
            if (AssetDatabase.CopyAsset(basePath, newPath))
            {
                createdCount++;
            }
        }

        // 刷新资源数据库
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 重新加载预制体列表
        LoadWorldMapPrefabs();

        // 显示结果
        string message = $"批量创建完成！\n成功创建: {createdCount} 个\n跳过已存在: {skippedCount} 个";
        EditorUtility.DisplayDialog("批量创建结果", message, "确定");

        Debug.Log($"批量创建完成！成功创建 {createdCount} 个预制体，跳过 {skippedCount} 个已存在的预制体");
    }

    /// <summary>
    /// 生成随机地图
    /// </summary>
    private void GenerateRandomMap()
    {
        if (currentTilemap == null || defaultTile == null)
        {
            return;
        }

        // 记录撤销操作
        Undo.RegisterCompleteObjectUndo(currentTilemap, "Generate Random Map");

        // 计算随机瓦片数量
        int totalTiles = sizePerArea * sizePerArea;
        int randomTileCount = Mathf.RoundToInt(totalTiles * randomTileRatio);

        // 创建位置列表用于随机选择
        List<Vector3Int> allPositions = new List<Vector3Int>();
        for (int x = 0; x < sizePerArea; x++)
        {
            for (int y = 0; y < sizePerArea; y++)
            {
                allPositions.Add(new Vector3Int(x, y, 0));
            }
        }

        // 先填充默认瓦片
        for (int x = 0; x < sizePerArea; x++)
        {
            for (int y = 0; y < sizePerArea; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                currentTilemap.SetTile(position, defaultTile);
            }
        }

        // 如果有随机瓦片，则随机替换部分瓦片
        if (randomTiles.Count > 0 && randomTileCount > 0)
        {
            // 随机打乱位置列表
            for (int i = 0; i < allPositions.Count; i++)
            {
                Vector3Int temp = allPositions[i];
                int randomIndex = Random.Range(i, allPositions.Count);
                allPositions[i] = allPositions[randomIndex];
                allPositions[randomIndex] = temp;
            }

            // 随机选择位置并放置随机瓦片
            for (int i = 0; i < randomTileCount && i < allPositions.Count; i++)
            {
                Vector3Int position = allPositions[i];
                TileBase randomTile = randomTiles[Random.Range(0, randomTiles.Count)];
                currentTilemap.SetTile(position, randomTile);
            }
        }


        RefreshTileDisplay();

        Debug.Log($"已生成 {sizePerArea}x{sizePerArea} 的随机地图，随机瓦片比例: {(randomTileRatio * 100):F0}%");
    }

    /// <summary>
    /// 添加随机瓦片
    /// </summary>
    private void AddRandomTile()
    {
        // 使用EditorGUIUtility.ShowObjectPicker显示对象选择器
        objectPickerControlID = EditorGUIUtility.GetControlID(FocusType.Passive);
        EditorGUIUtility.ShowObjectPicker<TileBase>(null, false, "", objectPickerControlID);
    }

    /// <summary>
    /// 打开选中的预制体
    /// </summary>
    private void OpenSelectedPrefab()
    {
        if (currentPrefab != null)
        {
            // 在Project窗口中高亮显示选中的预制体
            EditorGUIUtility.PingObject(currentPrefab);
            Selection.activeObject = currentPrefab;

            // 在预制体编辑器中打开预制体
            AssetDatabase.OpenAsset(currentPrefab);
        }
        else
        {
            EditorUtility.DisplayDialog("提示", "请先选择一个预制体", "确定");
        }
    }

}
