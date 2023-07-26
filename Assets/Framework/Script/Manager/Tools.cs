using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class Tools : MonoSingleton<Tools>
{
    public void SetButton(Button _button, Action _action, AudioSound _music = AudioSound.Sound_UIButton)
    {
        _button.onClick.AddListener(() =>
        {
            AudioMgr.Ins.PlaySound(_music);
            _action();
        });
    }

    public void SetDebugButton(Button _button, Action _action, AudioSound _music = AudioSound.Sound_UIButton)
    {
        if (!Application.isEditor)
        {
            _button.gameObject.SetActive(false);
            return;
        }
        _button.image.color = new Color(0.03f, 0.94f, 1);
        SetButton(_button, _action);
    }

    public void SetToggle(Toggle _toggle, Action<bool> _action = null, AudioSound _music = AudioSound.Sound_UIButton)
    {
        _toggle.onValueChanged.AddListener((_ison) =>
        {
            AudioMgr.Ins.PlaySound(_music);
            _action?.Invoke(_ison);
        });
    }

    public void SetDebugToggle(Toggle _toggle, Action<bool> _action = null, AudioSound _music = AudioSound.Sound_UIButton)
    {
        if (!Application.isEditor)
        {
            _toggle.gameObject.SetActive(false);
            return;
        }
        _toggle.image.color = new Color(0.03f, 0.94f, 1);
        SetToggle(_toggle, _action);
    }

    public GameObject Create2DGo(string _name, Transform _parent)
    {
        GameObject go = new GameObject(_name, typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        go.transform.SetParent(_parent);
        rect.anchoredPosition3D = Vector3.zero;
        rect.localEulerAngles = Vector3.zero;
        rect.localScale = Vector3.one;
        return go;
    }

    public GameObject Create3DGo(string _name, Transform _parent)
    {
        GameObject go = new GameObject(_name);
        go.transform.SetParent(_parent);
        go.transform.position = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;
        return go;
    }

    /// <summary>
    /// 锚点坐标转世界坐标
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_canvas"></param>
    public Vector3 AnchorPosToWorld(Vector2 _pos, Canvas _canvas)
    {
        Vector3 scale = _canvas.transform.localScale;
        return new Vector3(_pos.x * scale.x, _pos.y * scale.y, 0);
    }

    /// <summary>
    /// 世界坐标转锚点坐标
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_canvas"></param>
    public Vector2 WorldPosToAnchor(Vector3 _pos, Canvas _canvas)
    {
        Vector3 scale = _canvas.transform.localScale;
        return new Vector2(_pos.x / scale.x, _pos.y / scale.y);
    }

    List<string> getFormatGoldStr_suffix = new List<string>() { "", "K", "M", "B", "T", "P", "E", "Z", "Y" };
    /// <summary>
    /// 金钱数量数字化，比如1000转成"1K"
    /// </summary>
    /// <returns></returns>
    public string GetFormatGoldStr(int _num)
    {
        int index = 0;
        while (_num >= 10000)
        {
            if (_num < 10000)
            {
                break;
            }
            index++;
            _num /= 1000;
        }
        return Mathf.Round(_num) + getFormatGoldStr_suffix[index];
    }
}
