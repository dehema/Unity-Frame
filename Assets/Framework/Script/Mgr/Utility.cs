using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using System.Net.Sockets;
using System.Net;

public static class Utility
{
    /// <summary>
    /// log对象
    /// </summary>
    /// <param name="_obj"></param>
    public static void Dump(object _obj, string _prefix = "")
    {
        if (_obj == null)
        {
            return;
        }
        if (_obj.GetType() == typeof(string))
        {
            Debug.Log(_obj);
        }
        else if (_obj.GetType() == typeof(Dictionary<object, object>))
        {
            foreach (var item in _obj as Dictionary<object, object>)
            {
                Debug.Log($"{item.Key}:{item.Value}");
            }
        }
        else if (_obj.GetType() == typeof(Vector2))
        {
            Vector2 pos = (Vector2)_obj;
            Debug.Log(_prefix + $"Vector2({pos.x},{pos.y})");
        }
        else if (_obj.GetType() == typeof(Vector3))
        {
            Vector3 pos = (Vector3)_obj;
            Debug.Log(_prefix + $"Vector3({pos.x},{pos.y},{pos.z})");
        }
        else
        {
            Debug.Log("UnityLog.Dump//--" + _prefix + "\n" + JsonConvert.SerializeObject(_obj, Formatting.Indented));
        }
    }

    public static void Log(string _log, GameObject _go = null)
    {
        Debug.Log("UnityLog//------------" + _log, _go);
    }

    /// <summary>
    /// 十六进制转颜色
    /// </summary>
    /// <param name="_hex"></param>
    /// <returns></returns>
    public static Color ColorHexToRGB(string _hex)
    {
        if (string.IsNullOrEmpty(_hex))
        {
            return Color.clear;
        }
        if (!_hex.Contains("#"))
        {
            _hex = '#' + _hex;
        }
        Color color;
        bool res = ColorUtility.TryParseHtmlString(_hex, out color);
        if (!res)
        {
            return Color.clear;
        }
        return color;
    }

    /// <summary>
    /// 颜色转十六进制
    /// </summary>
    /// <param name="_color"></param>
    /// <returns></returns>
    public static string ColorRGBToHex(Color _color)
    {
        return ColorUtility.ToHtmlStringRGBA(_color);
    }

    public static void SetButton(this Button _button, Action _action, AudioSound _music = AudioSound.Sound_UIButton)
    {
        Tools.Ins.SetButton(_button, _action, _music);
    }

    public static void SetDebugButton(this Button _button, Action _action, AudioSound _music = AudioSound.Sound_UIButton)
    {
        Tools.Ins.SetDebugButton(_button, _action, _music);
    }

    public static void SetToggle(this Toggle _toggle, Action<bool> _action = null, AudioSound _music = AudioSound.Sound_UIButton)
    {
        Tools.Ins.SetToggle(_toggle, _action, _music);
    }

    public static void SetDebugToggle(this Toggle _toggle, Action<bool> _action = null, AudioSound _music = AudioSound.Sound_UIButton)
    {
        Tools.Ins.SetDebugToggle(_toggle, _action, _music);
    }

    /// <summary>
    /// 获取Text里字符串长度
    /// </summary>
    /// <param name="text"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static float GetTextWidth(Text text, string msg)
    {
        var generator = new TextGenerator();
        var rectTransform = text.GetComponent<RectTransform>();
        var size = rectTransform.rect.size;
        var settings = text.GetGenerationSettings(size);
        return generator.GetPreferredWidth(msg, settings) / text.pixelsPerUnit;
    }

    /// <summary>
    /// 根据文本组件和显示文字返回文本高度
    /// </summary>
    /// <param name="text"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static float GetTextHeight(Text text, string msg)
    {
        var generator = new TextGenerator();
        var rectTransform = text.GetComponent<RectTransform>();
        var size = rectTransform.rect.size;
        var settings = text.GetGenerationSettings(size);
        return generator.GetPreferredHeight(msg, settings) / text.pixelsPerUnit;
    }

