#define MAX
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using com.adjust.sdk;

public class ADMgr : MonoBehaviour
{
    public string MAX_SDK_KEY = "";
    public string MAX_REWARD_ID = "";
    public string MAX_INTER_ID = "";

    public bool isTest = false;
    public static ADMgr Ins { get; private set; }
    //public Text logText;
    public GameObject errorText;
    bool needPlay = true;
    bool isInit = false;
    bool isfirst = true;
    bool rewardVideoIsFinish = false;


    [HideInInspector]
    public bool isLeave = false;
    bool isAdFinish = false;
    [HideInInspector]
    public Action<bool> callBackAction;
    [HideInInspector]
    public string ad_video_index;
    [HideInInspector]
    public string ad_inter_index;
    [HideInInspector]
    public bool canInter = true;
    [HideInInspector]
    public bool canShowInter = true;
    [HideInInspector]
    public int noPlayAdType;
    [HideInInspector]
    public bool alreadyShow = false;
    bool isAutoStart = false;
    public bool bannerCanShow = false;
    string inter_networkname;
    string reward_networkname;
    string banner_networkname;
    bool sizeCanShowBanner = true;

    private Coroutine getAdjustAdidCoroutine;

    private void OnApplicationPause(bool pause)
    {
        if (NetInfoMgr.Ins.NetServerData == null)
        {
            return;
        }
#if IRONSOURCE
        IronSource.Agent.onApplicationPause(pause);
#endif
        if (!pause)
        {
            Debug.Log("ad:进入前台," + isLeave);
        }
        else
        {
            Debug.Log("进入后台");
        }

    }
    private void Awake()
    {
        float w = (float)Screen.width;
        float h = (float)Screen.height;
        float a = w / h;
        float b = (9f / 18f);

        if (a > b)
        {
            sizeCanShowBanner = false;
        }
    }
    private void Start()
    {

    }

    public void OnGameEnter()
    {
        if (!Application.isEditor)
        {
            Interstitial_Interval_Timer();
        }
    }

    public void initADCallBack()
    {
#if IRONSOURCE
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
        IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
        IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
        IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
        IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
        IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
        IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
        IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
        IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
        IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
#endif
#if MAX

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerRevenuePaidEvent;

        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;

        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
#endif
    }

    private void OnEnable()
    {
        Ins = this;
#if IRONSOURCE
        IronSource.Agent.init("1014089f1");
        initADCallBack();
        loadInterstital();
        LoadRewardedAd();
#endif
#if MAX
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            //MaxSdk.ShowMediationDebugger();

            Debug.Log("applovein init success");
            // AppLovin SDK is initialized, start loading ads
            initADCallBack();
            loadInterstital();
            //loadBannerAd();
            LoadRewardedAd();
            MaxSdk.SetCreativeDebuggerEnabled(true);
        };

        Action<string> InitMax = (_adjustId) =>
        {
#if UNITY_ANDROID
            string log = MAX_SDK_KEY;
            //log = GetSystemData.DecryptDES(MAX_SDK_KEY);
            Debug.Log(log);
            MaxSdk.SetSdkKey(log);
            // 将adjust id 传给Max
            if (!string.IsNullOrEmpty(_adjustId))
            {
                MaxSdk.SetUserId(_adjustId);
                MaxSdk.InitializeSdk();
            }
#else
            MaxSdk.SetSdkKey(MAX_SDK_KEY);
            MaxSdk.SetUserId(SaveDataManager.GetString(CConfig.sv_LocalUserId));
            MaxSdk.InitializeSdk();
#endif

#endif 
        };

