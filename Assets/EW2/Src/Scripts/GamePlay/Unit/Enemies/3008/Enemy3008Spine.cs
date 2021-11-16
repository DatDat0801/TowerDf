using Spine;
using Unity.Mathematics;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3008Spine : DummySpine
    {
        private Enemy3008 enemy;

        private const string skill1Name = "skill_1";

        public Enemy3008Spine(Unit owner) : base(owner)
        {
            appearName = "appear";
            
            idleName = "idle";
            
            moveName = "move";

            dieName = "die";

            stunName = "idle";

            attackMeleeName = "attack_melee_1";
            
            attackRangeName = "attack_range_1";
            
            enemy = (Enemy3008) owner;
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_range":
                    if(trackEntry.Animation.Name.Equals(attackRangeName))
                    {
                        enemy.SpawnBullet();
                    }
                    break;
            }
        }

        public override TrackEntry AttackMelee()
        {
            var entry = base.AttackMelee();
            if (entry.Animation.Name.Equals(attackMeleeName))
            {
                //3016_attack_melee
                //If we spawn this prefab not child of enemy, it not have correct rotation
                //ResourceUtils.GetVfx("Enemy", "3016_attack_melee", enemy.bulletSpawnPos.position, Quaternion.identity);
                var effect = ResourceUtils.GetVfx("Enemy", "3016_attack_melee", enemy.attackMeleePos.position,
                    Quaternion.identity);
                effect.transform.localScale = new Vector3(enemy.transform.localScale.x, enemy.transform.localScale.y, enemy.transform.localScale.z); ;
                enemy.normalAttackBox1.Trigger(0f,0.56f);
            }

            return entry;
        }

        public override TrackEntry SkillAttack()
        {
            if (owner.IsAlive == false)
                return null;

            return SetAnimation(0, skill1Name, false);
        }
        
    }
}