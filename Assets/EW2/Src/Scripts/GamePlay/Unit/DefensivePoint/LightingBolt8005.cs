using System;
using System.Collections.Generic;
using Constants;
using Cysharp.Threading.Tasks;
using DigitalRuby.LightningBolt;
using UnityEngine;

namespace EW2
{
    public class LightingBolt8005
    {
        public HeroBase Hero { get; set; }
        public int TakeDamageCount { get; set; }
        private readonly DefensivePointBase _defensivePoint;
        private readonly HashSet<EnemyBase> _affects = new HashSet<EnemyBase>();

        public LightingBolt8005(DefensivePointBase def)
        {
            this._defensivePoint = def;
        }

        /// <summary>
        /// Start bolt lighting
        /// </summary>
        /// <param name="passiveData"></param>
        public async void Bolt(DefensivePoint8005Passive passiveData)
        {
            if (TakeDamageCount < passiveData.attackTime) return;
            //Debug.LogAssertion($"Can bolt at {TakeDamageCount} damage count");
            if (passiveData.maxTargets <= 0) return;
            TakeDamageCount = 0;
            //GoSeeker.GetComponent<EnemySeekAlly>().ClearTargets();

            var lastTarget = Hero.SearchTarget.target.Value;
            this._affects.Add(lastTarget);
            if (passiveData.maxTargets == 1)
            {
                TakeDamage(lastTarget, passiveData.strikeDamage);
                return;
            }

            for (int i = 0; i < passiveData.maxTargets - 1; i++)
            {
                if (lastTarget == null) continue;
                var nextTarget = await Seek(lastTarget, passiveData.range);
                await UniTask.Delay(TimeSpan.FromSeconds(0.12f));

                // if (nextTarget != null)
                // {
                //     Debug.LogAssertion($"next target: {nextTarget.name},{nextTarget.GetInstanceID()}");
                // }
                // else
                // {
                //     Debug.LogAssertion($"next target is null");
                // }

                if (nextTarget == null) continue;

                //Debug.LogAssertion($"affected: " + this._affects.Count);
                // if (nextTarget == null || lastTarget == nextTarget) continue;
                //Debug.LogAssertion($"next target: {nextTarget.name}, {nextTarget.GetInstanceID()}");
                var go = ResourceUtils.GetVfx("Defensive_point", "8005_lighting_bolt", lastTarget.transform.position,
                    Quaternion.identity);
                var boltFx = go.GetComponent<LightningBoltScript>();
                boltFx.StartObject.transform.position =
                    lastTarget.gameObject.transform.position; //lastTarget.gameObject;
                boltFx.EndObject.transform.position =
                    nextTarget.gameObject.transform.position; //= nextTarget.gameObject;

                //boltFx.Trigger();
                //Debug.LogAssertion($"Bolt from {lastTarget.name},{lastTarget.GetInstanceID()} to {nextTarget.name},{nextTarget.GetInstanceID()}");
                //lastTarget = nextTarget;
                var damageAfterDecay = passiveData.strikeDamage *
                                       Mathf.Pow((1 - passiveData.decreaseRatioPerTarget), i + 2);
                TakeDamage(nextTarget, damageAfterDecay);

                if (nextTarget != null)
                {
                    this._affects.Add(nextTarget);
                    lastTarget = nextTarget;
                }
            }

            this._affects.Clear();
            GoSeeker.GetComponent<EnemySeekAlly>().ClearTargets();
        }

        private GameObject _goSeeker;

        public GameObject GoSeeker
        {
            get
            {
                if (this._goSeeker == null)
                {
                    this._goSeeker = new GameObject("seeker") {
                        tag = TagConstants.Enemy, layer = LayerMask.NameToLayer("ally_damage_box")
                    };
                    var collider = GoSeeker.AddComponent<CircleCollider2D>();
                    collider.isTrigger = true;
                    var seeker = GoSeeker.AddComponent<EnemySeekAlly>();
                    seeker.SetCollider(collider);
                    return this._goSeeker;
                }

                return this._goSeeker;
            }
        }

        private async UniTask<EnemyBase> Seek(EnemyBase origin, float radius)
        {
            GoSeeker.transform.parent = origin.transform;
            GoSeeker.transform.localPosition = Vector3.zero;
            GoSeeker.GetComponent<CircleCollider2D>().radius = radius;

            var seeker = GoSeeker.GetComponent<EnemySeekAlly>();

            seeker.GetCollider().enabled = false;

            seeker.SetOwner(origin);
            seeker.Trigger(0.1f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            var result = seeker.GetNearestEnemy(this._affects);
            //seeker.gameObject.AddComponent<DestroyMe>().deathtimer = 0.5f;
            return result;
        }

        private void TakeDamage(EnemyBase onEnemy, float damage)
        {
            var go = ResourceUtils.GetVfx("Defensive_point", "8005_lighting_impact", onEnemy.transform.position,
                Quaternion.identity);
            var impact = go.GetComponent<Bolt8005Impact>();
            impact.SetOwner(this._defensivePoint);

            impact.Initialize(damage);
            impact.Trigger(0.1f);
        }
    }
}