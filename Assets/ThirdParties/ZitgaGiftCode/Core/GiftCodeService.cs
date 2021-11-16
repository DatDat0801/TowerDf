using BestHTTP;
using Newtonsoft.Json;
using System;
using UnityEngine;
using static ZitgaGiftCode.Delegates;

namespace ZitgaGiftCode
{
    public class GiftCodeService
    {
        private readonly GiftCodeOption option;

        private readonly Uri checkUri;
        private readonly Uri claimUri;

        public GiftCodeService(GiftCodeOption option)
        {
            this.option = option;

            checkUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.CHECK_ROUTE));
            claimUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.CLAIM_ROUTE));
        }

        #region Check
        public void CheckGiftCode(GiftCodeOutbound outbound)
        {
            MakeRequest(outbound, checkUri, OnCheckFinished, option.CheckGiftCodeCallback);
        }

        private void OnCheckFinished(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeCallback(option.CheckGiftCodeCallback, LogicCode.FAIL);
                    return;
                }

                GiftCodeInbound inbound = JsonConvert.DeserializeObject<GiftCodeInbound>(response.DataAsText);
                if (inbound.Code != LogicCode.SUCCESS)
                {
                    InvokeCallback(option.CheckGiftCodeCallback, inbound.Code);
                    return;
                }

                InvokeCallback(option.CheckGiftCodeCallback, inbound.Code, inbound.Data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeCallback(option.CheckGiftCodeCallback, LogicCode.FAIL);
            }
        }
        #endregion

        #region Claim
        public void ClaimGiftCode(GiftCodeOutbound outbound)
        {
            MakeRequest(outbound, claimUri, OnClaimFinished, option.ClaimGiftCodeCallback);
        }

        private void OnClaimFinished(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeCallback(option.ClaimGiftCodeCallback, LogicCode.FAIL);
                    return;
                }

                GiftCodeInbound inbound = JsonConvert.DeserializeObject<GiftCodeInbound>(response.DataAsText);
                if (inbound.Code != LogicCode.SUCCESS)
                {
                    InvokeCallback(option.ClaimGiftCodeCallback, inbound.Code);
                    return;
                }

                InvokeCallback(option.ClaimGiftCodeCallback, inbound.Code, inbound.Data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeCallback(option.ClaimGiftCodeCallback, LogicCode.FAIL);
            }
        }
        #endregion

        #region Helper
        private void MakeRequest(GiftCodeOutbound outbound, Uri uri, OnRequestFinishedDelegate onRequestFinished, GiftCodeCallback callback)
        {
            try
            {
                if (!outbound.IsValid())
                {
                    InvokeCallback(callback, LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request = new HTTPRequest(uri, HTTPMethods.Post, false, false, onRequestFinished);

                request.AddHeader(BasicTag.API_VERSION, option.ApiVersion.ToString());

                request.AddField(BasicTag.AUTH_PROVIDER_TAG, ((int)outbound.Provider).ToString());
                request.AddField(BasicTag.AUTH_TOKEN_TAG, outbound.Token);

                request.AddField(BasicTag.GIFT_CODE_TAG, outbound.Code);
                request.AddField(BasicTag.GAME_ID_TAG, option.GameId.ToString());

                request.AddField(BasicTag.CREATE_TIME_TAG, outbound.CreateTime.ToString());

                request.AddField(BasicTag.INBOUND_HASH_TAG, outbound.CreateHash(option.SecretKey, option.GameId));

                request.Send();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeCallback(callback, LogicCode.FAIL);
            }
        }

        private void InvokeCallback(GiftCodeCallback callback, int logicCode, string data = null)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                callback(logicCode, data);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion
    }
}