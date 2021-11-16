using System;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;

namespace EW2
{
    public enum FBType
    {
        LIKE = 0,
        INVITE = 1,
        SHARE = 2,
        JOIN = 3,
        LOGIN = 4,
        LOGOUT = 5,
        NONE = 6,
    }

    public enum FacebookResultCode
    {
        Success = 0,
        Cancelled = 1,
        Error = 2,
    }

    public class FacebookService : Singleton<FacebookService>
    {
        private readonly List<string> FACEBOOK_PERMISSION = new List<string>() {"public_profile"};

        private Action<Action<FBType, FacebookResultCode>> finishInit;
        private Action<Action<FBType, FacebookResultCode>> finishLogin;

        private Action<FBType, FacebookResultCode> callback;
        private int inviteNumber;

        public string userId { get; private set; }

        private bool isOpenAnalytics = true;

        public void InitFB()
        {
#if UNITY_IOS
            userId = PlayerPrefs.GetString("user_id", string.Empty);
#endif

            if (!FB.IsInitialized)
            {
                FB.Init(OnInitFBComplete, null);
            }
            else
            {
                FB.ActivateApp();
                FB.Mobile.ShareDialogMode = ShareDialogMode.NATIVE;
            }

            // CheckLogAnalytics();
        }
//         public void CheckLogAnalytics()
//         {
//             isOpenAnalytics = true;
//             var gmail = BaseUser.Instance.UserInfoData.Gmail;
//             if (ConfigStatics.TestEmails.Contains(gmail) || !GameClient.Instance.IsValidBundle())
//             {
//                 isOpenAnalytics = false;
//             }
//             if (LockScene.Instance.IsTest)
//                 isOpenAnalytics = false;
// #if UNITY_EDITOR
//             isOpenAnalytics = false;
// #endif
//         }

        private void OnInitFBComplete()
        {
            FB.ActivateApp();
            FB.Mobile.ShareDialogMode = ShareDialogMode.NATIVE;

            if (finishInit != null)
            {
                finishInit(callback);
                finishInit = null;
            }
        }

        #region Login and logout

        public void Login(Action<FBType, FacebookResultCode> callback = null)
        {
            this.callback = callback;
            AutoLogin();
        }

        public void Logout(Action<FBType, FacebookResultCode> callback = null)
        {
            FB.LogOut();
            if (callback != null)
            {
                callback(FBType.LOGOUT, FacebookResultCode.Success);
                this.callback = null;
            }
        }

        private void AutoLogin(Action<FBType, FacebookResultCode> callback = null)
        {
            if (!FB.IsInitialized)
            {
                finishInit = AutoLogin;
                InitFB();
            }
            else
            {
                FB.LogInWithReadPermissions(FACEBOOK_PERMISSION, LoginCallback);
            }
        }

        private void LoginCallback(ILoginResult result)
        {
            if (FB.IsLoggedIn)
            {
                // AccessToken class will have session details
                var aToken = AccessToken.CurrentAccessToken;
                userId = aToken.UserId;
                foreach (string perm in aToken.Permissions)
                {
                    Log.Info(perm);
                }

                if (finishLogin != null)
                {
                    finishLogin(callback);
                    finishLogin = null;
                }

                Log.Info("[FACEBOOK] Logged In successfully: " + aToken);

#if UNITY_IOS
                PlayerPrefs.SetString("user_id", userId);
                PlayerPrefs.Save();
#endif
            }
            else
            {
                Log.Info("[FACEBOOK] User cancelled login");
            }

            if (callback != null)
            {
                callback(FBType.LOGIN, FB.IsLoggedIn ? FacebookResultCode.Success : FacebookResultCode.Error);
            }
        }

        #endregion

