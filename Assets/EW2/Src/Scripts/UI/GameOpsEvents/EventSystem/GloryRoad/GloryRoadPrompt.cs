using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class GloryRoadPrompt : MonoBehaviour, IUIPrompt
    {
        [SerializeField] private Image icon;
        public bool Status { get; set; }

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.OnRepaintGloryRoad, Notice);
        }

        private void OnDestroy()
        {
            EventManager.StopListening(GamePlayEvent.OnRepaintGloryRoad, Notice);
        }

        private void OnEnable()
        {
            Notice();
        }

        public void Notice()
        {
            var on = UserData.Instance.UserEventData.GloryRoadUser.CanClaimAnyReward();
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