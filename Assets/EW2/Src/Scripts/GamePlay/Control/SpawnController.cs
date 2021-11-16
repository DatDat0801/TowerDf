using Invoke;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class SpawnController : SpawnControllerBase
    {

        private bool IsEndSpawn { get; set; }


        private void Start()
        {
            CountEnemy = 0;

            IsEndSpawn = false;

            EventManager.StartListening(GamePlayEvent.OnEnemyDie, OnEnemyDie);
            EventManager.StartListening(GamePlayEvent.OnEndSpawn, OnEndSpawn);
        }

        protected void OnDestroy()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
            EventManager.StopListening(GamePlayEvent.OnEnemyDie, OnEnemyDie);
            EventManager.StopListening(GamePlayEvent.OnEndSpawn, OnEndSpawn);
        }

        private void OnEndSpawn()
        {
            //Debug.Log("End Spawn");

            IsEndSpawn = true;
        }

        private void OnEnemyDie()
        {
            CountEnemy -= 1;
            // Debug.LogAssertion("Count enemy: " + CountEnemy + " State: " + GamePlayController.Instance.State +
            //                    " is end spawn: " + IsEndSpawn);

            var enemyId = EventManager.GetInt(GamePlayEvent.OnEnemyDie);
            RemoveEnemyToMapCheck(enemyId);

            if (CheckClearAllEnemy() && GamePlayController.Instance.State != GamePlayState.End && IsEndSpawn)
            {
                EventManager.EmitEventData(GamePlayEvent.OnEndGame, true);
            }
        }

        private bool CheckClearAllEnemy()
        {
            var isClearAll = true;
            foreach (var numberEvemy in _mapCheckEnemy.Values)
            {
                if (numberEvemy > 0)
                {
                    isClearAll = false;
                    break;
                }
            }

            return isClearAll;
        }

    }
}