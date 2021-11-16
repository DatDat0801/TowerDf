using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

namespace EW2
{
    public class TreasureHouseController : MonoBehaviour
    {
        [SerializeField] private Transform[] posTargets;

        private TreasureSoldier6049Data.DataTreasureSoldier treasureSoldierData;

        private Coroutine coroutineCooldown;

        private TreasureSoldierController soldier;

        private void OnEnable()
        {
            EventManager.StartListening(GamePlayEvent.OnStartGame, StartRun);
        }

        private void OnDisable()
        {
            EventManager.StopListening(GamePlayEvent.OnStartGame, StartRun);
        }

        private void StartRun()
        {
            GetData();

            coroutineCooldown = CoroutineUtils.Instance.StartCoroutine(CooldownAttack());
        }

        private void GetData()
        {
            var treasureSoldier = GameContainer.Instance.Get<UnitDataBase>().Get<TreasureSoldier6049Data>();

            treasureSoldierData = treasureSoldier.GetDataTreasureSoldier();
        }

        private IEnumerator CooldownAttack()
        {
            if (treasureSoldierData == null) yield break;

            yield return new WaitForSeconds(treasureSoldierData.timeCooldown);

            CallSoldierGoOut();
        }

        private void CallSoldierGoOut()
        {
            if (soldier == null)
            {
                var go = ResourceUtils.GetUnit("6049", transform);

                if (go)
                {
                    go.transform.localPosition = Vector3.zero;

                    soldier = go.GetComponent<TreasureSoldierController>();

                    soldier.InitSoldier(CallbackClaim);

                    var paths = new List<Vector3>();

                    for (int i = 0; i < posTargets.Length; i++)
                    {
                        paths.Add(posTargets[i].position);
                    }

                    soldier.SetPath(paths);
                }
            }

            soldier.SoldierGoOut();
        }

        private void CallbackClaim()
        {
            GamePlayData.Instance.AddMoneyInGame(MoneyInGameType.Gold, treasureSoldierData.goldReceive);
            
            CoroutineUtils.Instance.StopCoroutine(coroutineCooldown);

            coroutineCooldown = CoroutineUtils.Instance.StartCoroutine(CooldownAttack());
            
            GamePlayController.Instance.TotalUseEnv++;
        }
    }
}