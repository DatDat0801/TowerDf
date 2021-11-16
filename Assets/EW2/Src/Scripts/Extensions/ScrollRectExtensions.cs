using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EW2.Tools
{
    /// <summary>
    /// Scrollrect extensions
    /// </summary>
    public static class ScrollRectExtensions
    {
        /// <summary>
        /// Scrolls a scroll rect to the top
        /// </summary>
        /// <param name="scrollRect"></param>
        /// <param name="smooth"></param>
        public static void MMScrollToTop(this ScrollRect scrollRect, bool smooth)
        {
            if (smooth)
            {
                scrollRect.normalizedPosition = new Vector2(0, 1);    
            }
            else
            {
                scrollRect.normalizedPosition = new Vector2(0, 1);    
            }
            
        }

        /// <summary>
        /// Scrolls a scroll rect to the bottom
        /// </summary>
        public static void MMScrollToBottom(this ScrollRect scrollRect, bool smooth, UnityAction onCompleted = null)
        {
            if (smooth)
            {
                scrollRect.DONormalizedPos(Vector2.zero, 0.8f).OnComplete(() => onCompleted?.Invoke());
            }
            else
            {
                scrollRect.normalizedPosition = Vector2.zero;
                onCompleted?.Invoke();
            }
            
        }


        public static void SnapTo(this ScrollRect scroller, RectTransform target, float offset, bool smooth = false, UnityAction onCompleted =null)
        {
            RectTransform content;
            (content = scroller.content).ForceUpdateRectTransforms();

            var contentPos = (Vector2) scroller.transform.InverseTransformPoint(content.position);
            var childPos = (Vector2) scroller.transform.InverseTransformPoint(target.position);
            var endPos = contentPos - childPos;
            // If no horizontal scroll, then don't change contentPos.x
            if (scroller.vertical)
            {
                endPos.x = scroller.content.anchoredPosition.x;
                endPos.y -= Screen.height / 2;
            }

            if (scroller.horizontal)
            {
                endPos.y = scroller.content.anchoredPosition.y;
                endPos.x += offset; //Screen.width / 2;
            }
            if (content.anchoredPosition == endPos)
            {
                return;
            }
            if (!smooth)
            {
                content.anchoredPosition = endPos;
                onCompleted?.Invoke();
            }
            else
            {
                content.DOAnchorPos(endPos, 0.8f).OnComplete(() => onCompleted?.Invoke());
            }

            //scroller.content.ForceUpdateRectTransforms();
        }

        /// <summary>
        /// Based on https://stackoverflow.com/a/50191835
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static void BringChildIntoView(this UnityEngine.UI.ScrollRect instance, RectTransform target)
        {
            instance.content.ForceUpdateRectTransforms();
            instance.viewport.ForceUpdateRectTransforms();

            // now takes scaling into account
            Vector2 viewportLocalPosition = instance.viewport.localPosition;
            Vector2 childLocalPosition = target.localPosition;
            Vector2 newContentPosition = new Vector2(
                0 - ((viewportLocalPosition.x * instance.viewport.localScale.x) +
                     (childLocalPosition.x * instance.content.localScale.x)),
                0 - ((viewportLocalPosition.y * instance.viewport.localScale.y) +
                     (childLocalPosition.y * instance.content.localScale.y))
            );

            // clamp positions
            instance.content.localPosition = newContentPosition;
            Rect contentRectInViewport = TransformRectFromTo(instance.content.transform, instance.viewport);
            float deltaXMin = contentRectInViewport.xMin - instance.viewport.rect.xMin;
            if (deltaXMin > 0) // clamp to <= 0
            {
                newContentPosition.x -= deltaXMin;
            }

            float deltaXMax = contentRectInViewport.xMax - instance.viewport.rect.xMax;
            if (deltaXMax < 0) // clamp to >= 0
            {
                newContentPosition.x -= deltaXMax;
            }

            float deltaYMin = contentRectInViewport.yMin - instance.viewport.rect.yMin;
            if (deltaYMin > 0) // clamp to <= 0
            {
                newContentPosition.y -= deltaYMin;
            }

            float deltaYMax = contentRectInViewport.yMax - instance.viewport.rect.yMax;
            if (deltaYMax < 0) // clamp to >= 0
            {
                newContentPosition.y -= deltaYMax;
            }

            // apply final position
            instance.content.localPosition = newContentPosition;
            instance.content.ForceUpdateRectTransforms();
        }

        /// <summary>
        /// Converts a Rect from one RectTransfrom to another RectTransfrom.
        /// Hint: use the root Canvas Transform as "to" to get the reference pixel positions.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Rect TransformRectFromTo(Transform from, Transform to)
        {
            RectTransform fromRectTrans = from.GetComponent<RectTransform>();
            RectTransform toRectTrans = to.GetComponent<RectTransform>();

            if (fromRectTrans != null && toRectTrans != null)
            {
                Vector3[] fromWorldCorners = new Vector3[4];
                Vector3[] toLocalCorners = new Vector3[4];
                Matrix4x4 toLocal = to.worldToLocalMatrix;
                fromRectTrans.GetWorldCorners(fromWorldCorners);
                for (int i = 0; i < 4; i++)
                {
                    toLocalCorners[i] = toLocal.MultiplyPoint3x4(fromWorldCorners[i]);
                }

                return new Rect(toLocalCorners[0].x, toLocalCorners[0].y, toLocalCorners[2].x - toLocalCorners[1].x,
                    toLocalCorners[1].y - toLocalCorners[0].y);
            }

            return default(Rect);
        }
    }
}