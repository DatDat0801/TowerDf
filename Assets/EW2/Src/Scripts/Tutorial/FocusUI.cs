using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;

namespace  EW2.Tutorial.UI
{
    public class FocusUI: MonoBehaviour
    {
        [SerializeField] private Transform focusTransform;
        [SerializeField] private Transform tooltipTransform;
        [SerializeField] private Text tooltipTxt;
        [SerializeField] private GameObject focusElement;
        
    
        public void ShowFocus(Vector3 focusPosition,string tooltip)
        {
            focusElement.SetActive(true);
            focusTransform.position = focusPosition;
            tooltipTransform.position = focusPosition;
            if (!string.IsNullOrEmpty(tooltip))
            {
                tooltipTransform.gameObject.SetActive(true);
                tooltipTxt.text = Localization.Current.Get("tutorial", tooltip);;
            }
            else
            {
                tooltipTransform.gameObject.SetActive(false);
            }
        }

        public void HideFocus()
        {
            focusElement.SetActive(false);
            tooltipTransform.gameObject.SetActive(false);
        }
        
    }
}