using System;
using System.Text;
using EW2;
using UnityEngine;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif

namespace SocialTD2
{
    
    public class TDLoginGooglePlayGame : TDLogin
    {
        private Action<bool> callback;

        
        public override void Login(Action<bool> callback)
        {
            try
            {
                Log.Info("[Login] Google start login");
                if (LoadSaveUtilities. IsAuthenticated())
                {
                    Log.Info("[Login] Ignore repeated call to Authenticate().");
                    SaveData();
                    if (callback != null)
                    {
                        callback(true);
                    }
                }
                else
                {
#if UNITY_ANDROID
                    PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequestEmail().Build();

                    PlayGamesPlatform.InitializeInstance(config);
                    // recommended for debugging
                    PlayGamesPlatform.DebugLogEnabled = true;
                    // Activate the Google Play Games platform
                    PlayGamesPlatform.Activate();
                    //Ultilities.ShowToastNoti(GooglePlayGames.PlayGamesPlatform.Instance.GetServerAuthCode());
#endif
                    
                    this.callback = callback;
                    Social.localUser.Authenticate(OnGooglePlayGamesLogin);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public override string GetUserID()
        {
            string id = string.Empty;
            try
            {
                var localUser = Social.localUser;
#if UNITY_ANDROID
                localUser = Social.localUser as PlayGamesLocalUser;
#endif
                if (localUser != null)
                {
                    if (!string.IsNullOrEmpty(localUser.id))
                    {
                        id = localUser.id;
                    }
                }

#if UNITY_EDITOR
                if (string.IsNullOrEmpty(id))
                {
                    id = SystemInfo.deviceUniqueIdentifier;
                }
#endif
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return id;
        }

        public override string GetUserName()
        {
            string name = SystemInfo.deviceName;
            try
            {
                var localUser = Social.localUser;
#if UNITY_ANDROID
                localUser = Social.localUser as PlayGamesLocalUser;
#endif
                if (localUser != null)
                {
                    if (!string.IsNullOrEmpty(localUser.userName))
                    {
                        name = localUser.userName;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return name;
        }

        public override string GetAccountName()
        {
            string name = string.Empty;
            try
            {
                var localUser = Social.localUser;
#if UNITY_ANDROID
                localUser = Social.localUser as PlayGamesLocalUser;
                if (localUser != null)
                {
                    if (!string.IsNullOrEmpty((localUser as PlayGamesLocalUser).Email))
                    {
                        name = (localUser as PlayGamesLocalUser).Email;
                    }
                }
#endif
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return name;
        }


        public override string GetLoginProviderString()
        {
            return "Google Play Games";
        }

        private void OnGooglePlayGamesLogin(bool success)
        {
            try
            {
                // handle success or failure
                if (success)
                {
                    Debug.Log(string.Format("User signed in successfully: {0} ({1})", GetUserName(), GetUserID()));
                    SaveData();
                    if (callback != null)
                    {
                        callback(true);
                    }
                }
                else
                {
                    Debug.LogWarning("User Signed Failed");
                    if (callback != null)
                    {
                        callback(false);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }
    }
}