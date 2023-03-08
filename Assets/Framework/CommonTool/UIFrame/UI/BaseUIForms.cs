using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 基础UI窗体脚本（父类，其他窗体都继承此脚本）
/// </summary>
public class BaseUIForms : BaseUI
{
    //当前（基类）窗口的类型
    public UIType _CurrentUIType = new UIType();
    [HideInInspector]
    public Button close_button;
    //属性，当前ui窗体类型
    internal UIType CurrentUIType
    {
        set
        {
            _CurrentUIType = value;
        }
        get
        {
            return _CurrentUIType;
        }
    }
    protected virtual void Awake()
    {
        FindChildAddComponent(gameObject);
        if (transform.Find("Window/Content/CloseBtn"))
        {
            close_button = transform.Find("Window/Content/CloseBtn").GetComponent<Button>();
            close_button.onClick.AddListener(() =>
            {
                UIManager.GetInstance().CloseOrReturnUIForms(this.GetType().Name);
            });
        }
        if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
        {
            gameObject.AddComponent<CanvasGroup>();
        }
        gameObject.name = GetType().Name;
    }


    public static void FindChildAddComponent(GameObject goParent)
    {
        Transform parent = goParent.transform;
        int childCount = parent.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform chile = parent.GetChild(i);
            if (chile.GetComponent<Button>())
            {
                chile.GetComponent<Button>().onClick.AddListener(() =>
                {
                    AudioMgr.Ins.PlaySound(AudioSound.Sound_UIButton);
                });
            }

            if (chile.childCount > 0)
            {
                FindChildAddComponent(chile.gameObject);
            }
        }
    }

    //页面显示
    public virtual void Display()
    {
        //Debug.Log(this.GetType().Name);
        this.gameObject.SetActive(true);
        // 设置模态窗体调用(必须是弹出窗体)
        if (_CurrentUIType.UIForms_Type == UIFormType.PopUp && _CurrentUIType.UIForm_LucencyType != UIFormLucenyType.NoMask)
        {
            UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject, _CurrentUIType.UIForm_LucencyType);
        }
        if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
        {

            //动画添加
            switch (_CurrentUIType.UIForm_animationType)
            {
                case UIFormShowAnimationType.scale:
                    AnimationController.PopShow(gameObject, () =>
                    {

                    });
                    break;

            }

        }
        //NewUserManager.GetInstance().TriggerEvent(TriggerType.panel_display);
    }
    //页面隐藏（不在栈集合中）
    public virtual void Hidding()
    {
        if (_CurrentUIType.UIForms_Type == UIFormType.PopUp && _CurrentUIType.UIForm_LucencyType != UIFormLucenyType.NoMask)
        {
            UIMaskMgr.GetInstance().HideMaskWindow();
        }

        //取消模态窗体调用

        if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
        {
            switch (_CurrentUIType.UIForm_animationType)
            {
                case UIFormShowAnimationType.scale:
                    AnimationController.PopHide(gameObject, () =>
                    {
                        this.gameObject.SetActive(false);
                        if (_CurrentUIType.UIForms_Type == UIFormType.PopUp && _CurrentUIType.UIForm_LucencyType != UIFormLucenyType.NoMask)
                        {
                            UIMaskMgr.GetInstance().CancelMaskWindow();
                        }
                        UIManager.GetInstance().ShowNextPopUp();
                    });
                    break;
                case UIFormShowAnimationType.none:
                    this.gameObject.SetActive(false);
                    if (_CurrentUIType.UIForms_Type == UIFormType.PopUp && _CurrentUIType.UIForm_LucencyType != UIFormLucenyType.NoMask)
                    {
                        UIMaskMgr.GetInstance().CancelMaskWindow();
                    }
                    UIManager.GetInstance().ShowNextPopUp();
                    break;

            }

        }
        else
        {
            this.gameObject.SetActive(false);
            if (_CurrentUIType.UIForms_Type == UIFormType.PopUp && _CurrentUIType.UIForm_LucencyType != UIFormLucenyType.NoMask)
            {
                UIMaskMgr.GetInstance().CancelMaskWindow();
            }
        }
    }
    //页面重新显示
    public virtual void Redisplay()
    {
        this.gameObject.SetActive(true);
        if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
        {
            UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject, _CurrentUIType.UIForm_LucencyType);
        }
    }
    //页面冻结（还在栈集合中）
    public virtual void Freeze()
    {
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// 注册按钮事件
    /// </summary>
    /// <param name="buttonName">按钮节点名称</param>
    /// <param name="delHandle">委托，需要注册的方法</param>
    protected void RigisterButtonObjectEvent(string buttonName, EventTriggerListener.VoidDelegate delHandle)
    {
        GameObject goButton = UnityHelper.FindTheChildNode(this.gameObject, buttonName).gameObject;
        //给按钮注册事件方法
        if (goButton != null)
        {
            EventTriggerListener.Get(goButton).onClick = delHandle;
        }
    }

    /// <summary>
    /// 打开ui窗体
    /// </summary>
    /// <param name="uiFormName"></param>
    protected void OpenUIForm(string uiFormName)
    {
        UIManager.GetInstance().ShowUIForms(uiFormName);
    }

    /// <summary>
    /// 关闭当前ui窗体
    /// </summary>
    protected void CloseUIForm(string uiFormName)
    {
        //处理后的uiform名称
        UIManager.GetInstance().CloseOrReturnUIForms(uiFormName);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msgType">消息的类型</param>
    /// <param name="msgName">消息名称</param>
    /// <param name="msgContent">消息内容</param>
    protected void SendMessage(string msgType, string msgName, object msgContent)
    {
        KeyValuesUpdate kvs = new KeyValuesUpdate(msgName, msgContent);
        MessageCenter.SendMessage(msgType, kvs);
    }

    /// <summary>
    /// 接受消息
    /// </summary>
    /// <param name="messageType">消息分类</param>
    /// <param name="handler">消息委托</param>
    public void ReceiveMessage(string messageType, MessageCenter.DelMessageDelivery handler)
    {
        MessageCenter.AddMsgListener(messageType, handler);
    }

    /// <summary>
    /// 显示语言
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string Show(string id)
    {
        string strResult = string.Empty;
        strResult = LauguageMgr.GetInstance().ShowText(id);
        return strResult;
    }
}
