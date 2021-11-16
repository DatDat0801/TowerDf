using Hellmade.Sound;
using Spine;

namespace EW2.Spell
{
    public class Warrior4004Spine : DummySpine
    {
        private readonly string _attackMelee1;
        private readonly Warrior4004 _warrior4004;
        public Warrior4004Spine(Unit owner) : base(owner)
        {
            this.attackMeleeName = "attack_melee_1";
            this._attackMelee1 = "attack_melee_2";
            this._warrior4004 = (Warrior4004)owner;
        }

        private string _lastAttackMelee = "attack_melee_1";
        

        public override TrackEntry AttackMelee()
        {
            if (owner.IsAlive == false)
                return null;
            //Debug.LogAssertion("Unite State: "+ hero.UnitState.Current);
            //ClearTrack(0);

            //Random.Range(1, 3) == 1 ? attackMeleeName : attackMelee2Form1
            var animationName = this._lastAttackMelee == this.attackMeleeName
                ? this._attackMelee1
                : this.attackMeleeName;
            _lastAttackMelee = animationName;
            this._warrior4004.ResetTimeTriggerAttackMelee(animationName);
            var entry = SetAnimation(0, animationName, false);
            //var entry = SetAnimation(0, attackMeleeName, false);
            entry.MixDuration = 0.1f;
            entry.MixTime = 0.1f;
            return entry;
        }
        
        public override void HandleEvent(TrackEntry trackEntry, Event e)
        {
            // if (trackEntry.Animation.Name.Equals(passive1Form2) ||
            //     trackEntry.Animation.Name.Equals(passive1Name))
            // {
            //     hero.ExecutePassiveSkill1();
            // }

            switch (e.Data.Name)
            {
                case "attack_melee":

                    if (trackEntry.Animation.Name.Equals(attackMeleeName))
                    {
                        var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4004_MELEE_1);
                        EazySoundManager.PlaySound(audioClip);
                        this._warrior4004.attackMelee1Box.Trigger();
                    }
                    else if (trackEntry.Animation.Name.Equals(this._attackMelee1) )
                    {
                        var audioClip = ResourceUtils.LoadSound(SoundConstant.SPELL_4004_MELEE_2);
                        EazySoundManager.PlaySound(audioClip);
                        this._warrior4004.attackMelee2Box.Trigger();
                    }

                    break;
            }
        }
    }
}