using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Text;
using System.IO;
using LitJson;
using AppLovinMax.Scripts.IntegrationManager.Editor;

[ExecuteInEditMode]
public class BuildEditScript : MonoBehaviour
{
    static SuperBuildWindow buildWindow;
    //static BuildInfoData info;
    [MenuItem("遇先生工具包/一键[打包]")]
    static void ShowSuperBuildWindow()
    {
        EditorWindow.GetWindow(typeof(SuperBuildWindow));

    }
    public static void Check(int buildTargetIndex, BuildInfoData info)
    {
        //info = infoData;
        //if (buildTargetIndex == 0)
        //{
        //    info = JsonMapper.ToObject<BuildInfoData>(BuildEditScript.ReadJsonFromStreamingAssetsPath("BuildJson_iOS.json"));
        //}
        //else
        //{
        //    info = JsonMapper.ToObject<BuildInfoData>(BuildEditScript.ReadJsonFromStreamingAssetsPath("BuildJson_Android.json"));
        //}
        bool isSame = true;
        if (PlayerSettings.productName != info.GameName)
        {
            isSame = false;
            Debug.Log("名字");
        }
        if (PlayerSettings.applicationIdentifier != info.PackageName)
        {
            isSame = false;
            Debug.Log("包名");
        }
        if (Resources.FindObjectsOfTypeAll<NetInfoMgr>()[0].BaseUrl != info.BaseUrl)
        {
            isSame = false;
            Debug.Log("域名");
        }
#if UNITY_ANDROID
        if (AppLovinSettings.Instance.SdkKey != info.Applovin_SDK_KEY)
        {
            isSame = false;
            Debug.Log("max key");
        }
#else
        if (AppLovinSettings.Instance.SdkKey != info.Applovin_SDK_KEY)
        {
            isSame = false;
            Debug.Log("max key");
        }
#endif
        if (Resources.FindObjectsOfTypeAll<ADMgr>()[0].MAX_SDK_KEY != info.Applovin_SDK_KEY)
        {
            isSame = false;
            Debug.Log("admanager key");
        }
        if (Resources.FindObjectsOfTypeAll<ADMgr>()[0].MAX_REWARD_ID != info.Applovin_REWARD_ID)
        {
            isSame = false;
            Debug.Log("reward id");
        }
        if (Resources.FindObjectsOfTypeAll<ADMgr>()[0].MAX_INTER_ID != info.Applovin_INTER_ID)
        {
            isSame = false;
            Debug.Log("inter id");
        }
        if (Resources.FindObjectsOfTypeAll<NetInfoMgr>()[0].GameCode != info.GameCode)
        {
            isSame = false;
            Debug.Log("gamecode");
        }
        if (GameObject.Find("Adjust").GetComponent<com.adjust.sdk.Adjust>().appToken != info.Adjust_APP_ID)
        {
            isSame = false;
            Debug.Log("adjust");
        }
        if (!isSame)
        {
            bool YesOrNo = EditorUtility.DisplayDialog("警告", "构建信息中存在与过去版本不一致,确认要继续构建吗?", "是", "否");
            if (YesOrNo)
            {
                Debug.Log("开始构建");
                SuperBuild(buildTargetIndex, info);
            }
            else
            {
                Debug.Log("取消构建");
            }
        }
        else
        {
            Debug.Log("开始构建");
            SuperBuild(buildTargetIndex, info);
        }
    }
    public static void SuperBuild(int buildTargetIndex, BuildInfoData info)
    {
        Debug.Log("Start Build");
        SuperBuildWindow buildWindow = (SuperBuildWindow)EditorWindow.GetWindow(typeof(SuperBuildWindow));
        //buildcode本地+1
        info.BuildCode += 1;
        info.Build_path = buildWindow.buildPath;
        if (buildTargetIndex == 0)
        {
            string jsonString = JsonMapper.ToJson(info);
            WriteJsonFromStreamingAssetsPath("BuildJson_iOS.json", jsonString);
        }
        else
        {
            string jsonString = JsonMapper.ToJson(info);
            WriteJsonFromStreamingAssetsPath("BuildJson_Android.json", jsonString);
        }
        //version
        PlayerSettings.bundleVersion = buildWindow.Version;
        PlayerSettings.productName = info.GameName;
        PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { buildWindow.LogoTexture });
        if (buildTargetIndex == 0)
        {
            PlayerSettings.iOS.buildNumber = info.BuildCode.ToString();
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, info.PackageName);
        }
        else
        {
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.bundleVersionCode = info.BuildCode;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, info.PackageName);
        }
        AppLovinIntegrationManagerWindow manager = EditorWindow.GetWindow<AppLovinIntegrationManagerWindow>(utility: true, title: "", focus: true);
#if UNITY_ANDROID
        AppLovinSettings.Instance.SdkKey = info.Applovin_SDK_KEY;
#else
        AppLovinSettings.Instance.SdkKey = info.Applovin_SDK_KEY;
