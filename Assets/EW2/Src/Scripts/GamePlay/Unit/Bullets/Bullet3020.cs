using Invoke;
using UnityEngine;

namespace EW2
{
    public class Bullet3020 : BaseBullet
    {
        private Vector3 direction;
        private float bulletSpeed = 10f;
        private float damageRate;

        public override void OnUpdate(float deltaTime)
        {
            var step = this.direction * deltaTime * bulletSpeed;

            transform.position += step;
        }

        public void InitBullet(Enemy3020 creator, Unit target)
        {
            Init(creator, target);

            this.direction = (target.transform.position - transform.position).normalized;

            this.damageRate = GameContainer.Instance.Get<UnitDataBase>().Get<EnemyData3020>()
                .GetDamageRate(creator.Level).damageRateRangePhase3;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            InvokeProxy.Iinvoke.Invoke(this, Despawn, 0.05f);

            SpawnImpact();

            damageInfo.value *= damageRate;

            return base.GetDamage(target);
        }

        private void SpawnImpact()
        {
            ResourceUtils.GetVfx("Enemy","3020_range_impact", transform.position, Quaternion.identity);
        }
    }
}