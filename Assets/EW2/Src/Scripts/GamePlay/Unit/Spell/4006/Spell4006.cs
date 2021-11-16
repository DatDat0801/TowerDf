using System;
using System.Collections.Generic;
using Invoke;
using Lean.Pool;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using Zitga.Observables;

namespace EW2.Spell
{
    public class Spell4006 : SpellUnitBase
    {
        public Spell4006PassiveData SkillData
        {
            get
            {
                Spell4006Data data = (Spell4006Data)SpellData;
                return data.passiveData[Level - 1];
            }
        }

        public override SpellData SpellData
        {
            get
            {
                if (spellData == null)
                {
                    spellData = GameContainer.Instance.Get<UnitDataBase>().GetSpellDataById(Id);
                }

                return spellData;
            }
        }

        #region Monobehaviour
        
        public override RPGStatCollection Stats => stats ?? (stats = new SpellStats(this, SpellStatBase));


        public override void OnEnable()
        {
            base.OnEnable();
            EventManager.StartListening(GamePlayEvent.ON_SPELL_INIT_READY, Initialize);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            EventManager.StopListening(GamePlayEvent.ON_SPELL_INIT_READY, Initialize);
        }

        #endregion

        void Initialize()
        {
            Hero.SearchTarget.target.ValueChanged += OnTargetChange;
        }

        void OnTargetChange(object sender, EventArgs args)
        {
            ObservableProperty<EnemyBase> enemy = (ObservableProperty<EnemyBase>)sender;
            if (enemy.Value != null)
            {
                enemy.Value.onGetHurt += OnGetHurt;
            }

            void OnGetHurt(DamageInfo damageInfo)
            {
                if (damageInfo.damageType != DamageType.Physical) return;

                int index = RandomFromDistribution.RandomChoiceFollowingDistribution(
                    new List<float>() {1 - SkillData.explosionRatio, SkillData.explosionRatio});
                if (index == 1)
                {
                    if (enemy.Value == null) return;
                    //Debug.LogAssertion($"Hero health BEFORE {Hero.Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue}");

                    var go = ResourceUtils.GetSpellUnit("4006_magical_landmine_passive",
                        enemy.Value.transform.position, Quaternion.identity);
                    var scale = SkillData.radius / 0.5f;
                    go.transform.localScale = new Vector3(scale, scale);
                    //go.GetComponent<CircleCollider2D>().radius = SkillData.radius;

                    InvokeProxy.Iinvoke.Invoke(this, () => { LeanPool.Despawn(go); }, 2f);
                    var damageBox = go.GetComponent<SpellDamageBox>();
                    DamageInfo customDamageInfo = new DamageInfo {
                        creator = this,
                        damageType = this.DamageType,
                        showVfxNormalAtk = true,
                        value = this.SkillData.damage,
                        isCritical = false
                    };
                    damageBox.SetCustomDamgeInfo(customDamageInfo);
                    damageBox.SetOwner(this);
                    damageBox.Trigger(0.01f, 0.3f);

                    OnSkillPassive?.Invoke(this, (EnemyBase)damageInfo.target);
                    //await UniTask.Delay(310);
                    //Debug.LogAssertion($"PASSIVE EXPLOSION ON {damageInfo.target}, Hero health {Hero.Stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue}");
                }
            }
        }


        public override void ActiveSkillToTarget(Vector3 target, UnityAction callback)
        {
            base.ActiveSkillToTarget(target, callback);
            var go = ResourceUtils.GetSpellUnit("4006_bomb", target, Quaternion.identity);

            var impact = go.GetComponent<Spell4006ExplosionImpact>();
            //remove bomb object after it explosion
            impact.OnRemoveBomb += delegate {
                LeanPool.Despawn(go);
                transform.position = target;
                //explosionDamageBox.Trigger(0.2f);
            };
            //impact.triggerBombCollider.SetOwner();
            impact.triggerBombCollider.Initialize(this, target, SkillData.startDelay);
            //Callback if success here
            callback?.Invoke();
        }
    }
}