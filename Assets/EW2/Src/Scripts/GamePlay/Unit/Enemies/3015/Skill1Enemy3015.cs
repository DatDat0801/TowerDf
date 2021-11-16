using System;
using UnityEngine;

namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3015 : SkillEnemy
    {
        [SerializeField] private Transform vfxSkill1SpawnTransform;
        [SerializeField] private string vfxSkill11Name;
        [SerializeField] private string vfxSkill12Name;

        private EnemyData3015.EnemyData3015Skill1 skill1Data;
        private Enemy3015 enemy;


        public override void Init(EnemyBase e)
        {
            base.Init(e);
            skill1Data = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3015>().GetSkill1(e.Level);
            enemy = (Enemy3015) e;
        }


        public StunStatus CalculateStunStatus(Unit target)
        {
            var stunStatus = new StunStatus(new StatusOverTimeConfig()
            {
                creator = enemy,
                owner = target,
                lifeTime = skill1Data.secondApply,
                chanceApply = skill1Data.percentChanceApply
            });
            stunStatus.SpawningAnyVfxStatusWhenExecute = SpawnSkill1Vfx;
            return stunStatus;
        }

        private void SpawnSkill1Vfx()
        {
            //var position = vfxSkill1SpawnTransform.position;
            //var vfx1Clone = ResourceUtils.GetVfx(EffectType.Enemy.ToString(), vfxSkill11Name, position,
            //    Quaternion.identity);
            // Debug.LogAssertion(vfxSkill1SpawnTransform.position);
            var vfx2Clone = ResourceUtils.GetVfx(EffectType.Enemy.ToString(), vfxSkill12Name, vfxSkill1SpawnTransform.position,
                Quaternion.identity);
            var localScale = vfxSkill1SpawnTransform.localScale;
            //vfx1Clone.transform.localScale = localScale;
            //vfx1Clone.transform.localRotation = Quaternion.identity;
            vfx2Clone.transform.localScale = localScale;
            vfx2Clone.transform.localRotation = Quaternion.identity;
        }
    }
}