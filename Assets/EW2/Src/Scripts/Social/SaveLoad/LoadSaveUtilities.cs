using System;
using EW2;
using GooglePlayGames;
using UnityEngine;
using ZitgaSaveLoad;

namespace SocialTD2
{
    public sealed class LoadSaveUtilities
    {
        public static bool IsSavedToday()
        {
            var settingData = UserData.Instance.SettingData;
            if (!settingData.lastSaved.Date.Equals(DateTime.Today.Date) || settingData.savedToday == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int GetSavedToday()
        {
            var setting = UserData.Instance.SettingData;
            if (!setting.lastSaved.Date.Equals(DateTime.Today.Date))
            {
                UserData.Instance.SettingData.savedToday = 0;
                UserData.Instance.Save();
                return 0;
            }
            else
            {
                return setting.savedToday;
            }
        }

        public static bool IsAuthenticated()
        {
            var accountData = UserData.Instance.AccountData;
            var token = accountData.tokenId; //;PlayerPrefs.GetString("token");

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            else
            {
                return true;
            }

            //return Social.localUser.authenticated;
        }

        /// <summary>
        /// user can sync data to server 4 times/day
        /// </summary>
        public static bool CanSyncDataToday()
        {
            if (GameLaunch.isCheat) return true;
            
            var setting = UserData.Instance.SettingData;
            if (DateTime.Today.Date.Equals(setting.lastSaved.Date))
            {
                if (setting.savedToday < 4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetUserID()
        {
            string id = string.Empty;
            try
            {
                var localUser = Social.localUser;
#if UNITY_ANDROID && !UNITY_EDITOR
                localUser = Social.localUser as PlayGamesLocalUser;

                if (localUser != null)
                {
                    if (!string.IsNullOrEmpty(localUser.id))
                    {
                        Debug.Log("User id: " + localUser.id);
                        id = localUser.id;
                    }
                }
#elif UNITY_IOS && !UNITY_EDITOR
                localUser = Social.Active.localUser;
                
                if (localUser != null)
                {
                    if (!string.IsNullOrEmpty(localUser.id))
                    {
                        Debug.Log("User id: " + localUser.id);
                        id = localUser.id;
                    }
                }
#endif
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(id))
                {
                    id = SystemInfo.deviceUniqueIdentifier;
                }
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return id;
        }

        public static bool AutoSave(bool showNotifyOnSuccess)
        {
            if (!IsAuthenticated())
            {
                Debug.LogError("Please log in first before auto save");
                return false;
            }

            EW2.Social.SaveLoad.ZitgaSaveLoad saveLoad =
                new EW2.Social.SaveLoad.ZitgaSaveLoad() {AutoSave = showNotifyOnSuccess};
            var currentPlatform = Application.platform;
            var userIdFromGPGS = GetUserID();
            if (string.IsNullOrEmpty(userIdFromGPGS) || userIdFromGPGS.Equals("0"))
            {
                //Ultilities.ShowToastNoti(L.popup.save_failed);
                var accountData = UserData.Instance.AccountData;
                var token = accountData.tokenId;
                userIdFromGPGS = accountData.userId;
                //return false;
            }

            switch (currentPlatform)
            {
                case RuntimePlatform.Android:
                    saveLoad.Save(AuthProvider.ANDROID_DEVICE, userIdFromGPGS);
                    break;
                case RuntimePlatform.IPhonePlayer:
                    saveLoad.Save(AuthProvider.IOS_DEVICE, userIdFromGPGS);
                    break;
                default:
                    saveLoad.Save(AuthProvider.WINDOWS_DEVICE, userIdFromGPGS);
                    break;
            }

            return true;
        }
    }
}