using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class NoticeItem : MonoBehaviour, IUIPrompt
    {
        public Image icon;
        public BadgeType type;
        void OnEnable()
        {
            EventManager.StartListening($"Enable_{type}", EnableNotice);
            EventManager.StartListening($"Disable_{type}", DisableNotice);
        }

        private void OnDisable()
        {
            EventManager.StopListening($"Enable_{type}", EnableNotice);
            EventManager.StopListening($"Disable_{type}", DisableNotice);
        }

        void EnableNotice()
        {
            icon.enabled = true;
            Status = true;
        }

        void DisableNotice()
        {
            icon.enabled = false;
            Status = false;
        }

        public bool Status { get; set; }
        public void Notice()
        {
            
        }
    }
}