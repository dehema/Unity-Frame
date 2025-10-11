# RainFramework Unity框架技术文档

## 框架概述

RainFramework是一个基于Unity的模块化游戏开发框架，专为SLG（策略类）游戏设计。该框架提供了完整的数据绑定系统、UI管理系统、模块化架构和热更新支持，能够快速构建复杂的游戏项目。

## 目录

- [RainFramework Unity框架技术文档](#rainframework-unity框架技术文档)
  - [目录](#目录)
  - [框架概述](#框架概述)
  - [核心特性](#核心特性)
    - [1. 模块化架构设计](#1-模块化架构设计)
    - [2. 强大的DB数据绑定系统](#2-强大的db数据绑定系统)
      - [2.1 核心组件](#21-核心组件)
      - [2.2 数据绑定特性](#22-数据绑定特性)
      - [2.3 事件传播机制](#23-事件传播机制)
      - [2.4 序列化支持](#24-序列化支持)
      - [2.5 数据绑定接口使用说明](#25-数据绑定接口使用说明)
        - [基础数据绑定](#基础数据绑定)
        - [复杂数据绑定](#复杂数据绑定)
      - [2.6 数据绑定使用示例](#26-数据绑定使用示例)
        - [创建新的数据类](#创建新的数据类)
        - [数据管理器接口使用](#数据管理器接口使用)
    - [3. 智能UI自动绑定功能](#3-智能ui自动绑定功能)
      - [3.1 UI管理器（UIMgr）](#31-ui管理器uimgr)
      - [3.2 自动绑定机制](#32-自动绑定机制)
      - [3.3 UI特性](#33-ui特性)
      - [3.4 UI管理器接口使用说明](#34-ui管理器接口使用说明)
        - [打开UI视图](#打开ui视图)
        - [关闭UI视图](#关闭ui视图)
        - [获取UI视图](#获取ui视图)
        - [层级管理](#层级管理)
      - [3.5 UI开发使用示例](#35-ui开发使用示例)
        - [创建新的UI视图](#创建新的ui视图)
        - [使用参数传递数据](#使用参数传递数据)
    - [4. GameLauncher启动器详解](#4-gamelauncher启动器详解)
      - [4.1 初始化流程](#41-初始化流程)
      - [4.2 核心模块功能](#42-核心模块功能)
      - [4.3 游戏启动流程](#43-游戏启动流程)
      - [4.4 各模块管理器接口使用说明](#44-各模块管理器接口使用说明)
        - [消息系统（MsgMgr）接口](#消息系统msgmgr接口)
        - [场景管理器（SceneMgr）接口](#场景管理器scenemgr接口)
        - [计时器系统接口](#计时器系统接口)
        - [资源管理器接口](#资源管理器接口)
        - [音频管理器接口](#音频管理器接口)
        - [配置管理器接口](#配置管理器接口)
    - [5. 城市建筑系统](#5-城市建筑系统)
      - [5.1 建筑管理（CityMgr）](#51-建筑管理citymgr)
      - [5.2 建筑控制器（BuildingController）](#52-建筑控制器buildingcontroller)
      - [5.3 城市管理器接口使用说明](#53-城市管理器接口使用说明)
        - [建筑管理](#建筑管理)
        - [建筑配置查询](#建筑配置查询)
        - [建筑事件监听](#建筑事件监听)
      - [5.4 建筑系统使用示例](#54-建筑系统使用示例)
        - [创建建筑控制器](#创建建筑控制器)
        - [建筑事件处理](#建筑事件处理)
    - [6. 场景管理系统](#6-场景管理系统)
      - [6.1 场景切换（SceneMgr）](#61-场景切换scenemgr)
      - [6.2 事件系统](#62-事件系统)
    - [7. 配置管理系统](#7-配置管理系统)
      - [7.1 配置管理器（ConfigMgr）](#71-配置管理器configmgr)
      - [7.2 配置类型](#72-配置类型)
    - [8. 热更新支持](#8-热更新支持)
      - [8.1 HybridCLR集成](#81-hybridclr集成)
      - [8.2 热更新脚本](#82-热更新脚本)
    - [9. 性能优化特性](#9-性能优化特性)
      - [9.1 内存管理](#91-内存管理)
      - [9.2 渲染优化](#92-渲染优化)
    - [10. 开发工具支持](#10-开发工具支持)
      - [10.1 编辑器工具](#101-编辑器工具)
      - [10.2 测试框架](#102-测试框架)
  - [使用示例](#使用示例)
    - [创建新的UI视图](#创建新的ui视图-1)
    - [创建新的数据类](#创建新的数据类-1)
    - [添加新的游戏模块](#添加新的游戏模块)
  - [最佳实践](#最佳实践)
    - [1. UI开发最佳实践](#1-ui开发最佳实践)
    - [2. 数据管理最佳实践](#2-数据管理最佳实践)
    - [3. 消息系统最佳实践](#3-消息系统最佳实践)
  - [总结](#总结)

## 核心特性

### 1. 模块化架构设计
- **模块中心（ModuleCenter）**：统一管理所有游戏模块的生命周期
- **单例模式**：确保模块的全局唯一性和易访问性
- **依赖注入**：支持模块间的松耦合设计
- **生命周期管理**：提供完整的初始化、更新、销毁流程

### 2. 强大的DB数据绑定系统

#### 2.1 核心组件
- **DBObject**：数据绑定的基础抽象类
- **DBClass**：支持反射的复杂数据结构
- **DBHandler**：绑定管理器，处理数据绑定关系
- **DBBinding**：绑定句柄，支持自动解绑

#### 2.2 数据绑定特性
```csharp
// 支持多种数据类型
public class PlayerData : DBClass
{
    public DBString playerName;      // 字符串类型
    public DBInt level;             // 整数类型
    public DBFloat hp;             // 浮点类型
    public DBLong gold;            // 长整型
    public DBDictClass<int, CityBuildingData> cityBuildings; // 字典类型
}
```

#### 2.3 事件传播机制
- **父子关系传播**：子对象变化自动通知父对象
- **事件分发器**：支持异步事件处理
- **自动解绑**：UI关闭时自动清理绑定关系
- **类型安全**：编译时类型检查，避免运行时错误

#### 2.4 序列化支持
- **JSON序列化**：使用Newtonsoft.Json进行数据持久化
- **反射机制**：自动识别字段类型和属性
- **版本兼容**：支持数据结构的向后兼容

#### 2.5 数据绑定接口使用说明

##### 基础数据绑定
```csharp
// 在UI中绑定数据
public class PlayerInfoView : BaseView
{
    public Text nameText;
    public Text levelText;
    public Text goldText;
    
    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        
        // 绑定玩家数据
        DataBind(DataMgr.Ins.playerData.playerName, OnPlayerNameChanged);
        DataBind(DataMgr.Ins.playerData.level, OnPlayerLevelChanged);
        DataBind(DataMgr.Ins.playerData.gold, OnPlayerGoldChanged);
    }
    
    private void OnPlayerNameChanged(DBModify modify)
    {
        nameText.text = modify.value.ToString();
    }
    
    private void OnPlayerLevelChanged(DBModify modify)
    {
        levelText.text = $"等级: {modify.value}";
    }
    
    private void OnPlayerGoldChanged(DBModify modify)
    {
        goldText.text = $"金币: {modify.value}";
    }
}
```

##### 复杂数据绑定
```csharp
// 绑定建筑数据
public class BuildingInfoView : BaseView
{
    public Text buildingNameText;
    public Text buildingLevelText;
    public Text buildingStateText;
    
    private CityBuildingData currentBuilding;
    
    public void SetBuilding(CityBuildingData building)
    {
        // 解除之前的绑定
        UnBindAllDataBind();
        
        currentBuilding = building;
        
        // 绑定新建筑数据
        DataBind(building.Level, OnBuildingLevelChanged);
        DataBind(building.State, OnBuildingStateChanged);
        
        // 更新显示
        UpdateDisplay();
    }
    
    private void OnBuildingLevelChanged(DBModify modify)
    {
        buildingLevelText.text = $"等级: {modify.value}";
    }
    
    private void OnBuildingStateChanged(DBModify modify)
    {
        BuildingState state = (BuildingState)modify.value;
        buildingStateText.text = GetStateText(state);
    }
}
```

#### 2.6 数据绑定使用示例

##### 创建新的数据类
```csharp
public class MyData : DBClass
{
    public DBString name;
    public DBInt value;
    
    public MyData()
    {
        name = new DBString();
        value = new DBInt();
    }
}
```

##### 数据管理器接口使用
```csharp
// 数据加载和保存
DataMgr.Ins.Load();                    // 加载所有数据
DataMgr.Ins.SavePlayerData();          // 保存玩家数据
DataMgr.Ins.SaveGameData();           // 保存游戏数据
DataMgr.Ins.SaveSettingData();        // 保存设置数据
DataMgr.Ins.SaveAllData();            // 保存所有数据

// 访问玩家数据
var playerData = DataMgr.Ins.playerData;
string playerName = playerData.playerName.Value;
int playerLevel = playerData.level.Value;
long playerGold = playerData.gold.Value;

// 修改玩家数据（会自动触发数据绑定）
playerData.level.Value = 10;
playerData.gold.Value += 1000;

// 建筑数据操作
var cityBuildings = playerData.cityBuildings;
var newBuilding = new CityBuildingData();
newBuilding.SlotID = 1;
newBuilding.BuildingType = CityBuildingType.House;
cityBuildings.Add(1, newBuilding);
```

### 3. 智能UI自动绑定功能

#### 3.1 UI管理器（UIMgr）
- **层级管理**：支持多层级UI显示和排序
- **视图配置**：基于配置文件的UI管理
- **生命周期**：完整的UI创建、显示、隐藏、销毁流程
- **异步加载**：支持UI资源的异步加载

#### 3.2 自动绑定机制
```csharp
// 在BaseView中提供的数据绑定方法
protected void DataBind(DBObject dBObject, Action<DBModify> callfunc)
{
    DBBinding handler = dBObject.Bind(callfunc);
    dbHandlers.Add(handler);
}

// 自动解绑机制
protected void UnBindAllDataBind()
{
    foreach (var item in dbHandlers)
    {
        item.UnBind();
    }
}
```

#### 3.3 UI特性
- **泛型支持**：类型安全的UI操作
- **参数传递**：支持UI间的参数传递
- **动画效果**：内置的UI显示/隐藏动画
- **事件处理**：统一的UI事件管理

#### 3.4 UI管理器接口使用说明

##### 打开UI视图
```csharp
// 泛型方式打开UI（推荐）
var loginView = UIMgr.Ins.OpenView<LoginView>();
var mainView = UIMgr.Ins.OpenView<MainView>(viewParam);

// 字符串方式打开UI
UIMgr.Ins.OpenView("LoginView");
UIMgr.Ins.OpenView("MainView", param);

// 异步加载UI
UIMgr.Ins.OpenViewAsync<LoginView>(param, () => {
    Debug.Log("UI加载完成");
});
```

##### 关闭UI视图
```csharp
// 泛型方式关闭UI
UIMgr.Ins.CloseView<LoginView>();

// 字符串方式关闭UI
UIMgr.Ins.CloseView("LoginView");
```

##### 获取UI视图
```csharp
// 获取已存在的UI视图
var loginView = UIMgr.Ins.GetView<LoginView>();
var mainView = UIMgr.Ins.GetView("MainView");

// 检查UI是否显示
bool isShow = UIMgr.Ins.IsShow<LoginView>();
```

##### 层级管理
```csharp
// 刷新所有UI层级
UIMgr.Ins.RefreshAllViewLayer();

// 刷新指定层级
UIMgr.Ins.RefreshViewLayer("DialogLayer");

// 获取指定层级的显示视图
var dialogViews = UIMgr.Ins.GetShowViewsByLayer("DialogLayer");
```

#### 3.5 UI开发使用示例

##### 创建新的UI视图
```csharp
public class MyView : BaseView
{
    public Text titleText;
    public Button closeButton;
    
    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        
        // 绑定数据
        DataBind(DataMgr.Ins.playerData.level, OnLevelChanged);
        
        // 设置按钮事件
        closeButton.onClick.AddListener(Close);
    }
    
    private void OnLevelChanged(DBModify modify)
    {
        Debug.Log($"等级变化: {modify.value}");
    }
}
```

##### 使用参数传递数据
```csharp
public class MyViewParam : IViewParam
{
    public string title;
    public int value;
}

// 打开UI并传递参数
var param = new MyViewParam { title = "标题", value = 100 };
UIMgr.Ins.OpenView<MyView>(param);
```

### 4. GameLauncher启动器详解

#### 4.1 初始化流程
```csharp
IEnumerator Start()
{
    // 1. 基础设置
    DontDestroyOnLoad(Camera.main);
    Application.targetFrameRate = 60;
    
    // 2. 模块中心初始化
    ModuleCenter.Initialize(this);
    
    // 3. 核心模块创建
    RA.Msg = ModuleCenter.CreateModule<MsgMgr>();        // 消息系统
    RA.Data = ModuleCenter.CreateModule<DataMgr>();     // 数据管理
    RA.Timer = ModuleCenter.CreateModule<TimerMgr>();   // 计时器
    RA.Asset = ModuleCenter.CreateModule<AssetMgr>();   // 资源管理
    RA.Audio = ModuleCenter.CreateModule<AudioMgr>();   // 音频管理
    RA.UIMgr = ModuleCenter.CreateModule<UIMgr>();       // UI管理
    RA.Log = ModuleCenter.CreateModule<LogMgr>();       // 日志系统
    
    // 4. 游戏初始化
    ConfigMgr.Ins.Init();
    ConfigMgr.Ins.LoadAllConfig();
    DataMgr.Ins.Load();
    LangMgr.Ins.Init();
    UIMgr.Ins.Init(ConfigMgr.Ins.UIViewConfig);
    CityMgr.Ins.Init();
    
    // 5. 启动游戏
    StartGame();
}
```

#### 4.2 核心模块功能

**消息系统（MsgMgr）**
- 事件驱动的消息分发
- 支持自定义事件类型
- 解耦模块间通信

**数据管理（DataMgr）**
- 玩家数据持久化
- 游戏设置管理
- 多语言支持
- 自动保存机制

**资源管理（AssetMgr）**
- 资源加载和卸载
- 内存管理优化
- 支持AssetBundle

**UI管理（UIMgr）**
- 视图生命周期管理
- 层级排序系统
- 异步加载支持

**音频管理（AudioMgr）**
- 音效和背景音乐管理
- 音量控制
- 音频资源优化

**日志系统（LogMgr）**
- 分级日志输出
- 性能监控
- 错误追踪

#### 4.3 游戏启动流程
```csharp
public void StartGame()
{
    LoginViewParam param = new LoginViewParam();
    param.action = () => { EnterGame(); };
    UIMgr.Ins.OpenView<LoginView>(param);
}

private void EnterGame()
{
    SceneMgr.Ins.ChangeScene(SceneID.MainCity, () => { });
}
```

#### 4.4 各模块管理器接口使用说明

##### 消息系统（MsgMgr）接口
```csharp
// 发送消息
MsgMgr.Ins.DispatchEvent(MsgEvent.SceneLoaded);
MsgMgr.Ins.DispatchEvent(MsgEvent.SelectCityBuilding, buildingData);

// 监听消息
MsgMgr.Ins.AddListener(MsgEvent.SceneLoaded, OnSceneLoaded);
MsgMgr.Ins.AddListener(MsgEvent.SelectCityBuilding, OnBuildingSelected);
```

##### 场景管理器（SceneMgr）接口
```csharp
// 场景切换
SceneMgr.Ins.ChangeScene(SceneID.MainCity, () => {
    Debug.Log("场景切换完成");
});

// 获取当前场景
SceneID currentScene = SceneMgr.Ins.currSceneID;
Camera sceneCamera = SceneMgr.Ins.Camera;
```

##### 计时器系统接口
```csharp
// 在BaseView中使用计时器
public class GameView : BaseView
{
    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        
        // 延迟1秒后执行
        AddTimer(1f, 1f, 0, null, () => {
            Debug.Log("1秒后执行");
        });
        
        // 每2秒执行一次，共执行5次
        AddTimer(2f, 0f, 5, () => {
            Debug.Log("每2秒执行一次");
        }, () => {
            Debug.Log("计时器完成");
        });
    }
}
```

##### 资源管理器接口
```csharp
// 异步加载资源
AssetMgr.Ins.LoadAsync<GameObject>("Prefab/Player", (prefab) => {
    GameObject player = Instantiate(prefab);
    Debug.Log("玩家预制体加载完成");
});

// 异步加载UI资源
AssetMgr.Ins.LoadAsync<Sprite>("UI/Icon/ItemIcon", (sprite) => {
    itemImage.sprite = sprite;
});
```

##### 音频管理器接口
```csharp
// 播放音效
AudioMgr.Ins.PlaySound("ButtonClick");

// 播放背景音乐
AudioMgr.Ins.PlayMusic("BackgroundMusic");

// 设置音量
AudioMgr.Ins.soundVolume = 0.8f;
AudioMgr.Ins.musicVolume = 0.6f;
```

##### 配置管理器接口
```csharp
// 获取建筑配置
var buildingConfig = ConfigMgr.CityBuilding.Get(CityBuildingType.House);

// 获取建筑等级配置
var levelConfig = ConfigMgr.CityBuildingLevel.Get(buildingID, level);

// 获取建筑槽位配置
var slotConfig = ConfigMgr.CityBuildingSlot.Get(slotID);

// 获取科技配置
var techConfig = ConfigMgr.Tech.Get(techID);
```

### 5. 城市建筑系统

#### 5.1 建筑管理（CityMgr）
- **建筑状态管理**：空槽位、建造中、已完成、升级中
- **槽位系统**：基于配置的建筑槽位管理
- **事件系统**：建筑建造、升级、完成事件
- **资源消耗**：建造和升级的资源检查

#### 5.2 建筑控制器（BuildingController）
- **动态模型切换**：根据建筑状态切换模型
- **数据绑定**：建筑数据与UI的自动绑定
- **状态更新**：实时更新建筑状态和显示

#### 5.3 城市管理器接口使用说明

##### 建筑管理
```csharp
// 选择建筑
CityMgr.Ins.SelectBuilding(buildingData);

// 建造建筑
bool success = CityMgr.Ins.BuildBuilding(buildingID, slotID, level);

// 升级建筑
bool success = CityMgr.Ins.UpgradeBuilding(instanceID);

// 获取建筑数据
var building = CityMgr.Ins.GetBuilding(instanceID);
```

##### 建筑配置查询
```csharp
// 获取建筑配置
var buildingConfig = CityMgr.Ins.GetCityBuildingConfig(CityBuildingType.House);

// 获取建筑等级配置
var levelConfig = CityMgr.Ins.GetCityBuildingLevelConfig(CityBuildingType.House, 5);

// 获取建筑槽位配置
var slotConfig = CityMgr.Ins.GetCityBuildingSlotConfig(slotID);
```

##### 建筑事件监听
```csharp
// 监听建筑事件
CityMgr.OnBuildingStarted += OnBuildingStarted;
CityMgr.OnBuildingCompleted += OnBuildingCompleted;
CityMgr.OnBuildingUpgraded += OnBuildingUpgraded;

private void OnBuildingStarted(CityBuildingData building)
{
    Debug.Log($"开始建造: {building.BuildingConfig.BuildingName}");
}

private void OnBuildingCompleted(CityBuildingData building)
{
    Debug.Log($"建筑完成: {building.BuildingConfig.BuildingName}");
}
```

#### 5.4 建筑系统使用示例

##### 创建建筑控制器
```csharp
public class MyBuildingController : BuildingController
{
    public override void Init(CityBuildingData buildingData)
    {
        base.Init(buildingData);
        
        // 绑定建筑数据变化
        DataBind(BuildingData.Level, OnBuildingLevelChanged);
        DataBind(BuildingData.State, OnBuildingStateChanged);
    }
    
    private void OnBuildingLevelChanged(DBModify modify)
    {
        Debug.Log($"建筑等级变化: {modify.value}");
        UpdateBuildingModel();
    }
    
    private void OnBuildingStateChanged(DBModify modify)
    {
        BuildingState state = (BuildingState)modify.value;
        Debug.Log($"建筑状态变化: {state}");
        UpdateBuildingModel();
    }
}
```

##### 建筑事件处理
```csharp
public class BuildingEventHandler : MonoBehaviour
{
    void Start()
    {
        // 监听建筑事件
        CityMgr.OnBuildingStarted += OnBuildingStarted;
        CityMgr.OnBuildingCompleted += OnBuildingCompleted;
        CityMgr.OnBuildingUpgraded += OnBuildingUpgraded;
    }
    
    void OnDestroy()
    {
        // 取消监听
        CityMgr.OnBuildingStarted -= OnBuildingStarted;
        CityMgr.OnBuildingCompleted -= OnBuildingCompleted;
        CityMgr.OnBuildingUpgraded -= OnBuildingUpgraded;
    }
    
    private void OnBuildingStarted(CityBuildingData building)
    {
        // 显示建造进度UI
        UIMgr.Ins.OpenView<BuildingProgressView>(building);
    }
    
    private void OnBuildingCompleted(CityBuildingData building)
    {
        // 播放完成特效
        PlayCompletionEffect(building);
        // 更新UI显示
        UIMgr.Ins.GetView<CityHUDView>()?.RefreshBuildingInfo();
    }
}
```

### 6. 场景管理系统

#### 6.1 场景切换（SceneMgr）
- **场景生命周期**：加载、卸载事件处理
- **UI自动管理**：场景切换时自动打开对应UI
- **相机管理**：场景相机的自动配置

#### 6.2 事件系统
```csharp
// 场景加载事件
private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    SceneChangeParam param = new SceneChangeParam();
    param.sceneName = scene.name;
    MsgMgr.Ins.DispatchEvent(MsgEvent.SceneLoaded, param);
    
    // 根据场景自动打开UI
    if (scene.name == SceneName.MainCity)
    {
        UIMgr.Ins.OpenView(ViewName.MainView);
        UIMgr.Ins.OpenView(ViewName.CityHUDView);
    }
}
```

### 7. 配置管理系统

#### 7.1 配置管理器（ConfigMgr）
- **多格式支持**：JSON、XML等配置文件格式
- **热更新支持**：运行时配置更新
- **类型安全**：强类型配置访问

#### 7.2 配置类型
- **UI配置**：视图层级、显示方式等
- **建筑配置**：建筑属性、升级条件等
- **科技配置**：科技树、解锁条件等
- **语言配置**：多语言文本管理

### 8. 热更新支持

#### 8.1 HybridCLR集成
- **AOT泛型支持**：AOTGenericReferences.cs
- **链接配置**：link.xml配置
- **代码生成**：自动生成热更新代码

#### 8.2 热更新脚本
- **HotUpdateScripts目录**：热更新脚本存放
- **模块化设计**：按功能模块组织代码
- **版本管理**：支持热更新版本控制

### 9. 性能优化特性

#### 9.1 内存管理
- **对象池**：GameObjectPool支持
- **资源卸载**：自动资源清理
- **垃圾回收优化**：减少GC压力

#### 9.2 渲染优化
- **URP支持**：Universal Render Pipeline
- **LOD系统**：多级细节模型
- **批处理**：Draw Call优化

### 10. 开发工具支持

#### 10.1 编辑器工具
- **UI导出工具**：自动生成UI代码
- **配置工具**：可视化配置编辑
- **调试工具**：运行时调试支持

#### 10.2 测试框架
- **单元测试**：模块化测试支持
- **集成测试**：端到端测试
- **性能测试**：性能监控和分析

## 使用示例

### 创建新的UI视图
```csharp
public class MyView : BaseView
{
    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        // 绑定数据
        DataBind(DataMgr.Ins.playerData.level, OnLevelChanged);
    }
    
    private void OnLevelChanged(DBModify modify)
    {
        // 处理等级变化
        Debug.Log($"等级变化: {modify.value}");
    }
}
```

### 创建新的数据类
```csharp
public class MyData : DBClass
{
    public DBString name;
    public DBInt value;
    
    public MyData()
    {
        name = new DBString();
        value = new DBInt();
    }
}
```

### 添加新的游戏模块
```csharp
public class MyModule : ModuleSingleton<MyModule>, IModule
{
    public void OnInit(object createParam) { }
    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void OnTermination() { }
}
```

## 最佳实践

### 1. UI开发最佳实践
```csharp
// 1. 使用泛型方式操作UI
var view = UIMgr.Ins.OpenView<MyView>(param);

// 2. 在UI中正确使用数据绑定
public class MyView : BaseView
{
    public override void Init(IViewParam viewParam = null)
    {
        base.Init(viewParam);
        // 绑定数据，会自动在关闭时解绑
        DataBind(DataMgr.Ins.playerData.level, OnLevelChanged);
    }
}

// 3. 使用参数传递数据
public class MyViewParam : IViewParam
{
    public string title;
    public int value;
}

var param = new MyViewParam { title = "标题", value = 100 };
UIMgr.Ins.OpenView<MyView>(param);
```

### 2. 数据管理最佳实践
```csharp
// 1. 使用DBClass创建数据结构
public class MyData : DBClass
{
    public DBString name;
    public DBInt value;
    
    public MyData()
    {
        name = new DBString();
        value = new DBInt();
    }
}

// 2. 在数据变化时自动保存
private void OnPlayerDataChanged(DBModify modify)
{
    DataMgr.Ins.SavePlayerData();
}
```

### 3. 消息系统最佳实践
```csharp
// 1. 使用强类型参数
public class BuildingSelectParam
{
    public CityBuildingData buildingData;
    public Vector3 position;
}

// 2. 在适当的时机发送消息
MsgMgr.Ins.DispatchEvent(MsgEvent.SelectCityBuilding, new BuildingSelectParam 
{ 
    buildingData = building, 
    position = worldPos 
});
```

## 总结

RainFramework是一个功能完整、设计精良的Unity游戏开发框架，特别适合开发SLG类游戏。其核心优势包括：

1. **模块化设计**：清晰的架构，易于维护和扩展
2. **数据绑定系统**：强大的响应式数据管理
3. **UI自动绑定**：减少重复代码，提高开发效率
4. **热更新支持**：支持游戏内容的动态更新
5. **性能优化**：内置多种性能优化机制
6. **开发工具**：完善的开发工具链支持

该框架为游戏开发提供了坚实的基础，能够显著提高开发效率和代码质量。通过合理使用上述接口和最佳实践，可以快速构建出高质量的游戏项目。
