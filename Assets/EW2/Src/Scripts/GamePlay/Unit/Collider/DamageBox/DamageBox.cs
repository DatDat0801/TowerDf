using UnityEngine;

namespace EW2
{
    public abstract class DamageBox<T> : ColliderTrigger<T>, IGetDamage, IUnitFlipByLocalPosition where T : Unit
    {
        protected abstract bool CanGetDamage(Unit target);

        public abstract DamageInfo GetDamage(Unit target);
        public Vector3 LocalPosition
        {
            get => transform.localPosition;
            set => this.transform.localPosition = value;
        }

        public string Name { get=>name; }
    }
}