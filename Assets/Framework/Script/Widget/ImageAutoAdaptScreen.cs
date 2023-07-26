using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 根据screen尺寸自动调整缩放，保证在比例不变的情况下永远占满屏幕
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageAutoAdaptScreen : BaseUI
{
    float scale = 1;
    void Awake()
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
    }

    private void Start()
    {
        StartCoroutine(Adapt());
    }

    IEnumerator Adapt()
    {
        yield return new WaitForEndOfFrame();
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        scale = transform.localScale.x;
        if (rect.rect.width * scale < canvasRect.rect.width || rect.rect.height * scale < canvasRect.rect.height)
        {
            scale = canvasRect.rect.width / rect.rect.width;
            scale = Mathf.Max(canvasRect.rect.height / rect.rect.height, scale);
        }
        transform.localScale = new Vector3(scale, scale, scale);
    }
}
