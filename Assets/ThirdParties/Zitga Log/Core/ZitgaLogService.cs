using BestHTTP;
using Newtonsoft.Json;
using SecurityHelper;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ZitgaLog
{
    public class ZitgaLogService
    {
        private readonly LogOption option;

        private readonly Uri transactionLogUri;

        public ZitgaLogService(LogOption option)
        {
            this.option = option;

            transactionLogUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.TRANSACTION_LOG_ROUTE));
        }

        public bool LogPurchase(AuthProvider authProvider, string authToken, PurchaseEventArgs eventArgs)
        {
            Transaction transaction = CreateTransactionLog(authProvider, authToken, option.GameVersion, eventArgs);
            string data = JsonConvert.SerializeObject(transaction);
            data = StringCompressor.CompressString(data);

            try
            {
                HTTPRequest request = new HTTPRequest(transactionLogUri, HTTPMethods.Post, false, false, option.OnTransactionLogFinished);

                request.AddHeader(BasicTag.API_VERSION, option.ApiVersion.ToString());
                request.AddHeader(BasicTag.INBOUND_HASH_TAG, HashHelper.HashSHA256(data + option.SecretKey));

                request.RawData = Encoding.UTF8.GetBytes(data);

                request.Send();
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;
        }

        #region Helper
        private Transaction CreateTransactionLog(AuthProvider authProvider, string authToken, string gameVersion, PurchaseEventArgs eventArgs)
        {
            Transaction transaction = new Transaction();

            transaction.AuthProvider = (int)authProvider;
            transaction.AuthToken = authToken;

            transaction.TransactionId = eventArgs.purchasedProduct.transactionID;
            //transaction.TransactionId = "TransactionId";

            transaction.ProductMetadata = JsonConvert.SerializeObject(eventArgs.purchasedProduct.metadata);
            transaction.ProductDefinition = JsonConvert.SerializeObject(eventArgs.purchasedProduct.definition);

            //transaction.ProductMetadata = "ProductMetadata";
            //transaction.ProductDefinition = "ProductDefinition";

            transaction.Receipt = eventArgs.purchasedProduct.receipt;
            //transaction.Receipt = "Receipt";
            transaction.GameVersion = gameVersion;

            transaction.DeviceModel = SystemInfo.deviceModel;
            transaction.DeviceName = SystemInfo.deviceName;
            transaction.DeviceId = SystemInfo.deviceUniqueIdentifier;

            return transaction;
        }
        #endregion
    }
}