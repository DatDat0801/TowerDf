using UnityEngine;

namespace ZitgaSaveLoad
{
    public class ZitgaSaveLoadExample : MonoBehaviour
    {
        public ZitgaSaveLoadExampleUIController uiController;

        public string Host;

        public int Port;

        public int ApiVersion;

        public string GameVersion;

        public string SecretKey;

        private SaveLoadService saveLoadService;

        public void Start()
        {
            SaveLoadOption option = new SaveLoadOption(Host, Port);

            option.ApiVersion = ApiVersion;
            option.GameVersion = GameVersion;
            option.SecretKey = SecretKey;

            option.SaveCallback = OnSave;
            option.LoadCallback = OnLoad;

            saveLoadService = new SaveLoadService(option);
        }

        #region Save
        public void Save()
        {
            SaveOutbound saveOutbound = new SaveOutbound(uiController.GetAuthProvider(), uiController.GetAuthToken());
            saveOutbound.SetData(uiController.GetData(), uiController.GetSnapshotType(), uiController.GetDescription());

            Debug.Log("Save sent");
            saveLoadService.Save(saveOutbound);
        }

        public void OnSave(int logicCode)
        {
            Debug.Log("Save: logicCode=" + logicCode);
        }
        #endregion

        #region Load
        public void Load()
        {
            LoadOutbound loadOutbound = new LoadOutbound(uiController.GetAuthProvider(), uiController.GetAuthToken());

            Debug.Log("Load sent");
            saveLoadService.Load(loadOutbound);
        }

        public void OnLoad(int logicCode, Snapshot snapshot)
        {
            Debug.Log("Load: logicCode=" + logicCode);

            if (snapshot != null)
            {
                Debug.Log("Load: playerId=" + snapshot.PlayerId);
                Debug.Log("Load: data=" + snapshot.Data);
            }
        }
        #endregion
    }
}