/*
*
*   功能：整个UI框架的核心，用户程序通过调用本类，来调用本框架的大多数功能。  
*           功能1：关于入“栈”与出“栈”的UI窗体4个状态的定义逻辑
*                 入栈状态：
*                     Freeze();   （上一个UI窗体）冻结
*                     Display();  （本UI窗体）显示
*                 出栈状态： 
*                     Hiding();    (本UI窗体) 隐藏
*                     Redisplay(); (上一个UI窗体) 重新显示
*          功能2：增加“非栈”缓存集合。 
* 
* 
* ***/

using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UI窗体管理器脚本（框架核心脚本）
/// 主要负责UI窗体的加载、缓存、以及对于“UI窗体基类”的各种生命周期的操作（显示、隐藏、重新显示、冻结）。
/// </summary>
public class UIManager : MonoBehaviour
{
    [HideInInspector]
    public Canvas MainCanvas;
    public GameObject RobloxCanvas;
    public Transform Pool = null;
    private static UIManager _Instance = null;
    //ui窗体预设路径（参数1，窗体预设名称，2，表示窗体预设路径）
    private Dictionary<string, string> _DicFormsPaths;
    //缓存所有的ui窗体
    private Dictionary<string, BaseUIForms> _DicALLUIForms;
    //栈结构标识当前ui窗体的集合
    private Stack<BaseUIForms> _StaCurrentUIForms;
    //当前显示的ui窗体
    private Dictionary<string, BaseUIForms> _DicCurrentShowUIForms;
    //临时关闭窗口
    private List<string> _WaitUIForms;
    //ui根节点
    private Transform _TraCanvasTransfrom = null;
    //全屏幕显示的节点
    private Transform _TraNormal = null;
    //固定显示的节点
    private Transform _TraFixed = null;
    //弹出节点
    private Transform _TraPopUp = null;
    //ui特效节点
    private Transform _Top = null;
    //ui管理脚本的节点
    private Transform _TraUIScripts = null;
    [HideInInspector]
    public string PanelName;
    List<string> PanelNameList;
    public Vector2 referenceResolution;
    public List<string> WaitUIForms
    {
        get
        {
            return _WaitUIForms;
        }
    }
    public static UIManager Ins
    {
        get { return _Instance; }
    }
    //得到实例
    public static UIManager GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_UIManager").AddComponent<UIManager>();

        }
        return _Instance;
    }
    //初始化核心数据，加载ui窗体路径到集合中
    public void Awake()
    {
        PanelNameList = new List<string>();
        //字段初始化
        _DicALLUIForms = new Dictionary<string, BaseUIForms>();
        _DicCurrentShowUIForms = new Dictionary<string, BaseUIForms>();
        _WaitUIForms = new List<string>();
        _DicFormsPaths = new Dictionary<string, string>();
        _StaCurrentUIForms = new Stack<BaseUIForms>();
        //初始化加载（根ui窗体）canvas预设
        InitRootCanvasLoading();
        //得到UI根节点，全屏节点，固定节点，弹出节点
        //Debug.Log("this.gameobject" + this.gameObject.name);
        _TraCanvasTransfrom = GameObject.FindGameObjectWithTag(SysDefine.SYS_TAG_CANVAS).transform;
        Pool = UnityHelper.FindTheChildNode(_TraCanvasTransfrom.gameObject, SysDefine.SYS_Pool_NODE);
        _TraNormal = UnityHelper.FindTheChildNode(_TraCanvasTransfrom.gameObject, SysDefine.SYS_NORMAL_NODE);
        _TraFixed = UnityHelper.FindTheChildNode(_TraCanvasTransfrom.gameObject, SysDefine.SYS_FIXED_NODE);
        _TraPopUp = UnityHelper.FindTheChildNode(_TraCanvasTransfrom.gameObject, SysDefine.SYS_POPUP_NODE);
        _Top = UnityHelper.FindTheChildNode(_TraCanvasTransfrom.gameObject, SysDefine.SYS_TOP_NODE);
        _TraUIScripts = UnityHelper.FindTheChildNode(_TraCanvasTransfrom.gameObject, SysDefine.SYS_SCRIPTMANAGER_NODE);
        //把本脚本作为“根ui窗体”的子节点
        UnityHelper.AddChildNodeToParentNode(_TraUIScripts, this.gameObject.transform);
        //根UI窗体在场景转换的时候，不允许销毁
        DontDestroyOnLoad(_TraCanvasTransfrom);
        //初始化ui窗体预设路径数据
        InitUIFormsPathsData();
    }
    public void AddPanel(string name)
    {
        if (!PanelNameList.Contains(name))
        {
            PanelNameList.Add(name);
            PanelName = name;
        }
    }
    public void SubPanel(string name)
    {
        if (PanelNameList.Contains(name))
        {
            PanelNameList.Remove(name);
        }
        if (PanelNameList.Count == 0)
        {
            PanelName = "";
        }
        else
        {
            PanelName = PanelNameList[0];
        }
    }
    //初始化加载（根ui窗体）canvas预设
    private void InitRootCanvasLoading()
    {
        MainCanvas = ResourcesMgr.GetInstance().LoadAsset(SysDefine.SYS_PATH_CANVAS, false).GetComponent<Canvas>();
        referenceResolution = MainCanvas.GetComponent<CanvasScaler>().referenceResolution;
        //RobloxCanvas = ResourcesMgr.GetInstance().LoadAsset("RobloxCanvas", false);
        //DontDestroyOnLoad(RobloxCanvas);
        //RobloxCanvas.GetComponent<Canvas>().worldCamera = MainCanvas.worldCamera;
    }
    /// <summary>
    /// 显示ui窗体
    /// 功能：1根据ui窗体的名称，加载到所有ui窗体缓存集合中
    /// 2,根据不同的ui窗体的显示模式，分别做不同的加载处理
    /// </summary>
    /// <param name="uiFormName">ui窗体预设的名称</param>
    public BaseUIForms ShowUIForms(string uiFormName)
    {
        AddPanel(uiFormName);
        //ui窗体的基类
        BaseUIForms baseUIForms = null;
        //参数的检查
        if (string.IsNullOrEmpty(uiFormName)) return null;
        //根据ui窗体的名称，把加载到所有ui窗体缓存集合中
        baseUIForms = LoadFormsToALLUIFormsCatch(uiFormName);
        if (baseUIForms == null) return null;
        //判断是否清空“栈”结构体集合
        if (baseUIForms.CurrentUIType.IsClearReverseChange)
        {
            ClearStackArray();
        }
        //音频 
        if (baseUIForms.CurrentUIType.UIForms_Type == UIFormType.PopUp)
        {
            AudioMgr.Ins.PlaySound(AudioSound.Sound_PopShow);
        }
        //根据不同的ui窗体的显示模式，分别做不同的加载处理
        switch (baseUIForms.CurrentUIType.UIForm_ShowMode)
        {
            case UIFormShowMode.Normal:
                EnterUIFormsCache(uiFormName);
                break;
            case UIFormShowMode.ReverseChange:
                PushUIForms(uiFormName);
                break;
            case UIFormShowMode.HideOther:
                EnterUIFormstToCacheHideOther(uiFormName);
                break;
            case UIFormShowMode.Wait:
                EnterUIFormsCacheWaitClose(uiFormName);
                break;
            default:
                break;
        }
        return baseUIForms;
    }

    public object OpenView(string uiName)
    {
        ShowUIForms(uiName);
        return GetUI(uiName);
    }

    public T OpenView<T>() where T : BaseUIForms
    {
        string uiName = typeof(T).ToString();
        ShowUIForms(uiName);
        return GetUI<T>();
    }

    public void CloseView<T>()
    {
        string uiName = typeof(T).ToString();
        CloseOrReturnUIForms(uiName);
    }

    public void CloseView(BaseUI baseUI)
    {
        string uiName = baseUI.GetType().ToString();
        CloseOrReturnUIForms(uiName);
    }

    /// <summary>
    /// 关闭或返回上一个ui窗体（关闭当前ui窗体）
    /// </summary>
    /// <param name="strUIFormsName"></param>
    public void CloseOrReturnUIForms(string strUIFormsName)
    {
        SubPanel(strUIFormsName);
        //Debug.Log("关闭窗体的名字是" + strUIFormsName);
        //ui窗体的基类
        BaseUIForms baseUIForms = null;
        if (string.IsNullOrEmpty(strUIFormsName)) return;
        _DicALLUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        //所有窗体缓存中没有记录，则直接返回
        if (baseUIForms == null) return;
        //判断不同的窗体显示模式，分别处理
        switch (baseUIForms.CurrentUIType.UIForm_ShowMode)
        {
            case UIFormShowMode.Normal:
                ExitUIFormsCache(strUIFormsName);
                break;

            case UIFormShowMode.ReverseChange:
                PopUIForms();
                break;
            case UIFormShowMode.HideOther:
                ExitUIFormsFromCacheAndShowOther(strUIFormsName);
                break;
            case UIFormShowMode.Wait:
                ExitUIFormsCache(strUIFormsName);
                break;
            default:
                break;
        }

    }
    /// <summary>
    /// 显示下一个弹窗如果有的话
    /// </summary>
    public void ShowNextPopUp()
    {
        if (_WaitUIForms.Count > 0)
        {
            ShowUIForms(_WaitUIForms[0]);
            _WaitUIForms.RemoveAt(0);
        }
    }

    /// <summary>
    /// 清空当前等待中的UI
    /// </summary>
    public void ClearWaitUIForms()
    {
        if (_WaitUIForms.Count > 0)
        {
            _WaitUIForms = new List<string>();
        }
    }
    /// <summary>
    /// 根据UI窗体的名称，加载到“所有UI窗体”缓存集合中
    /// 功能： 检查“所有UI窗体”集合中，是否已经加载过，否则才加载。
    /// </summary>
    /// <param name="uiFormsName">UI窗体（预设）的名称</param>
    /// <returns></returns>
    private BaseUIForms LoadFormsToALLUIFormsCatch(string uiFormName)
    {
        //加载的返回ui窗体基类
        BaseUIForms baseUIResult = null;
        _DicALLUIForms.TryGetValue(uiFormName, out baseUIResult);
        if (baseUIResult == null)
        {
            //加载指定名称ui窗体
            baseUIResult = LoadUIForm(uiFormName);

        }
        return baseUIResult;
    }
    /// <summary>
    /// 加载指定名称的“UI窗体”
    /// 功能：
    ///    1：根据“UI窗体名称”，加载预设克隆体。
    ///    2：根据不同预设克隆体中带的脚本中不同的“位置信息”，加载到“根窗体”下不同的节点。
    ///    3：隐藏刚创建的UI克隆体。
    ///    4：把克隆体，加入到“所有UI窗体”（缓存）集合中。
    /// 
    /// </summary>
    /// <param name="uiFormName">UI窗体名称</param>
    private BaseUIForms LoadUIForm(string uiFormName)
    {
        string strUIFormPaths = null;
        GameObject goCloneUIPrefabs = null;
        BaseUIForms baseUIForm = null;
        //根据ui窗体名称，得到对应的加载路径
        _DicFormsPaths.TryGetValue(uiFormName, out strUIFormPaths);
        if (!string.IsNullOrEmpty(strUIFormPaths))
        {
            //加载预制体
            goCloneUIPrefabs = ResourcesMgr.GetInstance().LoadAsset(strUIFormPaths, false);
        }
        //设置ui克隆体的父节点（根据克隆体中带的脚本中不同的信息位置信息）
        if (_TraCanvasTransfrom != null && goCloneUIPrefabs != null)
        {
            baseUIForm = goCloneUIPrefabs.GetComponent<BaseUIForms>();
            if (baseUIForm == null)
            {
                Debug.Log("baseUiForm==null! ,请先确认窗体预设对象上是否加载了baseUIForm的子类脚本！ 参数 uiFormName=" + uiFormName);
                return null;
            }
            switch (baseUIForm.CurrentUIType.UIForms_Type)
            {
                case UIFormType.Normal:
                    goCloneUIPrefabs.transform.SetParent(_TraNormal, false);
                    break;
                case UIFormType.Fixed:
                    goCloneUIPrefabs.transform.SetParent(_TraFixed, false);
                    break;
                case UIFormType.PopUp:
                    goCloneUIPrefabs.transform.SetParent(_TraPopUp, false);
                    break;
                case UIFormType.Top:
                    goCloneUIPrefabs.transform.SetParent(_Top, false);
                    break;
                default:
                    break;
            }
            //设置隐藏
            goCloneUIPrefabs.SetActive(false);
            //把克隆体，加入到所有ui窗体缓存集合中
            _DicALLUIForms.Add(uiFormName, baseUIForm);
            return baseUIForm;
        }
        else
        {
            Debug.Log("_TraCanvasTransfrom==null Or goCloneUIPrefabs==null!! ,Plese Check!, 参数uiFormName=" + uiFormName);
        }
        Debug.Log("出现不可以预估的错误，请检查，参数 uiFormName=" + uiFormName);
        return null;
    }
    /// <summary>
    /// 把当前窗体加载到当前窗体集合中
    /// </summary>
    /// <param name="uiFormName">窗体预设的名字</param>
    private void EnterUIFormsCache(string uiFormName)
    {
        //ui窗体基类
        BaseUIForms baseUIForm;
        //从所有窗体集合中得到的窗体
        BaseUIForms baseUIFormFromAllCache;
        //如果正在显示的集合中存在该窗体，直接返回
        _DicCurrentShowUIForms.TryGetValue(uiFormName, out baseUIForm);
        if (baseUIForm != null) return;
        //把当前窗体，加载到正在显示集合中
        _DicALLUIForms.TryGetValue(uiFormName, out baseUIFormFromAllCache);
        if (baseUIFormFromAllCache != null)
        {
            _DicCurrentShowUIForms.Add(uiFormName, baseUIFormFromAllCache);
            //显示当前窗体
            baseUIFormFromAllCache.Display();
        }
    }

    /// <summary>
    /// 卸载ui窗体从当前显示的集合缓存中
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void ExitUIFormsCache(string strUIFormsName)
    {
        //ui窗体基类
        BaseUIForms baseUIForms;
        //正在显示ui窗体缓存集合没有记录，则直接返回
        _DicCurrentShowUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        if (baseUIForms == null) return;
        //指定ui窗体，运行隐藏，且从正在显示ui窗体缓存集合中移除
        baseUIForms.Hidding();
        _DicCurrentShowUIForms.Remove(strUIFormsName);
    }

    /// <summary>
    /// 加载ui窗体到当前心事窗体集合，且，隐藏其他正在显示的页面
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void EnterUIFormstToCacheHideOther(string strUIFormsName)
    {
        //窗体基类
        BaseUIForms baseUIForms;
        //所有窗体集合中的窗体基类
        BaseUIForms baseUIFormsFromAllCache;
        _DicCurrentShowUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        //正在显示ui窗体缓存集合里有记录，直接返回
        if (baseUIForms != null) return;
        Debug.Log("关闭其他窗体");
        //正在显示ui窗体缓存 与栈缓存集合里所有的窗体进行隐藏处理
        List<BaseUIForms> ShowUIFormsList = new List<BaseUIForms>(_DicCurrentShowUIForms.Values);
        foreach (BaseUIForms baseUIFormsItem in ShowUIFormsList)
        {
            Debug.Log("_DicCurrentShowUIForms---------" + baseUIFormsItem.transform.name);
            if (baseUIFormsItem.CurrentUIType.UIForms_Type == UIFormType.PopUp)
            {
                //baseUIFormsItem.Hidding();
                ExitUIFormsFromCacheAndShowOther(baseUIFormsItem.GetType().Name);
            }
        }
        List<BaseUIForms> CurrentUIFormsList = new List<BaseUIForms>(_StaCurrentUIForms);
        foreach (BaseUIForms baseUIFormsItem in CurrentUIFormsList)
        {
            Debug.Log("_StaCurrentUIForms---------" + baseUIFormsItem.transform.name);
            //baseUIFormsItem.Hidding();
            ExitUIFormsFromCacheAndShowOther(baseUIFormsItem.GetType().Name);
        }
        //把当前窗体，加载到正在心事ui窗体缓存集合中 
        _DicALLUIForms.TryGetValue(strUIFormsName, out baseUIFormsFromAllCache);
        if (baseUIFormsFromAllCache != null)
        {
            _DicCurrentShowUIForms.Add(strUIFormsName, baseUIFormsFromAllCache);
            baseUIFormsFromAllCache.Display();
        }
    }
    /// <summary>
    /// 把当前窗体加载到当前窗体集合中
    /// </summary>
    /// <param name="uiFormName">窗体预设的名字</param>
    private void EnterUIFormsCacheWaitClose(string uiFormName)
    {
        //ui窗体基类
        BaseUIForms baseUIForm;
        //从所有窗体集合中得到的窗体
        BaseUIForms baseUIFormFromAllCache;
        //如果正在显示的集合中存在该窗体，直接返回
        _DicCurrentShowUIForms.TryGetValue(uiFormName, out baseUIForm);
        if (baseUIForm != null) return;
        bool havePopUp = false;
        foreach (BaseUIForms uiforms in _DicCurrentShowUIForms.Values)
        {
            if (uiforms.CurrentUIType.UIForms_Type == UIFormType.PopUp)
            {
                havePopUp = true;
                break;
            }
        }
        if (!havePopUp)
        {
            //把当前窗体，加载到正在显示集合中
            _DicALLUIForms.TryGetValue(uiFormName, out baseUIFormFromAllCache);
            if (baseUIFormFromAllCache != null)
            {
                _DicCurrentShowUIForms.Add(uiFormName, baseUIFormFromAllCache);
                //显示当前窗体
                baseUIFormFromAllCache.Display();

            }
        }
        else
        {
            _WaitUIForms.Add(uiFormName);
        }

    }
    /// <summary>
    /// 卸载ui窗体从当前显示窗体集合缓存中，且显示其他原本需要显示的页面
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void ExitUIFormsFromCacheAndShowOther(string strUIFormsName)
    {
        //ui窗体基类
        BaseUIForms baseUIForms;
        _DicCurrentShowUIForms.TryGetValue(strUIFormsName, out baseUIForms);
        if (baseUIForms == null) return;
        //指定ui窗体 ，运行隐藏状态，且从正在显示ui窗体缓存集合中删除
        baseUIForms.Hidding();
        _DicCurrentShowUIForms.Remove(strUIFormsName);
        //正在显示ui窗体缓存与栈缓存集合里素有窗体进行再次显示
        //foreach (BaseUIForms baseUIFormsItem in _DicCurrentShowUIForms.Values)
        //{
        //    baseUIFormsItem.Redisplay();
        //}
        //foreach (BaseUIForms baseUIFormsItem in _StaCurrentUIForms)
        //{
        //    baseUIFormsItem.Redisplay();
        //}
    }
    /// <summary>
    /// ui窗体入栈
    /// 功能：1判断栈里是否已经有窗体，有则冻结
    ///   2先判断ui预设缓存集合是否有指定的ui窗体，有则处理
    ///   3指定ui窗体入栈
    /// </summary>
    /// <param name="strUIFormsName"></param>
    private void PushUIForms(string strUIFormsName)
    {
        //ui预设窗体
        BaseUIForms baseUI;
        //判断栈里是否已经有窗体,有则冻结
        if (_StaCurrentUIForms.Count > 0)
        {
            BaseUIForms topUIForms = _StaCurrentUIForms.Peek();
            topUIForms.Freeze();
            //Debug.Log("topUIForms是" + topUIForms.name);
        }
        //先判断ui预设缓存集合是否有指定ui窗体，有则处理
        _DicALLUIForms.TryGetValue(strUIFormsName, out baseUI);
        if (baseUI != null)
        {
            baseUI.Display();
        }
        else
        {
            Debug.Log(string.Format("/PushUIForms()/ baseUI==null! 核心错误，请检查 strUIFormsName={0} ", strUIFormsName));
        }
        //指定ui窗体入栈
        _StaCurrentUIForms.Push(baseUI);

    }

    /// <summary>
    /// ui窗体出栈逻辑
    /// </summary>
    private void PopUIForms()
    {
        if (_StaCurrentUIForms.Count >= 2)
        {
            //出栈逻辑
            BaseUIForms topUIForms = _StaCurrentUIForms.Pop();
            //出栈的窗体进行隐藏
            topUIForms.Hidding();
            //出栈窗体的下一个窗体逻辑
            BaseUIForms nextUIForms = _StaCurrentUIForms.Peek();
            //下一个窗体重新显示处理
            nextUIForms.Redisplay();
            //Debug.Log("隐藏的页面是" + topUIForms.name);
        }
        else if (_StaCurrentUIForms.Count == 1)
        {
            BaseUIForms topUIForms = _StaCurrentUIForms.Pop();
            //出栈的窗体进行隐藏处理
            topUIForms.Hidding();
        }
    }


    /// <summary>
    /// 初始化ui窗体预设路径数据
    /// </summary>
    private void InitUIFormsPathsData()
    {
        IConfigManager configMgr = new ConfigManagerByJson(SysDefine.SYS_PATH_UIFORMS_CONFIG_INFO);
        if (_DicFormsPaths != null)
        {
            _DicFormsPaths = configMgr.AppSetting;
        }
    }

    /// <summary>
    /// 清空栈结构体集合
    /// </summary>
    /// <returns></returns>
    private bool ClearStackArray()
    {
        if (_StaCurrentUIForms != null && _StaCurrentUIForms.Count >= 1)
        {
            _StaCurrentUIForms.Clear();
            return true;
        }
        return false;
    }
    /// <summary>
    /// 获取当前弹框ui的栈
    /// </summary>
    /// <returns></returns>
    public Stack<BaseUIForms> GetCurrentFormStack()
    {
        return _StaCurrentUIForms;
    }

    public object GetUI(string _uiName)
    {
        return _DicALLUIForms[_uiName];
    }

    public T GetUI<T>() where T : BaseUIForms
    {
        string uiName = typeof(T).ToString();
        if (_DicALLUIForms != null && _DicALLUIForms.ContainsKey(uiName))
        {
            return _DicALLUIForms[uiName] as T;
        }
        return null;
    }

    /// <summary>
    /// 获取全部UI
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, BaseUIForms> GetALLUI()
    {
        return _DicALLUIForms;
    }

    /// <summary>
    /// 获取全部UI配置
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetAllUICofnig()
    {
        return _DicFormsPaths;
    }
}
