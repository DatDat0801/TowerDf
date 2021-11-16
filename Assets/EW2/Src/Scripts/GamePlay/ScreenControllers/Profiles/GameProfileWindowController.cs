using System;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.Localization;
using Zitga.UIFramework;

namespace EW2
{
    public class GameProfileWindowController : AWindowController
    {
        #region Label

        [SerializeField] private Text txtTitle;

        [SerializeField] private Text txtTitleCreate;

        [SerializeField] private Text txtDateCreate;

        [SerializeField] private Text txtName;

        [SerializeField] private Text txtId;

        [SerializeField] private Text txtVersion;

        [SerializeField] private Text txtCountry;

        [SerializeField] private Text txtTitleDataSave;

        [SerializeField] private Text txtTitleLink;

        [SerializeField] private Text txtTitleDateSave;

        [SerializeField] private Text txtLink;

        [SerializeField] private Text txtDateSave;

        [SerializeField] private Text txtCloudSave;

        [SerializeField] private Text txtCloudSaveDisable;

        [SerializeField] private Text txtSave;

        [SerializeField] private Text txtLoad;

        [SerializeField] private Text txtMoreGame;

        [SerializeField] private Text txtSupport;

        [SerializeField] private Text txtCommunity;

        [SerializeField] private Text txtGiftcode;

        [SerializeField] private Text txtPatchNote;

        [SerializeField] private Text txtSetting;

        #endregion

        #region Button

        [SerializeField] private Button btnEditName;

        [SerializeField] private Button btnChangeAvatar;

        [SerializeField] private Button btnCopyId;

        [SerializeField] private Button btnLanggue;

        // [SerializeField] private Button btnCloudSave;
        //
        // [SerializeField] private Button btnCloudSaveDisable;
        //
        // [SerializeField] private Button btnSave;
        //
        // [SerializeField] private Button btnLoad;

        [SerializeField] private Button btnMoreGame;

        [SerializeField] private Button btnSupport;

        [SerializeField] private Button btnCommunity;

        [SerializeField] private Button btnGiftcode;

        [SerializeField] private Button btnPatchNote;

        [SerializeField] private Button btnSetting;

        [SerializeField] private Button btnClose;

        #endregion

        [SerializeField] private Image avatarIcon;

        protected override void Awake()
        {
            base.Awake();

            SetButtonListen();
        }

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnChangeAvatarSuccess, ChangedAvatarSuccess);

            EventManager.StartListening(GamePlayEvent.OnChangeNameSuccess, ChangedNameSeccess);

