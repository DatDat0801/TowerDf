using UnityEngine;
using Zitga.ContextSystem;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class TournamentDataContainer
    {
        private const string ROOT = "CSV/TournamentMaps";
        //private const string ROOT_TEST = "CSV/MapCampaigns_";

        private readonly ServiceContainer _container;

        public TournamentDataContainer()
        {
            this._container = new ServiceContainer();
        }

        public T GetMap<T>(int tournamentMapId) where T : ScriptableObject
        {
            string key = $"{ROOT}/tournament_map_{tournamentMapId.ToString()}";

            var obj = this._container.Resolve<T>(key);
            if (obj == null)
            {
                obj = Resources.Load<T>(key);

                this._container.Register(key, obj);
            }

            return obj;
        }

        public T GetMapGoldDrop<T>(int tournamentMapId) where T : ScriptableObject
        {
            var obj = this._container.Resolve<T>($"tournament_gold_drop_map_{tournamentMapId.ToString()}");
            if (obj == null)
            {
                obj = LoadGoldDrop<T>(tournamentMapId);

                this._container.Register($"tournament_gold_drop_map_{tournamentMapId.ToString()}", obj);
            }

            return obj;
        }

        private T LoadGoldDrop<T>(int tournamentMapId) where T : ScriptableObject
        {
            string key = $"{ROOT}/tournament_gold_drop_map_{tournamentMapId.ToString()}";

            return Resources.Load<T>(key);
        }
    }
}