using System;
using Invoke;
using Spine;
using UnityEngine;

namespace EW2
{
    public class Hero1002PassiveSkill3 : UnitSkill
    {
        private HeroData1002.PassiveSkill3[] passiveSkill;
        private bool isReady;
        private float timeCooldown;
        private HeroData1002.PassiveSkill3 dataPassiveCurr;
        public HeroData1002.PassiveSkill3 DataPassiveCurr => dataPassiveCurr;

        private Hero1002 hero1002;
        private RangerSearchTarget rangeSearchTarget;
        public Action onComplete;

        public Hero1002PassiveSkill3(Unit owner, HeroData1002.PassiveSkill3[] passiveSkill) : base(owner)
        {
            this.passiveSkill = passiveSkill;
            hero1002 = owner as Hero1002;
            rangeSearchTarget = hero1002.SearchTarget as RangerSearchTarget;
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
            this.owner.UnitState.Set(ActionState.SkillPassive3);
            var trackEntry = owner.UnitSpine.SkillPassive3();
            if (onComplete != null)
            {
                void OnComplete(TrackEntry track)
                {
                    onComplete?.Invoke();
                    track.Complete -= OnComplete;
                }

                trackEntry.Complete += OnComplete;
            }
        }

        public override void Remove()
        {
            if (InvokeProxy.Iinvoke != null)
                InvokeProxy.Iinvoke.CancelInvoke(this);
        }

        private HeroData1002.PassiveSkill3 GetDataPassiveByLevel(int levelPassive = 1)
        {
            if (levelPassive < passiveSkill.Length)
                return passiveSkill[levelPassive - 1];
            return passiveSkill[passiveSkill.Length - 1];
        }

        public bool IsReady()
        {
            return isReady;
        }

        public void CooldownPassive()
        {
            if (rangeSearchTarget != null)
            {
                isReady = false;
                rangeSearchTarget.SetRangeAttack(this.hero1002.HeroStatBase.detectRangeAttack);
            }

            InvokeProxy.Iinvoke.Invoke(this, SetReady, timeCooldown);
        }

        private void SetReady()
        {
            isReady = true;

            if (rangeSearchTarget != null)
            {
                rangeSearchTarget.ScaleRangeDetect(dataPassiveCurr.regionDetect);
                Debug.LogWarning("[Hero 1002] Skill passive 3 detect range scale: " + dataPassiveCurr.regionDetect);
            }
        }
    }
}