using System.Collections;
using System.ComponentModel;
using Hellmade.Sound;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace EW2.Spell
{
    public class Spell4005 : SpellUnitBase
    {
        public Spell4005SkillData SkillData
        {
            get
            {
                Spell4005Data data = (Spell4005Data) SpellData;
                return data.healsData[Level - 1];
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

        private RegenHpOverTime regenHp;
        public VfxOverTime vfx { get; private set; }

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
            regenHp = new RegenHpOverTime(new StatusOverTimeConfig()
            {
                creator = this,
                owner = Hero,
                lifeTime = 2000,
                intervalTime = 1,
                delayTime = 0f,
                baseValue = SkillData.healPassiveHp // * Hero.Stats.GetStat<HealthPoint>(RPGStatType.Health).StatValue
            })
            {
                Stacks = true,
                OnStatusExecute = () =>
                {
                    new VfxOverTime() {owner = Hero, vfxName = "fx_status_bufhealth"}.DoPlayEffect(1f, 0f);
                }
            };
            //Hero.onGetHurt = RegenHp;
            Hero.Stats.GetStat<HealthPoint>(RPGStatType.Health).PropertyChanged += RegenHp;

            vfx = new VfxOverTime() {owner = Hero, vfxName = "fx_status_bufhealth"};
        }

        private bool addingHp;

        private void RegenHp(object sender, PropertyChangedEventArgs e)
        {
            //StartCoroutine(Execute(sender));
            Execute(sender);
        }

        protected void Execute(object sender)
        {
            var currentHeath = (HealthPoint) sender;
            if (addingHp == false)
            {
                // if (currentHeath.IsFull || regenHp.IsExecuting)
                //     regenHp.Remove();
                if (!currentHeath.IsFull)
                {
                    regenHp.Execute();
                }

                addingHp = true;
            }

            if (currentHeath.IsFull)
            {
                regenHp.Remove();
                addingHp = false;
            }
        }

        public override void ActiveSkillToTarget(Vector3 target, UnityAction callback)
        {
            base.ActiveSkillToTarget(target, callback);
            var go = ResourceUtils.GetSpellUnit("4005_blessing_of_nature_glow", target, Quaternion.identity);

            var impact = go.GetComponent<Spell4005Impact>();

            impact.Initialize(SpellStatBase, SkillData, this, Hero);
            //sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4005_HEAL);
            EazySoundManager.PlaySound(audioClip);
            //Callback if success here
            callback?.Invoke();
        }
    }
}