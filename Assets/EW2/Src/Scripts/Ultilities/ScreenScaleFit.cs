using System;
using UnityEngine;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [RequireComponent(typeof(Image))]
    public class ScreenScaleFit : MonoBehaviour
    {
        private RectTransform rectTransform;
        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();

            var rect = rectTransform.rect;

            var canvas = UIFrame.Instance.MainCanvas.GetComponent<RectTransform>().rect;
            
            var screenRatio = canvas.height * rect.width > canvas.width * rect.height;

            if (screenRatio)
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvas.height);
                
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width * canvas.height/rect.height);
            }
            else
            {
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvas.width);
                
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height * canvas.width / rect.width);
            }
        }
    }
}