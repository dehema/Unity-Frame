# 城镇管理系统 (CityMgr) 使用说明

## 概述

CityMgr 是一个完整的SLG游戏城镇建筑管理系统，支持建筑建造、升级、状态管理等功能。建筑位置固定，每个槽位可以建造的建筑类型由配置表决定。

## 核心组件

### 1. CityMgr (城镇管理器)
- 管理所有建筑实例
- 处理建筑建造和升级逻辑
- 管理建筑槽位配置
- 提供事件系统

### 2. BuildingController (建筑控制器)
- 控制单个建筑的行为和显示
- 处理建筑状态变化
- 管理建筑视觉效果

### 3. BuildingData (建筑数据模型)
- 存储建筑实例数据
- 包含建筑状态、等级、位置等信息

## 配置表结构

### BuildingConfig (建筑配置表)
```json
{
  "ID": 1,                    // 建筑ID
  "BuildingType": 1,          // 建筑类型枚举
  "Name": "市政厅",           // 建筑名称
  "Description": "城镇核心",   // 建筑描述
  "MaxLevel": 5,              // 最大等级
  "BuildTime": 300,           // 建造时间(秒)
  "UpgradeTime": 600,         // 升级时间(秒)
  "BaseCost": "{\"gold\":1000}", // 基础建造消耗(JSON)
  "UpgradeCost": "{\"gold\":500}", // 升级消耗(JSON)
  "ResourceProduction": "{\"gold\":10}", // 资源产出(JSON)
  "ResourceStorage": "{\"gold\":5000}", // 资源存储(JSON)
  "PopulationCapacity": 100,  // 人口容量
  "DefenseValue": 100,        // 防御值
  "AttackValue": 0,           // 攻击值
  "AttackRange": 0,           // 攻击范围
  "ModelPath": "Prefab/Buildings/TownHall", // 模型路径
  "IconPath": "UI/Icons/TownHall", // 图标路径
  "Prerequisites": "[]",      // 前置建筑要求(JSON)
  "UnlockTech": "[]",         // 解锁科技要求(JSON)
  "Category": "core"          // 建筑分类
}
```

### BuildingSlotConfig (建筑槽位配置表)
```json
{
  "SlotID": 1,                // 槽位ID
  "PositionX": 0,             // X坐标
  "PositionZ": 0,             // Z坐标
  "RotationY": 0,             // Y轴旋转
  "AllowedBuildings": "[1]",  // 允许的建筑类型(JSON)
  "IsLocked": false,          // 是否锁定
  "UnlockCondition": "{}",    // 解锁条件(JSON)
  "SlotType": "center",       // 槽位类型
  "Size": 3                   // 槽位大小
}
```

## 使用方法

### 1. 初始化
```csharp
// CityMgr会自动初始化，加载配置表和现有建筑
// 确保在场景中有CityMgr组件
```

### 2. 建造建筑
```csharp
// 在指定槽位建造指定建筑
bool success = CityMgr.Instance.BuildBuilding(buildingID, slotID, level);
```

### 3. 升级建筑
```csharp
// 升级指定建筑
bool success = CityMgr.Instance.UpgradeBuilding(instanceID);
```

### 4. 获取建筑信息
```csharp
// 获取建筑数据
BuildingData building = CityMgr.Instance.GetBuilding(instanceID);

// 获取所有建筑
Dictionary<int, BuildingData> allBuildings = CityMgr.Instance.GetAllBuildings();

// 获取槽位配置
BuildingSlotConfig slot = CityMgr.Instance.GetSlotConfig(slotID);
```

### 5. 事件监听
```csharp
// 监听建筑事件
CityMgr.OnBuildingStarted += OnBuildingStarted;
CityMgr.OnBuildingCompleted += OnBuildingCompleted;
CityMgr.OnBuildingUpgraded += OnBuildingUpgraded;
CityMgr.OnBuildingDestroyed += OnBuildingDestroyed;
CityMgr.OnSlotUnlocked += OnSlotUnlocked;

private void OnBuildingStarted(BuildingData building)
{
    Debug.Log($"开始建造: {building.GetConfig().Name}");
}

private void OnBuildingCompleted(BuildingData building)
{
    Debug.Log($"建造完成: {building.GetConfig().Name}");
}
```

## 建筑状态

- **Empty**: 空槽位
- **Building**: 建造中
- **Completed**: 建造完成
- **Upgrading**: 升级中
- **Damaged**: 损坏
- **Destroyed**: 被摧毁

## 槽位类型

- **center**: 中心槽位（通常用于市政厅）
- **outer**: 外围槽位（用于一般建筑）
- **defense**: 防御槽位（用于防御建筑）
- **special**: 特殊槽位（用于高级建筑）

## 建筑分类

- **core**: 核心建筑
- **military**: 军事建筑
- **production**: 生产建筑
- **resource**: 资源建筑
- **defense**: 防御建筑
- **storage**: 存储建筑

## 注意事项

1. 确保配置表文件放在 `StreamingAssets/Config/` 目录下
2. 建筑模型预制体需要放在 `Resources/` 目录下
3. 每个建筑预制体需要添加 `BuildingController` 组件
4. 建筑槽位的位置是固定的，不能动态改变
5. 资源检查逻辑需要根据实际游戏需求实现
6. 存档系统需要保存建筑数据到本地或服务器

## 扩展功能

可以根据需要添加以下功能：
- 建筑拆除
- 建筑移动
- 建筑修理
- 建筑特效
- 建筑音效
- 建筑动画
- 建筑交互
- 建筑升级预览
- 建筑信息面板
- 建筑建造队列
