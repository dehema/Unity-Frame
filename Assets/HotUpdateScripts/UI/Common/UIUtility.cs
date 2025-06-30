using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Rain.UI;
using UnityEngine;

    public static class UIUtility
    {

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
    /// 弹提示
    /// </summary>
    /// <param name="_"></param>
    public static void PopTips(string _tips)
    {
        UIMgr.Ins.OpenView<TipsView>().Tips(_tips);
    }
}
