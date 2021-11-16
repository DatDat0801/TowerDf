using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zitga.ContextSystem;
using Zitga.UIFramework;

namespace EW2
{

    [Serializable]
    public class ToastShortNotiPanelProperties : PanelProperties
    {
        public string content;
        public ToastShortNotiPanelProperties(string content)
        {
            this.content = content;
        }
    }
    /// <summary>
    /// Yes, this panel is there, all the time, just waiting for its moment to shine
    /// </summary>
    public class ToastShortNotiPanelController : APanelController<ToastShortNotiPanelProperties>
    {

        [SerializeField] private Text txtContent;

        private const float DelayHide = 1;
        
        protected override void Awake()
        {
            base.Awake();
            
            OutTransitionFinished += controller =>
            {
                var toastController = Context.Current.GetService<ToastController>();

                toastController.ShowNotiQueue();
            };

            InTransitionFinished += controller =>
            {
                StartCoroutine(IDelayHide());
            };
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            txtContent.text = Properties.content;
        }

        private IEnumerator IDelayHide()
        {
            yield return new WaitForSecondsRealtime(DelayHide);
            
            UIFrame.Instance.HidePanel(ScreenIds.toast_short_noti_panel);
        }
    }
}
