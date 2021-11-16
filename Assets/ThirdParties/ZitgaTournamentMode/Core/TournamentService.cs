using System;
using System.Text;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;
using ZitgaRankingDefendMode;
using ZitgaSaveLoad;

namespace ZitgaTournamentMode
{
    public class TournamentService
    {
        private readonly TournamentOption _option;

        private readonly Uri _updateUri;
        private readonly Uri _getUri;
        private readonly Uri _getTournamentInfoUri;

        public TournamentService(TournamentOption option)
        {
            this._option = option;

            this._updateUri =
                new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.UPDATE_RANKING));
            this._getUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.GET_RANKING));
            this._getTournamentInfoUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port,
                Route.GET_TOURNAMENT_INFO));
        }

        #region Update Ranking

        public void UpdateRanking(UpdateRankingOutbound outbound)
        {
            try
            {
                if (!outbound.IsValid())
                {
                    InvokeUpdateCallback(LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request =
                    new HTTPRequest(this._updateUri, HTTPMethods.Post, false, false, OnUpdateFinished);

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
                this._option.UpdateRankingTournamentCallback(logicCode);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion

        #region Get Ranking

        public void Load(GetRankingOutbound outbound)
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
                RankingOutbound inbound = JsonConvert.DeserializeObject<RankingOutbound>(dataJson);

                InvokeGetCallback(response.StatusCode, inbound);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeGetCallback(LogicCode.FAIL_CLIENT);
            }
        }


        private void InvokeGetCallback(int logicCode, RankingOutbound rankingTournament = null)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                this._option.GetRankingTournamentCallback(logicCode, rankingTournament);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion

        #region GetTournamentInfo

        public void LoadTournamentInfo(GetTournamentInfoOutbound outbound)
        {
            try
            {
                if (!outbound.IsValid())
                {
                    InvokeGetCallback(LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request = new HTTPRequest(this._getTournamentInfoUri, HTTPMethods.Get, false, false,
                    OnLoadTournamentInfoFinished);

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

        private void OnLoadTournamentInfoFinished(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeGetTournamentInfoCallback(request.Response.StatusCode);
                    return;
                }

                if (request.Response.StatusCode != LogicCode.SUCCESS)
                {
                    InvokeGetTournamentInfoCallback(request.Response.StatusCode);
                    return;
                }

                var dataJson = StringCompressor.DecompressString(response.DataAsText);
                TournamentInfoOutbound inbound = JsonConvert.DeserializeObject<TournamentInfoOutbound>(dataJson);

                InvokeGetTournamentInfoCallback(response.StatusCode, inbound);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeGetTournamentInfoCallback(LogicCode.FAIL_CLIENT);
            }
        }


        private void InvokeGetTournamentInfoCallback(int logicCode, TournamentInfoOutbound tournamentInfo = null)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                this._option.GetTournamentInfoCallback(logicCode, tournamentInfo);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}