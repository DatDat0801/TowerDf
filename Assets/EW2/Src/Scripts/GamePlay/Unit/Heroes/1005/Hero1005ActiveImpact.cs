using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Hero1005ActiveImpact : DamageBox<Hero1005>
    {
        private HashSet<Dummy> _enemiesGetHurt;
        //target, damageInfo
        public HashSet<Dummy> EnemiesGetHurt
        {
            get
            {
                if (this._enemiesGetHurt == null)
                {
                    this._enemiesGetHurt = new HashSet<Dummy>();
                }

                return this._enemiesGetHurt;
            }
        }
        public DamageInfo DamageInfo { get; private set; }
        
        //private GetDamageCalculation _damageCalculation;
        private HeroData1005.ActiveSkill _skillData;
        public void InitAOE(Hero1005 shooter)
        {
            owner = shooter;

            //this._damageCalculation = new GetDamageCalculation(owner);

            Trigger(0.1f);
            this._skillData = this.owner.ActiveSkill.SkillData;
            transform.localScale = (Vector3.one * this._skillData.range) / 1.5f;
        }


        protected override bool CanGetDamage(Unit target)
        {
            return isAoe && target.IsAlive;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            if (CanGetDamage(target) == false)
            {
                Debug.Log("Can't get damage");
                return null;
            }
            
            var damageInfo = new DamageInfo {
                creator = owner, damageType = owner.DamageType, showVfxNormalAtk = true, value = this._skillData.damage, target = target
            };
            

            //(damageInfo.value, damageInfo.isCritical) = this._damageCalculation.Calculate(target);
            this.owner.DoSkillPassive3(target as EnemyBase);
            
            EnemiesGetHurt.Add((Dummy)target);
            DamageInfo = damageInfo;
            //passive 1
            if (target is EnemyBase enemyBase)
            {
                var hero5 = (Hero1005)owner;
                hero5.AddEnemyToTrack(enemyBase);
            }

            return damageInfo;
        }
        /// <summary>
        /// Reset enemies get damage by this active skill
        /// </summary>
        public void ResetEnemyGetDamageByThisActive()
        {
            EnemiesGetHurt.Clear();
        }
    }
}