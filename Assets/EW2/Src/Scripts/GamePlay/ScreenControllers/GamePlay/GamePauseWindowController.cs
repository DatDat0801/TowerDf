using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class GamePauseWindowController : AWindowController
    {
        private enum ButtonState
        {
            Close,
            Replay,
            Home,
            Surrender
        }

        [SerializeField] private Button btnClose;

        [SerializeField] private Button btnReplay;

        [SerializeField] private Button btnHome;

        [SerializeField] private Button btnSurrender;

        [SerializeField] private Text lbReplay;

        [SerializeField] private Text lbHome;

        [SerializeField] private Text lbSurrender;

        [SerializeField] private Text lbTitle;

        [SerializeField] private Text lbGraphic;

        private ButtonState buttonState;

        protected override void Awake()
        {
            base.Awake();

            btnClose.onClick.AddListener(OnClose);

            btnReplay.onClick.AddListener(OnReplay);

            btnHome.onClick.AddListener(OnHome);

            btnSurrender.onClick.AddListener(OnSurrender);

            OutTransitionFinished += controller => {
                switch (buttonState)
                {
                    case ButtonState.Home:
                    case ButtonState.Replay:
                    case ButtonState.Surrender:
                        Time.timeScale = 1;
                        break;
                    case ButtonState.Close:
                        Time.timeScale = GamePlayController.Instance.Speed;
                        GamePlayController.Instance.IsPause = false;
                        break;
                }
            };
        }

        public override void SetLocalization()
        {
            lbTitle.text = L.popup.setting_txt;

            lbGraphic.text = L.popup.graphic_txt;

            lbReplay.text = L.button.btn_replay.ToUpper();

            lbHome.text = L.button.home_btn.ToUpper();

            this.lbSurrender.text = L.button.surrender_btn.ToUpper();
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            SetLocalization();
            SwitchButton();
        }

        private void SwitchButton()
        {
            if (GamePlayControllerBase.gameMode == GameMode.TournamentMode)
            {
                this.btnSurrender.gameObject.SetActive(true);
                this.btnHome.gameObject.SetActive(false);
                this.btnReplay.gameObject.SetActive(false);
            }
            else
            {
                this.btnSurrender.gameObject.SetActive(false);
                this.btnHome.gameObject.SetActive(true);
                this.btnReplay.gameObject.SetActive(true);
            }
        }

        private void OnClose()
        {
            buttonState = ButtonState.Close;
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseCurrentWindow();
        }

        private void OnHome()
        {
            FirebaseLogic.Instance.ButtonClick("stage", "home", GamePlayController.Instance.MapId);
            buttonState = ButtonState.Home;

            GamePlayController.playMode = PlayMode.None;

            var property = new PopupNoticeWindowProperties(L.popup.warning_txt, $"{L.popup.quit_notice}",
                PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btn_yes, GoHome, L.button.btn_no, null,
                false);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, property);

            void GoHome()
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.CONFIRM);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                LoadSceneUtils.LoadScene(SceneName.Start);
            }
        }

        private void OnReplay()
        {
            FirebaseLogic.Instance.ButtonClick("stage", "replay", GamePlayController.Instance.MapId);
            buttonState = ButtonState.Replay;

            GamePlayController.playMode = PlayMode.ReplayCampaign;

            var property = new PopupNoticeWindowProperties(L.popup.warning_txt, $"{L.popup.replay_notice}",
                PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btn_yes, Replay, L.button.btn_no, null,
                false);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, property);


            void Replay()
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.CONFIRM);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                if (GamePlayController.gameMode == GameMode.CampaignMode)
                {
                    LoadSceneUtils.LoadScene(SceneName.Start);
                }
                else if (GamePlayController.gameMode == GameMode.DefenseMode)
                {
                    LoadSceneUtils.LoadScene(SceneName.Start,
                        () => { UIFrame.Instance.OpenWindow(ScreenIds.hero_defend_mode_scene); });
                }
            }
        }

        private void OnSurrender()
        {
            FirebaseLogic.Instance.ButtonClick("tournament", "surrender",
                UserData.Instance.TournamentData.currentMapId);
            buttonState = ButtonState.Surrender;

            GamePlayController.playMode = PlayMode.None;

            var property = new PopupNoticeWindowProperties(L.popup.warning_txt, $"{L.popup.surrender_notice_txt}",
                PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btn_yes, Surrender, L.button.btn_no, null,
                false);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, property);


            void Surrender()
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.CONFIRM);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                LoadSceneUtils.LoadScene(SceneName.Start,
                    () => { UIFrame.Instance.OpenWindow(ScreenIds.tournament_lobby); });
            }
        }
    }
}