using Hellmade.Sound;
using Spine;
using UnityEngine;
using Event = Spine.Event;

namespace EW2
{
    public class Hero1001Spine : DummySpine
    {
        Hero1001 hero;
        private string _lastAttackMelee = "attack_melee_1";
        private readonly string _attackMelee1Name;
        public Hero1001Spine(Unit owner) : base(owner)
        {
            attackMeleeName = "attack_melee_1";
            activeSkillName = "skill_active";
            passive1Name = "skill_passive_1";
            passive3Name = "skill_passive_3";
            _attackMelee1Name = "attack_melee_2";
            hero = (Hero1001) owner;
        }

        public override TrackEntry AttackMelee()
        {
            var animationName = this._lastAttackMelee == this.attackMeleeName
                ? this._attackMelee1Name
                : this.attackMeleeName;
            _lastAttackMelee = animationName;
            var entry = SetAnimation(0, animationName, false);
            entry.MixDuration = 0.1f;
            entry.MixTime = 0.1f;
            return SetTrackEntryTimeScaleBySpeed(ref entry);
        }

        public override TrackEntry SkillAttack()
        {
            return SetAnimation(0, activeSkillName, false);
        }

        public override TrackEntry SkillPassive1()
        {
            return SetAnimation(0, passive1Name, false);
        }

        public override TrackEntry SkillPassive3()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1001_PASSIVE_3);
            EazySoundManager.PlaySound(audioClip);
            return SetAnimation(0, passive3Name, false);
        }

        public override TrackEntry Die()
        {
            var audioClip = ResourceUtils.LoadSound(SoundConstant.ENEMY_1001_DIE);
            EazySoundManager.PlaySound(audioClip);
            return base.Die();
        }

        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "dameged":
                    
                    // if(trackEntry.Animation.Name.Equals(attackMeleeName))
                    // {
                    //     // var audioClip = ResourceUtils.LoadSound(SoundConstant.Hero1001Melee());
                    //     // EazySoundManager.PlaySound(audioClip);
                    //     // hero.normalAttackBox.Trigger();
                    // }else if (trackEntry.Animation.Name.Equals(activeSkillName))
                    // {
                    //     hero.activeSkill.Trigger();
                    // }else 
                    if (trackEntry.Animation.Name.Equals(passive1Name))
                    {
                        var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1001_PASSIVE_1);
                        EazySoundManager.PlaySound(audioClip);
                        hero.passiveSkill1.Trigger();
                    }
                    break;
                
                case "shake":
                    Debug.Log("Shake screen");
                    break;
                case "cancel_anim":
                    var audioClip1 = ResourceUtils.LoadSound(SoundConstant.Hero1001Melee());
                    EazySoundManager.PlaySound(audioClip1);
                    hero.normalAttackBox.Trigger();
                    break;
                case "skill_active":
                    hero.activeSkill.Trigger();
                    this.hero.EndTeleport();
                    var audioClip3 = ResourceUtils.LoadSound(SoundConstant.HERO_1001_SKILL_IMPACT);
                    EazySoundManager.PlaySound(audioClip3);
                    break;
                case "start":
                    this.hero.StartTeleport();
                    var audioClip2 = ResourceUtils.LoadSound(SoundConstant.HERO_1001_SKILL_JUMP);
                    EazySoundManager.PlaySound(audioClip2);
                    break;
                case "off":
                    this.hero.OnTeleport();
                    break;
                
            }
        }
    }
}