using System;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class CommunityGetRewardPrompt : MonoBehaviour, IUIPrompt
    {
        [SerializeField] private UnityEngine.UI.Image icon;
        public bool Status { get; set; }
        [SerializeField] private CommunityChannel channel;

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.CLICK_COMMUNITY, Notice);
        }

        private void OnEnable()
        {
            Notice();
        }

        private void OnDestroy()
        {
            EventManager.StopListening(GamePlayEvent.CLICK_COMMUNITY, Notice);
        }

        public void Notice()
        {
            var userData = UserData.Instance.UserEventData.CommunityEventUserData;
            var receivedReward = userData.IsReceivedReward(this.channel);
            //var on = EventManager.GetBool(GamePlayEvent.DAILY_CHECKIN);
            if (!receivedReward)
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