using Rain.Core;
using Rain.Launcher;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameMgr : MonoBehaviour
{
    public static GameMgr Ins;
    public bool enterGame = false;

    private void Awake()
    {
        Ins = this;
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        ConfigMgr.Ins.Init();
        ConfigMgr.Ins.LoadAllConfig();
        DataMgr.Ins.Load();
        LangMgr.Ins.Init();
        if (Application.isEditor)
        {
            Application.runInBackground = true;
        }
        if (Application.isEditor)
        {
            UIMgr.Ins.OpenView<DebugView>();
        }
        else
        {
            StartGame();
        }



        //新模块
        ModuleCenter.Initialize(this);
        RA.Timer = ModuleCenter.CreateModule<TimerMgr>();
        RA.Message = ModuleCenter.CreateModule<MessageManager>();
        RA.Audio = ModuleCenter.CreateModule<AudioManager>();
        RA.Asset = ModuleCenter.CreateModule<AssetManager>();

        //AudioClip clip = RA.Asset.Load<AudioClip>("audio/Electronic high shot");
        //AudioManager.Ins.PlayAudioEffect(clip);
    }

    private void Update()
    {
        ModuleCenter.Update();

        if (Input.GetMouseButtonDown(0))
        {
            GameObject testprefab = RA.Asset.Load<GameObject>("testprefab");
            if (testprefab)
            {
                GameObject.Instantiate(testprefab);
            }

            AudioManager.Ins.PlayAudioEffect("Electronic high shot");


        }
    }

    public void StartGame()
    {
        SceneMgr.Ins.ChangeScene(SceneID.FPSDemo, () => { EnterGame(); });
    }

    /// <summary>
    /// 完成loading进入游戏
    /// </summary>
    public void EnterGame()
    {
        Debug.Log("-----------------------EnterGame-----------------------");
        enterGame = true;
        CheckFirstEnterGame();
        InitEnterGameTimer();
    }

    /// <summary>
    /// 数据初始化之后 首次进入游戏
    /// </summary>
    private void CheckFirstEnterGame()
    {
    }

    private void InitEnterGameTimer()
    {
        //每十秒保存数据 游戏时长
        //Timer.Ins.SetInterval(() =>
        //{
        //    DataMgr.Ins.gameData.lastOffLine = DateTime.Now;
        //    DataMgr.Ins.SaveGameData();
        //}, 10);
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause && !Application.isEditor)
        {
            DataMgr.Ins.SaveGameData();
        }
    }
}
