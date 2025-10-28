using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rain.Core;

/// <summary>
/// 世界地图管理器
/// </summary>
public class WorldMapMgr : MonoBehaviour
{
    public static WorldMapMgr Ins;

    [Header("地图设置")]
    [SerializeField] private int perMapSize = 100; // 单张地图尺寸
    [SerializeField] private int totalMapSize = 1000; // 总地图尺寸
    [SerializeField] private int maxLoadedMaps = 9; // 最大同时加载的地图数量（3x3）

    [Header("预制体设置")]
    [SerializeField] private GameObject worldMapPrefab; // 世界地图预制体（worldMap_0_0）
    [SerializeField] private Transform mapContainer; // 地图容器

    // 地图管理
    private Dictionary<Vector2Int, GameObject> loadedMaps = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentMapIndex = Vector2Int.zero;
    private int maxMapCount; // 每个轴最大地图数量

    private void Awake()
    {
        Ins = this;
        maxMapCount = totalMapSize / perMapSize;

        // 创建地图容器
        if (mapContainer == null)
        {
            GameObject container = new GameObject("MapContainer");
            mapContainer = container.transform;
            mapContainer.localEulerAngles = new Vector3(90, 0, 45);
            mapContainer.SetParent(transform);
        }

        // 监听相机移动事件
        MsgMgr.Ins.AddEventListener(MsgEvent.WorldMap_Camera_Move, OnCameraMove, this);
    }

    private void Start()
    {
        // 初始加载中心地图
        LoadSurroundingMaps(Vector2Int.zero);
    }

    private void OnDestroy()
    {
        MsgMgr.Ins.RemoveEventListener(MsgEvent.WorldMap_Camera_Move, OnCameraMove, this);
    }

    /// <summary>
    /// 相机移动回调
    /// </summary>
    private void OnCameraMove(params object[] obj)
    {
        CheckAndLoadMaps();
    }

    /// <summary>
    /// 检查并加载地图
    /// </summary>
    private void CheckAndLoadMaps()
    {

        Vector3 cameraPos = CameraController_WorldMap.Ins.GetCameraLookPos();
        Vector2Int newMapIndex = GetMapIndexFromPosition(cameraPos);

        // 如果相机移动到了新的地图区域
        if (newMapIndex != currentMapIndex)
        {
            currentMapIndex = newMapIndex;

            // 加载周围的地图
            LoadSurroundingMaps(currentMapIndex);

            // 卸载远离的地图
            UnloadDistantMaps(currentMapIndex);
        }
    }

    /// <summary>
    /// 根据位置计算地图索引
    /// </summary>
    public Vector2Int GetMapIndexFromPosition(Vector3 worldPos)
    {
        Vector2Int localPos = WorldPosToLocal(worldPos);
        Vector2Int index = new Vector2Int(localPos.x / perMapSize, localPos.y / perMapSize);
        return index;
    }