        string adjustId = SaveDataManager.GetString(CConfig.sv_AdjustAdid);
        if (!string.IsNullOrEmpty(adjustId))
        {
            Utility.Log("获取到ADID：" + adjustId);
            InitMax(adjustId);
        }
        else
        {
            getAdjustAdidCoroutine = StartCoroutine(setAdjustAdid(InitMax));
        }
    }

    IEnumerator setAdjustAdid(Action<string> _InitMax)
    {
        int i = 0;
        while (i < 10)
        {
            yield return new WaitForSeconds(2);
            if (CommonUtil.IsEditor())
            {
                MaxSdk.SetUserId(SaveDataManager.GetString(CConfig.sv_LocalUserId));
                MaxSdk.InitializeSdk();
                StopCoroutine(getAdjustAdidCoroutine);
                break;
            }
            else
            {
                string adjustId = Adjust.getAdid();
                if (!string.IsNullOrEmpty(adjustId))
                {
                    SaveDataManager.SetString(CConfig.sv_AdjustAdid, adjustId);
                    Utility.Log("获取到ADID：" + adjustId);
                    MaxSdk.SetUserId(adjustId);
                    MaxSdk.InitializeSdk();
                    StopCoroutine(getAdjustAdidCoroutine);
                    _InitMax(adjustId);
                    break;
                }
            }
            i++;
        }
        if (i == 5)
        {
            MaxSdk.SetUserId(SaveDataManager.GetString(CConfig.sv_LocalUserId));
            MaxSdk.InitializeSdk();
        }
    }
    private void LoadRewardedAd()
    {
#if MAX
        MaxSdk.LoadRewardedAd(MAX_REWARD_ID);
#endif
    }

    //激励视频
    public void showVideo()
    {
        Debug.Log("Developer show video....");
#if IRONSOURCE
        IronSource.Agent.showRewardedVideo();
        Debug.Log("showRewardedVideo");
#endif
#if MAX
        ADMgr.Ins.isLeave = true;
        string placement = (ad_video_index) + "_" + reward_networkname;
        MaxSdk.ShowRewardedAd(MAX_REWARD_ID, placement);
#endif
    }
    public void playRewardVideo(Action<bool> callBack, int _index)
    {
        playRewardVideo(callBack, _index.ToString());
    }
    public void playRewardVideo(Action<bool> callBack, string index)
    {
        if (isTest)
        {
            Debug.Log("play reward once");
            callBack(true);
            //RobloxShopDataManager.Ins.SetTaskValue(1002, 1);

#if SOHOShop
            // 提现商店完成任务，根据实际情况调整
            SOHOShopManager.instance.AddTaskValue();
#endif
            return;
        }
        ad_video_index = index;
        PostEventScript.GetInstance().SendEvent("9001", index);
        PostEventScript.GetInstance().SendEvent("9003", index);
        callBackAction = callBack;
        bool rewardVideoReady = false;
#if IRONSOURCE
        rewardVideoReady = IronSource.Agent.isRewardedVideoAvailable();
#endif
#if MAX
        rewardVideoReady = MaxSdk.IsRewardedAdReady(MAX_REWARD_ID);
#endif

        if (rewardVideoReady)
        {
            Debug.Log("need play is ready");
            PostEventScript.GetInstance().SendEvent("9002", index);
            showVideo();
        }
        else
        {
            Debug.Log("need play not ready");
            ToastManager.GetInstance().ShowToast("No ads right now, please try it later.");
            callBackAction(false);
        }


    }
    public void loadFail()
    {
        PostEventScript.GetInstance().SendEvent("9005");
    }
    public void loadSuccess()
    {
        //logText.text += "\n " + DateTime.Now.ToString() + "OnRewardedVideoLoadedEvent";
        needPlay = true;
        PostEventScript.GetInstance().SendEvent("9004");
    }


    public void loadBannerAd()
    {
#if IRONSOURCE
        IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
#endif
#if MAX
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        //if (sizeCanShowBanner)
        //{
        //    MaxSdk.CreateBanner(MAX_BANNER_ID, MaxSdkBase.BannerPosition.BottomCenter);
        //    MaxSdk.SetBannerBackgroundColor(MAX_BANNER_ID, Color.white);
        //    MaxSdk.SetBannerWidth(MAX_BANNER_ID, 320);
        //}
#endif
    }
    public void showBanner()
    {

    }

    public void loadInterstital()
    {
        if (PostEventScript.GetInstance())
        {
            PostEventScript.GetInstance().SendEvent("9103");
        }
#if IRONSOURCE
        IronSource.Agent.loadInterstitial();
#endif
#if MAX
        MaxSdk.LoadInterstitial(MAX_INTER_ID);
#endif
    }
    //播放插屏
    public void ShowInterStitialAd(string interCode)
    {
        if (!CanPlayInterstitial())
        {
            return;
        }
        if (isTest)
        {
#if SOHOShop
            // 提现商店完成任务，根据实际情况调整
            SOHOShopManager.instance.AddTaskValue();
#endif
            return;
        }
        Debug.Log("inter_ad:尝试播放插屏");
        ad_inter_index = (int.Parse(interCode) + 100).ToString();
        StartCoroutine(showInterstitial());
    }
    IEnumerator showInterstitial()
    {
        if (canInter)
        {
            PostEventScript.GetInstance().SendEvent("9101", ad_inter_index);
            bool interReady = false;
#if IRONSOURCE
            interReady = IronSource.Agent.isInterstitialReady();
#endif
#if MAX
            interReady = MaxSdk.IsInterstitialReady(MAX_INTER_ID);
#endif

            if (!interReady)
            {
                Debug.Log("inter_ad:插屏没有准备好");
                canInter = false;
                alreadyShow = false;
                StartCoroutine("reloadInterAction");
                loadInterstital();
            }
            else
            {
                alreadyShow = false;
                PostEventScript.GetInstance().SendEvent("9102", ad_inter_index);
                if (ad_inter_index == "110")
                {
                    SaveDataManager.SetInt("NoThanksCount", 0);
                }
                showInterstital();
            }
        }
        else
        {
            Debug.Log("inter_ad:距离上次重新加载没有超过五秒");
        }
        yield return null;

    }
    public void showInterstital()
    {
#if IRONSOURCE
        IronSource.Agent.showInterstitial();
#endif
#if MAX
        ADMgr.Ins.isLeave = true;
        string placement = ad_inter_index + "_" + inter_networkname;
        MaxSdk.ShowInterstitial(MAX_INTER_ID, placement);
#endif
        Clear_Interstitial_Interval_Timer();
    }

    IEnumerator reloadInterAction()
    {
        yield return new WaitForSeconds(5);
        canInter = true;
        alreadyShow = false;

    }


    public void BannerReadyAndShow()
    {
        if (ADMgr.Ins.bannerCanShow)
        {
#if IRONSOURCE
            IronSource.Agent.displayBanner();
#endif
#if MAX
            if (sizeCanShowBanner)
            {
                //MaxSdk.ShowBanner(MAX_BANNER_ID);
            }
#endif
        }
        else
        {
#if IRONSOURCE

            IronSource.Agent.hideBanner();
#endif
#if MAX
            if (sizeCanShowBanner)
            {
                //MaxSdk.HideBanner(MAX_BANNER_ID);
            }
#endif
        }
    }
    public void InterClose()
    {
        ADMgr.Ins.loadInterstital();
        PostEventScript.GetInstance().SendEvent("9107", ADMgr.Ins.ad_inter_index);
    }
    public void InterReady()
    {
        ADMgr.Ins.canInter = true;
        ADMgr.Ins.alreadyShow = false;
        ADMgr.Ins.StopCoroutine("reloadInterAction");
        PostEventScript.GetInstance().SendEvent("9104");
    }
    public void RewardVideoFinish()
    {
        PostEventScript.GetInstance().SendEvent("9007", ADMgr.Ins.ad_video_index);
        ADMgr.Ins.callBackAction(true);
    }

