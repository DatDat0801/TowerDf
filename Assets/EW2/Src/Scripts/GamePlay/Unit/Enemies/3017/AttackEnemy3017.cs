using System.Collections.Generic;
using Lean.Pool;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;


namespace EW2
{
    [System.Serializable]
    public class AttackEnemy3017 : SkillEnemy
    {
        public float rangeAttackMelee;
        
        [SerializeField] private EnemyTargetCollection enemyTargetCollection;
       
        private Enemy3017 enemy;
        
        public override void Init(EnemyBase enemyBase)
        {
            base.Init(enemyBase);
            enemy = (Enemy3017)enemyBase;
        }
        
        public  List<Dummy> CalculateAllEnemy()
        {
            return CalculateAllTarget();
        }

        public override void CastSkill()
        {
            enemy.AttackMelee();
        }
        
        private List<Dummy> CalculateAllTarget()
        {
            var  anyTargets = new List<Dummy>();
            foreach (var target in enemyTargetCollection.CalculateAllTarget())
            {
                if (target.IsAlive && target.gameObject.activeInHierarchy)
                {
                    anyTargets.Add(target);
                }
            }
            return anyTargets;
        }
    }
}