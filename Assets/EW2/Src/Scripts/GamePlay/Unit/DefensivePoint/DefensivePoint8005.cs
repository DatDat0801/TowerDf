using System;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public class DefensivePoint8005 : DefensivePointBase
    {
        public DefensivePoint8005Data DefensivePointData
        {
            get
            {
                if (this.defensivePointData == null)
                {
                    this.defensivePointData = GameContainer.Instance.Get<UnitDataBase>().Get<DefensivePoint8005Data>();
                }

                return this.defensivePointData as DefensivePoint8005Data;
            }
        }

        public DefensivePointStatBase StatBase => DefensivePointData.stats[0];
        public override RPGStatCollection Stats => stats ?? (stats = new DefensivePointStats(this, StatBase));

        public DefensivePoint8005Passive PassiveData => DefensivePointData.passiveStats[0];
        private Dictionary<HeroBase, LightingBolt8005> _bolts;

        private void Initialize()
        {
            this._bolts = new Dictionary<HeroBase, LightingBolt8005>();
            var heroes = GamePlayController.Instance.GetHeroes();
            foreach (HeroBase heroBase in heroes)
            {
                this._bolts.Add(heroBase, new LightingBolt8005(this) {Hero = heroBase});
                heroBase.SearchTarget.target.ValueChanged += OnTargetChange;
            }
        }

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

        void OnTargetChange(object sender, EventArgs args)
        {
            ObservableProperty<EnemyBase> enemy = (ObservableProperty<EnemyBase>)sender;
            if (enemy.Value != null)
            {
                enemy.Value.onGetHurt = OnGetHurt;
            }

            void OnGetHurt(DamageInfo damageInfo)
            {
                //if (damageInfo.damageType != DamageType.Physical) return;
                if (damageInfo.creator is HeroBase hero)
                {
                    var bolt = this._bolts[hero];
                    bolt.TakeDamageCount++;
                    bolt.Bolt(PassiveData);
                }
            }
        }

        public override void GetHurt(DamageInfo damageInfo)
        {
            base.GetHurt(damageInfo);
            if (IsAlive == false) return;
            var healthPoint = Stats.GetStat<HealthPoint>(RPGStatType.Health);
            int x = (int)((1 - healthPoint.CalculateCurrentPercent()) * 100);
            if (x / 25 >= this.getHurtCount)
            {
                this.getHurt.Execute();
                ResourceUtils.GetVfx("Defensive_point", "8005_get_hurt", transform.position, Quaternion.identity);
                ResourceUtils.GetVfx("Defensive_point", "8005_lighting_loop", transform.position, Quaternion.identity);
                this.getHurtCount++;
            }
        }
    }
}