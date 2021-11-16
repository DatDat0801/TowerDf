using Invoke;

namespace EW2
{
    public class Hero1004PassiveSkill3 : UnitSkill
    {
        private HeroData1004.PassiveSkill3[] passiveSkill;
        private bool isReady;
        private float timeCooldown;
        private HeroData1004.PassiveSkill3 dataPassiveCurr;
        public HeroData1004.PassiveSkill3 DataPassiveCurr => dataPassiveCurr;

        public Hero1004PassiveSkill3(Unit owner, HeroData1004.PassiveSkill3[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
        }

        public override void Init()
        {
            isReady = false;

            if (Level > 0)
            {
                dataPassiveCurr = GetDataPassiveByLevel(Level);

                timeCooldown = dataPassiveCurr.cooldown;

                InvokeProxy.Iinvoke.Invoke(this, SetReady, timeCooldown);
            }
        }

        public override void Execute()
        {
            if (isReady)
                CooldownPassive();
        }

        public override void Remove()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
        }

        public void CooldownPassive()
        {
            isReady = false;
            InvokeProxy.Iinvoke.Invoke(this, SetReady, timeCooldown);
        }

        private HeroData1004.PassiveSkill3 GetDataPassiveByLevel(int levelPassive = 1)
        {
            if (levelPassive < passiveSkill.Length)
                return passiveSkill[levelPassive - 1];
            return passiveSkill[passiveSkill.Length - 1];
        }

        public bool IsReady()
        {
            return isReady;
        }

        private void SetReady()
        {
            isReady = true;
        }
    }
}