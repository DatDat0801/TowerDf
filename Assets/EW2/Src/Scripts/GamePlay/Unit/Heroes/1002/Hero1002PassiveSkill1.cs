using UnityEngine;
using UnityEngine.UIElements;

namespace EW2
{
    public class Hero1002PassiveSkill1 : UnitSkill
    {
        private HeroData1002.PassiveSkill1[] _passiveSkill;
        private bool _activePassive;
        public HeroData1002.PassiveSkill1 dataPassiveCurr;

        public Hero1002PassiveSkill1(Unit owner, HeroData1002.PassiveSkill1[] passiveSkill) : base(owner)
        {
            this._passiveSkill = passiveSkill;
        }

        public override void Init()
        {
            this._activePassive = false;
            if (Level > 0)
                this.dataPassiveCurr = GetDataPassiveByLevel(Level);
        }

        public override void Execute()
        {
            _activePassive = false;
        }

        public override void Remove()
        {
        }

        private HeroData1002.PassiveSkill1 GetDataPassiveByLevel(int levelPassive = 1)
        {
            if (levelPassive < this._passiveSkill.Length)
                return this._passiveSkill[levelPassive - 1];
            return this._passiveSkill[this._passiveSkill.Length - 1];
        }

        public bool IsActive()
        {
            return this._activePassive;
        }

        public void CheckActivePassive()
        {
            if (this._activePassive) return;

            var rand = Random.Range(0f, 1f);
            if (rand <= this.dataPassiveCurr.chance)
                this._activePassive = true;
        }
    }
}