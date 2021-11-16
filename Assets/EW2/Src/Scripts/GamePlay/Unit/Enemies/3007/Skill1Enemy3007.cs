using System.Collections.Generic;
using System.Linq;
using Hellmade.Sound;
using Spine.Unity;
using UnityEngine;

namespace EW2
{
    [System.Serializable]
    public class Skill1Enemy3007 : SkillEnemy
    {
        [SerializeField] [SpineAnimation()] private string skill1AnimationName;
        [SerializeField] private AllyTargetCollection allyTargetCollection;

        private EnemyData3007.EnemyData3007Skill1 skill1Data;
        private Enemy3007 enemy;

        private List<EnemyBase> listEnemiesApply;

        public override bool CanCastSkill()
        {
            if (!enemy.IsAlive) return false;

            listEnemiesApply = new List<EnemyBase>();

            var baseResult = base.CanCastSkill();

            var listAllies = allyTargetCollection.CalculateAllTarget().Where(x => x.GetType() != typeof(Enemy3007));

            foreach (var ally in listAllies)
            {
                HealthPoint currentHp = ally.Stats.GetStat<RPGAttribute>(RPGStatType.Health) as HealthPoint;
                if (currentHp.CurrentValue < currentHp.StatValue)
                {
                    listEnemiesApply.Add(ally);
                }
            }

            return baseResult && listEnemiesApply.Count > 0;
        }

        public override void CastSkill()
        {
            if (!enemy.IsAlive) return;
            
            enemy.UnitSpine.AnimationState.SetAnimation(0, skill1AnimationName, false);
            //Sfx
            var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_3007_BUFF_HEAL);
            EazySoundManager.PlaySound(audioClip);
            base.CastSkill();
        }

        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);

            skill1Data = skill1Data ?? GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3007>().GetSkill1(enemyBase.Level);
            timeCooldown = skill1Data.cooldown;

            var circleCollider = allyTargetCollection.gameObject.GetComponent<CircleCollider2D>();
            circleCollider.radius = skill1Data.region;

            enemy = (Enemy3007) enemyBase;
        }

        public void ApplyEffectForAnyAllies()
        {
            allyTargetCollection.SetTargetType(MoveType.All);
            var anyAllies = listEnemiesApply.Take(skill1Data.maxUnit);

            foreach (var ally in anyAllies)
            {
                var status = new HealOverTime(new StatusOverTimeConfig()
                {
                    statusType = StatusType.HealInstantOverTime,
                    creator = null,
                    owner = ally,
                    lifeTime = 1,
                    baseValue = skill1Data.valueHp
                });

                ally.StatusController.AddStatus(status);
            }
        }
    }
}