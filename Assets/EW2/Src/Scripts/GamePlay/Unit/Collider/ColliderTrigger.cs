using UnityEngine;

namespace EW2
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class ColliderTrigger<T> : MonoBehaviour where T : Unit
    {
        [SerializeField] protected T owner;

        [SerializeField] protected Collider2D collider2D;

        [SerializeField] protected bool isAoe;

        private Coroutine delayTrigger;
        

        protected virtual void OnEnable()
        {
            collider2D.enabled = false;
        }

        public virtual void SetOwner(T owner)
        {
            this.owner = owner;
        }

        public virtual void Trigger(float time = 0, float delayTime = 0)
        {
            if (delayTrigger != null)
                CoroutineUtils.Instance.StopCoroutine(delayTrigger);
            
            delayTrigger = CoroutineUtils.Instance.DelayTriggerCollider(collider2D, time, delayTime);
        }
    }
}