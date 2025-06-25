using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

    public static class UIUtility
    {
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
    }
