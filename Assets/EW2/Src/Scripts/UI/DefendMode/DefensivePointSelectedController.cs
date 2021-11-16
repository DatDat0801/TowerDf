using System;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    public class DefensivePointSelectedController : MonoBehaviour
    {
        private const int DEFENSIVE_POINT_ID_DEFAULT = 8001;

        [SerializeField] public Button changeBtn;
        [SerializeField] public Image defensivePointAvatar;
        [SerializeField] public Text txtNumbTrialTitle;
        [SerializeField] public Text txtNameDefensivePoint;
        [SerializeField] public Text txtDescDefensivePoint;
        [SerializeField] public Text txtStoryDefensivePoint;

        private void Awake()
        {
            this.changeBtn.onClick.AddListener(ChangeClick);
            EventManager.StartListening(GamePlayEvent.OnRefreshDefensePoint, RefreshUi);
        }

        private void RefreshUi()
        {
            var userData = UserData.Instance.UserHeroDefenseData;
            var defensivePointSelected = userData.defensePointId;
            this.defensivePointAvatar.sprite =
                ResourceUtils.GetSpriteAtlas("defensive_point", $"defensive_point_{defensivePointSelected}_card");

            this.txtNumbTrialTitle.text = string.Format(L.playable_mode.trial_left_txt, userData.numberTrial);

            this.txtNameDefensivePoint.text =
                Localization.Current.Get("playable_mode", $"defensive_point_name_{defensivePointSelected}");

            this.txtDescDefensivePoint.text =
                Localization.Current.Get("playable_mode", $"defensive_point_skill_{defensivePointSelected}");

            this.txtStoryDefensivePoint.text =
                Localization.Current.Get("playable_mode", $"defensive_point_des_{defensivePointSelected}");
        }

        public void ShowInfo()
        {
            //this.changeBtn.GetComponentInChildren<Text>().text = L.button.change_btn;
            var userData = UserData.Instance.UserHeroDefenseData;
            var defensivePointSelected = userData.defensePointId;
            var numbTrialRemain = userData.numberTrial;

            if (defensivePointSelected <= 0 ||
                (numbTrialRemain <= 0 && !userData.CheckDefensePointUnlocked(defensivePointSelected)))
            {
                defensivePointSelected = DEFENSIVE_POINT_ID_DEFAULT;
                userData.defensePointId = defensivePointSelected;
                UserData.Instance.Save();
            }

            this.defensivePointAvatar.sprite =
                ResourceUtils.GetSpriteAtlas("defensive_point", $"defensive_point_{defensivePointSelected}_card");

            this.txtNumbTrialTitle.text = string.Format(L.playable_mode.trial_left_txt, numbTrialRemain);

            this.txtNameDefensivePoint.text =
                Localization.Current.Get("playable_mode", $"defensive_point_name_{defensivePointSelected}");

            this.txtDescDefensivePoint.text =
                Localization.Current.Get("playable_mode", $"defensive_point_skill_{defensivePointSelected}");

            this.txtStoryDefensivePoint.text =
                Localization.Current.Get("playable_mode", $"defensive_point_des_{defensivePointSelected}");
        }

        private void ChangeClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_select_defend_point);
        }
    }
}