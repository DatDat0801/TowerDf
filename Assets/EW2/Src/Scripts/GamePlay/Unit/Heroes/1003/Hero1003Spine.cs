using System.Collections.Generic;
using Hellmade.Sound;
using Spine;
using UnityEngine;
using UnityEngine.Events;
using Event = Spine.Event;

namespace EW2
{
    public class Hero1003Spine : DummySpine
    {
        private Hero1003 hero;
        private readonly string attackMelee1Form2;
        private readonly string attackMelee2Form2;
        private readonly string attackMelee3Form2;
        private readonly string dieForm2;
        private readonly string idleFrom2;
        private readonly string moveForm2;
        private readonly string transitionForm2ToForm1;
        private readonly string passive1Form2;

        private string passive2Form2;
        //private string passive3Form2;

        //form 1
        private string activeSkill2Form1;
        private readonly string attackMelee2Form1;

        public Hero1003Spine(Unit owner) : base(owner)
        {
            //form 1
            attackMeleeName = "form_1/attack_melee";
            attackMelee2Form1 = "form_1/attack_melee_1";
            dieName = "form_1/die";
            activeSkillName = "form_1/skill_active";
            activeSkill2Form1 = "form_1/skill_active2";
            passive1Name = "form_1/skill_passive_1";
            passive2Name = "form_1/skill_passive_2";
            //passive3Name = "form_1/skill_passive_3";
            idleName = "form_1/idle";
            moveName = "form_1/move";
            //form 2
            attackMelee1Form2 = "form_2/attack_melee_1_form_2";
            attackMelee2Form2 = "form_2/attack_melee_2_form_2";
            attackMelee3Form2 = "form_2/attack_melee_3_form_2";
            dieForm2 = "form_2/die_form_2";
            idleFrom2 = "form_2/idle_form_2";
            moveForm2 = "form_2/move_form_2";
            transitionForm2ToForm1 = "form_2/idle_form_2_to_form_1";
            passive1Form2 = "form_2/skill_passive_1_form_2";
            passive2Form2 = "form_2/skill_passive_2_form_2";
            hero = (Hero1003) owner;
        }
        
        string GetRandomMeleeForm2()
        {
            var i = Random.Range(0, 3);
            List<string> meleeNames = new List<string> {attackMelee1Form2, attackMelee2Form2, attackMelee3Form2};
            return meleeNames[i];
        }

        public override TrackEntry AttackMelee()
        {
            if (owner.IsAlive == false)
                return null;
            //Debug.LogAssertion("Unite State: "+ hero.UnitState.Current);
            ClearTrack(0);
            if (hero.FormState == Hero1003.Form.Form1)
            {
                //Random.Range(1, 3) == 1 ? attackMeleeName : attackMelee2Form1
                var animationName = Random.Range(1, 3) == 1 ? attackMeleeName : attackMelee2Form1;
                hero.ResetTimeTriggerAttackMelee(animationName);
                var entry = SetAnimation(0, animationName, false);
                //var entry = SetAnimation(0, attackMeleeName, false);
                entry.MixDuration = 0.1f;
                entry.MixTime = 0.1f;
                //Debug.LogAssertion("attack melee form 1 " + entry + "Time: " + Time.time);
                return entry;
            }
            else
            {
                var animationName = GetRandomMeleeForm2();
                hero.ResetTimeTriggerAttackMelee(animationName);
                var entry = SetAnimation(0, animationName, false);
                //entry.Reset();
                entry.MixTime = 0.2f;
                entry.MixDuration = 0.2f;
                //Debug.LogAssertion("attack melee form 2 " + entry+ "Time: " + Time.time);
                return entry;
            }
        }

        public override TrackEntry Move()
        {
            if (owner.IsAlive == false)
                return null;
            if (hero.FormState == Hero1003.Form.Form1)
            {
                return SetAnimation(0, moveName, true);
            }
            else
            {
                return SetAnimation(0, moveForm2, true);
            }
        }


        public override TrackEntry Die()
        {
            if (hero.FormState == Hero1003.Form.Form1)
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_DIE_FORM_1);
                EazySoundManager.PlaySound(audioClip);
                var entry = SetAnimation(0, dieName, false);
                entry.MixDuration = 0.2f;
                return entry;
            }
            else
            {
                var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_DIE_FORM_2);
                EazySoundManager.PlaySound(audioClip);
                var entry = SetAnimation(0, dieForm2, false);