#if IRONSOURCE

    //--------------ironsource--------------


    //<-------Banner------>
    //Invoked once the banner has loaded
    void BannerAdLoadedEvent()
    {
        Debug.Log("ironsource-BannerAdLoadedEvent");
        BannerReadyAndShow();
    }
    //Invoked when the banner loading process has failed.
    //@param description - string - contains information about the failure.
    void BannerAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("ironsource-BannerAdLoadFailedEvent:" + error.getErrorCode() + "-" + error.getDescription());
    }
    // Invoked when end user clicks on the banner ad
    void BannerAdClickedEvent()
    {
        Debug.Log("ironsource-BannerAdClickedEvent");
    }
    //Notifies the presentation of a full screen content following user click
    void BannerAdScreenPresentedEvent()
    {
        Debug.Log("ironsource-BannerAdScreenPresentedEvent");
    }
    //Notifies the presented screen has been dismissed
    void BannerAdScreenDismissedEvent()
    {
        Debug.Log("ironsource-BannerAdScreenDismissedEvent");
    }
    //Invoked when the user leaves the app
    void BannerAdLeftApplicationEvent()
    {
        Debug.Log("ironsource-BannerAdLeftApplicationEvent");
    }


    //<-------Inter------>
    // Invoked when the initialization process has failed.
    // @param description - string - contains information about the failure.
    void InterstitialAdLoadFailedEvent(IronSourceError error)
    {
        Debug.Log("ironsource-InterstitialAdLoadFailedEvent:"+error.getErrorCode()+"-"+error.getDescription());
        //loadInterstital();
    }
    // Invoked when the ad fails to show.
    // @param description - string - contains information about the failure.
    void InterstitialAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("ironsource-InterstitialAdShowFailedEvent:"+error.getErrorCode() + "-" + error.getDescription());
    }
    // Invoked when end user clicked on the interstitial ad
    void InterstitialAdClickedEvent()
    {
        Debug.Log("ironsource-InterstitialAdClickedEvent");
    }
    // Invoked when the interstitial ad closed and the user goes back to the application screen.
    void InterstitialAdClosedEvent()
    {
        Debug.Log("ironsource-InterstitialAdClosedEvent");
        InterClose();
    }
    // Invoked when the Interstitial is Ready to shown after load function is called
    void InterstitialAdReadyEvent()
    {
        Debug.Log("ironsource-InterstitialAdReadyEvent");
        InterReady();
    }
    // Invoked when the Interstitial Ad Unit has opened
    void InterstitialAdOpenedEvent()
    {
        Debug.Log("ironsource-InterstitialAdOpenedEvent");
    }
    // Invoked right before the Interstitial screen is about to open.
    // NOTE - This event is available only for some of the networks. 
    // You should treat this event as an interstitial impression, but rather use InterstitialAdOpenedEvent
    void InterstitialAdShowSucceededEvent()
    {
        Debug.Log("ironsource-InterstitialAdShowSucceededEvent");
        ADManager.Instance.pauseAutoPlayTime();
    }


    //<-------Video------>

    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy 
    //tasks till the video ad will be closed.
    void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("Ironsource-RewardedVideoAdOpenedEvent");
    }
    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    void RewardedVideoAdClosedEvent()
    {
        Debug.Log("Ironsource-RewardedVideoAdClosedEvent");
        
    }
    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available. 
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    void RewardedVideoAvailabilityChangedEvent(bool available)
    {
        Debug.Log("Ironsource-RewardedVideoAvailabilityChangedEvent:" + available);
        //Change the in-app 'Traffic Driver' state according to availability.
        if (available)
        {
            ADManager.Instance.loadSuccess();
        }
        else
        {
            ADManager.Instance.loadFail();
        }
    }

    //Invoked when the user completed the video and should be rewarded. 
    //If using server-to-server callbacks you may ignore this events and wait for 
    // the callback from the  ironSource server.
    //@param - placement - placement object which contains the reward data
    void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
    {
        Debug.Log("Ironsource-RewardedVideoAdRewardedEvent:"+placement);
        RewardVideoFinish();

    }
    //Invoked when the Rewarded Video failed to show
    //@param description - string - contains information about the failure.
    void RewardedVideoAdShowFailedEvent(IronSourceError error)
    {
        Debug.Log("Ironsource-RewardedVideoAdShowFailedEvent:" + error.getErrorCode() + "-" + error.getDescription());
        PostEventScript.Instance.SendEvent("9006",ad_video_index,error.getErrorCode().ToString());
    }

    // ----------------------------------------------------------------------------------------
    // Note: the events below are not available for all supported rewarded video ad networks. 
    // Check which events are available per ad network you choose to include in your build. 
    // We recommend only using events which register to ALL ad networks you include in your build. 
    // ----------------------------------------------------------------------------------------

    //Invoked when the video ad starts playing. 
    void RewardedVideoAdStartedEvent()
    {
        Debug.Log("Ironsource-RewardedVideoAdStartedEvent");
        ADManager.Instance.pauseAutoPlayTime();
    }
    //Invoked when the video ad finishes playing. 
    void RewardedVideoAdEndedEvent()
    {
        Debug.Log("Ironsource-RewardedVideoAdEndedEvent");
    }
