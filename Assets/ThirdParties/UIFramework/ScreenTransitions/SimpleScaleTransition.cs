using System;
using DG.Tweening;
using UnityEngine;

namespace Zitga.UIFramework
{
    public class SimpleScaleTransition : ATransitionComponent
    {
        [SerializeField] protected bool isOutAnimation;
        [SerializeField] protected float duration = 0.5f;
        [SerializeField] protected bool doFade;
        [SerializeField] protected float fadeDurationPercent = 0.5f;
        [SerializeField] protected Ease ease = Ease.Linear;

        public override void Animate(Transform target, Action callWhenFinished)
        {
            RectTransform rTransform = target as RectTransform;

            var canvasGroup = rTransform.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = rTransform.gameObject.AddComponent<CanvasGroup>();
            }

            if (doFade)
            {
                ShowFade(rTransform, canvasGroup);
            }

            rTransform.DOKill();

            if (isOutAnimation)
            {
                rTransform.DOScale(0f, duration).SetEase(ease)
                    .OnComplete(() => Cleanup(callWhenFinished, rTransform, canvasGroup))
                    .SetUpdate(true);
            }
            else
            {
                rTransform.localScale = Vector3.zero;

                rTransform.DOScale(1f, duration).SetEase(ease)
                    .OnComplete(() => Cleanup(callWhenFinished, rTransform, canvasGroup))
                    .SetUpdate(true);
            }
        }

        private void ShowFade(RectTransform rTransform, CanvasGroup canvasGroup)
        {
            var startValue = 0f;

            var endValue = 0f;

            if (isOutAnimation)
            {
                startValue = 1f;
                endValue = 0f;
            }
            else
            {
                startValue = 0f;
                endValue = 1f;
            }

            canvasGroup.alpha = startValue;

            canvasGroup.DOFade(endValue, duration * fadeDurationPercent).SetUpdate(true);
        }

        private void Cleanup(Action callWhenFinished, RectTransform rTransform, CanvasGroup canvasGroup)
        {
            callWhenFinished?.Invoke();

            rTransform.localScale = Vector3.one;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}