                return entry;
            }
        }

        public override TrackEntry Idle()
        {
            if (owner.IsAlive == false)
                return null;
            //ClearTrack(0);
            if (hero.FormState == Hero1003.Form.Form1)
            {
                var entry = SetAnimation(0, idleName, true);
                entry.MixTime = 0.1f;
                return entry;
            }
            else
            {
                var current = AnimationState.GetCurrent(0);
                if (current.Animation.Name.Equals(activeSkillName))
                {
                    var entry = SetAnimation(0, idleFrom2, true);
                    entry.MixTime = 0.2f;
                    return entry;
                }
                else
                {
                    var entry = SetAnimation(0, idleFrom2, true);
                    entry.MixTime = 0.1f;
                    return entry;
                }
            }
        }

        public override TrackEntry Stun()
        {
            if (owner.IsAlive == false)
                return null;
            if (hero.FormState == Hero1003.Form.Form1)
            {
                var entry = SetAnimation(0, idleName, true);
                
                entry.MixTime = 0.1f;
                return entry;
            }
            else
            {
                var current = AnimationState.GetCurrent(0);
                if (current.Animation.Name.Equals(activeSkillName))
                {
                    var entry = SetAnimation(0, idleFrom2, true);
                    entry.MixTime = 0.2f;
                    return entry;
                }
                else
                {
                    var entry = SetAnimation(0, idleFrom2, true);
                    entry.MixTime = 0.1f;
                    return entry;
                }
            }
        }

        public TrackEntry TurnIntoForm1(UnityAction OnComplete)
        {
            if (owner.IsAlive == false)
                return null;
            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_FORM2_TO_FORM1);
            EazySoundManager.PlaySound(audioClip);
            var entry = SetAnimation(0, transitionForm2ToForm1, false);
            entry.MixTime = 0.2f;
            entry.Complete += trackEntry => { OnComplete?.Invoke(); };
            return entry;
        }

        public override TrackEntry SkillAttack()
        {
            if (owner.IsAlive == false)
                return null;

            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_FORM1_TO_FORM2);
            EazySoundManager.PlaySound(audioClip);
            var entry = SetAnimation(0, activeSkillName, false);
            
            entry.MixTime = 0.2f;
            return entry;
        }


        public override TrackEntry SkillPassive1()
        {
            if (owner.IsAlive == false)
                return null;
            hero.ExecutePassiveSkill1();
            if (hero.FormState == Hero1003.Form.Form1)
            {
                var entry = SetAnimation(0, passive1Name, false); 
                entry.MixTime = 0.1f;
                var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_PASSIVE_1_FORM_1);
                EazySoundManager.PlaySound(audioClip);
                return entry;
            }
            else
            {
                var entry = SetAnimation(0, passive1Form2, false);
                entry.MixTime = 0.1f;
                var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_PASSIVE_1_FORM_2);
                EazySoundManager.PlaySound(audioClip);
                return entry;
            }
        }

        public override TrackEntry SkillPassive2()
        {
            if (owner.IsAlive == false)
                return null;

            var audioClip = ResourceUtils.LoadSound(SoundConstant.HERO_1003_PASSIVE_2);
            EazySoundManager.PlaySound(audioClip);

            if (hero.FormState == Hero1003.Form.Form1)
            {
                var entry = SetAnimation(0, passive2Name, false);
                entry.MixTime = 0.2f;
                return entry;
            }
            else
            {
                var entry = SetAnimation(0, passive2Form2, false);
                entry.MixTime = 0.2f;
                return entry;
            }
        }


        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            switch (e.Data.Name)
            {
                case "dameged":

                    if (trackEntry.Animation.Name.Equals(attackMeleeName) ||
                        trackEntry.Animation.Name.Equals(attackMelee2Form1))
                    {
                        var audioClip = ResourceUtils.LoadSound(SoundConstant.Hero1003BasicAttackForm1());
                        EazySoundManager.PlaySound(audioClip);
                        hero.normalAttackBox.Trigger();
                    }
                    else if (trackEntry.Animation.Name.Equals(attackMelee1Form2) ||
                             trackEntry.Animation.Name.Equals(attackMelee2Form2) ||
                             trackEntry.Animation.Name.Equals(attackMelee3Form2))
                    {
                        var audioClip = ResourceUtils.LoadSound(SoundConstant.Hero1003BasicAttackForm2());
                        EazySoundManager.PlaySound(audioClip);
                        hero.normalAttackBox.Trigger();
                    }
                    else if (trackEntry.Animation.Name.Equals(passive2Name) ||
                             trackEntry.Animation.Name.Equals(passive2Form2))
                    {
                        hero.counterAttackBox.Trigger();
                    }

                    break;
            }
        }
    }
}