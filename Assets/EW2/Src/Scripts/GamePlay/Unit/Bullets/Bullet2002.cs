using UnityEngine;

namespace EW2
{
    public class Bullet2002 : BaseBullet
    {
        private Tower2002BulletData data;

        private float damageReduce = 0;

        private float damageRate;

        private float rawDamage;

        private float countTime;

        private Vector3 direction;

        private bool IsAlive => damageRate > 0 && countTime < data.lifeTimeBullet;

        public override void OnUpdate(float deltaTime)
        {
            if (IsAlive == false)
            {
                Despawn();

                return;
            }

            var step = this.direction * deltaTime * data.bulletSpeed;

            transform.position += step;

            countTime += deltaTime;
        }

        public void Init(Soldier2002 creator, Unit target, Tower2002BulletData bulletData)
        {
            Init(creator, target);

            this.data = bulletData;

            this.rawDamage = damageInfo.value;

            this.damageRate = 1;

            this.countTime = 0;

            this.damageReduce = creator.Stats.GetStat<DamageReduceBullet>(RPGStatType.DamageReduceBullet).StatValue;

            this.direction = (target.transform.position - creator.pointSpawnBullet.position).normalized;
        }

        public override DamageInfo GetDamage(Unit target)
        {
            SpawnImpact();
            
            damageInfo.value = this.rawDamage * damageRate;

            damageRate -= this.damageReduce;

            return base.GetDamage(target);
        }

        private void SpawnImpact()
        {
            //explosion on level 6
            var o = (Soldier2002) owner;
            var tower2002 = o.Tower;

            if (tower2002.towerData.BonusStat.level == 6)
            {
                var explosion = ResourceUtils.GetVfxTower("2002_impact_skill6", transform.position, Quaternion.identity);
                //0.5radius = 1scale
                //x radius = y scale=>y=x/0.5
                explosion.transform.localScale = tower2002.towerData.BonusStat.level6Stat.range * Vector3.one;
                var impact = explosion.GetComponent<ExplosionImpact>();

                impact.Initialize(owner, tower2002.towerData.BonusStat.level6Stat.damage); 
                impact.Trigger(0.1f, 0.1f);
            }
            else
            {
                ResourceUtils.GetVfxTower("2002_attack_range_1_impact", transform.position, Quaternion.identity);
            }
        }
    }
}