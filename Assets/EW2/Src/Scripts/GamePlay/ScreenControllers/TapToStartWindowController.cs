using System.Collections;
using EW2.Tutorial.General;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class TapToStartWindowController : AWindowController
    {
        //[SerializeField] private GameObject bar;
        [SerializeField] private GameObject tapToStart;
        //[SerializeField] private ProgressBar progressBar;
        [SerializeField] private GameObject logo;
        [SerializeField] private Image background;
        [SerializeField] private GameObject loading;

        [SerializeField] private Button btnNews;
        [SerializeField] private Button btnLanguage;
        [SerializeField] private Button btnTapToStart;
        [SerializeField] private Button btnCheat1;
        [SerializeField] private Button btnCheat2;

        [SerializeField] private Text txtVersion;
        [SerializeField] private Text txtLanguage;
        [SerializeField] private Text txtNews;
        [SerializeField] private Text txtTapToStart;

        protected override void Awake()
        {
            btnNews.onClick.AddListener(OnNews);

            btnLanguage.onClick.AddListener(OnLanguage);

            btnTapToStart.onClick.AddListener(OnTapToStart);

            btnCheat1.onClick.AddListener(OnCheat1);
            btnCheat2.onClick.AddListener(OnCheat2);

            GameSettings.InitSoundSettings();
        }

        private void OnEnable()
        {
            txtVersion.text = $"{L.popup.version_txt} {Application.version}";

            txtTapToStart.text = L.popup.tap_to_start;

            txtNews.text = L.popup.news_txt;

            EazySoundManager.GlobalMusicVolume = UserData.Instance.SettingData.music ? 1 : 0.001f;

            EazySoundManager.PlayMusic(ResourceSoundManager.GetAudioMusic(SoundConstant.MusicMenu),
                EazySoundManager.GlobalMusicVolume, true, false);
            //update language text
            txtLanguage.text = Locale.GetCultureInfoByLanguage(UserData.Instance.SettingData.userLanguage).NativeName;
        }

        private void OnNews()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.patch_note);
        }

        private void OnLanguage()
        {
            // UIFrame.Instance.ShowPanel(ScreenIds.toast_short_noti_panel,
            //     new ToastShortNotiPanelProperties(L.common.coming_soon));
            UIFrame.Instance.OpenWindow(ScreenIds.popup_pick_language);
        }

        private void OnTapToStart()
        {
            SetState(false);

            GameLaunch.isStart = true;
            
            UIFrame.Instance.EnableCanKeyBack(false);
            
            StartCoroutine(ILoading(0, 100));
            //tracking
            FirebaseLogic.Instance.TapToStart();
        }

        protected override void OnPropertiesSet()
        {
            SetState(true);
        }

        private void SetState(bool isTapToStart)
        {
            tapToStart.SetActive(isTapToStart);
            //bar.SetActive(!isTapToStart);
            if (isTapToStart)
            {
                logo.SetActive(true);
                background.color = Color.white;
                loading.SetActive(false);
            }
            else
            {
                loading.SetActive(true);
                logo.SetActive(false);
                background.color = Color.black;
            }
        }

        IEnumerator ILoading(int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                //progressBar.SetData(i / 100.0f);

                //yield return null;
                yield return null;
            }


            //check existing user data
            var idCampaign0 = MapCampaignInfo.GetCampaignId(0, 0, 0);

            var isPlayedGame = UserData.Instance.CampaignData.GetStar(idCampaign0) > 0;

            if (isPlayedGame)
            {
                TutorialManager.Instance.ExecuteSkipTutorial();
                UIFrame.Instance.CloseCurrentWindow();
                UIFrame.Instance.OpenWindow(ScreenIds.home);
            }
            else
            {
                TutorialManager.Instance.ExecuteAutoGroupCompleteTutorial();
                if (!TutorialManager.Instance.CheckCompleteGroupTutorial(AnyTutorialConstants.GROUP_0))
                {
                    TutorialManager.Instance.ExecuteStepTutorial(AnyTutorialConstants.LOAD_MAP_0);
                }
                else
                {
                    UIFrame.Instance.CloseCurrentWindow();
                    UIFrame.Instance.OpenWindow(ScreenIds.home);
                }
            }
            
            UIFrame.Instance.EnableCanKeyBack(true);
        }

        private int countCheat = 0;

        private void OnCheat1()
        {
            if (countCheat % 2 == 0)
            {
                countCheat++;
            }
            else
            {
                countCheat = 0;
            }

            if (countCheat > 10)
            {
                var launch = FindObjectOfType<GameLaunch>();

                launch.SetCheat(true);
            }
        }

        private void OnCheat2()
        {
            if (countCheat % 2 == 1)
            {
                countCheat++;
            }
            else
            {
                countCheat = 0;
            }
        }
    }
}