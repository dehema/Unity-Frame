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

        //镜头
        CameraZoomIn,                   //镜头放大
        CameraZoomOut,                  //镜头缩小
        CameraZoomRatioChange,          //镜头倍率调整

        //场景
        SceneLoaded,                    //场景加载
        SceneUnload,                    //场景卸载

        //UI事件
        SelectCityBuilding,     //打开城市建筑详情界面
    }
}