            EventManager.StartListening(GamePlayEvent.OnLoginSuccess, OnLoginSuccess);
            //update language text
            txtCountry.text = Locale.GetCultureInfoByLanguage(UserData.Instance.SettingData.userLanguage).NativeName;
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnChangeAvatarSuccess, ChangedAvatarSuccess);

            EventManager.StopListening(GamePlayEvent.OnChangeNameSuccess, ChangedNameSeccess);

            EventManager.StopListening(GamePlayEvent.OnLoginSuccess, OnLoginSuccess);
        }

        protected override void OnPropertiesSet()
        {
            base.OnPropertiesSet();

            SetLabel();

            avatarIcon.sprite = ResourceUtils.GetSpriteAvatar(UserData.Instance.AccountData.avatarId);
        }

        void OnLoginSuccess()
        {
            txtId.text = $"{L.popup.id_txt}{UserData.Instance.AccountData.userId}";
        }

        private void SetLabel()
        {
            txtTitle.text = L.popup.profile_popup;

            txtTitleCreate.text = L.popup.since_txt;

            txtDateCreate.text =
                TimeManager.FormatToDDMMYYYY(UserData.Instance.AccountData.accountCreated.ToLocalTime());

            txtName.text = UserData.Instance.AccountData.userName;

            txtId.text = L.popup.id_txt + UserData.Instance.AccountData.userId;

            txtVersion.text = $"{L.popup.version_txt} {Application.version}";

            txtTitleDataSave.text = L.popup.data_save;

            txtTitleDateSave.text = L.popup.last_save;

            txtTitleLink.text = L.popup.link_info;
            var lastSaved = UserData.Instance.SettingData.lastSaved;
            if (lastSaved == default || lastSaved.Year < 2021)
            {
                txtDateSave.text = TimeManager.FormatToDDMMYYYYHHMM(TimeManager.NowUtc.ToLocalTime());
            }
            else
            {
                txtDateSave.text = TimeManager.FormatToDDMMYYYYHHMM(lastSaved.ToLocalTime());
            }

            //TimeManager.Instance.FormatToDDMMYYYYHHMM(TimeManager.Instance.Now);

            txtCloudSave.text = L.button.btn_cloud_save;

            txtCloudSaveDisable.text = L.button.btn_cloud_save;

            txtSave.text = L.button.btn_backup;

            txtLoad.text = L.button.btnr_restore;

            txtMoreGame.text = L.popup.more_game_txt;

            txtSupport.text = L.popup.support_txt;

            txtCommunity.text = L.popup.community_txt;

            txtGiftcode.text = L.popup.giftcode_txt;

            txtPatchNote.text = L.popup.patch_note_txt;

            txtSetting.text = L.popup.setting_txt;

            SetLinkAccountText();
        }

        void SetLinkAccountText()
        {
            var currentPlatform = Application.platform;

            switch (currentPlatform)
            {
                case RuntimePlatform.Android:
                    txtLink.text = "Google Play Game Service";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    txtLink.text = "Game Center";
                    break;
                default:
                    txtLink.text = "Unity Editor";
                    break;
            }
        }

        private void SetButtonListen()
        {
            btnChangeAvatar.onClick.AddListener(ChangeAvatarClick);

            btnEditName.onClick.AddListener(EditNameClick);

            btnClose.onClick.AddListener(CloseClick);

            btnCopyId.onClick.AddListener(CopyIdClick);

            btnLanggue.onClick.AddListener(ChangeLanggueClick);


            //btnCloudSaveDisable.onClick.AddListener(CloudSaveClickDisable);


            btnMoreGame.onClick.AddListener(MoreGameClick);

            btnSupport.onClick.AddListener(SupportClick);

            btnCommunity.onClick.AddListener(CommunityClick);

            btnGiftcode.onClick.AddListener(GiftcodeClick);

            btnPatchNote.onClick.AddListener(PatchNoteClick);

            btnSetting.onClick.AddListener(SettingClick);
        }

        #region Event Button

        // private void CloudSaveClickDisable()
        // {
        //     ShowComingSoon();
        // }

        private void SettingClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_setting);
        }

        private void PatchNoteClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.patch_note);
        }

        private void GiftcodeClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_giftcode);
        }

        private void CommunityClick()
        {
             //EventManager.EmitEvent(GamePlayEvent.OnJointFanpage);
            // Application.OpenURL(GameConfig.LinkCommunity);
            UIFrame.Instance.OpenWindow(ScreenIds.comunity_popup);
        }

        private void SupportClick()
        {
            Application.OpenURL(GameConfig.LinkSupport);
        }

        private void MoreGameClick()
        {
            ShowComingSoon();
        }


        private void ChangeLanggueClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_pick_language);
        }

        private void CopyIdClick()
        {
            UniClipboard.SetText(UserData.Instance.AccountData.userId);
            Ultilities.ShowToastNoti(L.popup.copy_to_clipboard);
        }

        private void CloseClick()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.POPUP_CLOSE);
            EazySoundManager.PlaySound(audioClip, EazySoundManager.GlobalSoundsVolume);
            UIFrame.Instance.CloseWindow(ScreenIds.popup_profile);
        }

        private void EditNameClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_change_name);
        }

        private void ChangeAvatarClick()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_change_avatar);
        }

        #endregion

        #region Listen Event Game

        private void ChangedNameSeccess()
        {
            txtName.text = UserData.Instance.AccountData.userName;
        }

        private void ChangedAvatarSuccess()
        {
            avatarIcon.sprite = ResourceUtils.GetSpriteAvatar(UserData.Instance.AccountData.avatarId);
        }

        #endregion


        private void ShowComingSoon()
        {
            Debug.Log("Show Coming Soon");
            EventManager.EmitEventData(GamePlayEvent.ShortNoti, L.common.coming_soon);
        }
    }
}