using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zitga.UIFramework;

namespace EW2
{
    [Serializable]
    public class QuitGameWindowProperty : WindowProperties
    {
        public bool showTutorial;

        public QuitGameWindowProperty(bool isTutorial = false)
        {
            this.showTutorial = isTutorial;
        }
    }

    public class QuitGameWindowController : AWindowController<QuitGameWindowProperty>
    {
        [SerializeField] private Text txtContent1;

        [SerializeField] private Button btnYes1;

        [SerializeField] private Button btnNo1;

        [SerializeField] private Text txtContent2;

        [SerializeField] private Button btnYes2;

        [SerializeField] private Button btnNo2;

        [SerializeField] private GameObject contentTutorial;

        [SerializeField] private GameObject contentNormal;

        protected override void Awake()
        {
            base.Awake();

            btnYes1.onClick.AddListener(OkOnClick);

            btnNo1.onClick.AddListener(NoOnClick);

            btnYes2.onClick.AddListener(OkOnClick);

            btnNo2.onClick.AddListener(NoOnClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            txtContent1.text = L.popup.quit_game_notice;

            txtContent2.text = L.popup.quit_game_notice;

            btnYes1.GetComponentInChildren<Text>().text = L.button.btn_yes;

            btnNo1.GetComponentInChildren<Text>().text = L.button.btn_no;

            btnYes2.GetComponentInChildren<Text>().text = L.button.btn_yes;

            btnNo2.GetComponentInChildren<Text>().text = L.button.btn_no;

            this.contentNormal.SetActive(!Properties.showTutorial);

            this.contentTutorial.SetActive(Properties.showTutorial);
        }


        private void NoOnClick()
        {
            UIFrame.Instance.CloseWindow(ScreenIds.popup_quit_game);
        }

        private void OkOnClick()
        {
            UserData.Instance.Save();
            Application.Quit();
        }
    }
}