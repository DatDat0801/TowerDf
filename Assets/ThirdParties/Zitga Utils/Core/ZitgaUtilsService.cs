using BestHTTP;
using Newtonsoft.Json;
using SecurityHelper;
using System;
using UnityEngine;

namespace ZitgaUtils
{
    public class ZitgaUtilsService
    {
        private readonly ZitgaUtilsOption _option;

        private readonly Uri _locationGetUri;
        
        private readonly Uri _eventGetUri;

        public ZitgaUtilsService(ZitgaUtilsOption option)
        {
            this._option = option;

            this._locationGetUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.LOCATION_GET_ROUTE));
            
            this._eventGetUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.GET_EVENT_INFO));
        }

        #region Get Location Info
        public void GetLocationInfo()
        {
            try
            {
                HTTPRequest request = new HTTPRequest(this._locationGetUri, HTTPMethods.Get, false, false, OnGetLocationInfo);

                request.AddHeader(BasicTag.API_VERSION_TAG, this._option.ApiVersion.ToString());

                long createdTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                request.AddHeader(BasicTag.CREATED_TIME_TAG, createdTime.ToString());

                string hash = HashHelper.HashSHA256(this._option.ApiVersion + createdTime + this._option.SecretKey);
                request.AddHeader(BasicTag.HASH_TAG, hash);

                request.Send();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeCallback(LogicCode.FAIL);
            }
        }

        private void OnGetLocationInfo(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeCallback(LogicCode.FAIL);
                    return;
                }

                LocationInbound inbound = JsonConvert.DeserializeObject<LocationInbound>(response.DataAsText);
                InvokeCallback(inbound.Code, inbound);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeCallback(LogicCode.FAIL);
            }
        }

        private void InvokeCallback(int logicCode, LocationInbound inbound = null)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                this._option.GetLocationInfoCallback(logicCode, inbound);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion

        #region GetEventTime

        public void GetEventInfo(GetEventDataInbound inbound)
        {
            try
            {
                if (!inbound.IsValid())
                {
                    InvokeGetEventInfoCallback(ZitgaTournamentMode.LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request = new HTTPRequest(this._eventGetUri, HTTPMethods.Get, false, false, OnGetEventInfo);

                request.AddHeader(ZitgaSaveLoad.BasicTag.AUTH_PROVIDER_TAG, ((int)inbound.Provider).ToString());
                request.AddHeader(ZitgaSaveLoad.BasicTag.AUTH_TOKEN_TAG, inbound.Token);
                request.AddHeader(ZitgaSaveLoad.BasicTag.API_VERSION, this._option.ApiVersion.ToString());

                request.Send();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeGetEventInfoCallback(ZitgaTournamentMode.LogicCode.FAIL_CLIENT);
            }
        }

        private void OnGetEventInfo(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeGetEventInfoCallback(ZitgaTournamentMode.LogicCode.FAIL_CLIENT);
                    return;
                }

                EventTimeOutbound inbound = JsonConvert.DeserializeObject<EventTimeOutbound>(response.DataAsText);
                InvokeGetEventInfoCallback(response.StatusCode, inbound);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeGetEventInfoCallback(ZitgaTournamentMode.LogicCode.FAIL_CLIENT);
            }
        }

        private void InvokeGetEventInfoCallback(int logicCode, EventTimeOutbound inbound = null)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                this._option.GetEventDataCallback(logicCode, inbound);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}