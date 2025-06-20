using Rain.Core;
using Rain.Launcher;
using UnityEngine;

namespace Rain.Tests
{
    public class DemoSDK : MonoBehaviour
    {
        void Start()
        {
            // 启动SDK，平台id，渠道id
            RA.SDK.SDKStart("1", "1");

            // 登录
            RA.SDK.SDKLogin();

            // 登出
            RA.SDK.SDKLogout();

            // 切换账号
            RA.SDK.SDKSwitchAccount();

            // 加载视频广告
            RA.SDK.SDKLoadVideoAd("1", "1");

            // 播放视频广告
            RA.SDK.SDKShowVideoAd("1", "1");

            // 支付
            RA.SDK.SDKPay("serverNum", "serverName", "playerId", "playerName", "amount", "extra", "orderId",
                "productName", "productContent", "playerLevel", "sign", "guid");

            // 更新用户信息
            RA.SDK.SDKUpdateRole("scenes", "serverId", "serverName", "roleId", "roleName", "roleLeve", "roleCTime",
                "rolePower", "guid");

            // SDK退出游戏
            RA.SDK.SDKExitGame();

            // 原生提示
            RA.SDK.SDKToast("Native Toast");
        }
    }
}
