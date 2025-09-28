using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 建筑状态枚举
/// </summary>
public enum BuildingState
{
    /// <summary>
    /// 空槽位
    /// </summary>
    Empty = 0,

    /// <summary>
    /// 建造中
    /// </summary>
    Building = 1,

    /// <summary>
    /// 建造完成
    /// </summary>
    Completed = 2,

    /// <summary>
    /// 升级中
    /// </summary>
    Upgrading = 3,

    /// <summary>
    /// 损坏
    /// </summary>
    Damaged = 4,

    /// <summary>
    /// 被摧毁
    /// </summary>
    Destroyed = 5,
}