#endif

#if MAX


    private void OnBannerLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        banner_networkname = adInfo.NetworkName;
        string placement = "201" + "_" + banner_networkname;
        MaxSdk.SetBannerPlacement(adUnitId, placement);
        BannerReadyAndShow();
    }
    private void OnBannerRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("OnBannerRevenuePaidEvent-" + "NetworkName:" + adInfo.NetworkName + ",AdUnitIdentifier:" + adInfo.AdUnitIdentifier + ",Revenue:" + adInfo.Revenue + ",CreativeIdentifier:" + adInfo.CreativeIdentifier + ",Placement:" + adInfo.Placement);
    }


    //<-------Inter------>

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        inter_networkname = adInfo.NetworkName;
        InterReady();
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        Debug.Log("OnInterstitialLoadFailedEvent" + errorInfo.Code + "," + errorInfo.Message);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

#if UNITY_IOS
        Time.timeScale = 0;
        AudioMgr.Ins.musicVolume = 0;
#endif

        PassRewardTime_Clear();
        SaveInterstitialTime();
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        //loadInterstital();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        ADMgr.Ins.isLeave = false;
#if UNITY_IOS
        Time.timeScale = 1;
        AudioMgr.Ins.musicVolume = 0;
#endif
        // Interstitial ad is hidden. Pre-load the next ad.
        InterClose();
        //ImpressionData data = new ImpressionData();
        //data.format = "inter";
        //data.placement = adInfo.Placement;
        //data.revenue = adInfo.Revenue;
        //WUDIServerManager.Ins.adWorth(data, ad_inter_index+100);
