using Cysharp.Threading.Tasks;
using Hellmade.Sound;
using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Soldier2004Spine : DummySpine
    {
        public Soldier2004Spine(Unit owner) : base(owner)
        {
            activeSkillName = "skill_active_1";
        }

        private int countAttack = 0;

        public override TrackEntry AttackMelee()
        {
            countAttack++;
            if (countAttack <= 2)
            {
                attackMeleeName = "attack_melee_1";
                return SetAnimation(0, attackMeleeName, false);
            }

            else
            {
                if (countAttack > 2)
                    countAttack = 0;

                attackMeleeName = "attack_melee_2";
                return SetAnimation(0, attackMeleeName, false);
            }
        }

        public override TrackEntry SkillAttack()
        {
            return SetAnimation(0, activeSkillName, false);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            var soldier = (Soldier2004) owner;

            switch (e.Data.Name)
            {
                case "attack_melee":

                    if (trackEntry.Animation.Name.Equals(attackMeleeName))
                    {
                        var idSoundRandom = Random.Range(1, 6);
                        var soundName = string.Format(SoundConstant.Tower2004Cast, idSoundRandom);
                        EazySoundManager.PlaySound(ResourceSoundManager.GetSoundTower(soldier.Tower.Id,soundName), EazySoundManager.GlobalSoundsVolume);
                        soldier.normalAttackBox.Trigger();
                    }

                    break;

                case "skill_active":
                    if (trackEntry.Animation.Name.Equals(activeSkillName))
                    {
                        EazySoundManager.PlaySound(ResourceSoundManager.GetSoundTower(soldier.Tower.Id,SoundConstant.Tower2004CastSkill), EazySoundManager.GlobalSoundsVolume);
                        soldier.passiveAttackBox.Trigger();
                    }

                    break;
            }
        }
    }
}