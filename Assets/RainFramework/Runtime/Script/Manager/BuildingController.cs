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
    private GameObject currModelGo;

    // 建筑数据
    public CityBuildingData BuildingData { get; private set; }
    public BuildingSlotConfig SlotConfig => BuildingData.SlotConfig;
    public CityBuildingConfig BuildingConfig => BuildingData.BuildingConfig;
    BuildingState _buildingState = BuildingState.None;

    DBHandler.Binding binding;
    void Awake()
    {
        gameObject.layer = 7;
    }

    private void OnDestroy()
    {
        binding.UnBind();
    }

    /// <summary>
    /// 初始化建筑
    /// </summary>
    public void Init(CityBuildingData buildingData)
    {
        BuildingData = buildingData;
        if (binding == null)
            binding = BuildingData.Level.Bind(OnBuildingDataChange);
        UpdateBuildingModel();
    }

    private void OnBuildingDataChange(DBModify _dm)
    {
        if (BuildingData.Level.Value > 0 && BuildingData.BuildingState != BuildingState.Completed)
        {
            BuildingData.State.Value = (int)BuildingState.Completed;
        }
        UpdateBuildingModel();
    }

    /// <summary>
    /// 更新建筑
    /// </summary>
    public void UpdateBuildingModel()
    {
        if (BuildingData == null) return;
        if (BuildingData.Level.Value == 0)
        {
            ReplaceModelByState(BuildingState.Empty);
        }
        else
        {
            switch (BuildingData.BuildingState)
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
    }

    private void ReplaceModelByState(BuildingState state)
    {
        if (_buildingState == state)
        {
            return;
        }
        _buildingState = state;
        string path = CityMgr.Ins.GetBuildingModelPath(BuildingData);
        if (string.IsNullOrEmpty(path)) return;
        if (currModelGo != null)
        {
            Destroy(currModelGo);
            currModelGo = null;
        }

        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogWarning($"BuildingController: 模型路径无效 {path}");
            return;
        }
        currModelGo = Instantiate(prefab, transform);
        currModelGo.name = BuildingConfig.BuildingType.ToString();
        currModelGo.transform.localPosition = Vector3.zero;
        currModelGo.transform.localRotation = Quaternion.identity;
        currModelGo.transform.localScale = Vector3.one;

        TransferColliders(currModelGo);
    }

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
        info += $"状态: {CityMgr.Ins.GetStateText(BuildingData.BuildingState)}\n";

        if (BuildingData.IsBuilding || BuildingData.IsUpgrading)
        {
            info += $"进度: {BuildingData.GetBuildProgress():P0}\n";
            info += $"剩余时间: {BuildingData.GetRemainingBuildTime()}秒\n";
        }

        return info;
    }

    /// <summary>
    /// 鼠标点击事件
    /// </summary>
    public void OnSelect()
    {
        if (BuildingData != null)
        {
            CityMgr.Ins.SelectBuilding(BuildingData);
        }
    }

    /// <summary>
    /// 将子对象的碰撞体剪切给当前对象
    /// </summary>
    /// <param name="childObject">子对象</param>
    private void TransferColliders(GameObject childObject)
    {
        MeshCollider collider = childObject.GetComponent<MeshCollider>();
        if (collider == null) return;
        MeshCollider newMeshCollider = gameObject.AddComponent<MeshCollider>();
        newMeshCollider.sharedMesh = collider.sharedMesh;
        if (newMeshCollider != null)
        {
            DestroyImmediate(collider);
        }
    }
}

