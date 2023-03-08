#define I2
#define Adjust
#define Applovin
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;
using AppLovinMax.Scripts.IntegrationManager.Editor;

public class SuperBuildWindow : UnityEditor.EditorWindow
{
    BuildInfoData info;
    public string Version;
    public Texture2D LogoTexture;
    public string buildPath;
    string[] buildTargets = new string[] { "iOS", "Android" };
    public int targetIndex;
    int nowTarget;
    //int index;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            targetIndex = 0;
            nowTarget = 0;
        }
        else
        {
            targetIndex = 1;
            nowTarget = 1;
        }
        refreshData();
    }
    void refreshData()
    {
        if (nowTarget == 0)
        {
            info = JsonMapper.ToObject<BuildInfoData>(BuildEditScript.ReadJsonFromStreamingAssetsPath("BuildJson_iOS.json"));
        }
        else
        {
            info = JsonMapper.ToObject<BuildInfoData>(BuildEditScript.ReadJsonFromStreamingAssetsPath("BuildJson_Android.json"));
        }
        buildPath = info.Build_path;
    }
    private void OnGUI()
    {
        GUILayout.Space(20);
        GUILayout.Label("***慎重填写校验以下信息!!!***");
        GUILayout.Label("***请确保场景中需要配置的各项id为public***");
        GUILayout.Label("***默认打包场景为当前编辑器打开的场景***");
        GUILayout.Label("***如果需要添加多个场景请联系遇先生***");
        GUILayout.Space(10);
        targetIndex = EditorGUILayout.Popup("构建平台", targetIndex, buildTargets, GUILayout.MaxWidth(300));
        GUILayout.Space(10);
        //水平布局
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("路径", GUILayout.Width(50f));
            buildPath = GUILayout.TextField(buildPath);
            if (GUILayout.Button("浏览", GUILayout.Width(50f)))
            {
                buildPath = EditorUtility.OpenFolderPanel("选择构建路径", Application.dataPath, info.GameName + (nowTarget == 0 ? "_iOS" : "_Android"));
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
        if (targetIndex != nowTarget)
        {
            nowTarget = targetIndex;
            refreshData();
        }
        GUILayout.Space(10);
        Version = EditorGUILayout.TextField("版本号", PlayerSettings.bundleVersion, GUILayout.MaxWidth(500));
        PlayerSettings.bundleVersion = Version;
        GUILayout.Space(10);
        LogoTexture = (Texture2D)EditorGUILayout.ObjectField("应用图标", PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown)[0], typeof(Texture2D), GUILayout.MaxWidth(200));
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { LogoTexture });
        GUILayout.Space(20);
        GUILayout.Label("应用名称:  " + info.GameName);
        GUILayout.Label("域名:" + info.BaseUrl);
        GUILayout.Label("PackageName:  " + info.PackageName);
        GUILayout.Label("Applovin_SDK_KEY:  " + info.Applovin_SDK_KEY);
        GUILayout.Label("Applovin_REWARD_ID:  " + info.Applovin_REWARD_ID);
        GUILayout.Label("Applovin_INTER_ID:  " + info.Applovin_INTER_ID);
        GUILayout.Label("Adjust_APP_ID:  " + info.Adjust_APP_ID);
        GUILayout.Label("GameCode:  " + info.GameCode);
        GUILayout.Label("Rate_Url:  " + info.Rate_Url);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        if (info.DesState == "DES")
        {
            if (GUILayout.Button("解密Applovin_SDK_KEY"))
            {
                info.DesState = "STR";
                info.Applovin_SDK_KEY = GetSystemData.DecryptDES(info.Applovin_SDK_KEY);

            }
        }
        else
        {
            if (GUILayout.Button("加密Applovin_SDK_KEY"))
            {
                info.DesState = "DES";
                info.Applovin_SDK_KEY = GetSystemData.EncryptDES(info.Applovin_SDK_KEY);

            }
        }
        
        
        GUILayout.EndHorizontal();

        if (GUILayout.Button("改值"))
        {
            BuildEditScript.Check(targetIndex,info);
        }
        if (GUILayout.Button("打包"))
        {
            BuildEditScript.build(targetIndex);
        }


    }


    // Update is called once per frame
    void Update()
    {

    }
}
