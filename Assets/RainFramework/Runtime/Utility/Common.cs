using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Rain.Core
{

    public static partial class Util
    {
        public static class Common
        {
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

            /// <summary>
            /// 获取Text里字符串长度
            /// </summary>
            /// <param name="text"></param>
            /// <param name="msg"></param>
            /// <returns></returns>
            public static float GetTextWidth(UnityEngine.UI.Text text, string msg)
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
            public static float GetTextHeight(UnityEngine.UI.Text text, string msg)
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

            ///// <summary>
            ///// 读取设置配置
            ///// </summary>
            ///// <typeparam name="T"></typeparam>
            ///// <param name="_id"></param>
            ///// <returns></returns>
            //public static string GetSetting(string _id)
            //{
            //    string val = ConfigMgr.Ins.settingConfig.Common[_id].val;
            //    return val;
            //}

            ///// <summary>
            ///// 读取设置配置
            ///// </summary>
            ///// <typeparam name="T"></typeparam>
            ///// <param name="_id"></param>
            ///// <returns></returns>
            //public static Vector2 GetSetting_Vector2(string _id)
            //{
            //    string val = ConfigMgr.Ins.settingConfig.Common[_id].val;
            //    string[] par = val.Split(',');
            //    Vector2 vec = new Vector2(float.Parse(par[0]), float.Parse(par[1]));
            //    return vec;
            //}


            ///// <summary>
            ///// 读取设置配置
            ///// </summary>
            ///// <typeparam name="T"></typeparam>
            ///// <param name="_id"></param>
            ///// <returns></returns>
            //public static T GetSetting<T>(string _id)
            //{
            //    string val = ConfigMgr.Ins.settingConfig.Common[_id].val;
            //    return JsonConvert.DeserializeObject<T>(val);
            //}

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
                    if (Debug.isDebugBuild && !Application.isEditor)
                        return true;
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

            /// <summary>
            /// 设置物体layer
            /// </summary>
            /// <param name="_layer"></param>
            /// <param name="_go"></param>
            public static void SetLayer(GameObject _go, int _layer)
            {
                _go.layer = _layer;
                foreach (Transform item in _go.transform)
                {
                    SetLayer(item.gameObject, _layer);
                }
            }

            /// <summary>
            /// 获取段落文本
            /// </summary>
            /// <param name="_strList"></param>
            /// <returns></returns>
            public static string GetParagraphText(List<string> _strList)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _strList.Count; i++)
                {
                    sb.Append(_strList[i]);
                    if (i != _strList.Count - 1)
                    {
                        sb.Append("\n");
                    }
                }
                return sb.ToString();
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
            /// 将UI坐标转换为3D世界坐标（位于相机视平面上）
            /// </summary>
            /// <param name="uiPos">UI坐标（通常是屏幕像素坐标）</param>
            /// <param name="camera">用于转换的相机</param>
            /// <param name="distanceFromCamera">距离相机的深度（默认为10米）</param>
            /// <returns>对应的3D世界坐标</returns>
            public static Vector3 GetWorldPositionFromUI(Vector2 uiPos, Camera camera, float distanceFromCamera = 10f)
            {
                // 确保相机存在
                if (camera == null)
                {
                    Debug.LogError("Camera is null!");
                    return Vector3.zero;
                }

                // 将UI坐标转换为世界坐标
                // 使用ScreenPointToRay将屏幕点转换为射线
                Ray ray = camera.ScreenPointToRay(uiPos);

                // 在射线上取距离相机指定距离的点
                return ray.GetPoint(distanceFromCamera);
            }

            /// <summary>
            /// 格式化资源数字显示
            /// </summary>
            /// <param name="resourceNum">资源数量</param>
            /// <returns>格式化后的字符串</returns>
            public static string FormatResourceNumber(long resourceNum)
            {
                if (resourceNum < 10000)
                {
                    return resourceNum.ToString();
                }
                else if (resourceNum < 100000000) // 小于1亿
                {
                    double wan = resourceNum / 10000.0;
                    return $"{wan:F1}万";
                }
                else // 大于等于1亿
                {
                    double yi = resourceNum / 100000000.0;
                    return $"{yi:F1}亿";
                }
            }

            /// <summary>
            /// 格式化倒计时显示
            /// </summary>
            /// <param name="seconds">倒计时秒数</param>
            /// <returns>格式化后的倒计时文本（x天 hh:mm:ss）</returns>
            public static string FormatCountdown(int seconds)
            {
                // 如果倒计时已结束或为负数，返回0天 00:00:00
                if (seconds <= 0)
                {
                    return "0天 00:00:00";
                }

                // 计算天数
                int days = seconds / 86400; // 86400秒 = 1天
                int remainingSeconds = seconds % 86400;

                // 计算小时
                int hours = remainingSeconds / 3600; // 3600秒 = 1小时
                remainingSeconds %= 3600;

                // 计算分钟
                int minutes = remainingSeconds / 60; // 60秒 = 1分钟
                int finalSeconds = remainingSeconds % 60;

                // 格式化输出
                if (days > 0)
                {
                    return $"{days}天 {hours:D2}:{minutes:D2}:{finalSeconds:D2}";
                }
                else
                {
                    return $"{hours:D2}:{minutes:D2}:{finalSeconds:D2}";
                }
            }

            /// <summary>
            /// 格式化倒计时显示（简化版本，不显示天数）
            /// </summary>
            /// <param name="seconds">倒计时秒数</param>
            /// <returns>格式化后的倒计时文本（hh:mm:ss）</returns>
            public static string FormatCountdownSimple(int seconds)
            {
                // 如果倒计时已结束或为负数，返回00:00:00
                if (seconds <= 0)
                {
                    return "00:00:00";
                }

                // 计算小时
                int hours = seconds / 3600;
                int remainingSeconds = seconds % 3600;

                // 计算分钟
                int minutes = remainingSeconds / 60;
                int finalSeconds = remainingSeconds % 60;

                // 格式化输出
                return $"{hours:D2}:{minutes:D2}:{finalSeconds:D2}";
            }
        }
    }
}