using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2.DailyCheckin
{
    public class DailyCheckinNotice : MonoBehaviour, IUIPrompt
    {
        [SerializeField] private Image icon;
        public bool Status { get; set; }

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.DAILY_CHECKIN, Notice);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(GamePlayEvent.DAILY_CHECKIN, Notice);
        }

        public void Notice()
        {
            var on = EventManager.GetBool(GamePlayEvent.DAILY_CHECKIN);
            if (on)
            {
                icon.enabled = true;
                Status = true;
            }
            else
            {
                icon.enabled = false;
                Status = false;
            }
        }
    }
}