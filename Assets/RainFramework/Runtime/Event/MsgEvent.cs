namespace Rain.Core
{
    public enum MsgEvent
    {
        // ����¼���10000��
        Empty = 10000,
        ApplicationFocus = 10001, // ��Ϸ�Խ�
        NotApplicationFocus = 10002, // ��Ϸʧ��
        ApplicationQuit = 10003, // ��Ϸ�˳�
        // SDK�ص���Ϣ
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
        RTSBattleUnitDie,       //��λ����
        RTSBattleUnitAdd,
        RTSBattleUnitRemove,
        RTSBattleUnitMove,
        RTSBattleEnd,

        //RTS ��λ
        RTSUnitHPChange,

        //���
        CameraZoomingIn,        //�ӽǷŴ�
        CameraZoomingOut,       //�ӽ���С
        CameraZoomRatioChange,  //������ű��ʸı�

    }
}

