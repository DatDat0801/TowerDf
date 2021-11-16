using Lean.Pool;
using UnityEngine;

namespace EW2.Spell
{
    public class SpellDamageImpactBase: MonoBehaviour, IGetDamage
    {
        protected Unit owner;
        private Coroutine delayTrigger;
        [SerializeField]
        protected Collider2D damageBoxAOE;
        public virtual DamageInfo GetDamage(Unit target)
        {
            throw new System.NotImplementedException();
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