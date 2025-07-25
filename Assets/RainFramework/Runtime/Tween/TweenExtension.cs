﻿using UnityEngine;
using UnityEngine.UI;

namespace Rain.Core
{
    public static class TweenExtension
    {
        public static BaseTween ScaleTween(this Transform t, Vector3 to, float time)
        {
            return F8Tween.Ins.ScaleTween(t, to, time);
        }

        public static BaseTween ScaleTween(this GameObject go, Vector3 to, float time)
        {
            return F8Tween.Ins.ScaleTween(go.transform, to, time);
        }

        public static BaseTween ScaleTween(this RectTransform rect, Vector3 to, float time)
        {
            return F8Tween.Ins.ScaleTween(rect, to, time);
        }

        public static BaseTween ScaleAtSpeed(this Transform t, Vector3 to, float speed)
        {
            return F8Tween.Ins.ScaleTweenAtSpeed(t, to, speed);
        }

        public static BaseTween ScaleAtSpeed(this GameObject go, Vector3 to, float speed)
        {
            return F8Tween.Ins.ScaleTweenAtSpeed(go, to, speed);
        }

        public static BaseTween ScaleAtSpeed(this RectTransform rect, Vector3 to, float speed)
        {
            return F8Tween.Ins.ScaleTweenAtSpeed(rect, to, speed);
        }

