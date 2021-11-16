using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Enemy3020Spine : DummySpine
    {
        private Enemy3020 enemy;

        public Enemy3020Spine(Unit owner) : base(owner)
        {
            enemy = (Enemy3020) owner;
            idleName = "Phase1/idle";
            stunName = idleName;
        }

        public void UpdateSpineName()
        {
            if (enemy.CurrentPhase == Phase3020.Phase1 || enemy.CurrentPhase == Phase3020.Phase3)
            {
                idleName = "Phase1/idle";
                stunName = idleName;
            }
            else
            {
                idleName = "Phase2/idle_2";
                stunName = idleName;
            }

            if (enemy.CurrentPhase == Phase3020.Phase1 || enemy.CurrentPhase == Phase3020.Phase3)
                moveName = "Phase3/move";
            else
                moveName = "Phase2/move_2";

            if (enemy.CurrentPhase == Phase3020.Phase3)
                attackMeleeName = "Phase3/attack";
            else if (enemy.CurrentPhase == Phase3020.Phase2)
                attackMeleeName = "Phase2/attack_2";

            attackRangeName = "Phase3/range";

            if (enemy.CurrentPhase == Phase3020.Phase3)
                dieName = "Phase3/die";
            else if (enemy.CurrentPhase == Phase3020.Phase2)
                dieName = "Phase2/die";

            passive1Name = "Phase1/skill_1";
            passive2Name = "Phase2/skill_2";
            passive3Name = "Phase2/skill_3";
            passive4Name = "Phase3/skill_4";
        }

        public override TrackEntry Idle()
        {
            return SetAnimation(0, idleName, true);
        }

        public override TrackEntry Die()
        {
            return SetAnimation(0, dieName, false);
        }

        public override TrackEntry Move()
        {
            return SetAnimation(0, moveName, true);
        }

        public override TrackEntry Stun()
        {
            if (owner.IsAlive == false)
                return null;

            return SetAnimation(0, stunName, true);
        }
        
        public override TrackEntry AttackMelee()
        {
            if (owner.IsAlive == false)
                return null;

            var trackEntry = SetAnimation(0, attackMeleeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public override TrackEntry AttackRange()
        {
            if (owner.IsAlive == false)
                return null;
            
            var trackEntry = SetAnimation(0, attackRangeName, false);
            return SetTrackEntryTimeScaleBySpeed(ref trackEntry);
        }

        public TrackEntry DissAppear()
        {
            return SetAnimation(0, "Phase2/die_dissapear", false);
        }

        public override TrackEntry SkillPassive1()
        {
            return SetAnimation(0, passive1Name, false);
        }

        public override TrackEntry SkillPassive2()
        {
            return SetAnimation(0, passive2Name, false);
        }

        public override TrackEntry SkillPassive3()
        {
            return SetAnimation(0, passive3Name, false);
        }

        public override TrackEntry SkillPassive4()
        {
            return SetAnimation(0, passive4Name, false);
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            var animationName = trackEntry.Animation.Name;
            var nameEvent = e.Data.Name;

            if (animationName.Equals(attackMeleeName) && nameEvent.Equals("damage"))
            {
                enemy.NormalAttackBox.Trigger();
            }

            if (animationName.Equals(attackRangeName) && nameEvent.Equals("damage"))
            {
                enemy.SpawnBullet();
            }
        }
    }
}