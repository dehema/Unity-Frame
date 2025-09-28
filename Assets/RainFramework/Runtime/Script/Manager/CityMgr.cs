using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rain.Core;

/// <summary>
/// 城镇管理器 - 管理SLG游戏的城镇建筑系统
/// </summary>
public class CityMgr : MonoSingleton<CityMgr>
{
    [Header("城镇设置")]
    [SerializeField] private Transform cityRoot; // 城镇根节点
    [SerializeField] private GameObject buildingPrefab; // 建筑预制体
    [SerializeField] private Material buildingPreviewMaterial; // 建筑预览材质

    // 建筑数据
    private Dictionary<int, BuildingData> buildings = new Dictionary<int, BuildingData>();
    private Dictionary<int, BuildingSlotConfig> buildingSlots = new Dictionary<int, BuildingSlotConfig>();

    // 事件系统
    public static event Action<BuildingData> OnBuildingStarted;
    public static event Action<BuildingData> OnBuildingCompleted;
    public static event Action<BuildingData> OnBuildingUpgraded;
    public static event Action<BuildingData> OnBuildingDestroyed;
    public static event Action<int> OnSlotUnlocked;

    // 当前选中的建筑
    private BuildingData selectedBuilding;
    private int selectedSlotID = -1;

    // 建筑实例ID计数器
    private int nextBuildingInstanceID = 1;

    void Start()
    {
        InitializeCity();
    }

    void Update()
    {
        UpdateBuildingStates();
        HandleInput();
    }

    /// <summary>
    /// 初始化城镇
    /// </summary>
    private void InitializeCity()
    {
        LoadBuildingSlots();
        LoadExistingBuildings();
    }

    /// <summary>
    /// 加载建筑槽位配置
    /// </summary>
    private void LoadBuildingSlots()
    {
        buildingSlots.Clear();

        //if (Tables.Instance?.TbBuildingSlot?.DataMap != null)
        //{
        //    foreach (var slot in Tables.Instance.TbBuildingSlot.DataMap.Values)
        //    {
        //        buildingSlots[slot.SlotID] = slot;
        //    }
        //}

        Debug.Log($"加载了 {buildingSlots.Count} 个建筑槽位");
    }

    /// <summary>
    /// 加载现有建筑数据
    /// </summary>
    private void LoadExistingBuildings()
    {
        // 这里应该从存档中加载建筑数据
        // 暂时创建一些示例数据
        CreateSampleBuildings();
    }

    /// <summary>
    /// 创建示例建筑（用于测试）
    /// </summary>
    private void CreateSampleBuildings()
    {
        // 在第一个槽位创建市政厅
        if (buildingSlots.Count > 0)
        {
            var firstSlot = buildingSlots[1]; // 假设槽位ID从1开始
            BuildBuilding(1, 1, 1); // 建造ID为1的建筑（市政厅）
        }
    }

    /// <summary>
    /// 更新建筑状态
    /// </summary>
    private void UpdateBuildingStates()
    {
        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        foreach (var building in buildings.Values)
        {
            if (building.IsBuilding || building.IsUpgrading)
            {
                if (currentTime >= building.BuildEndTime)
                {
                    CompleteBuilding(building);
                }
            }
        }
    }