#endif
        Resources.FindObjectsOfTypeAll<ADMgr>()[0].MAX_SDK_KEY = info.Applovin_SDK_KEY;
        Resources.FindObjectsOfTypeAll<ADMgr>()[0].MAX_REWARD_ID = info.Applovin_REWARD_ID;
        Resources.FindObjectsOfTypeAll<ADMgr>()[0].MAX_INTER_ID = info.Applovin_INTER_ID;

        GameObject.Find("Adjust").GetComponent<com.adjust.sdk.Adjust>().appToken = info.Adjust_APP_ID;
        Resources.FindObjectsOfTypeAll<NetInfoMgr>()[0].GameCode = info.GameCode;
        Resources.FindObjectsOfTypeAll<NetInfoMgr>()[0].BaseUrl = info.BaseUrl;
        Resources.FindObjectsOfTypeAll<NetInfoMgr>()[0].BaseLoginUrl = info.BaseUrl + CConfig.LoginUrl;
        Resources.FindObjectsOfTypeAll<NetInfoMgr>()[0].BaseConfigUrl = info.BaseUrl + CConfig.ConfigUrl;
        Resources.FindObjectsOfTypeAll<NetInfoMgr>()[0].BaseTimeUrl = info.BaseUrl + CConfig.TimeUrl;
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("scene:" + EditorSceneManager.GetActiveScene().path);

        //EditorApplication.SaveScene();
        Debug.Log("Build Success");


    }
    IEnumerator closeMaxWindow(AppLovinIntegrationManagerWindow manage)
    {
        yield return new WaitForSeconds(1f);
        manage.Close();
    }
    public static void build(int buildTargetIndex)
    {
        BuildInfoData info;
        if (buildTargetIndex == 0)
        {
            info = JsonMapper.ToObject<BuildInfoData>(BuildEditScript.ReadJsonFromStreamingAssetsPath("BuildJson_iOS.json"));
        }
        else
        {
            info = JsonMapper.ToObject<BuildInfoData>(BuildEditScript.ReadJsonFromStreamingAssetsPath("BuildJson_Android.json"));
        }
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { EditorSceneManager.GetActiveScene().path };
        buildPlayerOptions.locationPathName = info.Build_path;
        if (buildTargetIndex == 0)
        {
            buildPlayerOptions.target = BuildTarget.iOS;
            buildPlayerOptions.options = BuildOptions.AcceptExternalModificationsToPlayer;
        }
        else
        {
            buildPlayerOptions.target = BuildTarget.Android;
        }
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
    [MenuItem("遇先生工具包/一键[压图]")]
    static void UpdatePlatformSpecificImageCompression()
    {
        var destFmt = TextureImporterFormat.ASTC_5x5;
        var numChanges = 0;

        foreach (var guid in AssetDatabase.FindAssets("t:Texture", new String[] { "Assets" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = TextureImporter.GetAtPath(path) as TextureImporter;

            // Skip things like TMPro font atlases
            if (importer == null) continue;

            var def = importer.GetDefaultPlatformTextureSettings();
            var changed = false;

            Action<TextureImporterPlatformSettings> maybeChange = (platSettings) =>
            {
                if (
                    platSettings.format != destFmt ||
                    platSettings.compressionQuality != def.compressionQuality ||
                    platSettings.maxTextureSize != def.maxTextureSize ||
                    !platSettings.overridden
                )
                {
                    platSettings.format = destFmt;
                    platSettings.compressionQuality = def.compressionQuality;
                    platSettings.maxTextureSize = def.maxTextureSize;
                    platSettings.overridden = true;

                    changed = true;
                    importer.SetPlatformTextureSettings(platSettings);
                }
            };

            maybeChange(importer.GetPlatformTextureSettings("iPhone"));
            maybeChange(importer.GetPlatformTextureSettings("Android"));

            if (changed)
            {
                importer.SaveAndReimport();
                ++numChanges;
            }
        }

        Debug.Log(String.Format("Update Platform Specific Image Compression: {0} images updated", numChanges));
    }
    [MenuItem("遇先生工具包/一键[清档]")]
    static void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void AddXlua(string def)
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string xlua = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        if (xlua.Equals(string.Empty))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, def);
        }
        else
        {
            if (!xlua.Contains(def))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, xlua + ";" + def);
            }
            else
            {
                Debug.Log("已经包含");
            }
        }
        AssetDatabase.Refresh();
    }
    public static void RemoveXlua(string def)
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string xlua = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        int _index = xlua.IndexOf(def);
        if (_index < 0)
        {
            return;
        }
        if (_index > 0)//如果不在第一个  把前边的分号删掉
        {
            _index -= 1;
        }
        int _length = def.Length;
        if (xlua.Length > _length)//如果长度大于当前长度，才有分号
        {
            _length += 1;
        }
        xlua = xlua.Remove(_index, _length);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, xlua);
        AssetDatabase.Refresh();
    }
    public static string ReadJsonFromStreamingAssetsPath(string jsonName)
    {
        string url = Application.streamingAssetsPath + "/" + "BuildJson" + "/" + jsonName;
        url = url.Replace("StreamingAssets", "Resources");
        Encoding endoning = Encoding.UTF8;//识别Json数据内容中文字段
        StreamReader streamReader = new StreamReader(url, endoning);
        string jsonData = streamReader.ReadToEnd();
        streamReader.Close();
        return jsonData;
    }
    public static void WriteJsonFromStreamingAssetsPath(string jsonName, string jsonString)
    {
        string url = Application.streamingAssetsPath + "/" + "BuildJson" + "/" + jsonName;
        url = url.Replace("StreamingAssets", "Resources");
        StreamWriter streamWrite = new StreamWriter(url);
        streamWrite.WriteLine(jsonString);
        streamWrite.Dispose();
        streamWrite.Close();
    }

    private void OnEnable()
    {
    }
    private void OnGUI()
    {
        //if (EditorWindow.HasOpenInstances<UnityEditor.BuildPlayerWindow>())
        //{
        //    Debug.Log("BuildPlayerWindow Open");
        //}
    }
}
class BaseJsonData
{
    public BuildInfoData BuildInfo;
}
public class BuildInfoData
{
    public string BuildTarget;
    public string DesState;
    public string BaseUrl;
    public string GameName;
    public string PackageName;
    public int BuildCode;
    public string Applovin_SDK_KEY;
    public string Applovin_REWARD_ID;
    public string Applovin_INTER_ID;
    public string Adjust_APP_ID;
    public string GameCode;
    public string Rate_Url;
    public string Build_path;
}
