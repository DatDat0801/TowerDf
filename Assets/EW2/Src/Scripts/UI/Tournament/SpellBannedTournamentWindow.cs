using System;
using Hellmade.Sound;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class SpellBanTournamentWindowProperties : WindowProperties
    {
        public int heroEquipped;
        public UnityAction playTournament;
        public SpellBanTournamentWindowProperties(int heroId, UnityAction playTournament)
        {
            this.heroEquipped = heroId;
            this.playTournament = playTournament;
        }
    }
    public class SpellBannedTournamentWindow : AWindowController<SpellBanTournamentWindowProperties>
    {
        [SerializeField] private Button continueBtn;
        [SerializeField] private Button changeSpellBtn;
        [SerializeField] private Button closeBtn;
        [SerializeField] private UnitViewSpellUi spellItem;
        [SerializeField] private Text titleText;
        [SerializeField] private Text infoText;
        [SerializeField] private Text dontShowAgainText;
        [SerializeField] private Text continueText;
        [SerializeField] private Text changSpellText;

        [SerializeField] private Toggle checkbox;
        
        public const string TOURNAMENT_SPELL_BANNED_DONT_ASK_TODAY = "SpellBannedTournament";
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            SetLocalization();
            SetSpellBanned();
        }

        void SetSpellBanned()
        {
            var bannedSpellId = UserData.Instance.TournamentData.spellBanId;
            this.spellItem.ShowUi(OptionShowInfoTournament.Ban, bannedSpellId);
        }
        protected override void AddListeners()
        {
            this.closeBtn.onClick.AddListener(CloseClick);
            this.continueBtn.onClick.AddListener(ContinueClick);
            this.changeSpellBtn.onClick.AddListener(ChangSpellClick);
        }

        private void ChangSpellClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_tournament_spell_banned);
            UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene, new HeroRoomWindowProperties(Properties.heroEquipped));
            DontAskAgainToday();
        }

        private void ContinueClick()
        {
            Properties.playTournament();
            DontAskAgainToday();
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
            DontAskAgainToday();
        }

        public override void SetLocalization()
        {
            if (this.titleText)
            {
                this.titleText.text = L.playable_mode.ban_spell_notice_txt;
            }

            if (this.infoText)
            {
                this.infoText.text = L.playable_mode.ban_alert_txt;
            }

            if (this.dontShowAgainText)
            {
                this.dontShowAgainText.text = L.popup.dont_ask_again_today;
            }

            if (this.continueText)
            {
                this.continueText.text = L.button.btn_continue;
            }

            if (this.changSpellText)
            {
                this.changSpellText.text = L.button.change_btn;
            }
        }
        private void DontAskAgainToday()
        {
            if (checkbox.isOn)
            {
                var ask = new DontAskToday(){feature = TOURNAMENT_SPELL_BANNED_DONT_ASK_TODAY, lastAskTime = TimeManager.NowUtc};
                var json = JsonConvert.SerializeObject(ask);
                PlayerPrefs.SetString(TOURNAMENT_SPELL_BANNED_DONT_ASK_TODAY, json);
            }
        }
        
    }
}