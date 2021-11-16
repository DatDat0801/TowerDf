using System.Collections;
using UnityEngine;

namespace EW2
{
    public class AoeCountTime
    {
        public HeroData1005.PassiveSkill2 Passive2Data { get; set; }
        public Hero1005 Hero1005 { get; set; }

        public bool TimeOut { get; private set; } = true;


        public void ModifyAttackSpeed()
        {
            var moveSpeedAttribute = Hero1005.Stats.GetStat<RPGAttribute>(RPGStatType.AttackSpeed);
            var statusOverTimeConfig = new StatusOverTimeConfig()
            {
                creator = Hero1005,
                owner = Hero1005,
                lifeTime = Passive2Data.aoeDuration,
                intervalTime = 1f,
            };
             var modifierAttackSpeed = new RPGStatModifier(moveSpeedAttribute, Passive2Data.modifierType,
                 Passive2Data.attackSpeedIncrease, false, Hero1005, Hero1005);
                var speedAttack = new ModifierStatOverTime(statusOverTimeConfig, modifierAttackSpeed);
            Hero1005.StatusController.AddStatus(speedAttack);
        }
        public void Start()
        {
            ModifyAttackSpeed();
            CoroutineUtils.Instance.StartCoroutine(DoCountTime());
        }

        private IEnumerator DoCountTime()
        {
            TimeOut = false;
            yield return new WaitForSeconds(Passive2Data.aoeDuration);
            TimeOut = true;
        }
    }
}