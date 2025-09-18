namespace Rain.Core
{
    public enum MsgEvent
    {
        // 框架事件，10000起步
        Empty = 10000,
        ApplicationFocus = 10001, // 游戏对焦
        NotApplicationFocus = 10002, // 游戏失焦
        ApplicationQuit = 10003, // 游戏退出
        // SDK回调信息
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
        RTSBattleUnitDie,       //单位死亡
        RTSBattleUnitAdd,
        RTSBattleUnitRemove,
        RTSBattleUnitMove,
        RTSBattleEnd,

        //RTS 单位
        RTSUnitHPChange,

        //鼠标
        CameraZoomingIn,        //视角放大
        CameraZoomingOut,       //视角缩小
        CameraZoomRatioChange,  //相机缩放倍率改变

    }
}

