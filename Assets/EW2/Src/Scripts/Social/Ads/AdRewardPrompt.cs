using System;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class AdRewardPrompt : MonoBehaviour, IUIPrompt
    {
        [SerializeField] private Image icon;

        public bool Status { get; set; }

        private void Awake()
        {
            EventManager.StartListening(GamePlayEvent.VIEW_AD_VIDEO, Notice);
        }

        private void OnEnable()
        {
            if (AdRewardPrompt.CheckNotice())
            {
                EventManager.EmitEventData(GamePlayEvent.VIEW_AD_VIDEO, true);
            }
            else
            {
                EventManager.EmitEventData(GamePlayEvent.VIEW_AD_VIDEO, false);
            }
        }

        private void OnDestroy()
        {
            EventManager.StopListening(GamePlayEvent.VIEW_AD_VIDEO, Notice);
        }

        public void Notice()
        {
            var on = EventManager.GetBool(GamePlayEvent.VIEW_AD_VIDEO);
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

        public static bool CheckNotice()
        {
            var userAdWrapper = UserData.Instance.UserAdDataWrapper;
            var x =  userAdWrapper.RemoveAdUserDataNotToday();
            UserData.Instance.Save();
            int count = 0;
            //Check for ad id 0=>3
            for (var i = 0; i < userAdWrapper.userAdData.Count; i++)
            {
                if (i >= 4) break;
                if (userAdWrapper.userAdData[i].claimedReward == false)
                {
                    count++;
                }
            }

            return (count > 0 && userAdWrapper.userAdData.Count >= 4) ||(userAdWrapper.userAdData.Count < 4) ? true : false;
        }
    }
}