using System;

namespace EW2
{
    public class DamageInfo : IEquatable<DamageInfo>
    {
        public DamageInfo()
        {
            creator = null;

            target = null;

            value = 1;

            isCritical = false;

            damageType = DamageType.Physical;
            
            attackType = AttackType.Melee;

            showVfxNormalAtk = false;
        }
        public Unit creator;
        
        public Unit target;

        public float value;
        
        public bool isCritical;

        public DamageType damageType;

        public AttackType attackType;

        public bool showVfxNormalAtk;


        public override string ToString()
        {
            if (creator != null)
            {
                return $"name: {creator.name}, value: {value}, crit: {isCritical}," +
                       $" damageType: {damageType}, attackType: {attackType}, normalAttack: {showVfxNormalAtk}";
            }
            
            return $"name: from not an unit, value: {value}, crit: {isCritical}," +
            $" damageType: {damageType}, attackType: {attackType}, normalAttack: {showVfxNormalAtk}";
        }


        public bool Equals(DamageInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(this.creator, other.creator) && this.damageType == other.damageType && this.attackType == other.attackType && this.showVfxNormalAtk == other.showVfxNormalAtk;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((DamageInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.creator != null ? this.creator.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)this.damageType;
                hashCode = (hashCode * 397) ^ (int)this.attackType;
                hashCode = (hashCode * 397) ^ this.showVfxNormalAtk.GetHashCode();
                return hashCode;
            }
        }
    }

    public interface IGetDamage
    {
        DamageInfo GetDamage(Unit target);
    }
}