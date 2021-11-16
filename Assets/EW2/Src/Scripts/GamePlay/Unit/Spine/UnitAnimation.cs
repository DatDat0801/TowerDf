using Cysharp.Threading.Tasks;
using Spine;

namespace EW2
{
    public abstract class UnitAnimation
    {
        public string idleName,
            moveName,
            stunName,
            dieName,
            rebornName,
            attackMeleeName,
            attackRangeName,
            activeSkillName,
            passive1Name,
            passive2Name,
            passive3Name,
            passive4Name,
            appearName;


        public virtual TrackEntry Idle()
        {
            return null;
        }

        public virtual TrackEntry Move()
        {
            return null;
        }

        public virtual TrackEntry Stun()
        {
            return null;
        }

        public virtual TrackEntry Die()
        {
            return null;
        }

        public virtual TrackEntry Reborn()
        {
            return null;
        }

        public virtual TrackEntry AttackMelee()
        {
            return null;
        }

        public virtual TrackEntry AttackRange()
        {
            return null;
        }

        public virtual TrackEntry SkillAttack()
        {
            return null;
        }

        public virtual TrackEntry SkillPassive1()
        {
            return null;
        }

        public virtual TrackEntry SkillPassive2()
        {
            return null;
        }

        public virtual TrackEntry SkillPassive3()
        {
            return null;
        }

        public virtual TrackEntry SkillPassive4()
        {
            return null;
        }
        
        public virtual TrackEntry Appear()
        {
            return null;
        }
        
        public virtual TrackEntry Turn()
        {
            return null;
        }

        public virtual TrackEntry GetHurt()
        {
            return null;
        }
    }
}