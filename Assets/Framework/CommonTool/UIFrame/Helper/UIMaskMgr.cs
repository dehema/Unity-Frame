/*
        主题： UI遮罩管理器  

        “弹出窗体”往往因为需要玩家优先处理弹出小窗体，则要求玩家不能(无法)点击“父窗体”，这种窗体就是典型的“模态窗体”
  5  *    Description: 
  6  *           功能： 负责“弹出窗体”模态显示实现
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIMaskMgr : MonoBehaviour
{
    private static UIMaskMgr _Instance = null;
    //ui根节点对象
    private GameObject _GoCanvasRoot = null;
    //ui脚本节点对象
    private Transform _TraUIScriptsNode = null;
    //顶层面板
    private GameObject _GoToPanel;
    //遮罩面板
    private GameObject _GoMaskPanel;
    //ui摄像机
    private Camera _UICamera;
    //ui摄像机原始的层深
    private float _OriginalUICameraDepth;
    //获取实例
    public static UIMaskMgr GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new GameObject("_UIMaskMgr").AddComponent<UIMaskMgr>();
        }
        return _Instance;
    }
    private void Awake()
    {
        _GoCanvasRoot = GameObject.FindGameObjectWithTag(SysDefine.SYS_TAG_CANVAS);
        _TraUIScriptsNode = UnityHelper.FindTheChildNode(_GoCanvasRoot, SysDefine.SYS_SCRIPTMANAGER_NODE);
        //把脚本实例，座位脚本节点对象的子节点
        UnityHelper.AddChildNodeToParentNode(_TraUIScriptsNode, this.gameObject.transform);
        //获取顶层面板，遮罩面板
        _GoToPanel = _GoCanvasRoot;
        _GoMaskPanel = UnityHelper.FindTheChildNode(_GoCanvasRoot, "_UIMaskPanel").gameObject;
        //得到uicamera摄像机原始的层深
        _UICamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
        if (_UICamera != null)
        {
            //得到ui相机原始的层深
            _OriginalUICameraDepth = _UICamera.depth;
        }
        else
        {
            Debug.Log("UI_Camera is Null!,Please Check!");
        }
    }

    /// <summary>
    /// 设置遮罩状态
    /// </summary>
    /// <param name="goDisplayUIForms">需要显示的ui窗体</param>
    /// <param name="lucenyType">显示透明度属性</param>
    public void SetMaskWindow(GameObject goDisplayUIForms,UIFormLucenyType lucenyType = UIFormLucenyType.Lucency)
    {
        //顶层窗体下移
        _GoToPanel.transform.SetAsLastSibling();
        switch (lucenyType)
        {
               //完全透明 不能穿透
            case UIFormLucenyType.Lucency:
                _GoMaskPanel.SetActive(true);
                Color newColor = new Color(255 / 255F, 255 / 255F, 255 / 255F, 0F / 255F);
                _GoMaskPanel.GetComponent<Image>().color = newColor;
                break;
                //半透明，不能穿透
            case UIFormLucenyType.Translucence:
                _GoMaskPanel.SetActive(true);
                Color newColor2 = new Color(0 / 255F, 0 / 255F, 0 / 255F, 220 / 255F);
                _GoMaskPanel.GetComponent<Image>().color = newColor2;
                MessageCenterLogic.GetInstance().Send(CConfig.mg_WindowOpen);
                break;
                //低透明，不能穿透
            case UIFormLucenyType.ImPenetrable:
                _GoMaskPanel.SetActive(true);
                Color newColor3 = new Color(50 / 255F, 50 / 255F, 50 / 255F, 240F / 255F);
                _GoMaskPanel.GetComponent<Image>().color = newColor3;
                break;
                //可以穿透
            case UIFormLucenyType.Penetrable:
                if (_GoMaskPanel.activeInHierarchy)
                {
                    _GoMaskPanel.SetActive(false);
                }
                break;
            default:
                break;
        }
        //遮罩窗体下移
        _GoMaskPanel.transform.SetAsLastSibling();
        //显示的窗体下移
        goDisplayUIForms.transform.SetAsLastSibling();
        //增加当前ui摄像机的层深（保证当前摄像机为最前显示）
        if (_UICamera != null)
        {
            _UICamera.depth = _UICamera.depth + 100;
        }
    }
    public void HideMaskWindow()
    {
        Color newColor3 = new Color(_GoMaskPanel.GetComponent<Image>().color.r, _GoMaskPanel.GetComponent<Image>().color.g, _GoMaskPanel.GetComponent<Image>().color.b,0);
        _GoMaskPanel.GetComponent<Image>().color = newColor3;
    }
    /// <summary>
    /// 取消遮罩状态
    /// </summary>
    public void CancelMaskWindow()
    {
        if (UIManager.GetInstance().WaitUIForms.Count > 0)
        {
            return;
        }
        //顶层窗体上移
        _GoToPanel.transform.SetAsFirstSibling();
        //禁用遮罩窗体
        if (_GoMaskPanel.activeInHierarchy)
        {
            _GoMaskPanel.SetActive(false);
            MessageCenterLogic.GetInstance().Send(CConfig.mg_WindowClose);
        }
        //恢复当前ui摄像机的层深
        if (_UICamera != null)
        {
            _UICamera.depth = _OriginalUICameraDepth;
        }
    }
}
