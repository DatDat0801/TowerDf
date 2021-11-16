using System;
using EW2;
using UnityEngine;
//using Data;

namespace SocialTD2
{
    public abstract class TDLogin
    {

        public abstract void Login(Action<bool> callback);

        public abstract string GetUserID();

        public abstract string GetUserName();
        public abstract string GetAccountName();

        /// <summary>
        /// login methods guest=0, google play=1, game center=2, facebook=3
        /// </summary>
        //public abstract int GetLoginProvider();

        public abstract string GetLoginProviderString();

        protected void SaveData()
        {
            // var info = UserData.Instance.AccountData;//BaseUser.Instance.UserInfoData;
            // info.token_id = SocialManager.Instance.Logins.GetUserID();
            // info.auth_provider = SocialManager.Instance.Logins.GetLoginProvider();
            // info.google_name = SocialManager.Instance.Logins.GetUserName();
            //
            // var email = SocialManager.Instance.Logins.GetAccountName();
            //
            // Log.Info("[Login] email: " + email);
            // if (info.CheckAccountOldName(email))
            // {
            //     info.SetAccountOldName(info.Gmail);
            // }
            // else
            // {
            //     info.SetAccountOldName(email);
            // }
            // info.SetAccountName(email);
            // info.AddStackSaveData();
        }
    }


}