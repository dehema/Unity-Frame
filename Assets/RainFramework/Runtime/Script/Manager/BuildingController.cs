using UnityEngine;
using Rain.Core;

/// <summary>
/// 建筑控制器 - 控制单个建筑的行为和显示
/// </summary>
public class BuildingController : MonoBehaviour
{
    [Header("建筑设置")]
    [SerializeField] private GameObject buildingModel;
    [SerializeField] private GameObject constructionEffect;
    [SerializeField] private GameObject upgradeEffect;
    [SerializeField] private GameObject selectionIndicator;
    
    // 建筑数据
    public BuildingData BuildingData { get; private set; }
    
    // 组件引用
    private Renderer buildingRenderer;
    private Collider buildingCollider;
    private Animator buildingAnimator;
    
    // 状态
    private bool isSelected = false;
    private bool isHovered = false;
    
    void Awake()
    {
        // 获取组件引用
        buildingRenderer = GetComponent<Renderer>();
        buildingCollider = GetComponent<Collider>();
        buildingAnimator = GetComponent<Animator>();
        
        // 如果没有Collider，添加一个
        if (buildingCollider == null)
        {
            buildingCollider = gameObject.AddComponent<BoxCollider>();
        }
    }
    
    void Start()
    {
        // 初始化选择指示器
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
    }
    
    void Update()
    {
        UpdateBuildingVisuals();
    }
    
    /// <summary>
    /// 初始化建筑
    /// </summary>
    public void Initialize(BuildingData buildingData)
    {
        BuildingData = buildingData;
        UpdateBuildingVisuals();
    }
    
    /// <summary>
    /// 更新建筑数据
    /// </summary>
    public void UpdateBuilding(BuildingData buildingData)
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
        
        // 根据建筑状态更新视觉效果
        switch (BuildingData.State)
        {
            case BuildingState.Building:
                ShowConstructionState();
                break;
            case BuildingState.Upgrading:
                ShowUpgradeState();
                break;
            case BuildingState.Completed:
                ShowCompletedState();
                break;
            case BuildingState.Damaged:
                ShowDamagedState();
                break;
            case BuildingState.Destroyed:
                ShowDestroyedState();
                break;
        }
        
        // 更新选择状态
        UpdateSelectionVisuals();
    }
    
    /// <summary>
    /// 显示建造中状态
    /// </summary>
    private void ShowConstructionState()
    {
        // 显示建造特效
        if (constructionEffect != null)
        {
            constructionEffect.SetActive(true);
        }
        
        // 隐藏升级特效
        if (upgradeEffect != null)
        {
            upgradeEffect.SetActive(false);
        }
        
        // 设置半透明材质
        if (buildingRenderer != null)
        {
            SetBuildingTransparency(0.5f);
        }
        
        // 播放建造动画
        if (buildingAnimator != null)
        {
            buildingAnimator.SetBool("IsBuilding", true);
        }
    }
    
    /// <summary>
    /// 显示升级中状态
    /// </summary>
    private void ShowUpgradeState()
    {
        // 隐藏建造特效
        if (constructionEffect != null)
        {
            constructionEffect.SetActive(false);
        }
        
        // 显示升级特效
        if (upgradeEffect != null)
        {
            upgradeEffect.SetActive(true);
        }
        
        // 设置半透明材质
        if (buildingRenderer != null)
        {
            SetBuildingTransparency(0.7f);
        }
        
        // 播放升级动画
        if (buildingAnimator != null)
        {
            buildingAnimator.SetBool("IsUpgrading", true);
        }
    }
    
    /// <summary>
    /// 显示完成状态
    /// </summary>
    private void ShowCompletedState()
    {
        // 隐藏所有特效
        if (constructionEffect != null)
        {
            constructionEffect.SetActive(false);
        }
        
        if (upgradeEffect != null)
        {
            upgradeEffect.SetActive(false);
        }
        
        // 设置不透明材质
        if (buildingRenderer != null)
        {
            SetBuildingTransparency(1f);
        }
        
        // 停止动画
        if (buildingAnimator != null)
        {
            buildingAnimator.SetBool("IsBuilding", false);
            buildingAnimator.SetBool("IsUpgrading", false);
        }
    }
    
    /// <summary>
    /// 显示损坏状态
    /// </summary>
    private void ShowDamagedState()
    {
        // 设置损坏材质
        if (buildingRenderer != null)
        {
            SetBuildingTransparency(0.8f);
            // 可以添加损坏贴图
        }
        
        // 播放损坏动画
        if (buildingAnimator != null)
        {
            buildingAnimator.SetBool("IsDamaged", true);
        }
    }
    
    /// <summary>
    /// 显示摧毁状态
    /// </summary>
    private void ShowDestroyedState()
    {
        // 隐藏建筑模型
        if (buildingModel != null)
        {
            buildingModel.SetActive(false);
        }
        
        // 显示废墟模型（如果有的话）
        // 可以添加废墟特效
    }
    
    /// <summary>
    /// 设置建筑透明度
    /// </summary>
    private void SetBuildingTransparency(float alpha)
    {
        if (buildingRenderer != null)
        {
            Material[] materials = buildingRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].HasProperty("_Color"))
                {
                    Color color = materials[i].color;
                    color.a = alpha;
                    materials[i].color = color;
                }
            }
        }
    }
    
    /// <summary>
    /// 更新选择视觉效果
    /// </summary>
    private void UpdateSelectionVisuals()
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(isSelected);
        }
    }
    
    /// <summary>
    /// 设置选中状态
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateSelectionVisuals();
    }
    
    /// <summary>
    /// 设置悬停状态
    /// </summary>
    public void SetHovered(bool hovered)
    {
        isHovered = hovered;
        
        // 可以添加悬停效果
        if (buildingRenderer != null)
        {
            if (hovered)
            {
                // 添加发光效果
                buildingRenderer.material.SetFloat("_Emission", 0.2f);
            }
            else
            {
                // 移除发光效果
                buildingRenderer.material.SetFloat("_Emission", 0f);
            }
        }
    }
    
    /// <summary>
    /// 获取建筑信息文本
    /// </summary>
    public string GetBuildingInfo()
    {
        if (BuildingData == null) return "无建筑数据";
        
        var config = BuildingData.GetConfig();
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
            case BuildingState.Damaged: return "损坏";
            case BuildingState.Destroyed: return "已摧毁";
            default: return "未知状态";
        }
    }
    
    /// <summary>
    /// 鼠标进入事件
    /// </summary>
    void OnMouseEnter()
    {
        SetHovered(true);
    }
    
    /// <summary>
    /// 鼠标离开事件
    /// </summary>
    void OnMouseExit()
    {
        SetHovered(false);
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

