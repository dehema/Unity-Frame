using System;
using System.Collections.Generic;
using Rain.Core;
using UnityEngine;

/// <summary>
/// 建筑数据模型
/// </summary>
[Serializable]
public class BuildingData
{
    /// <summary>
    /// 建筑实例ID
    /// </summary>
    public int InstanceID;

    /// <summary>
    /// 建筑配置ID
    /// </summary>
    public int BuildingID;

    /// <summary>
    /// 槽位ID
    /// </summary>
    public int SlotID;

    /// <summary>
    /// 当前等级
    /// </summary>
    public int Level;

    /// <summary>
    /// 建造开始时间
    /// </summary>
    public long BuildStartTime;

    /// <summary>
    /// 建造结束时间
    /// </summary>
    public long BuildEndTime;

    /// <summary>
    /// 建筑状态
    /// </summary>
    public BuildingState State;

    /// <summary>
    /// 建筑位置
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// 建筑旋转
    /// </summary>
    public Quaternion Rotation;

    /// <summary>
    /// 是否已解锁
    /// </summary>
    public bool IsUnlocked;

    /// <summary>
    /// 解锁时间
    /// </summary>
    public long UnlockTime;

    /// <summary>
    /// 建筑实例对象
    /// </summary>
    [NonSerialized]
    public GameObject BuildingObject;

    /// <summary>
    /// 获取建筑配置
    /// </summary>
    public CityBuildingConfig GetConfig()
    {
        return new CityBuildingConfig(null);
        //return Tables.Instance.TbBuilding.GetOrDefault(BuildingID);
    }

    /// <summary>
    /// 获取槽位配置
    /// </summary>
    public BuildingSlotConfig GetSlotConfig()
    {
        return new BuildingSlotConfig(null);
        //return Tables.Instance.TbBuildingSlot.GetOrDefault(SlotID);
    }

    /// <summary>
    /// 是否正在建造中
    /// </summary>
    public bool IsBuilding => State == BuildingState.Building;

    /// <summary>
    /// 是否建造完成
    /// </summary>
    public bool IsCompleted => State == BuildingState.Completed;

    /// <summary>
    /// 是否正在升级中
    /// </summary>
    public bool IsUpgrading => State == BuildingState.Upgrading;

    /// <summary>
    /// 获取剩余建造时间（秒）
    /// </summary>
    public int GetRemainingBuildTime()
    {
        if (!IsBuilding && !IsUpgrading)
            return 0;

        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long remainingTime = BuildEndTime - currentTime;
        return Mathf.Max(0, (int)remainingTime);
    }

    /// <summary>
    /// 获取建造进度（0-1）
    /// </summary>
    public float GetBuildProgress()
    {
        if (!IsBuilding && !IsUpgrading)
            return 1f;

        long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long totalTime = BuildEndTime - BuildStartTime;
        long elapsedTime = currentTime - BuildStartTime;

        if (totalTime <= 0)
            return 1f;

        return Mathf.Clamp01((float)elapsedTime / totalTime);
    }
}
