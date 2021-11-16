using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invoke;
using Lean.Pool;
using Map;
using Zitga.Observables;
using Spine;
using TigerForge;
using Unity.Mathematics;
using UnityEngine;

namespace EW2
{
    public class Ship6045Controller : Dummy
    {
        private const string SkinShipNormal = "6045_01";

        private const string SkinShipAttack = "6045_02";

        [SerializeField] private bool isShootCanon;

        [SerializeField] private Ship6045TargetCollection targetCollection;

        public override UnitSpine UnitSpine => dummySpine ?? (dummySpine = new Ship6045Spine(this));

        private Idle idle;

        private AttackRange attackRange;

        private Ship6045Data.DataCanon dataCanon;

        private bool isReadyAttack;

        private List<Dummy> listTarget = new List<Dummy>();

        private List<GameObject> listWarning = new List<GameObject>();

        private Coroutine coroutineCooldown;

        protected override void Awake()
        {
            base.Awake();

            InitAction();

            if (isShootCanon)
            {
                UnitSpine.SetSkinSpine(SkinShipAttack);

                var dataShip = GameContainer.Instance.Get<UnitDataBase>().Get<Ship6045Data>();

                dataCanon = dataShip.GetDataCanon();

                EventManager.StartListening(GamePlayEvent.OnStartGame, () =>
                {
                    coroutineCooldown = CoroutineUtils.Instance.StartCoroutine(CooldownAttack());

                    EventManager.StopListening(GamePlayEvent.OnStartGame, "Ship6045");
                }, "Ship6045");
            }
            else
            {
                UnitSpine.SetSkinSpine(SkinShipNormal);

                GamePlayEvent.onCallWaveSpecial += CallEnemy;
            }

            idle.Execute();
        }

        public override void OnDisable()
        {
            if (gameObject != null && CoroutineUtils.Instance != null && coroutineCooldown != null)
                CoroutineUtils.Instance.StopCoroutine(coroutineCooldown);

            EventManager.StopListening(GamePlayEvent.OnStartGame, "Ship6045");

            GamePlayEvent.onCallWaveSpecial -= CallEnemy;

            base.OnDisable();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (isReadyAttack && listTarget.Count > 0)
            {
                attackRange.Execute(deltaTime);
            }
        }

        public override void Remove()
        {
        }

        protected override void InitAction()
        {
            idle = new Idle(this);

            attackRange = new AttackRange(this);
        }

        public override void AttackMelee()
        {
        }

        public override void AttackRange()
        {
            attackRange.onComplete += OnShootCanonComplete;

            isReadyAttack = true;

            InvokeProxy.Iinvoke.Invoke(this, () => { CoroutineUtils.Instance.StartCoroutine(ImpactCanon()); }, 0.3f);
        }

        public override bool IsAlive => true;

        #region Lander

        private void CallEnemy()
        {
            ResourceUtils.GetVfx("Assets", "6045_door_warring", Vector3.zero, quaternion.identity, transform);

            UnitSpine.Appear();

            InvokeProxy.Iinvoke.Invoke(this, OnOpenDoorComplete, 1.267f);

            GamePlayEvent.onCallWaveSpecial = null;
        }

        private void OnOpenDoorComplete()
        {
            ResourceUtils.GetVfx("Assets", "6045_impact_door", Vector3.zero, quaternion.identity, transform);
        }

        #endregion

        #region Canon

        private IEnumerator CooldownAttack()
        {
            if (dataCanon == null) yield break;

            yield return new WaitForSeconds(dataCanon.timeCooldown);

            ShowWarning();
        }

        private void ShowWarning()
        {
            listTarget.AddRange(targetCollection.GetListTargets());

            if (listTarget.Count > 0)
            {
                var objId = 0;
                for (int i = 0; i < listTarget.Count; i++)
                {
                    if (listTarget[i] == null) continue;
                    if (objId != listTarget[i].GetInstanceID())
                    {
                        objId = listTarget[i].GetInstanceID();
                    }
                    else
                    {
                        continue;
                    }

                    var warning = ResourceUtils.GetVfx("Assets", "6045_impact_warring",
                        listTarget[i].transform.position, Quaternion.identity);

                    listWarning.Add(warning);
                }

                InvokeProxy.Iinvoke.Invoke(this, AttackRange, 3f);
            }
            else
            {
                CoroutineUtils.Instance.StopCoroutine(coroutineCooldown);

                CoroutineUtils.Instance.StartCoroutine(CooldownAttack());
            }
        }

        private void OnShootCanonComplete()
        {
            isReadyAttack = false;

            listTarget.Clear();

            listWarning.Clear();

            CoroutineUtils.Instance.StopCoroutine(coroutineCooldown);

            coroutineCooldown = CoroutineUtils.Instance.StartCoroutine(CooldownAttack());
        }

        private IEnumerator ImpactCanon()
        {
            for (int i = 0; i < listTarget.Count; i++)
            {
                var posSpawn = Vector3.zero;

                if (i < listWarning.Count)
                {
                    posSpawn = listWarning[i].transform.position;
                }
                else
                {
                    posSpawn = listWarning[0].transform.position;
                }

                var impact = ResourceUtils.GetVfx("Assets", "6045_impact_canon", posSpawn, Quaternion.identity);

                if (impact)
                {
                    var control = impact.GetComponent<CanonImpact>();

                    this.DamageType = dataCanon.damageType;

                    control.InitAOE(this, dataCanon.damage);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion
    }
}