#if SOHOShop
        // 提现商店完成任务，根据实际情况调整
        SOHOShopManager.instance.AddTaskValue();
#endif
        Interstitial_Interval_Timer();
    }
    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {

        Debug.Log("OnInterstitialRevenuePaidEvent-" + "NetworkName:" + adInfo.NetworkName + ",AdUnitIdentifier:" + adInfo.AdUnitIdentifier + ",Revenue:" + adInfo.Revenue + ",CreativeIdentifier:" + adInfo.CreativeIdentifier + ",Placement:" + adInfo.Placement);
    }

    //<-------Video------>
    int VideoRetryAttempt;
    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        reward_networkname = adInfo.NetworkName;
        VideoRetryAttempt = 0;
        ADMgr.Ins.loadSuccess();
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        Debug.Log("OnRewardedAdLoadFailedEvent");
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).


        VideoRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, VideoRetryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
        //PostEventScript.Instance.SendEvent("9006", ad_video_index, error.getErrorCode().ToString());
        ADMgr.Ins.loadFail();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("ad:广告出现");

#if UNITY_IOS
        Time.timeScale = 0;
        AudioMgr.Ins.musicVolume = 0;
#endif
        PassRewardTime_Clear();
        SaveRewardTime();
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        Debug.Log("ad:广告播放完成");
        ADMgr.Ins.isLeave = false;
        if (rewardVideoIsFinish)
        {
            //RobloxShopDataManager.Ins.SetTaskValue(1002, 1);

#if UNITY_IOS
            Time.timeScale = 1;
            AudioMgr.Ins.musicVolume = 1;
#endif
            LoadRewardedAd();
            Debug.Log("OnRewardedAdHiddenEvent-finish");
            rewardVideoIsFinish = false;
            RewardVideoFinish();

#if SOHOShop
            // 提现商店完成任务，根据实际情况调整
            SOHOShopManager.instance.AddTaskValue();
#endif
        }
        else
        {
            LoadRewardedAd();
            Debug.Log("OnRewardedAdHiddenEvent-unfinish");
            callBackAction(false);
        }
    }
    IEnumerator addRewardVideTaskWaitTime()
    {
        yield return new WaitForSeconds(3);

    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("OnRewardedAdReceivedRewardEvent");
        // The rewarded ad displayed and the user should receive the reward.
        rewardVideoIsFinish = true;
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("OnRewardedAdRevenuePaidEvent-" + "NetworkName:" + adInfo.NetworkName + ",AdUnitIdentifier:" + adInfo.AdUnitIdentifier + ",Revenue:" + adInfo.Revenue + ",CreativeIdentifier:" + adInfo.CreativeIdentifier + ",Placement:" + adInfo.Placement);
        // Ad revenue paid. Use this callback to track user revenue.
    }