    /// <summary>
    /// 加载周围的地图
    /// </summary>
    private void LoadSurroundingMaps(Vector2Int centerIndex)
    {
        // 如果当前加载的地图数量已达到上限，先卸载最远的地图
        if (loadedMaps.Count >= maxLoadedMaps)
        {
            UnloadFarthestMap(centerIndex);
        }

        // 加载3x3区域的地图
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int mapIndex = centerIndex + new Vector2Int(x, y);

                // 检查是否在总地图范围内
                if (IsMapIndexValid(mapIndex))
                {
                    LoadMap(mapIndex);
                }
            }
        }
    }

    /// <summary>
    /// 卸载最远的地图
    /// </summary>
    private void UnloadFarthestMap(Vector2Int centerIndex)
    {
        Vector2Int farthestMapIndex = Vector2Int.zero;
        float maxDistance = 0f;

        foreach (var kvp in loadedMaps)
        {
            Vector2Int mapIndex = kvp.Key;
            float distance = Vector2Int.Distance(mapIndex, centerIndex);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestMapIndex = mapIndex;
            }
        }

        if (maxDistance > 0f)
        {
            UnloadMap(farthestMapIndex);
            Debug.Log($"卸载最远地图: {farthestMapIndex} 距离: {maxDistance:F2}");
        }
    }

    /// <summary>
    /// 卸载远离的地图
    /// </summary>
    private void UnloadDistantMaps(Vector2Int centerIndex)
    {
        List<Vector2Int> mapsToUnload = new List<Vector2Int>();

        foreach (var kvp in loadedMaps)
        {
            Vector2Int mapIndex = kvp.Key;

            // 如果地图距离中心超过2格，则卸载
            if (Vector2Int.Distance(mapIndex, centerIndex) > 2f)
            {
                mapsToUnload.Add(mapIndex);
            }
        }

        // 卸载地图
        foreach (Vector2Int mapIndex in mapsToUnload)
        {
            UnloadMap(mapIndex);
        }
    }

    /// <summary>
    /// 检查地图索引是否有效
    /// </summary>
    private bool IsMapIndexValid(Vector2Int mapIndex)
    {
        return mapIndex.x >= 0 && mapIndex.x < maxMapCount &&
               mapIndex.y >= 0 && mapIndex.y < maxMapCount;
    }

    /// <summary>
    /// 加载指定索引的地图
    /// </summary>
    private void LoadMap(Vector2Int mapIndex)
    {
        if (loadedMaps.ContainsKey(mapIndex))
        {
            return; // 地图已加载
        }

        // 检查是否在总地图范围内
        if (!IsMapIndexValid(mapIndex))
        {
            Debug.LogWarning($"地图索引 {mapIndex} 超出总地图范围");
            return;
        }

        // 获取预制体（始终是同一个）
        GameObject prefab = GetMapPrefab(mapIndex);
        if (prefab == null)
        {
            Debug.LogError($"无法获取地图预制体，索引: {mapIndex}");
            return;
        }

        // 计算地图位置
        Vector3 mapPosition = GetMapPosition(mapIndex);

        // 实例化地图
        GameObject mapInstance = Instantiate(prefab, mapContainer);
        mapInstance.transform.localPosition = mapPosition;
        mapInstance.transform.localEulerAngles = Vector3.zero;

        mapInstance.name = $"Map_{mapIndex.x}_{mapIndex.y}";

        // 添加到已加载地图字典
        loadedMaps[mapIndex] = mapInstance;

        Debug.Log($"加载地图: {mapIndex} 位置: {mapPosition} 旋转: (90,0,45) 预制体: {prefab.name}", prefab);
    }

    /// <summary>
    /// 卸载指定索引的地图
    /// </summary>
    private void UnloadMap(Vector2Int mapIndex)
    {
        if (loadedMaps.TryGetValue(mapIndex, out GameObject mapInstance))
        {
            DestroyImmediate(mapInstance);
            loadedMaps.Remove(mapIndex);

            Debug.Log($"卸载地图: {mapIndex}");
        }
    }

    /// <summary>
    /// 根据地图索引获取预制体
    /// </summary>
    private GameObject GetMapPrefab(Vector2Int mapIndex)
    {
        if (worldMapPrefab == null)
        {
            worldMapPrefab = Resources.Load<GameObject>("Prefab/WorldMapTile/worldMap_0_0");
        }
        return worldMapPrefab;
    }

    /// <summary>
    /// 根据地图索引计算世界位置
    /// </summary>
    private Vector3 GetMapPosition(Vector2Int mapIndex)
    {
        float x = mapIndex.x * perMapSize;
        float y = mapIndex.y * perMapSize;

        return new Vector3(y, x, 0);
    }

    /// <summary>
    /// 世界坐标转本地坐标
    /// </summary>
    /// <returns></returns>
    public Vector2Int WorldPosToLocal(Vector3 _worldPos)
    {
        float angleRad = 45f * Mathf.Deg2Rad;
        Vector2Int localPos = new Vector2Int(
            Mathf.FloorToInt(Mathf.Abs(_worldPos.x * Mathf.Cos(angleRad) - _worldPos.z * Mathf.Sin(angleRad))),
            Mathf.FloorToInt(Mathf.Abs(_worldPos.x * Mathf.Sin(angleRad) + _worldPos.z * Mathf.Cos(angleRad))));
        return localPos;
    }

    /// <summary>
    /// 获取当前加载的地图数量
    /// </summary>
    public int GetLoadedMapCount()
    {
        return loadedMaps.Count;
    }

    /// <summary>
    /// 获取当前地图索引
    /// </summary>
    public Vector2Int GetCurrentMapIndex()
    {
        return currentMapIndex;
    }

    /// <summary>
    /// 获取当前加载的地图信息
    /// </summary>
    public Dictionary<Vector2Int, GameObject> GetLoadedMaps()
    {
        return new Dictionary<Vector2Int, GameObject>(loadedMaps);
    }
}