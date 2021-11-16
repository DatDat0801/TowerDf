using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using ZitgaSaveLoad;

namespace EW2.Social.SaveLoad
{
    public class ZitgaSaveLoad
    {
        public const string PRODUCTION_HOST = "34.87.170.169";
        private const string SANDBOX_HOST = "35.198.248.251";
        public const int PORT = 8900;
        public const int API_VERSION = 1;

        public const string SECRET_KEY = "2hKh6YWG8EXC5QB2";

        //private const string Key = "user_data";
        public bool AutoSave { get; set; }
        public UnityAction<int> OnLoadResult = delegate(int arg0) { };

        private SaveLoadService saveLoadService;

        public ZitgaSaveLoad()
        {
            SaveLoadOption option = new SaveLoadOption(GameLaunch.isCheat ? SANDBOX_HOST : PRODUCTION_HOST, PORT);

            option.ApiVersion = API_VERSION;
            option.GameVersion = Application.version;
            option.SecretKey = SECRET_KEY;

            option.SaveCallback = OnSave;
            option.LoadCallback = OnLoad;

            saveLoadService = new SaveLoadService(option);
        }

        #region Save

        public void Save(AuthProvider provider, string uniqueIdentifier)
        {
            SaveOutbound saveOutbound = new SaveOutbound(provider, uniqueIdentifier);
            var jsonSerializerSettings =
                new JsonSerializerSettings() {TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects};
            var json = JsonConvert.SerializeObject(UserData.Instance, jsonSerializerSettings);
            saveOutbound.SetData(json, SnapshotType.AUTO, "description");
            //Debug.LogAssertion(json);
            Debug.Log($"Save sent player id: {uniqueIdentifier}, data: {json}");
            saveLoadService.Save(saveOutbound);
        }

        public void OnSave(int logicCode)
        {
            Debug.Log("Save: logicCode=" + logicCode);
            EventManager.EmitEventData(GamePlayEvent.OnSaveData, logicCode);
        }

        #endregion

        #region Load

        public void Load(AuthProvider provider, string uniqueIdentifier)
        {
            LoadOutbound loadOutbound = new LoadOutbound(provider, uniqueIdentifier);

            Debug.Log("Load sent");
            saveLoadService.Load(loadOutbound);
        }

        public async void OnLoad(int logicCode, Snapshot snapshot)
        {
            Debug.Log("Load: logicCode=" + logicCode);
            OnLoadResult?.Invoke(logicCode);
            if (snapshot != null)
            {
                Debug.Log("Load: playerId=" + snapshot.PlayerId);
                Debug.Log("Load: data=" + snapshot.Data);
                if (string.IsNullOrEmpty(snapshot.Data))
                {
                    Ultilities.ShowToastNoti(L.popup.no_data_found);
                    return;
                }

                var jsonSerializerSettings =
                    new JsonSerializerSettings() {TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects};
                var userData = JsonConvert.DeserializeObject<UserData>(snapshot.Data, jsonSerializerSettings);
                UserData.Instance.SetData(userData);
                await UniTask.Yield();
                UserData.Instance.Save();
                LoadSceneUtils.LoadScene(SceneName.Start);
            }
        }

        #endregion
    }
}