using BestHTTP;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ZitgaLog
{
    public class ZitgaLogExample : MonoBehaviour
    {
        public string Host;

        public int Port;

        public int ApiVersion;

        public string GameVersion;

        public string SecretKey;

        public AuthProvider authProvider;
        public string authToken;

        private ZitgaLogService logService;

        public void Start()
        {
            LogOption option = new LogOption(Host, Port);

            option.ApiVersion = ApiVersion;
            option.GameVersion = GameVersion;
            option.SecretKey = SecretKey;

            option.OnTransactionLogFinished = OnTransactionLogFinished;

            logService = new ZitgaLogService(option);
            LogPurchase(null);
        }

        public void LogPurchase(PurchaseEventArgs eventArgs)
        {
            logService.LogPurchase(authProvider, authToken, eventArgs);
            Debug.Log("Log sent");
        }

        #region Callback
        private void OnTransactionLogFinished(HTTPRequest request, HTTPResponse response)
        {
            if (response == null)
            {
                Debug.LogError("Response is null");
                return;
            }

            Debug.LogError(string.Format("Response code = {0}, data = {1}", response.StatusCode, response.DataAsText));
        }
        #endregion
    }
}