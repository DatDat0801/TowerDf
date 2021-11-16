using UnityEngine;

namespace EW2.Spell
{
    public class FreezeActiveImpact : ColliderTrigger<HeroBase>
    {
        private SpellUnitBase _spell;
        private EnemyFreezeOverTime _freezeStatus;

        private Spell4001ActiveData _spell4001ActiveData;

        public void Initialize(SpellUnitBase spell, Spell4001ActiveData spellActiveData)
        {
            this._spell = spell;
            this._spell4001ActiveData = spellActiveData;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(tag))
            {
                var body = other.GetComponent<BodyCollider>();
                if (body != null)
                {
                    var enemy = (EnemyBase)body.Owner;
                    if (enemy.MoveType == MoveType.Fly || !enemy.IsAlive)
                    {
                        return;
                    }

                    this._freezeStatus = new EnemyFreezeOverTime(new StatusOverTimeConfig() {
                        creator = this._spell,
                        owner = body.Owner,
                        lifeTime = this._spell.SpellStatBase.duration,
                        statusType = StatusType.Freeze
                    });

                    this._freezeStatus.OnComplete = () => {
                        var coldStatus = new EnemyColdOverTime(
                            new StatusOverTimeConfig() {
                                creator = this._spell,
                                owner = body.Owner,
                                lifeTime = this._spell4001ActiveData.coolDuration,
                                statusType = StatusType.Cold
                            }, this._spell4001ActiveData.slowDownPercent);
                        body.Owner.StatusController.AddStatus(coldStatus);
                    };
                    body.Owner.StatusController.AddStatus(this._freezeStatus);
                }
            }
        }
    }
}