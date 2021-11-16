using EW2.Spell;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace EW2
{
    public class Spell4011 : SpellUnitBase
    {
        public Spell4011PassiveData SkillData
        {
            get
            {
                Spell4011Data data = (Spell4011Data)SpellData;
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

        //public SpellStatBase SpellStatBase => SpellData.spellStats[Level - 1];
        public override RPGStatCollection Stats => stats ?? (stats = new SpellStats(this, SpellStatBase));

        private int _countDamageReceived = 0;

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
            Hero.onGetHurt += OnHeroGetDamage;
        }

        private void OnHeroGetDamage(DamageInfo damageInfo)
        {
            //environment_unit tag is the unit such as poison dump
            if (!damageInfo.creator.CompareTag("environment_unit"))
            {
                this._countDamageReceived++;
                if (this._countDamageReceived >= SkillData.hitsToTerrify) //4011_totem_impact
                {
                    this._countDamageReceived = 0;

                    var go = ResourceUtils.GetSpellUnit("4011_totem_impact_passive", Hero.transform.position,
                        Quaternion.identity);
                    go.transform.localScale = Vector3.one * SkillData.radius / 3;
                    var impact = go.GetComponent<TotemPassiveImpact>();
                    impact.Initialize(SkillData, Hero);
                    impact.Trigger(0.1f);
                    //sfx
                    var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4011_PASSIVE);
                    EazySoundManager.PlaySound(audioClip);
                }
            }
        }

        public override void ActiveSkillToTarget(Vector3 target, UnityAction callback)
        {
            base.ActiveSkillToTarget(target, callback);
            var go = ResourceUtils.GetSpellUnit("4011_totem", target, Quaternion.identity);

            var totem = go.GetComponent<Totem>();
            totem.InitData(Stats, SpellStatBase);
            transform.localScale = Vector2.one * (2 * this.SpellStatBase.range) / 9;
            //sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4011_APPEAR);
            EazySoundManager.PlaySound(audioClip);
            //Callback if success here
            callback?.Invoke();
        }
    }
}