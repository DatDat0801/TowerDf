using System.Collections.Generic;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;


namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3011 : SkillEnemy
    {
        private EnemyData3011.EnemyData3011Skill1 skill1Data;
        private Enemy3011 enemy;
       

        
        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            skill1Data = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3011>().GetSkill1(enemyBase.Level);
            enemy = (Enemy3011)enemyBase;
        }

      

        public BleedStatus CalculateBleedStatus(Unit target)
        {
            var bleedStatus=new BleedStatus(new StatusOverTimeConfig()
            {
                creator = enemy,
                owner = target,
                lifeTime = skill1Data.secondApply,
                baseValue = skill1Data.hpLost,
            }){
                Stacks = false
            };;
            
            return bleedStatus;
        }
        
        
       
    }
}