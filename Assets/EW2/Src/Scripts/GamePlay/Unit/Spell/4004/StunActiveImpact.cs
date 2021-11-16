using UnityEngine;

namespace EW2.Spell
{
    public class StunActiveImpact : ColliderTrigger<SpellUnitBase>, IGetDamage
    {
        private SpellStatBase _spellStat;
        private SpellUnitBase _spellUnit;
        private StunStatus _stunStatus;

        public void Initialize(SpellStatBase warrior4004Stat, SpellUnitBase spell)
        {
            this._spellStat = warrior4004Stat;
            this._spellUnit = spell;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(tag))
            {
                var body = other.GetComponent<BodyCollider>().Owner as EnemyBase;
                if (body != null)
                {
                    if (((EnemyBase) body).MoveType == MoveType.Fly)
                    {
                        return;
                    }
                    
                    this._stunStatus = new StunStatus(new StatusOverTimeConfig() {
                        creator = this._spellUnit,
                        owner = body,
                        lifeTime = this._spellStat.duration,
                        statusType = StatusType.Stun
                    });
                    //this._stunStatus.Execute();
                    body.StatusController.AddStatus(this._stunStatus);
                }
            }
        }

        public DamageInfo GetDamage(Unit target)
        {
            var damageInfo = new DamageInfo
            {
                creator = this._spellUnit, 
                
                damageType = this._spellUnit.DamageType,
                
                value = this._spellUnit.Stats.GetStat<Damage>(RPGStatType.Damage).StatValue//_spellStat.damage
            };

            return damageInfo;
        }
    }
}