        // #region Like
        // public void LikeFB(Action<FBType, FacebookResultCode> callback)
        // {
        //     this.callback = callback;
        //     if (callback != null)
        //     {
        //         callback(FBType.LIKE, FacebookResultCode.Success);
        //         this.callback = null;
        //     }
        //
        //     SocialUtils.OpenFacebookFanpage();
        // }
        // #endregion
        //
        // #region Share
        // public void ShareFB(Action<FBType, FacebookResultCode> callback)
        // {
        //     this.callback = callback;
        //     if (FB.IsLoggedIn)
        //     {
        //         FB.ShareLink(new Uri(SocialUtils.SHARE_LINK), callback: ShareCallback);
        //     }
        //     else
        //     {
        //         finishLogin = ShareFB;
        //         AutoLogin();
        //     }
        // }
        //
        // private void ShareCallback(IShareResult result)
        // {
        //     FacebookResultCode isSuccess = FacebookResultCode.Error;
        //     if (result != null)
        //     {
        //         // Some platforms return the empty string instead of null.
        //         if (!string.IsNullOrEmpty(result.Error))
        //         {
        //             // error
        //             //log.text = "share error";
        //             Log.Info("share error: " + result.Error);
        //             isSuccess = FacebookResultCode.Error;
        //         }
        //         else if (result.Cancelled)
        //         {
        //             // canceled
        //             //log.text = "share canceled";
        //             Log.Info("share cancelled");
        //             isSuccess = FacebookResultCode.Cancelled;
        //         }
        //         else
        //         {
        //             // success
        //             Log.Info("share success");
        //             isSuccess = FacebookResultCode.Success;
        //         }
        //     }
        //     if (callback != null)
        //     {
        //         callback(FBType.SHARE, isSuccess);
        //         callback = null;
        //     }
        // }
        // #endregion
        //
        // #region Invite
        // public void InviteFB(Action<FBType, FacebookResultCode> callback)
        // {
        //     this.callback = callback;
        //     if (FB.IsLoggedIn)
        //     {
        //         FB.AppRequest(message: LocalizeUtils.GetText(LocalizeKey.INVITE_DIALOG_MESSAGE), title: LocalizeUtils.GetText(LocalizeKey.INVITE_DIALOG_TITLE),
        //             callback: InviteCallback);
        //     }
        //     else
        //     {
        //         finishLogin = InviteFB;
        //         AutoLogin();
        //     }
        // }
        //
        // public int GetNumberInvite()
        // {
        //     int invite = inviteNumber;
        //     inviteNumber = 0;
        //     return invite;
        // }
        //
        // private void InviteCallback(IAppRequestResult result)
        // {
        //     var inviteNumber = -1;
        //     var inviteResult = FacebookResultCode.Error;
        //     // Some platforms return the empty string instead of null.
        //     if (!string.IsNullOrEmpty(result.Error))
        //     {
        //         // error
        //         Log.Info("Invite error: " + result.Error);
        //         inviteResult = FacebookResultCode.Error;
        //     }
        //     else if (result.Cancelled)
        //     {
        //         // canceled
        //         //log.text = "invitation is canceled";
        //         Log.Info("Invite cancelled");
        //         inviteResult = FacebookResultCode.Cancelled;
        //     }
        //     else if (!string.IsNullOrEmpty(result.RawResult))
        //     {
        //         // success
        //         if (null != result.To)
        //         {
        //             inviteNumber = result.To.Count();
        //             Log.Info("Invite success: " + inviteNumber);
        //         }
        //         else
        //         {
        //             inviteNumber = 0;
        //             Log.Info("Invited 0 friend.");
        //         }
        //         inviteResult = FacebookResultCode.Success;
        //     }
        //     else
        //     {
        //         // other cases
        //         //log.text = "unknown";
        //         Log.Info("Invite unknown");
        //     }
        //     this.inviteNumber = inviteNumber < 0 ? 0 : inviteNumber;
        //     if (callback != null)
        //     {
        //         callback(FBType.INVITE, inviteResult);
        //         callback = null;
        //     }
        // }
        // #endregion
        //
        // #region Join group
        // public void JoinGroupFB(Action<FBType, FacebookResultCode> callback)
        // {
        //     this.callback = callback;
        //     if (callback != null)
        //     {
        //         callback(FBType.JOIN, FacebookResultCode.Success);
        //         this.callback = null;
        //     }
        //     SocialUtils.OpenFacebookGroup();
        // }
        // #endregion
        //
        // #region User Info
        // public void GetUserName()
        // {
        //     if (FB.IsLoggedIn)
        //     {
        //         FB.API("/me", HttpMethod.GET, GetUserInfoCallBack);
        //     }
        // }
        //
        // public void GetUserAvatar()
        // {
        //     if (FB.IsLoggedIn)
        //     {
        //         FB.API("/me/picture?type=square&height=60&width=60", HttpMethod.GET, GetUserAvatarCallBack);
        //     }
        // }
        //
        // private void GetUserInfoCallBack(IGraphResult result)
        // {
        //     if (result.Error != null)
        //     {
        //         // error
        //         Log.Error(result.Error);
        //     }
        //     else
        //     {
        //         // success
        //         Log.Error("GetUserInfo success");
        //     }
        // }
        //
        // private void GetUserAvatarCallBack(IGraphResult result)
        // {
        //     if (result.Error != null)
        //     {
        //         // error
        //         Log.Error(result.Error);
        //     }
        //     else
        //     {
        //         // success
        //         Log.Error("GetUserAvatar success");
        //     }
        // }
        // #endregion
        //
        // #region Log event
        // public void LogTutorialEvent()
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(AnalyticsConstants.ANALYTICS_EVENT_COMPLETE_TUTORIAL);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        //
        // public void LogCompleteMap(string eventName)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(eventName);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        // public void LogStartMap(string eventName)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(eventName);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        // public void LogFailMap(string eventName)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(eventName);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        // public void LogReplayMap(string eventName)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(eventName);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        //
        // public void LogReceiveDailyGift(int dailyGiftId)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(AnalyticsConstants.ANALYTICS_RECEIVE_DAILY_GIFT + "_" + (dailyGiftId + 1));
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        //
        // public void LogOpenDailyGift(int dailyGiftId)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(AnalyticsConstants.ANALYTICS_OPEN_DAILY_GIFT + "_" + (dailyGiftId + 1));
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        //
        // public void LogAppPurchase(Product purchasedProduct)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         var productId = purchasedProduct.definition.storeSpecificId;
        //         var iapParameters = new Dictionary<string, object>();
        //         iapParameters["id"] = purchasedProduct.definition.storeSpecificId;
        //         iapParameters["transaction_id"] = purchasedProduct.transactionID;
        //
        //         FB.LogPurchase((float)purchasedProduct.metadata.localizedPrice,
        //             purchasedProduct.metadata.isoCurrencyCode, iapParameters);
        //
        //         if (!string.IsNullOrEmpty(productId))
        //         {
        //             var splits = productId.Split('_');
        //             var eventName = string.Empty;
        //             for (int i = 0; i < splits.Length; i++)
        //             {
        //                 if (i >= splits.Length - 2)
        //                     eventName += splits[i];
        //             }
        //             FB.LogAppEvent(AnalyticsConstants.ANALYTICS_IAP + "_" + eventName);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        //
        // public void LogVIP(int vip)
        // {
        //     if (!isOpenAnalytics)
        //         return;
        //     try
        //     {
        //         FB.LogAppEvent(AnalyticsConstants.ANALYTICS_VIP + "_" + vip);
        //     }
        //     catch (Exception e)
        //     {
        //         Log.Error(e);
        //     }
        // }
        // #endregion
    }
}