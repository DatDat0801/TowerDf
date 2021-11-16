using System.Collections.Generic;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;


namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3009 : SkillEnemy
    {
        [SerializeField] private Transform _vfxSkill1SpawnTransform;
        [SerializeField] private string _vfxSkill1Name;
        
        private EnemyData3009.EnemyData3009Skill1 skill1Data;
        private Enemy3009 enemy;
       

        
        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            skill1Data = skill1Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3009>().GetSkill1(enemyBase.Level);
            enemy = (Enemy3009)enemyBase;
        }

      

        public StunStatus CalculateStunStatus(Unit target)
        {
            var stunStatus = new StunStatus(new StatusOverTimeConfig()
            {
                creator = enemy,
                owner = target,
                lifeTime = skill1Data.secondApply,
                chanceApply = skill1Data.percentChanceApply
            });//{SpawningAnyVfxStatusWhenExecute = SpawnSkill1Vfx};
            return stunStatus;
        }
        
        private void SpawnSkill1Vfx()
        {
            var vfxClone= ResourceUtils.GetVfx(EffectType.Enemy.ToString(), _vfxSkill1Name, _vfxSkill1SpawnTransform.position, Quaternion.identity);
            vfxClone.transform.SetParent(_vfxSkill1SpawnTransform);
            vfxClone.transform.localScale = Vector3.one;
            vfxClone.transform.localRotation = Quaternion.identity;
        }
       
    }
}