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
    private int areaSize = 100;       // 单张地图尺寸
    private int mapSize = 1000;    // 总地图尺寸
    private int maxLoadedAreas = 10000;  // 最大同时加载的地图数量（3x3）
    private int currMapLayer = 0;       //当前地图lod层级

    [Header("预制体设置")]
    [SerializeField] private Transform mapContainer; // 地图容器

    // 地图管理
    private Dictionary<Vector2Int, GameObject> loadedAreas = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int currentAreaIndex = Vector2Int.zero;
    private Vector2Int lastVisibleAreaRange = Vector2Int.zero; // 上次的可见地图范围
    private int maxMapCount; // 每个轴最大地图数量

    private void Awake()
    {
        Ins = this;
        maxMapCount = mapSize / areaSize;

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
        MsgMgr.Ins.AddEventListener(MsgEvent.WorldMap_Camera_Zoom, OnCameraZoom, this);
    }

    private void Start()
    {
        // 初始加载中心地图
        Vector2Int initialRange = CalculateVisibleAreaRange();
        LoadSurroundingAreas(Vector2Int.zero, initialRange);
    }

    private void OnDestroy()
    {
        MsgMgr.Ins.RemoveEventListener(MsgEvent.WorldMap_Camera_Move, OnCameraMove, this);
        MsgMgr.Ins.RemoveEventListener(MsgEvent.WorldMap_Camera_Zoom, OnCameraZoom, this);
    }

    #region 相机
    /// <summary>
    /// 相机移动回调
    /// </summary>
    private void OnCameraMove(params object[] obj)
    {
        CheckAndLoadAreas();
    }

    private void OnCameraZoom(params object[] obj)
    {
        CheckAndLoadAreas();
    }
    #endregion

    #region 地图

    /// <summary>
    /// 检查并加载地图
    /// </summary>
    private void CheckAndLoadAreas()
    {
        if (CameraController_WorldMap.Ins == null || CameraController_WorldMap.Ins.mainCamera == null)
        {
            return;
        }
        //相机注视坐标
        Vector3 cameraPos = CameraController_WorldMap.Ins.GetCameraLookPos();
        //计算地图索引
        Vector2Int newAreaIndex = GetAreaIndexFromPosition(cameraPos);

        // 计算相机当前能看到的视野范围
        Vector2Int visibleAreaRange = CalculateVisibleAreaRange();

        // 如果相机移动到了新的地图区域或者缩放级别发生变化
        if (newAreaIndex != currentAreaIndex || visibleAreaRange != lastVisibleAreaRange)
        {
            currentAreaIndex = newAreaIndex;
            lastVisibleAreaRange = visibleAreaRange;

            // 加载周围的区域
            LoadSurroundingAreas(currentAreaIndex, visibleAreaRange);

            // 卸载远离的区域
            UnloadDistantAreas(currentAreaIndex, visibleAreaRange);
        }
    }
    #endregion

    #region 区域

    /// <summary>
    /// 根据位置计算区域索引
    /// </summary>
    public Vector2Int GetAreaIndexFromPosition(Vector3 _worldPos)
    {
        Vector2Int localPos = WorldPosToLocal(_worldPos);
        Vector2Int index = new Vector2Int(localPos.x / areaSize, localPos.y / areaSize);
        return index;
    }

    /// <summary>
    /// 计算相机当前能看到的视野范围（以地图数量为单位）
    /// </summary>
    private Vector2Int CalculateVisibleAreaRange()
    {
        if (CameraController_WorldMap.Ins == null || CameraController_WorldMap.Ins.mainCamera == null)
        {
            return Vector2Int.one; // 默认返回1x1范围
        }

        Camera camera = CameraController_WorldMap.Ins.mainCamera;

        // 获取相机的正交尺寸
        float orthographicSize = camera.orthographicSize;

        // 计算相机在世界坐标中的视野范围
        // 对于正交相机，orthographicSize表示相机视野高度的一半
        // 视野宽度 = orthographicSize * aspect ratio
        float aspectRatio = (float)Screen.width / Screen.height;
        float visibleHeight = orthographicSize * 2f; // 总高度
        float visibleWidth = visibleHeight * aspectRatio; // 总宽度

        // 考虑相机的旋转角度（45度）
        // 由于相机旋转了45度，实际可见范围需要考虑对角线
        float diagonalSize = Mathf.Sqrt(visibleWidth * visibleWidth + visibleHeight * visibleHeight);

        // 添加一些缓冲区域，确保边缘的地图也能被看到
        float bufferFactor = 1.5f; // 缓冲系数
        float effectiveSize = diagonalSize * bufferFactor;

        // 将世界坐标范围转换为地图数量
        // 由于地图容器旋转了45度，需要考虑这个变换
        int mapRangeX = Mathf.CeilToInt(effectiveSize / areaSize);
        int mapRangeY = Mathf.CeilToInt(effectiveSize / areaSize);

        // 确保最小范围为1，最大范围不超过总地图数量
        mapRangeX = Mathf.Clamp(mapRangeX, 1, maxMapCount);
        mapRangeY = Mathf.Clamp(mapRangeY, 1, maxMapCount);

        return new Vector2Int(mapRangeX, mapRangeY);
    }

    /// <summary>
    /// 加载周围的地图
    /// </summary>
    private void LoadSurroundingAreas(Vector2Int centerIndex, Vector2Int visibleRange)
    {
        // 如果当前加载的地图数量已达到上限，先卸载最远的地图
        if (loadedAreas.Count >= maxLoadedAreas)
        {
            UnloadFarthestArea(centerIndex);
        }

        // 根据可见范围动态加载地图
        int halfRangeX = visibleRange.x / 2;
        int halfRangeY = visibleRange.y / 2;

        // 计算加载范围的起始和结束位置
        int startX = Mathf.Max(0, centerIndex.x - halfRangeX);
        int endX = Mathf.Min(maxMapCount - 1, centerIndex.x + halfRangeX);
        int startY = Mathf.Max(0, centerIndex.y - halfRangeY);
        int endY = Mathf.Min(maxMapCount - 1, centerIndex.y + halfRangeY);

        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                Vector2Int areaIndex = new Vector2Int(x, y);

                // 检查是否在总地图范围内
                if (IsAreaIndexValid(areaIndex))
                {
                    LoadArea(areaIndex);
                }
            }
        }

        Debug.Log($"加载地图范围: 中心{centerIndex}, 范围{visibleRange}, 实际加载范围({startX},{startY})到({endX},{endY}), 总共{(endX - startX + 1) * (endY - startY + 1)}张地图");
    }

    /// <summary>
    /// 卸载最远的地图
    /// </summary>
    private void UnloadFarthestArea(Vector2Int _centerIndex)
    {
        Vector2Int farthestAreaIndex = Vector2Int.zero;
        float maxDistance = 0f;

        foreach (var kvp in loadedAreas)
        {
            Vector2Int areaIndex = kvp.Key;
            float distance = Vector2Int.Distance(areaIndex, _centerIndex);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestAreaIndex = areaIndex;
            }
        }

        if (maxDistance > 0f)
        {
            UnloadArea(farthestAreaIndex);
            Debug.Log($"卸载最远地图: {farthestAreaIndex} 距离: {maxDistance:F2}");
        }
    }

    /// <summary>
    /// 卸载远离的地图
    /// </summary>
    private void UnloadDistantAreas(Vector2Int centerIndex, Vector2Int visibleRange)
    {
        List<Vector2Int> mapsToUnload = new List<Vector2Int>();

        // 计算卸载距离阈值（比可见范围稍大一些，避免频繁加载卸载）
        int unloadThresholdX = visibleRange.x / 2 + 1;
        int unloadThresholdY = visibleRange.y / 2 + 1;

        // 计算应该保留的地图范围
        int keepStartX = Mathf.Max(0, centerIndex.x - unloadThresholdX);
        int keepEndX = Mathf.Min(maxMapCount - 1, centerIndex.x + unloadThresholdX);
        int keepStartY = Mathf.Max(0, centerIndex.y - unloadThresholdY);
        int keepEndY = Mathf.Min(maxMapCount - 1, centerIndex.y + unloadThresholdY);

        foreach (var kvp in loadedAreas)
        {
            Vector2Int areaIndex = kvp.Key;

            // 如果地图不在保留范围内，则卸载
            if (areaIndex.x < keepStartX || areaIndex.x > keepEndX ||
                areaIndex.y < keepStartY || areaIndex.y > keepEndY)
            {
                mapsToUnload.Add(areaIndex);
            }
        }

        // 卸载地图
        foreach (Vector2Int areaIndex in mapsToUnload)
        {
            UnloadArea(areaIndex);
        }

        if (mapsToUnload.Count > 0)
        {
            Debug.Log($"卸载了{mapsToUnload.Count}张远离的地图，保留范围({keepStartX},{keepStartY})到({keepEndX},{keepEndY})");
        }
    }

    /// <summary>
    /// 检查地图索引是否有效
    /// </summary>
    private bool IsAreaIndexValid(Vector2Int _areaIndex)
    {
        return _areaIndex.x >= 0 && _areaIndex.x < maxMapCount &&
               _areaIndex.y >= 0 && _areaIndex.y < maxMapCount;
    }

    /// <summary>
    /// 加载指定索引的地图
    /// </summary>
    private void LoadArea(Vector2Int _areaIndex)
    {
        if (loadedAreas.ContainsKey(_areaIndex))
        {
            return; // 地图已加载
        }

        // 检查是否在总地图范围内
        if (!IsAreaIndexValid(_areaIndex))
        {
            Debug.LogWarning($"地图索引 {_areaIndex} 超出总地图范围");
            return;
        }

        // 获取预制体（始终是同一个）
        GameObject prefab = GetAreaPrefab(_areaIndex);
        if (prefab == null)
        {
            Debug.LogError($"无法获取地图预制体，索引: {_areaIndex}");
            return;
        }

        // 计算地图位置
        Vector3 mapPosition = GetAreaPosition(_areaIndex);

        // 实例化地图
        GameObject mapInstance = Instantiate(prefab, mapContainer);
        mapInstance.transform.localPosition = mapPosition;
        mapInstance.transform.localEulerAngles = Vector3.zero;

        mapInstance.name = $"Map_{_areaIndex.x}_{_areaIndex.y}";

        // 添加到已加载地图字典
        loadedAreas[_areaIndex] = mapInstance;

        Debug.Log($"加载地图: {_areaIndex} 位置: {mapPosition} 旋转: (90,0,45) 预制体: {prefab.name}", prefab);
    }

    /// <summary>
    /// 卸载指定索引的区域
    /// </summary>
    private void UnloadArea(Vector2Int _areaIndex)
    {
        if (loadedAreas.TryGetValue(_areaIndex, out GameObject areaInstance))
        {
            DestroyImmediate(areaInstance);
            loadedAreas.Remove(_areaIndex);

            Debug.Log($"卸载地图: {_areaIndex}");
        }
    }

    /// <summary>
    /// 根据地图索引获取预制体
    /// </summary>
    private GameObject GetAreaPrefab(Vector2Int _areaIndex)
    {
        GameObject worldMapPrefab = AssetMgr.Ins.Load<GameObject>("worldMap_0_0");
        return worldMapPrefab;
    }

    /// <summary>
    /// 根据地图索引计算世界位置
    /// </summary>
    private Vector3 GetAreaPosition(Vector2Int _areaIndex)
    {
        float x = _areaIndex.x * areaSize;
        float y = _areaIndex.y * areaSize;

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
    /// 获取当前加载的区域数量
    /// </summary>
    public int GetLoadedAreaCount()
    {
        return loadedAreas.Count;
    }

    /// <summary>
    /// 获取当前地图索引
    /// </summary>
    public Vector2Int GetCurrentAreaIndex()
    {
        return currentAreaIndex;
    }

    /// <summary>
    /// 获取当前加载的地图信息
    /// </summary>
    public Dictionary<Vector2Int, GameObject> GetLoadedAreas()
    {
        return new Dictionary<Vector2Int, GameObject>(loadedAreas);
    }

    /// <summary>
    /// 获取当前可见的地图范围
    /// </summary>
    public Vector2Int GetCurrentVisibleAreaRange()
    {
        return lastVisibleAreaRange;
    }

    #endregion

    #region 调试
    /// <summary>
    /// 获取相机视野信息（用于调试）
    /// </summary>
    public string GetCameraViewInfo()
    {
        if (CameraController_WorldMap.Ins == null || CameraController_WorldMap.Ins.mainCamera == null)
        {
            return "相机未找到";
        }

        Camera camera = CameraController_WorldMap.Ins.mainCamera;
        float orthographicSize = camera.orthographicSize;
        float aspectRatio = (float)Screen.width / Screen.height;
        Vector2Int visibleRange = CalculateVisibleAreaRange();

        return $"相机尺寸: {orthographicSize:F2}, 屏幕比例: {aspectRatio:F2}, 可见地图范围: {visibleRange}, 已加载地图: {loadedAreas.Count}";
    }
    #endregion
}