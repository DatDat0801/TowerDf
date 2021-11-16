using System;
using System.Collections.Generic;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using Zitga.Observables;

namespace EW2.Spell
{
    public class Spell4001 : SpellUnitBase
    {
        public Spell4001PassiveData SkillData
        {
            get
            {
                Spell4001Data data = (Spell4001Data)SpellData;
                return data.passive[Level - 1];
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

        public SpellStatBase SpellStatBase => SpellData.spellStats[Level - 1];

        public Spell4001ActiveData SpellActiveData
        {
            get
            {
                Spell4001Data data = (Spell4001Data)SpellData;
                return data.active[Level - 1];
            }
        }

        #region MonobehaviorMethod

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
                    new List<float>() {1 - SkillData.freezeRatio, SkillData.freezeRatio});
                if (index == 1)
                {
                    if (enemy.Value == null) return;
                    if (enemy.Value.MySize == UnitSize.Boss || enemy.Value.MySize == UnitSize.MiniBoss)
                    {
                        return;
                    }
                    var coldStatus = new EnemyColdOverTime(
                        new StatusOverTimeConfig() {
                            creator = this,
                            owner = enemy.Value,
                            lifeTime = this.SkillData.duration,
                            statusType = StatusType.Cold
                        }, this.SkillData.slowDownPercent);
                    enemy.Value.StatusController.AddStatus(coldStatus);
                }
            }
        }

        public async override void ActiveSkillToTarget(Vector3 target, UnityAction callback)
        {
            base.ActiveSkillToTarget(target, callback);
            //Callback if success here
            callback?.Invoke();

            var meteor = ResourceUtils.GetSpellUnit("4001_blizzard", target, Quaternion.identity);
            meteor.transform.localScale = Vector3.one * SpellStatBase.range / 3;
            //sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4001_APPEAR);
            EazySoundManager.PlaySound(audioClip);

            var impact = meteor.GetComponent<FreezeActiveImpact>();
            impact.Initialize(this, SpellActiveData);
            impact.Trigger(0.3f);
        }
    }
}