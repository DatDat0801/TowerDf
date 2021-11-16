using System.Collections.Generic;
using UnityEngine;

namespace EW2
{
    public class Enemy3020Skill2 : SkillEnemy
    {
        private Enemy3020 enemy;
        private EnemyData3020.EnemyData3020Skill2 skillData;
        private Enemy3020Spine enemySpine;
        private List<Dummy> targets = new List<Dummy>();

        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);

            enemy = (Enemy3020) enemyBase;
            enemySpine = (Enemy3020Spine) enemy.UnitSpine;
            if (skillData == null)
                skillData = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3020>().GetSkill2(enemyBase.Level);
            timeCooldown = skillData.cooldown;
        }

        public override void CastSkill()
        {
            base.CastSkill();

            foreach (var target in targets)
            {
                var impact = ResourceUtils.GetVfx("Enemy", "3020_skill_2_impact", target.transform.position,
                    Quaternion.identity);

                if (impact != null)
                {
                    impact.GetComponent<Enemy3020Skill2Impact>().InitImpact(enemy, skillData);
                }
            }
        }

        public override bool CanCastSkill()
        {
            if (base.CanCastSkill())
            {
                if (CalculateAllTarget().Count != 0)
                {
                    targets.Clear();

                    targets.AddRange(CalculateAllTarget());

                    return true;
                }
            }

            return false;
        }

        private List<Dummy> CalculateAllTarget()
        {
            var anyTargets = new List<Dummy>();
            foreach (var target in enemy.rangeSearchTarget.Targets)
            {
                if (anyTargets.Count == skillData.numberTarget)
                    break;

                if (target.IsAlive && target.gameObject.activeInHierarchy)
                {
                    if (target is Soldier2004)
                    {
                        if (CheckSoldierSame((Soldier2004) target, anyTargets))
                            continue;
                    }

                    anyTargets.Add(target);
                }
            }

            return anyTargets;
        }

        private bool CheckSoldierSame(Soldier2004 soldier2004, List<Dummy> listFilter)
        {
            foreach (var target in listFilter)
            {
                if (target is Soldier2004)
                {
                    if (((Soldier2004) target).Tower.GetInstanceID() == soldier2004.Tower.GetInstanceID()) return true;
                }
            }

            return false;
        }
    }
}