using System;
using System.Collections.Generic;
using Rain.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Ease = DG.Tweening.Ease;

namespace Rain.UI
{
    //生命周期优先级 Awake->OnEnable->Init->OnOpen->Start
    public class BaseView : BaseUI, IBaseView
    {
        [HideInInspector]
        public string _viewName;
        private Canvas _canvas;
        public Canvas canvas { get { if (_canvas == null) _canvas = GetComponent<Canvas>(); return _canvas; } }
        [HideInInspector]
        public CanvasGroup canvasGroup;
        public ViewConfigModel viewConfig;
        private Image __imgBg;
        private RectTransform __content;

        public virtual void Init(params object[] _params)
        {
            _viewName = GetType().ToString();
            Utility.Log(_viewName + ".Init()", gameObject);
            //canvas
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            //CanvasScaler
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            //canvasScaler.referenceResolution = new Vector2(1920, 1080);
            //bg
            __imgBg = transform.GetChild(0)?.GetComponent<Image>();
            Color bgColor = Utility.ColorHexToRGB(viewConfig.bgColor);
            __imgBg.gameObject.SetActive(viewConfig.hasBg);
            __imgBg.raycastTarget = viewConfig.hasBg;
            __imgBg.color = bgColor;
            __content = transform.GetChild(1).GetComponent<RectTransform>();
            if (viewConfig.hasBg && viewConfig.bgClose)
            {
                Button bt = __imgBg.GetComponent<Button>();
                Tools.Ins.SetButton(bt, Close);
            }
            _LoadUI();
        }

        internal CanvasGroup CanvasGroup
        {
            get
            {
                if (canvasGroup == null)
                {
                    canvasGroup = GetComponent<CanvasGroup>();
                }
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                return canvasGroup;
            }
        }

        public virtual void OnOpen(params object[] _params)
        {
            //Utility.Log(_viewName + ".OnOpen()", gameObject);
            if (viewConfig.showMethod == ViewShowMethod.pop)
            {
                CanvasGroup.alpha = 0;
                CanvasGroup.DOFade(1, 0.3f);
                __content.localScale = Vector3.zero;
                __content.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            }
            //AudioMgr.Ins.PlaySound(AudioSound.Sound_PopShow);
        }

        public virtual void OnClose(Action _cb)
        {
            //Utility.Log(_viewName + ".OnClose()", gameObject);
            //UI
            if (viewConfig.showMethod == ViewShowMethod.pop)
            {
                CanvasGroup.DOFade(0, 0.3f);
                __content.transform.DOScale(0, 0.5f).SetEase(Ease.OutBack).onComplete = () =>
                {
                    _cb?.Invoke();
                };
            }
            else
            {
                _cb?.Invoke();
            }
            //timer
            //Timer.Ins.RemoveTimerGroup(GetTimerGroupName());
            //unbind
            UnBindAllDataBind();
        }

        public void Close()
        {
            UIMgr.Ins.CloseView(_viewName);
        }


        internal virtual void _LoadUI()
        {

        }

        /// <summary>
        /// 计时器
        /// </summary>
        //protected TimerHandler SetTimeOut(Action<TimerDispatcher> _action, float _totalTime)
        //{
        //    return Timer.Ins.SetTimeOut(_action, _totalTime, GetTimerGroupName());
        //}

        ///// <summary>
        ///// 定时器
        ///// </summary>
        //public TimerHandler SetInterval(Action<TimerDispatcher> _action, float _interval, float _totalTime = int.MaxValue)
        //{
        //    return Timer.Ins.SetInterval(_action, _interval, _totalTime, GetTimerGroupName());
        //}

        ///// <summary>
        ///// 倒计时
        ///// </summary>
        //public TimerHandler SetCountDown(Action<TimerDispatcher> _action, float _totalTime, float _startTime = 0)
        //{
        //    return Timer.Ins.SetCountDown(_action, _totalTime, _startTime, GetTimerGroupName());
        //}

        /// <summary>
        /// 获取计时器组名
        /// </summary>
        /// <returns></returns>
        private string GetTimerGroupName()
        {
            //string groupName = Timer.Ins.GetGroupName(_viewName);
            //return groupName;
            return "";
        }

        List<DBHandler.Binding> dbHandlers = new List<DBHandler.Binding>();
        /// <summary>
        /// 数据绑定 使用这个方法绑定的事件在关闭页面时自动解绑
        /// </summary>
        /// <param name="binding"></param>
        protected void DataBind(DBObject dBObject, Action<DBModify> callfunc)
        {
            DBHandler.Binding handler = dBObject.Bind(callfunc);
            dbHandlers.Add(handler);
        }

        /// <summary>
        /// 解除所有绑定
        /// </summary>
        protected void UnBindAllDataBind()
        {
            foreach (var item in dbHandlers)
            {
                item.UnBind();
            }
        }
    }
}