#endif


    private Decimal ChangeDataToD(string strData)
    {
        Decimal dData = 0.0M;
        if (strData.Contains("E"))
        {
            dData = Decimal.Parse(strData.ToString(), System.Globalization.NumberStyles.Float);
        }
        return dData;
    }

    /// <summary>
    /// 跳过一个激励视频
    /// </summary>
    public void PassRewardVideo()
    {
        DataMgr.Ins.gameData.passRewardVideoTime++;
    }

    /// <summary>
    /// 检查是否能播放跳过激励视频
    /// </summary>
    public void CheckPassRewardVideo()
    {
        if (DataMgr.Ins.gameData.passRewardVideoTime >= Utility.GetSetting<int>(SettingField.ad_Interstitial_pass))
        {
            ShowInterStitialAd(((int)ADInterstitalType.PassReward).ToString());
        }
    }


    /// <summary>
    /// 清除pass激励视频的数据
    /// </summary>
    public void PassRewardTime_Clear()
    {
        DataMgr.Ins.gameData.passRewardVideoTime = 0;
        DataMgr.Ins.SaveGameData();
    }

    /// <summary>
    /// 最后一次播放插屏广告的时间
    /// </summary>
    public void SaveInterstitialTime()
    {
        DataMgr.Ins.gameData.lastADInterstitialTime = DateTime.Now;
        DataMgr.Ins.SaveGameData();
    }

    /// <summary>
    /// 最后一次播放插屏广告的时间
    /// </summary>
    public void SaveRewardTime()
    {
        DataMgr.Ins.gameData.lastADRewardTime = DateTime.Now;
        DataMgr.Ins.SaveGameData();
    }

    /// <summary>
    /// 查看插屏广告间隔
    /// </summary>
    public void CheckInterstitialInterval()
    {
        double interSpin = (DateTime.Now - DataMgr.Ins.gameData.lastADInterstitialTime).TotalSeconds;
        double rewardSpin = (DateTime.Now - DataMgr.Ins.gameData.lastADRewardTime).TotalSeconds;
        if (rewardSpin < interSpin)
        {
            interSpin = rewardSpin;
        }
        if (interSpin >= Utility.GetSetting<int>(SettingField.ad_Interstitial_interval))
        {
            ShowInterStitialAd(((int)ADInterstitalType.Timing).ToString());
        }
    }

    TimerHandler interstitial_Interval_Timer = null;
    /// <summary>
    /// 插屏广告计时器
    /// </summary>
    public void Interstitial_Interval_Timer()
    {
        if (interstitial_Interval_Timer != null)
        {
            return;
        }
        int interval = Utility.GetSetting<int>(SettingField.ad_Interstitial_interval);
        interstitial_Interval_Timer = Timer.Ins.SetTimeOut(() =>
        {
            CheckInterstitialInterval();
        }, interval);
    }

    /// <summary>
    /// 清除插屏广告计时器
    /// </summary>
    private void Clear_Interstitial_Interval_Timer()
    {
        interstitial_Interval_Timer?.Remove();
        interstitial_Interval_Timer = null;
    }

    /// <summary>
    /// 是否能播放激励视频
    /// </summary>
    /// <returns></returns>
    public bool CanPlayInterstitial()
    {
        double spin = (DateTime.Now - DataMgr.Ins.gameData.lastADInterstitialTime).TotalSeconds;
        if (spin < Utility.GetSetting<int>(SettingField.ad_Interstitial_interval_limit))
        {
            //播放激励广告最小时间间隔
            return false;
        }
        return true;
    }
}