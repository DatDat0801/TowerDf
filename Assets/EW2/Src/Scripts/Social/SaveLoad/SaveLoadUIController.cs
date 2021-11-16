using System;
using GooglePlayGames;
using Sirenix.OdinInspector;
using SocialTD2;
using TigerForge;
using UnityEngine;
using UnityEngine.UI;
using Zitga.TrackingFirebase;
using Zitga.UIFramework;
using ZitgaSaveLoad;

namespace EW2
{
    public class SaveLoadUIController : MonoBehaviour
    {
        [SerializeField] private Button loginBtn;
        [SerializeField] private Button loginDisableBtn;
        [SerializeField] private Button saveBtn;
        [SerializeField] private Button loadBtn;
        [SerializeField] private Image loginNoticeIcon;
        [SerializeField] private Image saveNoticeIcon;

        private bool loginInProgress;

        //public static event UnityAction OnLoginSuccess = delegate {  };
        private void Start()
        {
            loginBtn.onClick.AddListener(CloudSaveClick);
            saveBtn.onClick.AddListener(SaveClick);

            loadBtn.onClick.AddListener(LoadClick);
            Init();
        }

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnSaveData, OnSaveSuccess);
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnSaveData, OnSaveSuccess);
        }

        void Init()
        {
            if (!LoadSaveUtilities.IsAuthenticated())
            {
                loginDisableBtn.gameObject.SetActive(false);
                loginBtn.gameObject.SetActive(true);
                loginNoticeIcon.enabled = true;
                saveNoticeIcon.enabled = false;
                return;
            }
            else
            {
                loginBtn.gameObject.SetActive(false);
                loginDisableBtn.gameObject.SetActive(true);
                loginNoticeIcon.enabled = false;
            }

            //reset saved today to zero
            var settingData = UserData.Instance.SettingData;
            if (!settingData.lastSaved.Date.Equals(DateTime.Today.Date))
            {
                settingData.savedToday = 0;
                UserData.Instance.Save();
            }

            EmitSaveEvent();
        }

        void EmitSaveEvent()
        {
            //var settingData = UserData.Instance.SettingData;
            if (LoadSaveUtilities.IsSavedToday() == false)
            {
                EventManager.EmitEvent($"Enable_{nameof(BadgeType.SaveLoad)}");
                saveNoticeIcon.enabled = true;
            }
            else
            {
                // if (settingData.savedToday == 0)
                // {
                //     EventManager.EmitEvent($"Enable_{nameof(BadgeType.SaveLoad)}");
                //     saveNoticeIcon.enabled = true;
                // }
                // else
                // {
                //     EventManager.EmitEvent($"Disable_{nameof(BadgeType.SaveLoad)}");
                //     saveNoticeIcon.enabled = false;
                // }
                EventManager.EmitEvent($"Disable_{nameof(BadgeType.SaveLoad)}");
                saveNoticeIcon.enabled = false;
            }
        }

        //login
        private void CloudSaveClick()
        {
            //ShowComingSoon();
            if (loginInProgress == true)
            {
                return;
            }


            if (LoadSaveUtilities.IsAuthenticated())
            {
                Ultilities.ShowToastNoti("Logged in");
            }
            else
            {
#if PLATFORM_ANDROID
                var property = new PopupNoticeWindowProperties(L.popup.notice_txt, L.popup.data_save_note,
                    PopupNoticeWindowProperties.PopupType.OneOption, L.button.btn_google, OnLink);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, property);

#endif
#if UNITY_IPHONE
                var property = new PopupNoticeWindowProperties(L.popup.notice_txt, L.popup.data_save_note,
                    PopupNoticeWindowProperties.PopupType.OneOption, L.button.btn_apple, OnLink);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, property);
