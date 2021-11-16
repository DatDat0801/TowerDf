using System;
using Cysharp.Threading.Tasks;
using EW2.Spell;
using Hellmade.Sound;
using Invoke;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Totem : Dummy
    {
        [SerializeField]
        private Spell4011ActiveImpact _activeImpact;
        private UnitState unitState;
        public override UnitState UnitState => unitState ?? (unitState = new DummyState(this));

        protected AllySearchTarget searchTarget;

        public AllySearchTarget SearchTarget
        {
            get
            {
                if (searchTarget == null)
                {
                    searchTarget = GetComponentInChildren<AllySearchTarget>();
                }

                return searchTarget;
            }
        }
        public Spell4011PassiveData SkillData
        {
            get
            {
                Spell4011Data data = (Spell4011Data) SpellData;
                return data.passiveData[Level - 1];
            }
        }

        private SpellData _spellData;
        public SpellData SpellData
        {
            get
            {
                if (_spellData == null)
                {
                    _spellData = GameContainer.Instance.Get<UnitDataBase>().GetSpellDataById(Id);
                }

                return _spellData;
            }
        }
        public SpellStatBase SpellStatBase { get; private set; }
        private TotemSpine _totemSpine;
        public override UnitSpine UnitSpine => _totemSpine ?? (_totemSpine = new TotemSpine(this));
        private UnitAction _die;
        public void InitData(RPGStatCollection rpgStatCollection, SpellStatBase statBase)
        {
            SearchTarget.SetBlockNumber(1000);
            this.stats = rpgStatCollection;
            this.stats.ConfigureStats();
            
            InitAction();
            SpellStatBase = statBase;
            this._activeImpact.Init(this, rpgStatCollection);
            UnitSpine.Idle();
            
            if (HealthBar)
            {
                HealthBar.SetHealthBar(this.stats.GetStat<HealthPoint>(RPGStatType.Health));
            }
            
            var healthPoint = this.stats.GetStat<HealthPoint>(RPGStatType.Health);
            healthPoint.CurrentValue = healthPoint.StatValue;
            InvokeProxy.Iinvoke.Invoke(this, Remove, SpellStatBase.duration);
        }

        #region NotImplement
        
        //public float health = 0;
        public override void OnUpdate(float deltaTime)
        {
            //this.health = this.stats.GetStat<HealthPoint>(RPGStatType.Health).CurrentValue;
        }

        public async override void Remove()
        {
            this._die.Execute();
            //sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4011_DISAPPEAR);
            EazySoundManager.PlaySound(audioClip);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            LeanPool.Despawn(gameObject);
            //Destroy(this.gameObject);

        }

        protected override void InitAction()
        {
            _die = new Die(this);
        }
        public override void AttackMelee()
        {
            throw new System.NotImplementedException();
        }

        public override void AttackRange()
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}