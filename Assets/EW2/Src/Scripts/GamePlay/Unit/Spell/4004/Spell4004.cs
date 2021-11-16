using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;

namespace EW2.Spell
{
    public class Spell4004 : SpellUnitBase
    {
        public Spell4004PassiveData SkillData
        {
            get
            {
                Spell4004Data data = (Spell4004Data)SpellData;
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

        public SpellStatBase SpellStatBase => SpellData.spellStats[Level - 1];

        public Warrior4004Stat Warrior4004Stat
        {
            get
            {
                var spell4004Data = (Spell4004Data)SpellData;
                return spell4004Data.warriorStats[Level - 1];
            }
        }

        public override RPGStatCollection Stats => stats ?? (stats = new SpellStats(this, SpellStatBase));

        private List<Warrior4004> _warriors;

        public List<Warrior4004> Warriors
        {
            get
            {
                if (this._warriors == null)
                {
                    this._warriors = new List<Warrior4004>();
                }

                return this._warriors;
            }
        }

        //Modifier by 4004
        private RPGStatModifier _armor;

        private RPGStatModifier _resistance;

        //fx
        private GameObject _passiveFx;


        // #region MonobehaviorMethod
        //
        // public override void OnEnable()
        // {
        //     base.OnEnable();
        //     EventManager.StartListening(GamePlayEvent.ON_SPELL_INIT_READY, Initialize);
        // }
        //
        // public override void OnDisable()
        // {
        //     base.OnDisable();
        //     EventManager.StopListening(GamePlayEvent.ON_SPELL_INIT_READY, Initialize);
        // }
        //
        // #endregion
        //
        // void Initialize()
        // {
        //     //AddVirtualHp();
        // }

        private void AddVirtualHp()
        {
            Hero.AddVirtualHealthBar(SkillData.virtualHp);
        }

        private void AddPassiveFx()
        {
            if (this._passiveFx == null)
            {
                this._passiveFx =
                    ResourceUtils.GetSpellUnit("4004_shield", Vector3.zero, Quaternion.identity, Hero.transform);
            }
            else
            {
                LeanPool.Spawn(this._passiveFx, Hero.transform);
            }

        }

        public async override void ActiveSkillToTarget(Vector3 target, UnityAction callback)
        {
            base.ActiveSkillToTarget(target, callback);
            //Callback if success here
            callback?.Invoke();
            
            var meteor = ResourceUtils.GetSpellUnit("4004_star_guardian_meteor", target, Quaternion.identity);
            //todo RANGE
            meteor.transform.localScale = Vector3.one * (SpellStatBase.range / 3);
            //sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4004_APPEAR);
            EazySoundManager.PlaySound(audioClip);

            await UniTask.Delay(TimeSpan.FromSeconds(0.7f));
            var impact = meteor.GetComponent<StunActiveImpact>();
            impact.Initialize(SpellStatBase, this);
            impact.Trigger(0.3f);
            var go = ResourceUtils.GetSpellUnit("4004_star_guardian", target, Quaternion.identity);
            var warrior = go.GetComponent<Warrior4004>();
            AddBonusStat(warrior);
            AddVirtualHp();
            AddPassiveFx();

        }


        void AddBonusStat(Warrior4004 warrior)
        {
            if (Warriors.Count <= 0)
            {
                var armorModifiable = Ultilities.GetStatModifiable(RPGStatType.Armor);
                this._armor = new RPGStatModifier(armorModifiable, ModifierType.TotalPercent,
                    SkillData.armorRatio, false);
                Hero.Stats.AddStatModifier(RPGStatType.Armor, _armor);

                var resistanceModifiable = Ultilities.GetStatModifiable(RPGStatType.Resistance);
                _resistance = new RPGStatModifier(resistanceModifiable, ModifierType.TotalPercent,
                    SkillData.magicResistantRatio, false);
                Hero.Stats.AddStatModifier(RPGStatType.Resistance, _resistance);
            }

            warrior.onDead = (victim, killer) => {
                Warriors.Remove(warrior);
                if (Warriors.Count <= 0)
                {
                    Hero.Stats.RemoveStatModifier(RPGStatType.Armor, _armor, true);
                    Hero.Stats.RemoveStatModifier(RPGStatType.Resistance, _resistance, true);
                    LeanPool.Despawn(this._passiveFx);
                }
            };
        }
    }
}