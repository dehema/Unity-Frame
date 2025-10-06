using UnityEngine;
using Rain.Core;
using System.Collections.Generic;
using UnityEngine.Events;
using Rain.UI;

/// <summary>
/// 建筑控制器 - 控制单个建筑的行为和显示
/// </summary>
public class BuildingController : MonoBehaviour
{
    // 通过路径加载动态切换模型（不使用序列化模型和选择指示器）
    private GameObject currentModelInstance;
    private string currentModelPath;
    private int slotID;

    // 建筑数据
    public CityBuildingData BuildingData { get; private set; }
    public BuildingSlotConfig SlotConfig => BuildingData.SlotConfig;
    public CityBuildingConfig BuildingConfig => BuildingData.BuildingConfig;

    // 组件引用
    private Collider buildingCollider;

    void Awake()
    {
        // 获取组件引用（兼容由MainCityCreator挂载的模型）
        buildingCollider = GetComponent<Collider>();

        // 如果没有Collider，添加一个
        if (buildingCollider == null)
        {
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(8, 8, 8);
            boxCollider.center = new Vector3(0, 1.5f, 0);
            buildingCollider = boxCollider;
        }
    }

    void Update()
    {
        UpdateBuildingVisuals();
    }

    public void SetConfig()
    {

    }

    /// <summary>
    /// 初始化建筑
    /// </summary>
    public void Init(CityBuildingData buildingData)
    {
        BuildingData = buildingData;
        UpdateBuildingVisuals();
    }

    /// <summary>
    /// 更新建筑数据
    /// </summary>
    public void UpdateBuilding(CityBuildingData buildingData)
    {
        BuildingData = buildingData;
        UpdateBuildingVisuals();
    }

    /// <summary>
    /// 更新建筑视觉效果
    /// </summary>
    private void UpdateBuildingVisuals()
    {
        if (BuildingData == null) return;

        // 根据建筑状态更新视觉效果（通过模型替换）
        switch (BuildingData.State)
        {
            case BuildingState.Building:
                ReplaceModelByState(BuildingState.Building);
                break;
            case BuildingState.Upgrading:
                ReplaceModelByState(BuildingState.Upgrading);
                break;
            case BuildingState.Completed:
                ReplaceModelByState(BuildingState.Completed);
                break;
            case BuildingState.Empty:
                ReplaceModelByState(BuildingState.Empty);
                break;
        }

    }

    private void ReplaceModelByState(BuildingState state)
    {
        string path = GetStateModelPath(state);
        if (string.IsNullOrEmpty(path)) return;
        if (currentModelInstance != null && currentModelPath == path) return;

        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
            currentModelInstance = null;
        }

        var prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogWarning($"BuildingController: 模型路径无效 {path}");
            currentModelPath = null;
            return;
        }
        currentModelInstance = Instantiate(prefab, transform);
        currentModelInstance.transform.localPosition = Vector3.zero;
        currentModelInstance.transform.localRotation = Quaternion.identity;
        currentModelInstance.transform.localScale = Vector3.one;
        currentModelPath = path;
    }

    // 移除损坏/摧毁状态逻辑

    // 以显隐模型替代透明度/动画/特效
    private void SetActiveModel(GameObject go, bool active)
    {
        if (go == null) return;
        if (go.activeSelf != active) go.SetActive(active);
    }

    // 取消选择与悬停可视化
    // 移除材质发光相关代码

    /// <summary>
    /// 获取建筑信息文本
    /// </summary>
    public string GetBuildingInfo()
    {
        if (BuildingData == null) return "无建筑数据";

        var config = BuildingData.BuildingConfig;
        if (config == null) return "无建筑配置";

        string info = $"建筑: {config.BuildingName}\n";
        info += $"等级: {BuildingData.Level}/{config.MaxLevel}\n";
        info += $"状态: {GetStateText(BuildingData.State)}\n";

        if (BuildingData.IsBuilding || BuildingData.IsUpgrading)
        {
            info += $"进度: {BuildingData.GetBuildProgress():P0}\n";
            info += $"剩余时间: {BuildingData.GetRemainingBuildTime()}秒\n";
        }

        return info;
    }

    /// <summary>
    /// 获取状态文本
    /// </summary>
    private string GetStateText(BuildingState state)
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

    private string GetStateModelPath(BuildingState state)
    {
        string modelName;
        switch (state)
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
        return CityMgr.Ins.GetBuildingModelPath(BuildingConfig.PlotModel);
    }

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    void OnMouseDown()
    {
        if (BuildingData != null)
        {
            CityMgr.Ins.SelectBuilding(BuildingData);
        }
    }
}

