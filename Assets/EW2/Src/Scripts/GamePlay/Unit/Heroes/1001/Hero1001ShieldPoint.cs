using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Hero1001ShieldPoint : ShieldPoint
    {
        private GameObject shieldVfx;

        private GameObject shieldBar;

        public Hero1001ShieldPoint(StatusOverTimeConfig config) : base(config)
        {
            // do nothing 
        }

        public override void Prepare()
        {
            RemoveShield();

            RemoveShieldBar();

            shieldVfx = ResourceUtils.GetVfxHero("1001_skill_passive_3_shield", Vector3.zero, Quaternion.identity,
                config.owner.Transform);

            var dummy = config.owner as Dummy;

            if (dummy != null)
            {
                var transform = dummy.HealthBar.transform;

                var posHealthBar = transform.localPosition;

                var posShieldBar = new Vector3(0, posHealthBar.y + (0.1f * transform.localScale.y), 0);
                if (dummy.StatusBar != null)
                    shieldBar = dummy.StatusBar.AddShieldBar();

                if (shieldBar)
                {
                    shieldBar.GetComponent<ShieldBarController>().SetShieldBar(this);

                    dummy.HealthBar.Show();
                }
            }
        }

        public override void Remove()
        {
            RemoveShield();

            RemoveShieldBar();

            base.Remove();
        }

        private void RemoveShield()
        {
            if (shieldVfx)
            {
                LeanPool.Despawn(shieldVfx);

                shieldVfx = null;
            }
        }

        private void RemoveShieldBar()
        {
            if (shieldBar)
            {
                LeanPool.Despawn(shieldBar);
                ((Dummy)config.owner).StatusBar.RemoveIconStatus(StatusType.ShieldPoint);
                shieldBar = null;
            }
        }
    }
}