        public static BaseTween ScaleX(this Transform obj, float value, float t)
        {
            return F8Tween.Ins.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleX(this GameObject obj, float value, float t)
        {
            return F8Tween.Ins.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleX(this RectTransform obj, float value, float t)
        {
            return F8Tween.Ins.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleXAtSpeed(this Transform obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleXAtSpeed(this GameObject obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleXAtSpeed(this RectTransform obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleY(this Transform obj, float value, float t)
        {
            return F8Tween.Ins.ScaleY(obj, value, t);
        }

        public static BaseTween ScaleY(this GameObject obj, float value, float t)
        {
            return F8Tween.Ins.ScaleY(obj, value, t);
        }

        public static BaseTween ScaleY(this RectTransform obj, float value, float t)
        {
            return F8Tween.Ins.ScaleY(obj, value, t);
        }

        public static BaseTween ScaleYAtSpeed(this Transform obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleYAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleYAtSpeed(this GameObject obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleYAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleYAtSpeed(this RectTransform obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleYAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleZ(this Transform obj, float value, float t)
        {
            return F8Tween.Ins.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleZ(this GameObject obj, float value, float t)
        {
            return F8Tween.Ins.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleZ(this RectTransform obj, float value, float t)
        {
            return F8Tween.Ins.ScaleX(obj, value, t);
        }

        public static BaseTween ScaleZAtSpeed(this Transform obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleZAtSpeed(this GameObject obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween ScaleZAtSpeed(this RectTransform obj, float value, float speed)
        {
            return F8Tween.Ins.ScaleXAtSpeed(obj, value, speed);
        }

        public static BaseTween RotateTween(this Transform t, Vector3 axis, float to, float time)
        {
            return F8Tween.Ins.RotateTween(t, axis, to, time);
        }

        public static BaseTween RotateTween(this Transform t, Vector3 to, float time)
        {
            return F8Tween.Ins.RotateTween(t, to, time);
        }

        public static BaseTween RotateTween(this Transform t, Quaternion to, float time)
        {
            return F8Tween.Ins.RotateTween(t, to, time);
        }

        public static BaseTween RotateTween(this GameObject go, Vector3 to, float time)
        {
            return F8Tween.Ins.RotateTween(go.transform, to, time);
        }

        public static BaseTween RotateTween(this GameObject go, Vector3 axis, float to, float time)
        {
            return F8Tween.Ins.RotateTween(go, axis, to, time);
        }

        public static BaseTween RotateTween(this GameObject go, Quaternion to, float time)
        {
            return F8Tween.Ins.RotateTween(go.transform, to, time);
        }

        public static BaseTween RotateTween(this RectTransform rect, Vector3 to, float time)
        {
            return F8Tween.Ins.RotateTween(rect, to, time);
        }

        public static BaseTween RotateTween(this RectTransform rect, Quaternion to, float time)
        {
            return F8Tween.Ins.RotateTween(rect, to, time);
        }

        public static BaseTween RotateTween(this RectTransform rect, Vector3 axis, float to, float time)
        {
            return F8Tween.Ins.RotateTween(rect, axis, to, time);
        }

        public static BaseTween FadeOutAtSpeed(this CanvasGroup cg, float speed)
        {
            return F8Tween.Ins.FadeOutAtSpeed(cg, speed);
        }

        public static BaseTween FadeIn(this CanvasGroup cg, float t)
        {
            return F8Tween.Ins.FadeIn(cg, t);
        }

        public static BaseTween FadeInAtSpeed(this CanvasGroup cg, float speed)
        {
            return F8Tween.Ins.FadeInAtSpeed(cg, speed);
        }

        public static BaseTween Fade(this CanvasGroup cg, float to, float t)
        {
            return F8Tween.Ins.Fade(cg, to, t);
        }

        public static BaseTween FadeAtSpeed(this CanvasGroup cg, float to, float speed)
        {
            return F8Tween.Ins.FadeAtSpeed(cg, to, speed);
        }

        public static BaseTween Fade(this Image image, float to, float t)
        {
            return F8Tween.Ins.Fade(image, to, t);
        }

        public static BaseTween FadeAtSpeed(this Image img, float to, float speed)
        {
            return F8Tween.Ins.FadeAtSpeed(img, to, speed);
        }

        public static BaseTween FadeOut(this Image image, float t)
        {
            return F8Tween.Ins.FadeOut(image, t);
        }

        public static BaseTween FadeOutAtSpeed(this Image img, float speed)
        {
            return F8Tween.Ins.FadeOutAtSpeed(img, speed);
        }

        public static BaseTween FadeIn(this Image image, float t)
        {
            return F8Tween.Ins.FadeIn(image, t);
        }

        public static BaseTween FadeInAtSpeed(this Image img, float speed)
        {
            return F8Tween.Ins.FadeInAtSpeed(img, speed);
        }

        public static BaseTween Fade(this SpriteRenderer sprite, float to, float t)
        {
            return F8Tween.Ins.Fade(sprite, to, t);
        }

        public static BaseTween FadeAtSpeed(this SpriteRenderer sprite, float to, float speed)
        {
            return F8Tween.Ins.FadeAtSpeed(sprite, to, speed);
        }

        public static BaseTween FadeOut(this CanvasGroup cg, float t)
        {
            return F8Tween.Ins.FadeOut(cg, t);
        }

        public static BaseTween FadeOut(this SpriteRenderer sprite, float t)
        {
            return F8Tween.Ins.FadeOut(sprite, t);
        }

        public static BaseTween FadeOutAtSpeed(this SpriteRenderer sprite, float speed)
        {
            return F8Tween.Ins.FadeOutAtSpeed(sprite, speed);
        }

        public static BaseTween FadeIn(this SpriteRenderer sprite, float t)
        {
            return F8Tween.Ins.FadeIn(sprite, t);
        }

        public static BaseTween FadeInAtSpeed(this SpriteRenderer sprite, float speed)
        {
            return F8Tween.Ins.FadeInAtSpeed(sprite, speed);
        }

        public static BaseTween ColorTween(this Material material, Color to, float t)
        {
            return F8Tween.Ins.ColorTween(material, to, t);
        }

        public static BaseTween ColorTween(this SpriteRenderer sprite, Color to, float t)
        {
            return F8Tween.Ins.ColorTween(sprite, to, t);
        }

        public static BaseTween ColorTweenAtSpeed(this SpriteRenderer sprite, Color to, float speed)
        {
            return F8Tween.Ins.ColorTweenAtSpeed(sprite, to, speed);
        }

        public static BaseTween ColorTween(this Image image, Color to, float t)
        {
            return F8Tween.Ins.ColorTween(image, to, t);
        }

        public static BaseTween ColorTweenAtSpeed(this Image img, Color to, float speed)
        {
            return F8Tween.Ins.ColorTweenAtSpeed(img, to, speed);
        }

        public static BaseTween FillAmountTween(this Image img, float to, float t)
        {
            return F8Tween.Ins.FillAmountTween(img, to, t);
        }

        public static BaseTween FillAmountTweenAtSpeed(this Image img, float to, float speed)
        {
            return F8Tween.Ins.FillAmountTween(img, to, speed);
        }

        public static BaseTween Move(this Transform obj, Transform to, float t)
        {
            return F8Tween.Ins.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this Transform obj, Transform to, float t)
        {
            return F8Tween.Ins.LocalMove(obj, to, t);
        }

        public static BaseTween MoveAtSpeed(this Transform obj, Transform to, float speed)
        {
            return F8Tween.Ins.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this Transform obj, Transform to, float speed)
        {
            return F8Tween.Ins.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween Move(this Transform obj, Vector3 to, float t)
        {
            return F8Tween.Ins.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this Transform obj, Vector3 to, float t)
        {
            return F8Tween.Ins.LocalMove(obj, to, t);
        }

        public static BaseTween MoveAtSpeed(this Transform obj, Vector3 to, float speed)
        {
            return F8Tween.Ins.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this Transform obj, Vector3 to, float speed)
        {
            return F8Tween.Ins.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween Move(this GameObject obj, Transform to, float t)
        {
            return F8Tween.Ins.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this GameObject obj, Transform to, float t)
        {
            return F8Tween.Ins.LocalMove(obj, to, t);
        }

        public static BaseTween MoveAtSpeed(this GameObject obj, Transform to, float speed)
        {
            return F8Tween.Ins.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this GameObject obj, Transform to, float speed)
        {
            return F8Tween.Ins.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween Move(this GameObject obj, Vector3 to, float t)
        {
            return F8Tween.Ins.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this GameObject obj, Vector3 to, float t)
        {
            return F8Tween.Ins.LocalMove(obj, to, t);
        }

        public static BaseTween MoveAtSpeed(this GameObject obj, Vector3 to, float speed)
        {
            return F8Tween.Ins.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this GameObject obj, Vector3 to, float speed)
        {
            return F8Tween.Ins.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween Move(this GameObject obj, GameObject to, float t)
        {
            return F8Tween.Ins.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this GameObject obj, GameObject to, float t)
        {
            return F8Tween.Ins.LocalMove(obj, to, t);
        }

        public static BaseTween MoveAtSpeed(this GameObject obj, GameObject to, float speed)
        {
            return F8Tween.Ins.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this GameObject obj, GameObject to, float speed)
        {
            return F8Tween.Ins.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween Move(this Transform obj, GameObject to, float t)
        {
            return F8Tween.Ins.Move(obj, to, t);
        }

        public static BaseTween LocalMove(this Transform obj, GameObject to, float t)
        {
            return F8Tween.Ins.LocalMove(obj, to, t);
        }

        public static BaseTween MoveAtSpeed(this Transform obj, GameObject to, float speed)
        {
            return F8Tween.Ins.MoveAtSpeed(obj, to, speed);
        }

        public static BaseTween LocalMoveAtSpeed(this Transform obj, GameObject to, float speed)
        {
            return F8Tween.Ins.LocalMoveAtSpeed(obj, to, speed);
        }

        public static BaseTween Move(this RectTransform rect, Vector2 pos, float t)
        {
            return F8Tween.Ins.Move(rect, pos, t);
        }

        public static BaseTween MoveAtSpeed(this RectTransform rect, Vector2 pos, float speed)
        {
            return F8Tween.Ins.MoveAtSpeed(rect, pos, speed);
        }

        public static BaseTween MoveUI(this RectTransform rect, Vector2 absolutePosition, RectTransform canvas, float t,
            PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return F8Tween.Ins.MoveUI(rect, absolutePosition, canvas, t, pivotPreset);
        }

        public static BaseTween MoveUIAtSpeed(this RectTransform rect, Vector2 absolutePosition, RectTransform canvas,
            float speed, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return F8Tween.Ins.MoveUIAtSpeed(rect, absolutePosition, canvas, speed, pivotPreset);
        }

        public static BaseTween TranslateUI(this RectTransform rect, Vector2 translation, RectTransform canvas, float t,
            PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return F8Tween.Ins.TranslateUI(rect, translation, canvas, t, pivotPreset);
        }

        public static BaseTween TranslateUIAtSpeed(this RectTransform rect, Vector2 translation, RectTransform canvas,
            float speed, PivotPreset pivotPreset = PivotPreset.MiddleCenter)
        {
            return F8Tween.Ins.TranslateUIAtSpeed(rect, translation, canvas, speed, pivotPreset);
        }

        public static Sequence ShakePosition(this Transform obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakePosition(obj, vibrato, shakeCount, t);
        }

        public static Sequence ShakePosition(this GameObject obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakePosition(obj.transform, vibrato, shakeCount, t);
        }

        public static Sequence ShakePositionAtSpeed(this Transform obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return F8Tween.Ins.ShakePositionAtSpeed(obj, vibrato, shakeCount, speed);
        }

        public static Sequence ShakePositionAtSpeed(this GameObject obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return F8Tween.Ins.ShakePositionAtSpeed(obj, vibrato, shakeCount, speed);
        }

        public static Sequence ShakeRotation(this Transform obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeRotation(obj, vibrato, shakeCount, t);
        }

        public static Sequence ShakeRotation(this GameObject obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeRotation(obj.transform, vibrato, shakeCount, t);
        }

        public static Sequence ShakeRotationAtSpeed(this Transform obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeRotationAtSpeed(obj, vibrato, shakeCount, speed);
        }

        public static Sequence ShakeRotationAtSpeed(this GameObject obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f, bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeRotationAtSpeed(obj, vibrato, shakeCount, speed);
        }

        public static Sequence ShakeScale(this Transform obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeScale(obj, vibrato, shakeCount, t);
        }

        public static Sequence ShakeScale(this GameObject obj, Vector3 vibrato, int shakeCount = 8, float t = 0.05f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeScale(obj.transform, vibrato, shakeCount, t);
        }

        public static Sequence ShakeScaleAtSpeed(this Transform obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeScaleAtSpeed(obj, vibrato, shakeCount, speed);
        }

        public static Sequence ShakeScaleAtSpeed(this GameObject obj, Vector3 vibrato, int shakeCount = 8,
            float speed = 5f,
            bool fadeOut = false)
        {
            return F8Tween.Ins.ShakeScaleAtSpeed(obj, vibrato, shakeCount, speed);
        }

        public static void CancelAllTweens(this GameObject go)
        {
            F8Tween.Ins.CancelTween(go);
        }

        public static void CancelTween(this GameObject go, int id)
        {
            F8Tween.Ins.CancelTween(id);
        }
    }
}