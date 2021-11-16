using UnityEngine;

namespace EW2
{
    public class Hero1001PassiveSkill3Collider : ColliderTrigger<Hero1001>, IAddEffect
    {
        private const int SkillId = 2;
        
        private Hero1001PassiveSkill3 passiveSkill3;

        private void Awake()
        {
            passiveSkill3 = (Hero1001PassiveSkill3)owner.SkillController.GetSkillPassive(SkillId);
        }

        public override void Trigger(float time = 0, float delayTime = 0)
        {
            ((CircleCollider2D)collider2D).radius = passiveSkill3.SkillData.region;
            
            base.Trigger(time, delayTime);
        }

        public void AddEffect(Unit target)
        {
            var hp = target.Stats.GetStat<HealthPoint>(RPGStatType.Health);
            
            hp.AddShieldPoint(passiveSkill3.GetShield(target));
            
            var armor = target.Stats.GetStat<ArmorPhysical>(RPGStatType.Armor);
            
            armor.AddModifier(passiveSkill3.GetBonusStat(armor, target));
        }
    }
}