using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invoke;
using Unity.Mathematics;
using UnityEngine;

namespace EW2
{
    public class TreasureSoldierController : Unit
    {
        private const string MoveLeftName = "move_left";

        private const string MoveRightName = "move_right";

        protected UnitState treasureSoldierState;
        public override UnitState UnitState => treasureSoldierState ?? (treasureSoldierState = new DummyState(this));

        protected TreasureSoldierSpine dummySpine;
        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new TreasureSoldierSpine(this));

        public override RPGStatCollection Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new TreasureSoldierStat(this);
                }

                return stats;
            }
        }

        private Idle idle;

        private MultiMove multiMove;

        private SingleMove singleMove;

        private bool isMoveOut, isMoveIn, isShowEffect;

        private List<Vector3> wayPoints = new List<Vector3>();

        private List<Vector3> wayPointsReverse = new List<Vector3>();

        private Action callbackClaim;

        private Coroutine coroutineInstanceCoin;

        public void InitSoldier(Action callback)
        {
            InitAction();

            callbackClaim = callback;
        }

        public void SetPath(List<Vector3> paths)
        {
            wayPoints.AddRange(paths);

            wayPointsReverse.AddRange(paths);

            wayPointsReverse.Reverse();

            wayPointsReverse.RemoveAt(0);

            wayPointsReverse.Add(transform.position);
        }

        public void SoldierGoOut()
        {
            gameObject.SetActive(true);
            if (transform.position.x < wayPoints[wayPoints.Count - 1].x)
                dummySpine.moveName = MoveRightName;
            else
                dummySpine.moveName = MoveLeftName;

            dummySpine.ChestClose();

            multiMove.SetTarget(wayPoints);

            isMoveOut = true;

            isShowEffect = false;
        }

        public void SoldierGoIn()
        {
            if (dummySpine.moveName.Equals(MoveLeftName))
                dummySpine.moveName = MoveRightName;
            else
                dummySpine.moveName = MoveLeftName;

            multiMove.SetTarget(wayPointsReverse);

            isMoveIn = true;
        }

        public void ClaimGold()
        {
            if (isMoveOut || isMoveIn || isShowEffect) return;

            isShowEffect = true;

            dummySpine.ChestOpen();

            InvokeProxy.Iinvoke.Invoke(this, InstanceEffectCoin, 0.3f);

            InvokeProxy.Iinvoke.Invoke(this, SoldierGoIn, 1f);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (isMoveOut || isMoveIn)
                multiMove.Execute();
        }

        public override void Remove()
        {
        }

        protected override void InitAction()
        {
            idle = new Idle(this);

            idle.Execute();

            multiMove = new MultiMove(this);

            multiMove.onFinish += () =>
            {
                isMoveOut = false;

                if (isMoveIn)
                {
                    isMoveIn = false;
                    
                    gameObject.SetActive(false);
                }

                idle.Execute();
            };

            singleMove = new SingleMove(this);
        }

        private void InstanceEffectCoin()
        {
            var iconGold = GameObject.Find("IconGold");

            if (coroutineInstanceCoin != null)
                CoroutineUtils.Instance.StopCoroutine(coroutineInstanceCoin);

            coroutineInstanceCoin = CoroutineUtils.Instance.StartCoroutine(InstanceCoinDelay(iconGold.transform));
        }

        private IEnumerator InstanceCoinDelay(Transform target)
        {
            for (int i = 0; i < 5; i++)
            {
                var effect = ResourceUtils.GetVfx("Assets", "fx_common_coin", transform.position, quaternion.identity);

                if (effect)
                {
                    if (i == 0)
                    {
                        effect.GetComponent<CoinController>().InitCoint(target.position, InstanceImpactCoin);
                    }
                    else
                    {
                        effect.GetComponent<CoinController>().InitCoint(target.position);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void InstanceImpactCoin(Vector3 posSpawn)
        {
            ResourceUtils.GetVfx("Assets", "fx_common_coin_impact", posSpawn, quaternion.identity);

            callbackClaim?.Invoke();
        }

        public override void Flip(float positionX)
        {
            return;
        }
    }
}