using System;
using EW2.Tools;
using Hellmade.Sound;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class SelectLanguageWindowProperties : WindowProperties
    {
        public string title;

        public SelectLanguageWindowProperties(string title)
        {
            this.title = title;
        }
    }

    public class SelectLanguageController : AWindowController<SelectLanguageWindowProperties>
    {
        [SerializeField] private Transform container;
        [SerializeField] private GameObject btnLanguagePrefab;
        [SerializeField] private SystemLanguage[] languageNames;

        [SerializeField] private Button btnClose;
        [SerializeField] private Text txtTitle;
        protected override void Awake()
        {
            base.Awake();
            btnClose.onClick.AddListener(() => {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
                EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
                UIFrame.Instance.CloseCurrentWindow();
            });
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();
            txtTitle.text = L.popup.language_txt;
            container.DestroyAllChildren();
            for (var i = 0; i < languageNames.Length; i++)
            {
                var btnLanguage = LeanPool.Spawn(btnLanguagePrefab, container);
                var button = btnLanguage.GetComponent<Button>();


                var text = btnLanguage.GetComponentInChildren<Text>();
                var cultureInfo = Locale.GetCultureInfoByLanguage(languageNames[i]);
                text.text = cultureInfo.NativeName;

                var localCultureInfo = Localization.Current.localCultureInfo;
                //Debug.LogAssertion("local english language name " + localCultureInfo.DisplayName);

                if (!cultureInfo.DisplayName.Equals(localCultureInfo.DisplayName))
                {
                    //Debug.LogAssertion(cultureInfo.DisplayName);
                    btnLanguage.GetComponentInChildren<Outline>().enabled = false;
                    btnLanguage.GetComponentInChildren<Shadow>().enabled = false;
                    text.color = new Color(0.08235294f, 0.2039216f, 0.3254902f);
                    btnLanguage.GetComponent<Image>().color = new Color(0.5471698f, 0.5471698f, 0.5471698f);
                    var i1 = i;
                    button.onClick.AddListener(() => ChangeLanguage(languageNames[i1]));
                }
            }
        }

        public void ChangeLanguage(SystemLanguage language)
        {
            UserData.Instance.SettingData.userLanguage = language;
            UserData.Instance.Save();
            Localization.Current.localCultureInfo = Locale.GetCultureInfoByLanguage(language);
            LoadSceneUtils.LoadScene(SceneName.Start);
            EventManager.EmitEvent(GamePlayEvent.OnChangeLanggueSuccess);
        }
    }
}
