using System;
using System.Text;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;
using ZitgaSaveLoad;

namespace ZitgaRankingDefendMode
{
    public class RankingDefenseModeService
    {
        private readonly RankingDefenseOption _option;

        private readonly Uri _updateUri;
        private readonly Uri _getUri;

        public RankingDefenseModeService(RankingDefenseOption option)
        {
            this._option = option;

            this._updateUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.UPDATE_RANKING));
            this._getUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.GET_RANKING));
        }

        #region Update Ranking

        public void UpdateRanking(UpdateRankingDefenseOutbound outbound)
        {
            try
            {
                if (!outbound.IsValid())
                {
                    InvokeUpdateCallback(LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request = new HTTPRequest(this._updateUri, HTTPMethods.Post, false, false, OnUpdateFinished);

                request.AddHeader(BasicTag.API_VERSION, this._option.ApiVersion.ToString());
                request.AddHeader(BasicTag.AUTH_PROVIDER_TAG, ((int)outbound.Provider).ToString());
                request.AddHeader(BasicTag.AUTH_TOKEN_TAG, outbound.Token);
                request.AddHeader(BasicTag.INBOUND_HASH_TAG, outbound.CreateHash(this._option.SecretKey));

                request.RawData = Encoding.UTF8.GetBytes(outbound.Data);

                request.Send();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeUpdateCallback(LogicCode.FAIL_CLIENT);
            }
        }

        private void OnUpdateFinished(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeUpdateCallback(request.Response.StatusCode);
                    return;
                }

                InvokeUpdateCallback(response.StatusCode);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeUpdateCallback(LogicCode.FAIL_CLIENT);
            }
        }

        private void InvokeUpdateCallback(int logicCode)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                this._option.UpdateRankingDefenseCallback(logicCode);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Get Ranking

        public void Load(GetRankingDefenseModeOutbound outbound)
        {
            try
            {
                if (!outbound.IsValid())
                {
                    InvokeGetCallback(LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request = new HTTPRequest(this._getUri, HTTPMethods.Get, false, false, OnLoadFinished);

                request.AddHeader(BasicTag.AUTH_PROVIDER_TAG, ((int)outbound.Provider).ToString());
                request.AddHeader(BasicTag.AUTH_TOKEN_TAG, outbound.Token);
                request.AddHeader(BasicTag.API_VERSION, this._option.ApiVersion.ToString());

                request.Send();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeGetCallback(LogicCode.FAIL_CLIENT);
            }
        }

        private void OnLoadFinished(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeGetCallback(request.Response.StatusCode);
                    return;
                }

                if (request.Response.StatusCode != LogicCode.SUCCESS)
                {
                    InvokeGetCallback(request.Response.StatusCode);
                    return;
                }

                var dataJson = StringCompressor.DecompressString(response.DataAsText);
                RankingDefenseOutbound inbound = JsonConvert.DeserializeObject<RankingDefenseOutbound>(dataJson);

                InvokeGetCallback(response.StatusCode, inbound);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeGetCallback(LogicCode.FAIL_CLIENT);
            }
        }


        private void InvokeGetCallback(int logicCode, RankingDefenseOutbound rankingDefense = null)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                this._option.GetRankingDefenseCallback(logicCode, rankingDefense);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}