#endif
            }


            void OnLink()
            {
                FirebaseLogic.Instance.ButtonClick("profile", "google_save", 7);
                loginInProgress = true;

#if UNITY_EDITOR
                OnLoginSuccess();
                loginInProgress = false;
#endif

#if UNITY_IOS
                Debug.Log("Login Unity iPhone");
                TDLoginGameCenter loginGameCenter = new TDLoginGameCenter();
                loginGameCenter.Login(b => {
                    if (b)
                    {
                        loginInProgress = false;
                        OnLoginSuccess();
                        FirebaseLogic.Instance.SetPlayerId();
                    }
                    else
                    {
                        loginInProgress = false;
                        Ultilities.ShowToastNoti(L.popup.linking_failed);
                        
                    }
                });
#endif
#if PLATFORM_ANDROID && !UNITY_EDITOR
                TDLoginGooglePlayGame loginGooglePlayGame = new TDLoginGooglePlayGame();
                loginGooglePlayGame.Login(b =>
                {
                    if (b)
                    {
                        loginInProgress = false;
                        OnLoginSuccess();
                        FirebaseLogic.Instance.SetPlayerId();
                    }
                    else
                    {
                        loginInProgress = false;
                        Ultilities.ShowToastNoti(L.popup.linking_failed);

                    }
                });
#endif
            }
        }

        void OnLoginSuccess()
        {
            UIFrame.Instance.CloseCurrentWindow();
            Ultilities.ShowToastNoti(L.popup.login_successful_txt);
            EmitSaveEvent();
            var settingData = UserData.Instance.AccountData;
            //Save token generated by GPGS
            settingData.SetToken(LoadSaveUtilities.GetUserID());
#if PLATFORM_ANDROID && !UNITY_EDITOR
            settingData.SetProvider(ZitgaLog.AuthProvider.GOOGLE_PLAY_SERVICE);
#endif

#if UNITY_IOS
            settingData.SetProvider(ZitgaLog.AuthProvider.APPLE_SIGN_IN);
#endif
#if UNITY_EDITOR

            settingData.SetProvider(ZitgaLog.AuthProvider.WINDOWS_DEVICE_ID);
#endif
            UserData.Instance.Save();

            EventManager.EmitEvent(GamePlayEvent.OnLoginSuccess);
            
            Init();
            
            FirebaseLogic.Instance.SetPlayerId();
            
            Debug.Log($"SAVED TOKEN: {settingData.tokenId}, AUTH PROVIDER: {settingData.authProvider}");
        }

        [Button]
        private void LoadClick()
        {
            FirebaseLogic.Instance.ButtonClick("profile", "load", 6);
            if (!LoadSaveUtilities.IsAuthenticated())
            {
                CloudSaveClick();
            }
            else
            {
                var property = new PopupNoticeWindowProperties(L.popup.notice_txt, $"{L.popup.restore_question}",
                    PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btnr_restore, Load, L.button.btn_cancel);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, property);
            }

            void Load()
            {
                try
                {
                    Social.SaveLoad.ZitgaSaveLoad saveLoad = new Social.SaveLoad.ZitgaSaveLoad();
                    saveLoad.OnLoadResult = OnLoadResult;
                    var currentPlatform = Application.platform;

                    var userId = UserData.Instance.AccountData.tokenId;
                    //Debug.LogAssertion("TOKEN INPUT TO LOAD: " + userId);

                    switch (currentPlatform)
                    {
                        case RuntimePlatform.Android:
                            saveLoad.Load(AuthProvider.ANDROID_DEVICE, userId);
                            break;
                        case RuntimePlatform.IPhonePlayer:
                            saveLoad.Load(AuthProvider.IOS_DEVICE, userId);
                            break;
                        default:
                            saveLoad.Load(AuthProvider.WINDOWS_DEVICE, userId);
                            break;
                    }
                }
                catch (Exception e)
                {
                    Ultilities.ShowToastNoti(L.popup.restore_failed);
                }
            }

            void OnLoadResult(int code)
            {
                switch (code)
                {
                    case 0:
                        if (UserData.Instance.AccountData.isFirstOpen)
                        {
                            AppsflyerUtils.Instance.SetFirstOpen();

                            UserData.Instance.AccountData.isFirstOpen = false;

                            UserData.Instance.Save();
                        }

                        FirebaseLogic.Instance.SetPlayerId();

                        EventManager.EmitEvent(GamePlayEvent.OnLoadDataSuccess);
                        Ultilities.ShowToastNoti(L.popup.restore_complete);
                        break;
                    case 4:
                        Ultilities.ShowToastNoti(L.popup.no_data_found);
                        break;
                    default:
                        Ultilities.ShowToastNoti($"{L.popup.restore_failed}, ERROR CODE: {code}");
                        break;
                }
            }
        }

        private void SaveClick()
        {
            FirebaseLogic.Instance.ButtonClick("profile", "save", 5);
            if (!LoadSaveUtilities.IsAuthenticated())
            {
                CloudSaveClick();
                return;
            }

            if (LoadSaveUtilities.CanSyncDataToday())
            {
                int savedToday = LoadSaveUtilities.GetSavedToday();
                var str = string.Format(L.popup.save_limit, 4 - savedToday, 4);
                var property = new PopupNoticeWindowProperties(L.popup.notice_txt,
                    $"{L.popup.data_save_question}\n{str}",
                    PopupNoticeWindowProperties.PopupType.TwoOption, L.button.btn_backup, SaveClick, L.button.btn_cancel);
                UIFrame.Instance.OpenWindow(ScreenIds.popup_notice, property);
            }
            else
            {
                Ultilities.ShowToastNoti(L.popup.save_limit_notice);
            }

            void SaveClick()
            {
                //Do save
                UserData.Instance.SettingData.savedToday++;
                UserData.Instance.SettingData.lastSaved = TimeManager.NowUtc;
                UserData.Instance.Save();
                LoadSaveUtilities.AutoSave(true);
            }
        }

        private void OnSaveSuccess()
        {
            int logicCode = EventManager.GetInt(GamePlayEvent.OnSaveData);
            if (logicCode == LogicCode.SUCCESS)
            {
                Ultilities.ShowToastNoti(L.popup.save_success);
                EmitSaveEvent();
            }
            else
            {
                Ultilities.ShowToastNoti($"{L.popup.save_failed}, ERROR CODE: {logicCode}");
            }

        }
    }
}