using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Events;

namespace EW2.Spell
{
    public class Spell4003 : SpellUnitBase
    {
        public Spell4003PassiveData PassiveData
        {
            get
            {
                Spell4003Data data = (Spell4003Data) SpellData;
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
        public override RPGStatCollection Stats => stats ?? (stats = new SpellStats(this, SpellStatBase));

        private async void Start()
        {
            //Cause the hero list init delay 1000ms, so we need init after a little bit
            await UniTask.Delay(1100);
            Hero.onGetHurt += OnHeroGetDamage;
        }

        private void OnHeroGetDamage(DamageInfo damageInfo)
        {
            //environment_unit tag is the unit such as poison dump
            if (!damageInfo.creator.CompareTag("environment_unit"))
            {
                if (damageInfo.target == null) return;
                var index = RandomFromDistribution.RandomChoiceFollowingDistribution(new List<float>()
                    {1 - PassiveData.explosionRatio, PassiveData.explosionRatio});
                if (index == 0) return;
                var go = ResourceUtils.GetSpellUnit("4003_blast_spirit_passive", Vector3.zero, Quaternion.identity,
                    Hero.transform);
                
                //sfx
                var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1001_PASSIVE_1);
                EazySoundManager.PlaySound(audioClip);
                
                var impact = go.GetComponent<Spell4003PassiveDamageImpact>();
                impact.Initialize(PassiveData, this);
                try
                {
                    OnSkillPassive?.Invoke(this, damageInfo.creator);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                
            }
        }

        public override void ActiveSkillToTarget(Vector3 target, UnityAction callback)
        {
            base.ActiveSkillToTarget(target, callback);
            var go = ResourceUtils.GetSpellUnit("4003_blast_spirit", target, Quaternion.identity);

            var impact = go.GetComponent<Spell4003DamageImpact>();

            impact.Initialize(SpellStatBase, this);
            PlaySoundDelay(350);
            //TODO Spawn spell 4003 here
            callback?.Invoke();
            //Callback if success here
        }

        private async void PlaySoundDelay(int miliseconds)
        {
            await UniTask.Delay(miliseconds);
            var audioClip = ResourceUtils.LoadSound(SoundConstant.SPIRIT_EXPLOSION);
            EazySoundManager.PlaySound(audioClip);
        }
    }
}