    /// <summary>
    ///  设置粒子层级
    /// </summary>
    /// <param name="_layer"></param>
    public static void SetParticleOrder(GameObject _go, int _layer)
    {
        ParticleSystem[] pss = _go.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var item in pss)
        {
            item.GetComponent<ParticleSystemRenderer>().sortingOrder = _layer;
        }
    }

    /// <summary>
    /// 弹提示
    /// </summary>
    /// <param name="_"></param>
    public static void PopTips(string _tips)
    {
        UIMgr.Ins.OpenView<TipsView>().Tips(_tips);
    }

    /// <summary>
    /// 数值tween动画
    /// </summary>
    /// <param name="_startVal"></param>
    /// <param name="_endVal"></param>
    /// <param name="_updateCB"></param>
    /// <param name="_duration"></param>
    /// <returns></returns>
    public static TweenerCore<float, float, FloatOptions> DONumVal(float _startVal, float _endVal, Action<float> _updateCB, float _duration = 1)
    {
        float _num = _startVal;
        var Tween = DOTween.To(() => _num, x => _num = x, _endVal, _duration);
        Tween.onUpdate = () =>
        {
            //Debug.LogError(_num);
            _updateCB(_num);
        };
        return Tween;
    }

    /// <summary>
    /// 放大弹出一个物体
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="_onComplete"></param>
    public static void DoPopScale(Transform trans, Action _onComplete = null, float _duration = 0.5f)
    {
        trans.DOKill();
        CanvasGroup cg = trans.GetComponent<CanvasGroup>();
        trans.gameObject.SetActive(true);
        trans.localScale = Vector3.one * 0.8f;
        Tween t = trans.DOScale(1, _duration);
        t.SetAutoKill(true);
        t.onComplete = delegate ()
        {
            _onComplete?.Invoke();
        };
        t.SetEase(Ease.OutBack);
        t.Play();
        if (cg != null)
        {
            cg.DOKill();
            cg.alpha = 0.7f;
            cg.DOFade(1, 0.15f);
        }
    }

    /// <summary>
    /// 获取本机的ip地址
    /// </summary>
    /// <returns></returns>
    public static string GetIPAdress()
    {

        IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
        for (int i = 0; i < ips.Length; i++)
        {
            IPAddress address = ips[i];
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                return address.ToString();//返回ipv4的地址的字符串
            }
        }
        //找不到就返回本地
        return "127.0.0.1";
    }

    /// <summary>
    /// 读取设置配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_id"></param>
    /// <returns></returns>
    public static string GetSetting(string _id)
    {
        string val = ConfigMgr.Ins.settingConfig.Setting[_id].val;
        return val;
    }

    /// <summary>
    /// 读取设置配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_id"></param>
    /// <returns></returns>
    public static T GetSetting<T>(string _id)
    {
        string val = ConfigMgr.Ins.settingConfig.Setting[_id].val;
        return JsonConvert.DeserializeObject<T>(val);
    }

    /// <summary>
    /// /是否是调试模式 
    /// </summary>
    /// <returns></returns>
    public static bool IsDebug
    {
        get
        {
            if (Debug.isDebugBuild)
            {
                //编辑器 或者 Development Build
                return true;
            }
            //设备唯一ID加入后台调试名单
            if (IsDebugDevice)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 是否为调试设备
    /// </summary>
    /// <returns></returns>
    public static bool IsDebugDevice
    {
        get
        {
            if (!string.IsNullOrEmpty(DeviceIdentifier) && !string.IsNullOrEmpty(NetInfoMgr.Ins.NetServerData?.DebugDeviceID))
            {
                List<string> devices = JsonConvert.DeserializeObject<List<string>>(NetInfoMgr.Ins.NetServerData.DebugDeviceID);
                if (devices.Contains(DeviceIdentifier))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 获取设备标识符
    /// </summary>
    public static string DeviceIdentifier
    {
        get
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
    }

    /// <summary>
    /// 获取设备型号
    /// </summary>
    public static string DeviceModel
    {
        get
        {
#if !UNITY_EDITOR && UNITY_IPHONE
        return UnityEngine.iOS.Device.generation.ToString();
#else
            return SystemInfo.deviceModel;
#endif
        }
    }


    /// </summary>
    /// 获取千分位数值
    /// </summary>
    /// <param name="_val"></param>
    /// <param name="_decimals">小数位数</param>
    /// <returns></returns>
    public static string GetValByThousands(double _val, int _decimals = 0, SystemLanguage _lang = SystemLanguage.Unknown)
    {
        if (_lang == SystemLanguage.Unknown)
            _lang = LangMgr.Ins.currLang;
        char decimalSign = (_lang == SystemLanguage.Portuguese ? ',' : '.');
        char thouandSign = (_lang == SystemLanguage.Portuguese ? '.' : ',');
        string result = "";
        string str = _val.ToString("f" + _decimals);
        string[] strArr = str.Split('.');
        if (strArr.Length > 1)
        {
            //有小数部分
            result = decimalSign + strArr[1];
        }
        int _intVal = (int)_val;
        if (_intVal == 0)
        {
            result = '0' + result;
        }
        while (_intVal > 0)
        {
            string tempVal = (_intVal % 1000).ToString();
            if (_intVal >= 1000)
            {
                tempVal = tempVal.PadLeft(3, '0');
                result = thouandSign + tempVal + result;
            }
            else
            {
                result = tempVal + result;
            }
            _intVal /= 1000;
        }
        return result;
    }
}