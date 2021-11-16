using Invoke;
using Lean.Pool;
using UnityEngine;

namespace EW2
{
    public class Hero1002SkillImpact : MonoBehaviour, IGetDamage
    {
        public ParticleSystem particleParent;
        public GameObject damageBoxAOE;
        private float _damage;
        private Hero1002 _owner;
        private HeroData1002.ActiveSkill _dataSkill;
        private bool _isSubImpact;

        public void Init(Hero1002 shooter, HeroData1002.ActiveSkill dataActiveSkill, bool isSubImpact)
        {
            this._owner = shooter;
            this._dataSkill = dataActiveSkill;
            this._damage = this._owner.Stats.GetStat<Damage>(RPGStatType.Damage).StatValue;
            this._isSubImpact = isSubImpact;
            damageBoxAOE.SetActive(true);
            damageBoxAOE.SetActive(false);
            // InvokeProxy.Iinvoke.Invoke(this, HideEffect, this._dataSkill.timeLife);
        }

        public DamageInfo GetDamage(Unit target)
        {
            var damageImpact = this._isSubImpact
                ? this._damage * this._dataSkill.damageBaseOnAtk * this._dataSkill.ratioDamageSubImpact
                : this._damage * this._dataSkill.damageBaseOnAtk;

            var damageInfo = new DamageInfo {
                creator = this._owner, damageType = this._owner.DamageType, value = damageImpact, target = target
            };

            return damageInfo;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            var bodyCollider = other.GetComponent<BodyCollider>();
            if (bodyCollider != null)
            {
                var enemy = bodyCollider.Owner as EnemyBase;
                if (enemy != null && enemy.CanChooseIsTarget)
                {
                    var damageOverTime = new DamageOverTime(new StatusOverTimeConfig() {
                        creator = this._owner,
                        owner = enemy,
                        lifeTime = this._dataSkill.time,
                        intervalTime = this._dataSkill.timeEachDealDamage,
                        delayTime = this._dataSkill.timeEachDealDamage,
                        baseValue = this._dataSkill.damageBurning
                    });
                    enemy.StatusController.AddStatus(damageOverTime);
                }
            }
        }

        // private void OnDisable()
        // {
        //     InvokeProxy.Iinvoke.CancelInvoke(this, HideEffect);
        // }

        void HideEffect()
        {
            particleParent.Simulate(0, true, true);

            LeanPool.Despawn(this);
        }
    }
}