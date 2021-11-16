using System;
using Cysharp.Threading.Tasks;
using Invoke;
using UnityEngine;

namespace EW2
{
    public class Tower2003Skill1Impact : MonoBehaviour
    {
        public Transform goCrack;
        public Transform boostedGoCrack;

        public ParticleSystem mainEffect;

        public Tower2003CrackTrigger[] crackTriggers;
        public Tower2003CrackTrigger[] boostedCrackTrigger;

        private TowerData2003.Skill1 dataSkill;
        
        public Tower2003SkillSlowTrigger slowTrigger;

        public async void InitImpact(Tower2003 shooter, TowerData2003.Skill1 data2003SkillActive, Unit target)
        {
            var angle = 0f;

            if (target != null)
            {
                angle = CaculateAngle(target.Transform.position);
            }
            
            dataSkill = data2003SkillActive;
            var normalDamage = 0f;
            //add from tower upgrade system
            var level = shooter.towerData.BonusStat.level;
            //level 5
            if (level >= 5)
            {
                var bonusDamage = shooter.towerData.BonusStat.bonusDamageSkill1;
                normalDamage = dataSkill.damage * (1 + bonusDamage);
            }
            else
            {
                normalDamage = dataSkill.damage;
            }

            //level 6
            if (level >= 6)
            {
                boostedGoCrack.gameObject.SetActive(true);
                goCrack.gameObject.SetActive(false);
                
                ResetVfx(6);
                boostedGoCrack.rotation = Quaternion.Euler(0, 0, angle);
                
                foreach (var boostedCrack in boostedCrackTrigger)
                {
                    boostedCrack.InitTrigger(shooter, normalDamage);
                    //Debug.LogAssertion("<color=green>Trigger first damage</color>");
                }

                await UniTask.Delay(1000);
                foreach (var boostedCrack in boostedCrackTrigger)
                {
                    boostedCrack.InitTrigger(shooter, shooter.towerData.BonusStat.level6Stat.secondDamage);
                    //Debug.LogAssertion("<color=orange>Trigger second damage</color>");
                }
                slowTrigger.InitTrigger(shooter, shooter.towerData.BonusStat.level6Stat.slowTime, shooter.towerData.BonusStat.level6Stat.slowDown, ModifierType.TotalPercent);
            }
            else
            {
                boostedGoCrack.gameObject.SetActive(false);
                goCrack.gameObject.SetActive(true);
                
                ResetVfx(1);
                goCrack.rotation = Quaternion.Euler(0, 0, angle);
                
                foreach (var crack in crackTriggers)
                {
                    crack.InitTrigger(shooter, normalDamage);
                }
            }

        }

        
        private float CaculateAngle(Vector3 targetPos)
        {
            var angleCrack = 30f;

            var dir = transform.position - targetPos;

            angleCrack = (float) (Math.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            if (angleCrack < 0)
            {
                angleCrack = 180 + angleCrack;
            }
            else if (angleCrack > 0)
            {
                angleCrack = -180 + angleCrack;
            }

            return angleCrack;
        }

        private void ResetVfx(int level)
        {
            mainEffect.Simulate(0, true, true);
            mainEffect.Play(true);
            if (level >= 6)
            {
                boostedGoCrack.GetComponent<ParticleSystem>().Simulate(0, true, true);
                boostedGoCrack.GetComponent<ParticleSystem>().Play(true);
            }
            else
            {
                goCrack.GetComponent<ParticleSystem>().Simulate(0, true, true);
                goCrack.GetComponent<ParticleSystem>().Play(true);
            }
        }
    }
}