using System;
using UnityEngine;

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
namespace SocialTD2
{
    public class TDLoginGameCenter : TDLogin
    {
        private Action<bool> callback;

        // public override bool IsAuthenticated()
        // {
        //     return Social.localUser.authenticated;
        // }

        public override void Login(Action<bool> callback)
        {
            try
            {
                if (LoadSaveUtilities.IsAuthenticated())
                {
                    SaveData();
                    if (callback != null)
                    {
                        callback(true);
                    }
                }
                else
                {
                    this.callback = callback;
                    Social.localUser.Authenticate(OnGameCenterLogin);
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
                if (!string.IsNullOrEmpty(Social.localUser.userName))
                {
                    name = Social.localUser.userName;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            return name;
        }


        public override string GetLoginProviderString()
        {
            return "Game Center";
        }

        private void OnGameCenterLogin(bool success)
        {
            try
            {
                if (success)
                {
                    Log.Info(string.Format("User signed in successfully: {0} ({1})", GetUserName(), GetUserID()));
                    SaveData();
                    if (callback != null)
                    {
                        callback(true);
                    }
                }
                else
                {
                    Log.Warning("User Signed Failed");
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

        public override string GetAccountName()
        {
            return string.Empty;
        }
    }
}