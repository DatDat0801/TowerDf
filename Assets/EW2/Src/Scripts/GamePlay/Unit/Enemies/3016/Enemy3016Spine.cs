using Hellmade.Sound;
using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3016Spine : DummySpine
    {
        private Enemy3016 enemy;

        //private const string fxAnimation = "fx";
        private const string skill1Name = "skill_1";
        private const string skill2Name = "skill_2";

        public Enemy3016Spine(Unit owner) : base(owner)
        {
            appearName = "appear";

            idleName = "idle";

            moveName = "move";

            dieName = "die";

            stunName = "idle";

            attackMeleeName = "attack_melee_1";

            attackRangeName = "attack_range_1";

            enemy = (Enemy3016) owner;
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "attack_range":
                    if (trackEntry.Animation.Name.Equals(attackRangeName))
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
                //Debug.LogAssertion("Track Entry Melee");
                // var go = ResourceUtils.GetVfx("Enemy", "3016_attack_melee", enemy.bulletSpawnPos.position, Quaternion.identity,
                //     enemy.attackMeleePos);
                var go = ResourceUtils.GetVfx("Enemy", "3016_attack_melee", enemy.attackMeleePos.position,
                    Quaternion.identity);
                //go.transform.localScale = Vector3.one * 4;
                go.transform.localScale = enemy.transform.localScale;
                enemy.normalAttackBox1.Trigger(0f, 0.56f);
            }

            return entry;
        }

        public override TrackEntry SkillAttack()
        {
            if (owner.IsAlive == false)
                return null;

            return SetAnimation(0, skill1Name, false);
        }

        public TrackEntry Skill2Attack()
        {
            if (owner.IsAlive == false)
                return null;

            return SetAnimation(0, skill2Name, false);
        }

        public override TrackEntry Die()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_3016_DIE);
            EazySoundManager.PlaySound(audioClip);

            return base.Die();
        }
    }
}