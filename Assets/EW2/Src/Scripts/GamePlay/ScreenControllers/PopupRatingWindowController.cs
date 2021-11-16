using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;

namespace EW2
{
    public class PopupRatingWindowController : AWindowController
    {
        private const int APPLE_GAME_ID = 1571236234;

        [SerializeField] private Text txtTile;

        [SerializeField] private Text txtContent;

        [SerializeField] private Button btnYes;

        [SerializeField] private Button btnNo;

        protected override void Awake()
        {
            base.Awake();

            btnYes.onClick.AddListener(OkOnClick);

            btnNo.onClick.AddListener(NoOnClick);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            txtTile.text = L.popup.rating_title.ToUpper();

            txtContent.text = L.popup.require_rating_txt;

            btnYes.GetComponentInChildren<Text>().text = L.button.rate_btn;

            this.btnNo.GetComponentInChildren<Text>().text = L.button.later_name;
        }


        private void NoOnClick()
        {
            FirebaseLogic.Instance.ButtonClick("stage", "later", UserData.Instance.AccountData.idMapShowRating);
            HandleButtonClick();
            UIFrame.Instance.CloseWindow(ScreenIds.popup_rating);
        }

        private void OkOnClick()
        {
            FirebaseLogic.Instance.ButtonClick("stage", "rate", UserData.Instance.AccountData.idMapShowRating);
            UserData.Instance.AccountData.isCompleteRating = true;
            HandleButtonClick();

            // var resultSearch = FindObjectOfType<RatingController>();
            //
            // if (resultSearch != null)
            //     resultSearch.HandleRatingCallback();

            Application.OpenURL(GetStoreLink());

            UIFrame.Instance.CloseWindow(ScreenIds.popup_rating);
        }


        private void HandleButtonClick()
        {
            var userData = UserData.Instance.AccountData;
            userData.mapCompleteShowRating.Add(userData.idMapShowRating);
            userData.idMapShowRating = -1;
            UserData.Instance.Save();
        }

        private string GetStoreLink()
        {
            var linkStore = "https://play.google.com/store/apps/details?id=" + Application.identifier;
#if UNITY_ANDROID
            linkStore = "market://details?id=" + Application.identifier;
#elif UNITY_IOS
            linkStore = "itms-apps://itunes.apple.com/app/id" + APPLE_GAME_ID;
#endif
            return linkStore;
        }
    }
}