    /// <summary>
    /// 处理输入
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    /// <summary>
    /// 处理鼠标点击
    /// </summary>
    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 检查是否点击了建筑
            var building = hit.collider.GetComponent<BuildingController>();
            if (building != null)
            {
                SelectBuilding(building.BuildingData);
            }
            else
            {
                // 检查是否点击了建筑槽位
                int slotID = GetSlotIDFromPosition(hit.point);
                if (slotID > 0)
                {
                    SelectSlot(slotID);
                }
            }
        }
    }

    /// <summary>
    /// 根据位置获取槽位ID
    /// </summary>
    private int GetSlotIDFromPosition(Vector3 position)
    {
        float minDistance = float.MaxValue;
        int closestSlotID = -1;

        foreach (var slot in buildingSlots.Values)
        {
            Vector3 slotPosition = new Vector3(slot.PosX, slot.PosY, slot.PosZ);
            float distance = Vector3.Distance(position, slotPosition);

            if (distance < minDistance && distance < 2f) // 2米范围内
            {
                minDistance = distance;
                closestSlotID = slot.SlotID;
            }
        }

        return closestSlotID;
    }

    /// <summary>
    /// 选择建筑
    /// </summary>
    public void SelectBuilding(BuildingData building)
    {
        selectedBuilding = building;
        selectedSlotID = -1;

        Debug.Log($"选中建筑: {building.GetConfig()?.BuildingName} (等级: {building.Level})");
    }

    /// <summary>
    /// 选择槽位
    /// </summary>
    private void SelectSlot(int slotID)
    {
        selectedSlotID = slotID;
        selectedBuilding = null;

        var slot = buildingSlots[slotID];
        Debug.Log($"选中槽位: {slotID} (位置: {slot.PosX}, {slot.PosZ})");
    }

    /// <summary>
    /// 建造建筑
    /// </summary>
    public bool BuildBuilding(int buildingID, int slotID, int level = 1)
    {
        //var buildingConfig = Tables.Instance?.TbBuilding?.GetOrDefault(buildingID);
        //var slotConfig = buildingSlots.GetValueOrDefault(slotID);

        //if (buildingConfig == null || slotConfig == null)
        //{
        //    Debug.LogError($"建筑配置或槽位配置不存在: BuildingID={buildingID}, SlotID={slotID}");
        //    return false;
        //}

        //// 检查槽位是否已被占用
        //if (IsSlotOccupied(slotID))
        //{
        //    Debug.LogWarning($"槽位 {slotID} 已被占用");
        //    return false;
        //}

        //// 检查是否允许在此槽位建造此建筑
        //if (!CanBuildInSlot(buildingID, slotID))
        //{
        //    Debug.LogWarning($"不允许在槽位 {slotID} 建造建筑 {buildingID}");
        //    return false;
        //}

        //// 检查资源是否足够
        //if (!HasEnoughResources(buildingConfig.BaseCost))
        //{
        //    Debug.LogWarning("资源不足，无法建造建筑");
        //    return false;
        //}

        //// 创建建筑数据
        //var buildingData = new BuildingData
        //{
        //    InstanceID = nextBuildingInstanceID++,
        //    BuildingID = buildingID,
        //    SlotID = slotID,
        //    Level = level,
        //    State = BuildingState.Building,
        //    BuildStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        //    BuildEndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + buildingConfig.BuildTime,
        //    Position = new Vector3(slotConfig.PositionX, 0, slotConfig.PositionZ),
        //    Rotation = Quaternion.Euler(0, slotConfig.RotationY, 0),
        //    IsUnlocked = true,
        //    UnlockTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        //};

        //// 消耗资源
        //ConsumeResources(buildingConfig.BaseCost);

        //// 添加到建筑列表
        //buildings[buildingData.InstanceID] = buildingData;

        //// 创建建筑对象
        //CreateBuildingObject(buildingData);

        //// 触发事件
        //OnBuildingStarted?.Invoke(buildingData);

        //Debug.Log($"开始建造建筑: {buildingConfig.Name} 在槽位 {slotID}");
        return true;
    }

    /// <summary>
    /// 升级建筑
    /// </summary>
    public bool UpgradeBuilding(int instanceID)
    {
        var building = buildings.GetValueOrDefault(instanceID);
        if (building == null || !building.IsCompleted)
        {
            Debug.LogWarning("建筑不存在或未完成建造");
            return false;
        }

        var config = building.GetConfig();
        if (building.Level >= config.MaxLevel)
        {
            Debug.LogWarning("建筑已达到最大等级");
            return false;
        }

        // 检查资源是否足够
        //if (!HasEnoughResources(config.cost))
        //{
        //    Debug.LogWarning("资源不足，无法升级建筑");
        //    return false;
        //}

        // 消耗资源
        //ConsumeResources(config.UpgradeCost);

        // 更新建筑状态
        building.State = BuildingState.Upgrading;
        building.BuildStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //building.BuildEndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + config.UpgradeTime;

        //Debug.Log($"开始升级建筑: {config.Name} 到等级 {building.Level + 1}");
        return true;
    }

    /// <summary>
    /// 完成建筑建造/升级
    /// </summary>
    private void CompleteBuilding(BuildingData building)
    {
        if (building.IsBuilding)
        {
            building.State = BuildingState.Completed;
            OnBuildingCompleted?.Invoke(building);
            Debug.Log($"建筑建造完成: {building.GetConfig()?.BuildingName}");
        }
        else if (building.IsUpgrading)
        {
            building.Level++;
            building.State = BuildingState.Completed;
            OnBuildingUpgraded?.Invoke(building);
            Debug.Log($"建筑升级完成: {building.GetConfig()?.BuildingName} 到等级 {building.Level}");
        }

        // 更新建筑对象
        UpdateBuildingObject(building);
    }

    /// <summary>
    /// 创建建筑对象
    /// </summary>
    private void CreateBuildingObject(BuildingData building)
    {
        var config = building.GetConfig();
        if (config == null) return;

        // 加载建筑模型
        //GameObject modelPrefab = Resources.Load<GameObject>(config.ModelPath);
        //if (modelPrefab == null)
        //{
        //    Debug.LogError($"无法加载建筑模型: {config.ModelPath}");
        //    return;
        //}

        //// 创建建筑对象
        //GameObject buildingObj = Instantiate(modelPrefab, cityRoot);
        //buildingObj.transform.position = building.Position;
        //buildingObj.transform.rotation = building.Rotation;
        //buildingObj.name = $"{config.BuildingName}_{building.InstanceID}";

        //// 添加建筑控制器
        //var controller = buildingObj.AddComponent<BuildingController>();
        //controller.Initialize(building);

        //building.BuildingObject = buildingObj;
    }

    /// <summary>
    /// 更新建筑对象
    /// </summary>
    private void UpdateBuildingObject(BuildingData building)
    {
        if (building.BuildingObject != null)
        {
            var controller = building.BuildingObject.GetComponent<BuildingController>();
            controller?.UpdateBuilding(building);
        }
    }

    /// <summary>
    /// 检查槽位是否被占用
    /// </summary>
    private bool IsSlotOccupied(int slotID)
    {
        foreach (var building in buildings.Values)
        {
            if (building.SlotID == slotID && building.State != BuildingState.Destroyed)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 检查是否可以在指定槽位建造指定建筑
    /// </summary>
    private bool CanBuildInSlot(int buildingID, int slotID)
    {
        //var slotConfig = buildingSlots.GetValueOrDefault(slotID);
        //if (slotConfig == null) return false;

        //// 解析允许的建筑类型
        //try
        //{
        //    var allowedBuildings = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(slotConfig.AllowedBuildings);
        //    return Array.Exists(allowedBuildings, id => id == buildingID);
        //}
        //catch
        //{
        //    return false;
        //}
        return false;
    }

    /// <summary>
    /// 检查是否有足够的资源
    /// </summary>
    private bool HasEnoughResources(string costJson)
    {
        // 这里应该检查玩家资源
        // 暂时返回true
        return true;
    }

    /// <summary>
    /// 消耗资源
    /// </summary>
    private void ConsumeResources(string costJson)
    {
        // 这里应该消耗玩家资源
        Debug.Log($"消耗资源: {costJson}");
    }

    /// <summary>
    /// 获取建筑数据
    /// </summary>
    public BuildingData GetBuilding(int instanceID)
    {
        return buildings.GetValueOrDefault(instanceID);
    }

    /// <summary>
    /// 获取所有建筑
    /// </summary>
    public Dictionary<int, BuildingData> GetAllBuildings()
    {
        return new Dictionary<int, BuildingData>(buildings);
    }

    ///// <summary>
    ///// 获取槽位配置
    ///// </summary>
    //public BuildingSlotConfig GetSlotConfig(int slotID)
    //{
    //    return buildingSlots.GetValueOrDefault(slotID);
    //}

    ///// <summary>
    ///// 获取所有槽位
    ///// </summary>
    //public Dictionary<int, BuildingSlotConfig> GetAllSlots()
    //{
    //    return new Dictionary<int, BuildingSlotConfig>(buildingSlots);
    //}
}
