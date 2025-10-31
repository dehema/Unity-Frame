namespace Rain.Core
{
    public enum MsgEvent
    {
        Empty = 10000,
        ApplicationFocus = 10001,       // 游戏获焦
        NotApplicationFocus = 10002,    // 游戏失焦
        ApplicationQuit = 10003,        // 游戏退出

        // SDK
        SDKOnInitSuccess = 10004,
        SDKOnInitFail = 10005,
        SDKOnLoginSuccess = 10006,
        SDKOnLoginFail = 10007,
        SDKOnSwitchAccountSuccess = 10008,
        SDKOnLogoutSuccess = 10009,
        SDKOnPaySuccess = 10010,
        SDKOnPayFail = 10011,
        SDKOnPayCancel = 10012,
        SDKOnExitSuccess = 10013,

        //RTS
        RTSBattleStart,
        RTSBattleUnitDie,               //RTS单位死亡
        RTSBattleUnitAdd,
        RTSBattleUnitRemove,
        RTSBattleUnitMove,
        RTSBattleEnd,
        RTSUnitHPChange,                //RTS单位血量变化

        //RTS相机
        RTS_Camera_Move,                //RTS相机移动
        RTS_Camera_Zoom,                //RTS相机缩放

        //城镇相机
        City_Camera_Move,               //城镇相机移动
        City_Camera_Zoom,               //城镇相机缩放

        //世界地图相机
        WorldMap_Camera_Move,           //世界地图相机移动
        WorldMap_Camera_Zoom,           //世界地图相机缩放
        WorldMap_SelectTile,            //世界地图选择地块（vector3世界坐标）


        //场景
        SceneLoaded,                    //场景加载
        SceneUnload,                    //场景卸载

        //UI事件
        SelectCityBuilding,             //打开城市建筑详情界面
    }
}

