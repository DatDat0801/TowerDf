using System;
using Coffee.UIEffects;
using Coffee.UIExtensions;
using EW2.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class AdTabUI : MonoBehaviour
    {
        [SerializeField] private Transform rewardContainer;
        [SerializeField] private Text adQuantityText;
        [SerializeField] private Text adProgressText;
        [SerializeField] private Button playAdButton;
        [SerializeField] private Text claimedText;

        public AdEntity AdEntity { get; private set; }
        public bool Available { get; private set; }

        public UnityAction OnAdClick  = delegate {  };


        public void Repaint(AdEntity adEntity, UserAdData userAdData, bool available = false)
        {
            AdEntity = adEntity;
            Available = available;
            rewardContainer.DestroyAllChildren();
            for (var i = 0; i < adEntity.rewards.Length; i++)
            {
                var rewardUi = ResourceUtils.GetRewardUi(adEntity.rewards[i].type);
                rewardUi.SetData(adEntity.rewards[i]);
                rewardUi.SetParent(rewardContainer);
            }

            if (adQuantityText != null)
            {
                adQuantityText.text = string.Format(L.popup.watch_ads_txt, adEntity.adQuantity.ToString());
            }

            if (adProgressText != null)
            {
                if (userAdData.progress < adEntity.adQuantity)
                {
                    adProgressText.text =string.Format(L.popup.checkin_progress, $"<color=#e85a15>{userAdData.progress}</color>" , adEntity.adQuantity);
                }
                else
                {
                    adProgressText.text =string.Format(L.popup.checkin_progress, userAdData.progress, adEntity.adQuantity);
                }
                
            }

            if (userAdData.claimedReward)
            {
                playAdButton.gameObject.SetActive(false);
                claimedText.gameObject.SetActive(true);
            }
            else
            {
                playAdButton.gameObject.SetActive(true);
                claimedText.gameObject.SetActive(false);
            }

            var adButtonGraphic = playAdButton.targetGraphic.GetComponent<UIEffect>();
            if (Available)
            {
                playAdButton.onClick.RemoveAllListeners();
                playAdButton.onClick.AddListener(PlayAd);
                adButtonGraphic.enabled = false;
                var claimTxt = playAdButton.GetComponentInChildren<UIEffect>();
                claimTxt.enabled = false;
                playAdButton.GetComponentInChildren<Text>().color = Color.white;
                playAdButton.GetComponentInChildren<Shadow>().enabled = true;
                playAdButton.GetComponentInChildren<Outline>().enabled = true;
            }
            else
            {
                playAdButton.onClick.RemoveAllListeners();
                playAdButton.onClick.AddListener(OnUnavailableClick);
                adButtonGraphic.enabled = true;
                var claimTxt = playAdButton.GetComponentInChildren<UIEffect>();
                claimTxt.enabled = true;
                playAdButton.GetComponentInChildren<Text>().color = new Color(0.3333333f,0.3333333f,0.3333333f);
                playAdButton.GetComponentInChildren<Shadow>().enabled = false;
                playAdButton.GetComponentInChildren<Outline>().enabled = false;
            }
        }

        private void PlayAd()
        {
            OnAdClick?.Invoke();
            VideoAdPlayer.Instance.PlayAdClick(AdEntity.placementName);
        }

        private void OnUnavailableClick()
        {
            //Ultilities.ShowToastNoti(L.popup.no_ads_available_txt);
        }
        

    }
}