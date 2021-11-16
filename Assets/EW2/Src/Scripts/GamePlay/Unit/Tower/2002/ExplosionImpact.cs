using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class ExplosionImpact : MonoBehaviour, IGetDamage
    {
        protected Unit owner;
        private Coroutine delayTrigger;
        [SerializeField]
        protected Collider2D damageBoxAOE;

        private float explosionDamage;

        public void Initialize(Unit unit, float damage)
        {
            explosionDamage = damage;
            owner = unit;
        }
        
        public DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo
            {
                creator = owner,

                damageType = owner.DamageType,

                value = explosionDamage
            };

            return damageInfo;
        }
        public virtual void Trigger(float time = 0, float delayTime = 0)
        {
            if (delayTrigger != null)
                CoroutineUtils.Instance.StopCoroutine(delayTrigger);
            
            delayTrigger = CoroutineUtils.Instance.DelayTriggerCollider(damageBoxAOE, time, delayTime);
        }
        protected void RemoveFx()
        {
            LeanPool.Despawn(this);
        }
    }
}