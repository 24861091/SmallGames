using System;
using System.Globalization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Extensions
{
    public static class DoTweenExtension
    {
        // 抄bingo1的
        public static EaseFunction EaseInOut(float rate)
        {
            return (time, duration, amplitude, period) =>
            {
                var dt = time / duration;
                dt *= 2;
                if (dt < 1f)
                {
                    return 0.5f * (float)Math.Pow(dt, rate);
                }

                return 1.0f - 0.5f * (float)Math.Pow(2 - dt, rate);
            };
        }

        // 抄bingo1的
        // public static EaseFunction EaseBezierByTime(float a, float b, float c, float d)
        // {
        //     return (time, duration, overshootOrAmplitude, period) =>
        //     {
        //         return (float)BezierUtils.BezierByTime(new double[] { a, b, c, d }, time / duration);
        //     };
        // }
        
        // 抄bingo1的
        public static EaseFunction EaseIn(float rate)
        {
            return (time, duration, amplitude, period) => (float)Math.Pow(time / duration, rate);
        }
        
        // 抄bingo1的
        public static EaseFunction EaseOut(float rate)
        {
            return (time, duration, amplitude, period) => (float)Math.Pow(time / duration, 1 / rate);
        }

        // public static Tweener DOAlpha(this TextAgent textAgent, float value, float duration)
        // {
        //     var t = DOTween.To(() => textAgent.Alpha, v => textAgent.Alpha = v, value, duration);
        //     t.SetTarget(textAgent);
        //     return t;
        // }

        public static Tweener DOSize(this Scrollbar scrollbar, float value, float duration)
        {
            var t = DOTween.To(() => scrollbar.size, v => scrollbar.size = v, value, duration);
            t.SetTarget(scrollbar);
            return t;
        }

        // public static Tweener DOProgress(this ProgressBarAgent progressBarAgent, float value, float duration)
        // {
        //     var t = DOTween.To(() => progressBarAgent.value, v => progressBarAgent.value = v, value, duration);
        //     t.SetTarget(progressBarAgent);
        //     return t;
        // }
        //
        // public static Tweener DOText2(this TextAgent textAgent, float cur, float dest, float duration)
        // {
        //     var t = DOTween.To(v => textAgent.text = Mathf.Round(v).ToString(CultureInfo.InvariantCulture), cur, dest,
        //                        duration);
        //     return t;
        // }

        public static Tweener DOImageFill(this Image image, float cur, float dest, float duration)
        {
            var t = DOTween.To(v => image.fillAmount = v, cur, dest, duration);
            return t;
        }

        public static Tweener DOShaderFValue(this Material mat, string prop, float cur, float dest, float duration)
        {
            var t = DOTween.To(v => mat.SetFloat(prop, v), cur, dest, duration);
            return t;
        }

        public static Tweener DOAlpha(this CanvasGroup cvg, float cur, float dest, float duration)
        {
            var t = DOTween.To(v => cvg.alpha = v, cur, dest, duration);
            return t;
        }

        public static Tweener DOSliderValue(this Slider slider, float cur, float dest, float duration,
                                            Action<float> cb = null)
        {
            var t = DOTween.To(v =>
            {
                slider.value = v;
                cb?.Invoke(v);
            }, cur, dest, duration);
            return t;
        }

        public static Tweener DOLookAt(this Transform transform, Transform parent, Vector3 point, float duration)
        {
            var worldPos = parent.TransformPoint(point);
            var forward  = worldPos - transform.position;
            var v        = Quaternion.LookRotation(forward, transform.up);
            return transform.DORotateQuaternion(v, duration);
        }
    }
}