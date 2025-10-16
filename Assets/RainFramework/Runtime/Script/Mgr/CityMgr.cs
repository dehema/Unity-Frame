using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rain.Core;
using Rain.UI;

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
    private Dictionary<int, CityBuildingData> buildings = new Dictionary<int, CityBuildingData>();
    private Dictionary<int, BuildingSlotConfig> buildingSlots = new Dictionary<int, BuildingSlotConfig>();
    public Dictionary<int, BuildingSlotConfig> BuildingSlots { get => buildingSlots; set => buildingSlots = value; }

    // 事件系统
    public static event Action<CityBuildingData> OnBuildingStarted;
    public static event Action<CityBuildingData> OnBuildingCompleted;
    public static event Action<CityBuildingData> OnBuildingUpgraded;
    public static event Action<CityBuildingData> OnBuildingDestroyed;
    public static event Action<int> OnSlotUnlocked;

    // 当前选中的建筑
    private CityBuildingData selectedBuilding;
    private int selectedSlotID = -1;

    private void Start()
    {
    }

    void Update()
    {
        if (SceneMgr.Ins.currSceneID != SceneID.MainCity)
            return;
        UpdateBuildingStates();
    }

    /// <summary>
    /// 初始化城镇
    /// </summary>
    public void Init()
    {
        LoadBuildingSlots();
    }

    /// <summary>
    /// 加载建筑槽位配置
    /// </summary>
    private void LoadBuildingSlots()
    {
        BuildingSlots.Clear();
        foreach (BuildingSlotConfig slot in ConfigMgr.CityBuildingSlot.DataList)
        {
            if (slot.IsLocked)
            {
                continue;
            }
            BuildingSlots[slot.SlotID] = slot;
        }
        Debug.Log($"加载了 {BuildingSlots.Count} 个建筑槽位");
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
    /// 根据位置获取槽位ID
    /// </summary>
    private int GetSlotIDFromPosition(Vector3 position)
    {
        float minDistance = float.MaxValue;
        int closestSlotID = -1;

        foreach (var slot in BuildingSlots.Values)
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
    public void SelectBuilding(CityBuildingData buildingData)
    {
        selectedBuilding = buildingData;
        CityBuildingConfig config = buildingData.BuildingConfig;
        Debug.Log($"选中建筑: {config.BuildingName}");

        MsgMgr.Ins.DispatchEvent(MsgEvent.SelectCityBuilding, buildingData);
    }

    /// <summary>
    /// 选择槽位
    /// </summary>
    private void SelectSlot(int slotID)
    {
        selectedSlotID = slotID;
        selectedBuilding = null;

        var slot = BuildingSlots[slotID];
        Debug.Log($"选中槽位: {slotID} (位置: {slot.PosX}, {slot.PosZ})");
    }

    /// <summary>
    /// 升级建筑
    /// </summary>
    public void UpgradeBuilding(CityBuildingData _buildingData)
    {
        int targetLevel = _buildingData.Level.Value + 1;
        CityBuildingLevelConfig levelConfig = GetCityBuildingLevelConfig(_buildingData.BuildingType, targetLevel);
        if (levelConfig == null)
        {
            Debug.LogWarning("已达最高等级，无法升级");
            return;
        }
        //花费
        Dictionary<ResType, long> costRes = levelConfig.CostRes;
        bool isResEnough = PlayerMgr.Ins.IsResEnough(costRes);
        if (!isResEnough)
        {
            Debug.LogWarning("资源不足，无法升级");
            return;
        }
        //扣除资源
        foreach (var item in costRes)
        {
            PlayerMgr.Ins.AddResNum(item.Key, -item.Value);
        }
        //升级
        _buildingData.BuildStartTime = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
        _buildingData.BuildEndTime = _buildingData.BuildStartTime + levelConfig.CostTime;
        _buildingData.State.Value = (int)BuildingState.Building;
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

        var config = building.BuildingConfig;
        if (building.Level.Value >= config.MaxLevel)
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
        //building.BuildStartTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //building.BuildEndTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + config.UpgradeTime;
        building.State.Value = (int)BuildingState.Upgrading;

        //Debug.Log($"开始升级建筑: {config.Name} 到等级 {building.Level + 1}");
        return true;
    }

    /// <summary>
    /// 完成建筑建造/升级
    /// </summary>
    private void CompleteBuilding(CityBuildingData building)
    {
        if (building.IsBuilding)
        {
            building.State.Value = (int)BuildingState.Completed;
            OnBuildingCompleted?.Invoke(building);
            Debug.Log($"建筑建造完成: {building.BuildingConfig.BuildingName}");
        }
        else if (building.IsUpgrading)
        {
            building.Level.Value++;
            building.State.Value = (int)BuildingState.Completed;
            OnBuildingUpgraded?.Invoke(building);
            Debug.Log($"建筑升级完成: {building.BuildingConfig.BuildingName} 到等级 {building.Level}");
        }

        // 更新建筑对象
        UpdateBuildingObject(building);
    }

    /// <summary>
    /// 创建建筑对象
    /// </summary>
    private void CreateBuildingObject(CityBuildingData building)
    {
        var config = building.BuildingConfig;
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
    private void UpdateBuildingObject(CityBuildingData building)
    {
        if (building.BuildingObject != null)
        {
            var controller = building.BuildingObject.GetComponent<BuildingController>();
            controller?.UpdateBuildingModel();
        }
    }

    /// <summary>
    /// 检查槽位是否被占用
    /// </summary>
    private bool IsSlotOccupied(int slotID)
    {
        foreach (var building in buildings.Values)
        {
            if (building.SlotID == slotID && building.BuildingState != BuildingState.Destroyed)
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
    public CityBuildingData GetBuilding(int instanceID)
    {
        return buildings.GetValueOrDefault(instanceID);
    }

    public string GetBuildingModelPath(CityBuildingData _buildingData)
    {
        CityBuildingConfig BuildingConfig = _buildingData.BuildingConfig;
        string modelName;
        switch (_buildingData.BuildingState)
        {
            case BuildingState.Empty:
            case BuildingState.Destroyed:
                modelName = BuildingConfig.PlotModel;
                break;
            case BuildingState.Building:
            case BuildingState.Upgrading:
                modelName = BuildingConfig.ConstructionModel;
                break;
            case BuildingState.Completed:
            case BuildingState.Damaged:
            default:
                modelName = BuildingConfig.BuildingModel;
                break;
        }
        return $"Prefab/CityBuilding/{modelName}";
    }


    /// <summary>
    /// 获取状态文本
    /// </summary>
    public string GetStateText(BuildingState state)
    {
        switch (state)
        {
            case BuildingState.Empty: return "空槽位";
            case BuildingState.Building: return "建造中";
            case BuildingState.Completed: return "已完成";
            case BuildingState.Upgrading: return "升级中";
            default: return "未知状态";
        }
    }

    #region 配置数据
    /// <summary>
    /// 建筑信息
    /// </summary>
    /// <param name="_type"></param>
    /// <returns></returns>
    public CityBuildingConfig GetCityBuildingConfig(CityBuildingType _type)
    {
        return ConfigMgr.CityBuilding.Get(_type);
    }

    /// <summary>
    /// 建筑等级配置
    /// </summary>
    /// <returns></returns>
    public CityBuildingLevelConfig GetCityBuildingLevelConfig(CityBuildingType _type, int _lv)
    {
        CityBuildingLevelConfig config = ConfigMgr.CityBuildingLevel.Get(_type, _lv);
        return config;
    }

    /// <summary>
    /// 建筑位置配置
    /// </summary>
    /// <returns></returns>
    public BuildingSlotConfig GetCityBuildingSlotConfig(int _slotID)
    {
        return ConfigMgr.CityBuildingSlot.Get(_slotID);
    }
    #endregion
}
