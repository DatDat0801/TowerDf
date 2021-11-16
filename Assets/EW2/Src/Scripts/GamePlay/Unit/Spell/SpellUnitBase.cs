using UnityEngine;
using UnityEngine.Events;

namespace EW2.Spell
{
    //Note: Need a game object spell for initialize independently from hero, or UI And Easily to write various behaviour
    public class SpellUnitBase : Unit
    {
        public HeroBase Hero { get; private set; }

        protected SpellData spellData;
        public virtual SpellData SpellData => spellData;
        public int Level { get; protected set; }
        //public int Id { get; protected set; }
        
        public SpellStatBase SpellStatBase => SpellData.spellStats[Level - 1];
        private UnitState unitState;
        public override UnitState UnitState => unitState ?? (unitState = new DummyState(this));
        /// <summary>
        /// Creator, Target
        /// </summary>
        public UnityAction<Unit, Unit> OnSkillPassive { get; set; }

        public void SetHero(HeroBase hero)
        {
            Hero = hero;
        }
        public void InitSpellData()
        {
            //Id = spellId;
            SpellItem data = (SpellItem) UserData.Instance.GetInventory(InventoryType.Spell, Id);

            Level = data.Level;
            DamageType = SpellStatBase.damageType;
            
        }

        public override void OnUpdate(float deltaTime)
        {
            
        }

        public override void Remove()
        {
            
        }

        protected override void InitAction()
        {
            
        }

        public virtual void UseSpellActiveSkill()
        {
        }

        public virtual void ActiveSkillToTarget(Vector3 target, UnityAction callback)
        {
            
        }
    }
}