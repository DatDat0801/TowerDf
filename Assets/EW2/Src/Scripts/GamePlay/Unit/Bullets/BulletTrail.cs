using Invoke;
using UnityEngine;

namespace EW2
{
    public abstract class BulletTrail : BaseBullet
    {
        public GameObject[] trails;

        public override void OnEnable()
        {
            base.OnEnable();

            HideTrail();

            InvokeProxy.Iinvoke.Invoke(this, ShowTrail, 0.03f);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            HideTrail();
        }

        public override void Despawn()
        {
            HideTrail();

            base.Despawn();
        }

        protected abstract void ShowTrail();

        protected virtual void HideTrail()
        {
            foreach (var trail in trails)
            {
                if (trail && trail.activeSelf)
                {
                    trail.GetComponent<TrailRenderer>().Clear();
                }
            }
        }
        
    }
}