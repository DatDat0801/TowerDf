using System;
using UnityEngine;
using UnityEngine.UI;

namespace Zitga.Localization.Tutorials
{
    public class LocalizationExamples : MonoBehaviour
    {
        [SerializeField]
        private Button english;
        [SerializeField]
        private Button indonesia;
        [SerializeField]
        private Text content;

        private int score = 0;

        private Localization localization;

        void Awake ()
        {
            localization = Localization.Current;
            
            localization.localCultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
            
            InitButton();

            InitListener();
        }

        private void InitListener()
        {
            localization.CultureInfoChanged += UpdateText;
        }

        private void InitButton()
        {
            english.onClick.AddListener(OnClickEn);
            indonesia.onClick.AddListener(OnClickId);
        }

        private void OnClickEn()
        {
            localization.localCultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.English);
        }

        private void OnClickId()
        {
            localization.localCultureInfo = Locale.GetCultureInfoByLanguage(SystemLanguage.Indonesian);
        }
        
        private void UpdateText(object sender, EventArgs e)
        {
            var value = R.mail.event_arena_subject;
            score++;
            content.text = value + " " + score;
        }
    }
}