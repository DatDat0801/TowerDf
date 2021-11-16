using System.Collections.Generic;
using TigerForge;
using Zitga.UIFramework;

namespace EW2
{
    public class ToastController
    {
        private Queue<string> ContentQueue { get; set; }
        
        public string CurrentContent { get; private set; }

        public ToastController()
        {
            ContentQueue = new Queue<string>();
            
            AddListeners();
        }
        
        /// <summary>
        /// We're making this respond to the same signal as the PlayerWindow does.
        /// This allows us to have the toast if it's present, but there's no issues if
        /// it's not present for any reason.
        /// </summary>
        private void AddListeners() {
            EventManager.StartListening(GamePlayEvent.ShortNoti, OnShowShortNoti);
        }

        private void OnShowShortNoti()
        {
            var content = EventManager.GetString(GamePlayEvent.ShortNoti);
            CurrentContent = content;
            // if (UIFrame.Instance.IsPanelOpen(ScreenIds.toast_short_noti_panel))
            // {
            //     ContentQueue.Enqueue(content);
            // }
            // else
            //{
            if (UIFrame.Instance.IsPanelOpen(ScreenIds.toast_short_noti_panel))
            {
                UIFrame.Instance.CloseCurrentPanel(false);
                UIFrame.Instance.ShowPanel(ScreenIds.toast_short_noti_panel,
                    new ToastShortNotiPanelProperties(content));
            }
            else
            {
                UIFrame.Instance.ShowPanel(ScreenIds.toast_short_noti_panel,
                    new ToastShortNotiPanelProperties(content));
            }
                
            //}
        }

        public void ShowNotiQueue()
        {
            if(ContentQueue.Count == 0)
                return;

            //string content = ContentQueue.Dequeue();
            
            UIFrame.Instance.ShowPanel(ScreenIds.toast_short_noti_panel, new ToastShortNotiPanelProperties(CurrentContent));
        }
    }
}