﻿using UnityEngine;

namespace Rain.Core
{
    
    // 用户信息，登录回调中使用
    public class UserInfo
    {
        public string userId;
        public string userName;
        public string token;
    }
    
    // 支付信息，支付回调中使用
    public class PayResult
    {
        public string orderId;
        public string cpOrderId;
        public string extraParam;
    }
    
    /// <summary>
    /// 管理SDK的模块
    /// </summary>
    public class SDKManager : ModuleSingletonMono<SDKManager>, IModule
    {
        private SDKBase sdk; // SDK实例

        public string platformId = ""; // 平台id
        public string channelId = ""; // 渠道id
        public string userId = ""; // 用户ID
        public string userName = ""; // 用户名
        public string token = ""; // 令牌

        /// <summary>
        /// 模块初始化
        /// </summary>
        /// <param name="createParam">创建参数</param>
        public void OnInit(object createParam)
        {
            
        }

        public void OnUpdate()
        {
            
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            
        }

        /// <summary>
        /// 模块终止
        /// </summary>
        public void OnTermination()
        {
            Destroy(gameObject);
        }
        

        /// <summary>
        /// 启动SDK
        /// </summary>
        /// <param name="platformId">平台ID</param>
        /// <param name="channelId">渠道ID</param>
        public void SDKStart(string platformId, string channelId)
        {
            this.platformId = platformId;
            this.channelId = channelId;
#if UNITY_EDITOR || UNITY_STANDALONE
            sdk = new SDKBase();
#elif UNITY_ANDROID
            sdk = new SDKForAndroid();
#elif UNITY_IPHONE || UNITY_IOS
            sdk = new SDKForiOS();
#elif UNITY_WEBGL
            sdk = new SDKBase();
#else
            sdk = new SDKBase();
#endif
            SDKInit();
        }

        /// <summary>
        /// 初始化SDK
        /// </summary>
        public void SDKInit()
        {
            if (sdk == null) return;
            sdk.Init();
        }

        /// <summary>
        /// 登录SDK
        /// </summary>
        public void SDKLogin()
        {
            if (sdk == null) return;
            sdk.Login();
        }

        /// <summary>
        /// 登出SDK
        /// </summary>
        public void SDKLogout()
        {
            if (sdk == null) return;
            sdk.Logout();
        }

        /// <summary>
        /// 切换账号
        /// </summary>
        public void SDKSwitchAccount()
        {
            if (sdk == null) return;
            sdk.SwitchAccount();
        }
        
        /// <summary>
        /// 加载视频广告
        /// </summary>
        public void SDKLoadVideoAd(string id, string userId)
        {
            if (sdk == null) return;
            sdk.LoadVideoAd(id, userId);
        }
        
        /// <summary>
        /// 播放视频广告
        /// </summary>
        public void SDKShowVideoAd(string id, string userId)
        {
            if (sdk == null) return;
            sdk.ShowVideoAd(id, userId);
        }
        
        /// <summary>
        /// 使用SDK进行付费
        /// </summary>
        public void SDKPay(string serverNum, string serverName, string playerId, string playerName, string amount, string extra, string orderId,
            string productName, string productContent, string playerLevel, string sign, string guid)
        {
            if (sdk == null) return;
            sdk.Pay(serverNum, serverName, playerId, playerName, amount, extra, orderId,
                productName, productContent, playerLevel, sign, guid);
        }
        
        /// <summary>
        /// 更新角色信息
        /// </summary>
        public void SDKUpdateRole(string scenes, string serverId, string serverName, string roleId, string roleName,
            string roleLeve, string roleCTime, string rolePower, string guid)
        {
            if (sdk == null) return;
            sdk.UpdateRole(scenes, serverId, serverName, roleId, roleName, roleLeve, roleCTime, rolePower, guid);
        }
        
        /// <summary>
        /// SDK退出游戏
        /// </summary>
        public void SDKExitGame()
        {
            if (sdk == null) return;
            sdk.ExitGame();
        }
        
        /// <summary>
        /// 原生提示
        /// </summary>
        public void SDKToast(string msg)
        {
            if (sdk == null) return;
            sdk.Toast(msg);
        }
        
        
        /*----------------------------------------以下回调使用UnitySendMessage接收----------------------------------------*/
        
        /// <summary>
        /// SDK初始化成功回调
        /// </summary>
        /// <param name="msg">消息</param>
        public void OnInitSuccess(string msg)
        {
            RLog.LogSDK("OnInitSuccess：" + msg);
            
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnInitSuccess);
        }

        /// <summary>
        /// SDK初始化失败回调
        /// </summary>
        /// <param name="msg">消息</param>
        public void OnInitFail(string msg)
        {
            RLog.LogSDK("OnInitFail：" + msg);
            
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnInitFail);
        }

        /// <summary>
        /// 登录成功回调
        /// </summary>
        /// <param name="jsonString">JSON字符串</param>
        public void OnLoginSuccess(string jsonString)
        {
            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(jsonString);

            this.userId = userInfo.userId;
            this.userName = userInfo.userName;
            this.token = userInfo.token;
            
            RLog.LogSDK("OnLoginSuccess：" + this.userId);
            
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnLoginSuccess);
        }
        
        /// <summary>
        /// 登录失败回调
        /// </summary>
        /// <param name="msg">消息</param>
        public void OnLoginFail(string msg)
        {
            RLog.LogSDK("OnLoginFail：" + msg);
            
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnLoginFail);
        }
        
        /// <summary>
        /// 切换账号成功回调
        /// </summary>
        /// <param name="jsonString">JSON字符串</param>
        public void OnSwitchAccountSuccess(string jsonString)
        {
            UserInfo userInfo = JsonUtility.FromJson<UserInfo>(jsonString);

            this.userId = userInfo.userId;
            this.userName = userInfo.userName;
            this.token = userInfo.token;
            
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnSwitchAccountSuccess);
        }

        /// <summary>
        /// 登出成功回调
        /// </summary>
        /// <param name="msg">消息</param>
        public void OnLogoutSuccess(string msg)
        {
            RLog.LogSDK("OnLogoutSuccess：" + msg);
            
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnLogoutSuccess);
        }
        
        /// <summary>
        /// 支付成功回调
        /// </summary>
        /// <param name="jsonString">JSON字符串</param>
        public void OnPaySuccess(string jsonString)
        {
            PayResult payInfo = JsonUtility.FromJson<PayResult>(jsonString);
            
            RLog.LogSDK("OnLoginSuccess：" + payInfo.orderId);
            RLog.LogSDK("OnLoginSuccess：" + payInfo.cpOrderId);
            
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnPaySuccess);
        }

        /// <summary>
        /// 支付失败回调
        /// </summary>
        /// <param name="msg">消息</param>
        public void OnPayFail(string msg)
        {
            RLog.LogSDK("OnPayFail：" + msg);
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnPayFail);
        }

        /// <summary>
        /// 支付取消回调
        /// </summary>
        /// <param name="msg">消息</param>
        public void OnPayCancel(string msg)
        {
            RLog.LogSDK("OnPayCancel：" + msg);
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnPayCancel);
        }

        /// <summary>
        /// 退出成功回调
        /// </summary>
        /// <param name="msg">消息</param>
        public void OnExitSuccess(string msg)
        {
            RLog.LogSDK("OnExitSuccess：" + msg);
            MessageManager.Ins.DispatchEvent(MessageEvent.SDKOnExitSuccess);
        }
    }
}