using BestHTTP;
using SocialTD2;
using UnityEngine;
using UnityEngine.Purchasing;
using ZitgaLog;
using LogOption = ZitgaLog.LogOption;

namespace EW2
{
    /// <summary>
    /// Log success purchase to zitga server
    /// </summary>
    public class PurchasingLog
    {
        public string Host
        {
            get
            {
                if (GameLaunch.isCheat)
                {
                    return "35.198.248.251";
                }
                else
                {
                    return "34.87.170.169";
                }
            }
        }

        private const int PORT = 8901;

        private const int API_VERSION = 1;

        private string GameVersion
        {
            get
            {
                return Application.version;
            }
        }

        private const string SECRET_KEY = "hy25sbwGkAcxJnXg";

        public AuthProvider AuthProvider
        {
            get
            {
                var accountData = UserData.Instance.AccountData;
                return accountData.GetProvider();
            }
        }

        public string AuthToken
        {
            get
            {
                var accountData = UserData.Instance.AccountData;
                if (string.IsNullOrEmpty(accountData.tokenId))
                {
                    return accountData.userId;
                }

                return accountData.tokenId;
            }
        }

        private ZitgaLogService logService;

        public PurchasingLog()
        {
            LogOption option = new LogOption(Host, PORT);

            option.ApiVersion = API_VERSION;
            option.GameVersion = GameVersion;
            option.SecretKey = SECRET_KEY;

            option.OnTransactionLogFinished = OnTransactionLogFinished;

            logService = new ZitgaLogService(option);
        }
        
        public void LogPurchase(PurchaseEventArgs eventArgs)
        {
            logService.LogPurchase(AuthProvider, AuthToken, eventArgs);
            Debug.Log($"log sent {AuthProvider}");
        }
        
        
        #region Callback
        private void OnTransactionLogFinished(HTTPRequest request, HTTPResponse response)
        {
            if (response == null)
            {
                Debug.LogError("Response is null");
                return;
            }

            Debug.LogAssertion(string.Format("Response code = {0}, data = {1}", response.StatusCode, response.DataAsText));
        }
        #endregion
    }
}