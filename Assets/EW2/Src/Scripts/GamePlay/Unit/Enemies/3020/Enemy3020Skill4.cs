using System.Collections;
using UnityEngine;

namespace EW2
{
    public class Enemy3020Skill4 : SkillEnemy
    {
        public float timeStun = 0;
        private Enemy3020 enemy;
        private EnemyData3020.EnemyData3020Skill4 skillData;

        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);

            enemy = (Enemy3020) enemyBase;
            if (skillData == null)
                skillData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3020>().GetSkill4(enemyBase.Level);
            timeCooldown = skillData.cooldown;
            timeStun = skillData.timeStun;
        }

        public override void CastSkill()
        {
            base.CastSkill();
            enemy.ImpactSkill4.InitImpactSkill(enemy, skillData);
        }
    }
}