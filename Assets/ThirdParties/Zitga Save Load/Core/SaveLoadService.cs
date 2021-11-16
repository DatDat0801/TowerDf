using BestHTTP;
using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

namespace ZitgaSaveLoad
{
    public class SaveLoadService
    {
        private readonly SaveLoadOption option;

        private readonly Uri saveUri;
        private readonly Uri loadUri;

        public SaveLoadService(SaveLoadOption option)
        {
            this.option = option;

            saveUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.SAVE_ROUTE));
            loadUri = new Uri(string.Format("http://{0}:{1}/{2}", option.Host, option.Port, Route.LOAD_ROUTE));
        }

        #region Save
        public void Save(SaveOutbound outbound)
        {
            try
            {
                if (!outbound.IsValid())
                {
                    InvokeSaveCallback(LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request = new HTTPRequest(saveUri, HTTPMethods.Post, false, false, OnSaveFinished);

                request.AddHeader(BasicTag.API_VERSION, option.ApiVersion.ToString());
                request.AddHeader(BasicTag.AUTH_PROVIDER_TAG, ((int)outbound.Provider).ToString());
                request.AddHeader(BasicTag.AUTH_TOKEN_TAG, outbound.Token);

                SaveMetadata metadata = outbound.CreateSaveMetadata(option.SecretKey, option.GameVersion);
                string metadataString = JsonConvert.SerializeObject(metadata);
                metadataString = StringCompressor.CompressString(metadataString);

                request.AddHeader(BasicTag.METADATA_TAG, metadataString);

                request.RawData = Encoding.UTF8.GetBytes(outbound.Data);

                request.Send();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeSaveCallback(LogicCode.FAIL);
            }
        }

        private void OnSaveFinished(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeSaveCallback(LogicCode.FAIL);
                    return;
                }

                SaveLoadInbound inbound = JsonConvert.DeserializeObject<SaveLoadInbound>(response.DataAsText);
                InvokeSaveCallback(inbound.Code);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeSaveCallback(LogicCode.FAIL);
            }
        }

        private void InvokeSaveCallback(int logicCode)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                option.SaveCallback(logicCode);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion

        #region Load
        public void Load(LoadOutbound outbound)
        {
            try
            {
                if (!outbound.IsValid())
                {
                    InvokeLoadCallback(LogicCode.INVALID_INPUT_DATA);
                    return;
                }

                HTTPRequest request = new HTTPRequest(loadUri, HTTPMethods.Post, false, false, OnLoadFinished);

                request.AddHeader(BasicTag.API_VERSION, option.ApiVersion.ToString());
                request.AddHeader(BasicTag.AUTH_PROVIDER_TAG, ((int)outbound.Provider).ToString());
                request.AddHeader(BasicTag.AUTH_TOKEN_TAG, outbound.Token.ToString());

                request.AddHeader(BasicTag.GAME_VERSION_TAG, option.GameVersion);

                request.AddHeader(BasicTag.CREATED_TIME_TAG, outbound.CreatedTime.ToString());

                request.AddHeader(BasicTag.INBOUND_HASH_TAG, outbound.CreateHash(option.SecretKey, option.GameVersion));

                request.Send();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeLoadCallback(LogicCode.FAIL);
            }
        }

        private void OnLoadFinished(HTTPRequest request, HTTPResponse response)
        {
            try
            {
                if (response == null)
                {
                    InvokeLoadCallback(LogicCode.FAIL);
                    return;
                }

                SaveLoadInbound inbound = JsonConvert.DeserializeObject<SaveLoadInbound>(response.DataAsText);
                if (inbound.Code != LogicCode.SUCCESS)
                {
                    InvokeLoadCallback(inbound.Code);
                    return;
                }

                Snapshot snapshot = new Snapshot();
                snapshot.PlayerId = inbound.PlayerId;
                snapshot.Data = StringCompressor.DecompressString(inbound.Data);

                InvokeLoadCallback(inbound.Code, snapshot);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                InvokeLoadCallback(LogicCode.FAIL);
            }
        }

        private void InvokeLoadCallback(int logicCode, Snapshot snapshot = null)
        {
            // surround with try/catch to make sure callback is only called once
            try
            {
                option.LoadCallback(logicCode, snapshot);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion
    }
}